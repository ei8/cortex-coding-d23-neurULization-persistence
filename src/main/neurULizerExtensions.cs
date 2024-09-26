using ei8.Cortex.Coding.d23.Grannies;
using ei8.Cortex.Coding.d23.neurULization.Processors.Writers;
using ei8.Cortex.Coding.Properties;
using ei8.Cortex.Coding.Properties.Neuron;
using neurUL.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    public static class neurULizerExtensions
    {
        public static async Task<Ensemble> neurULizeAsync<TValue>(
            this IneurULizer neurULizer, 
            TValue value, 
            IEnsembleRepository ensembleRepository, 
            Processors.Writers.IInstanceProcessor instanceProcessor, 
            IDictionary<string, Ensemble> ensembleCache,
            string appUserId,
            string cortexLibraryOutBaseUrl,
            int queryResultLimit,
            string userId
        )
        {
            AssertionConcern.AssertArgumentNotNull(value, nameof(value));
            AssertionConcern.AssertArgumentNotNull(instanceProcessor, nameof(instanceProcessor));

            var result = new Ensemble();

            string valueClassKey = ExternalReference.ToKeyString(value.GetType());

            var propertyData = value.GetType().GetProperties()
                .Select(pi => pi.ToPropertyData(value))
                .Where(pd => pd != null);

            var neuronProperties = propertyData.Where(pd => pd.NeuronProperty != null).Select(pd => pd.NeuronProperty);
            var grannyProperties = propertyData.Where(pd => pd.NeuronProperty == null);
            Guid? regionId = neuronProperties.OfType<RegionIdProperty>().SingleOrDefault()?.Value;

            var propertyKeys = grannyProperties.Select(gp => gp.Key)
                .Concat(grannyProperties.Select(gp => gp.ClassKey))
                .Distinct();

            // use key to retrieve external reference url from library
            var externalReferences = await ensembleRepository.GetExternalReferencesAsync(
                appUserId,
                cortexLibraryOutBaseUrl,
                new string[] {
                    valueClassKey
                }.Concat(
                    propertyKeys
                ).ToArray()
            );

            IdProperty idp = null;

            // Unnecessary to validate null id and tag values since another service can be
            // responsible for pruning grannies containing null or empty values.
            // Null values can also be considered as valid new values.
            var instance = await instanceProcessor.ObtainAsync<IInstance, IInstanceProcessor, Processors.Readers.Deductive.IInstanceParameterSet>(
                    result,
                    new Processors.Readers.Deductive.InstanceParameterSet(
                        (idp = neuronProperties.OfType<IdProperty>().SingleOrDefault()) != null ?
                            idp.Value :
                            Guid.NewGuid(),
                        neuronProperties.OfType<TagProperty>().SingleOrDefault()?.Value,
                        neuronProperties.OfType<ExternalReferenceUrlProperty>().SingleOrDefault()?.Value,
                        regionId,
                        externalReferences[valueClassKey],
                        grannyProperties.Select(async gp =>
                            await neurULizerExtensions.CreatePropertyAssociationParams(
                                gp, 
                                regionId, 
                                ensembleRepository, 
                                externalReferences,
                                userId,
                                cortexLibraryOutBaseUrl,
                                queryResultLimit
                            )
                        )
                            .Select(t => t.Result)
                            .Where(i => i != null)
                            .ToArray()
                    )
                );

            await ensembleRepository.UniquifyAsync(
                appUserId,
                result,
                cortexLibraryOutBaseUrl,
                queryResultLimit,
                ensembleCache
            );

            return result;
        }

        private static async Task<Processors.Readers.Deductive.PropertyAssociationParameterSet> CreatePropertyAssociationParams(
            PropertyData gp, 
            Guid? regionId, 
            IEnsembleRepository ensembleRepository, 
            IDictionary<string, Neuron> externalReferences,
            string userId,
            string cortexLibraryOutBaseUrl,
            int queryResultLimit
        )
        {
            return new Processors.Readers.Deductive.PropertyAssociationParameterSet(
                externalReferences[gp.Key],
                (
                    gp.ValueMatchBy == ValueMatchBy.Id ?
                        ((await ensembleRepository.GetByQueryAsync(
                            userId,
                            new Library.Common.NeuronQuery()
                            {
                                Id = new string[] { gp.Value }
                            },
                            cortexLibraryOutBaseUrl,
                            queryResultLimit
                        )).GetItems<Neuron>().Single()) :
                        Neuron.CreateTransient(gp.Value, null, regionId)
                ),
                externalReferences[gp.ClassKey],
                gp.ValueMatchBy
            );
        }

        public static async Task<IEnumerable<TValue>> DeneurULizeAsync<TValue>(
            this IneurULizer neurULizer,
            Ensemble value,
            IEnsembleRepository ensembleRepository, 
            Processors.Readers.Inductive.IInstanceProcessor instanceProcessor,
            IPrimitiveSet primitives,
            IGrannyService grannyService,
            string appUserId,
            string identityAccessOutBaseUrl,
            string cortexLibraryOutBaseUrl,
            int queryResultLimit,
            string userId
        )
            where TValue : class, new()
        {
            List<TValue> result = new List<TValue>();

            string valueClassKey = ExternalReference.ToKeyString(typeof(TValue));

            // get properties
            var props = typeof(TValue).GetProperties().ToArray();
            var pds = props.Select(pi => pi.ToPropertyData(new TValue())).ToArray();
            var propertyData = pds.Where(pd => pd != null);

            var neuronProperties = propertyData.Where(pd => pd.NeuronProperty != null).Select(pd => pd.NeuronProperty);
            var grannyProperties = propertyData.Where(pd => pd.NeuronProperty == null);

            var propertyKeys = grannyProperties.Select(gp => gp.Key)
                .Concat(grannyProperties.Select(gp => gp.ClassKey))
                .Distinct();

            // use key to retrieve external reference url from library
            var externalReferences = await ensembleRepository
                .GetExternalReferencesAsync(
                    userId,
                    cortexLibraryOutBaseUrl,
                    (new string[] {
                        valueClassKey
                    }).Concat(
                        propertyKeys
                    ).ToArray()
                );

            // TODO: use GrannyCacheService
            var instantiatesClassResult = await grannyService.TryObtainPersistAsync<
                IInstantiatesClass,
                Coding.d23.neurULization.Processors.Readers.Deductive.IInstantiatesClassProcessor,
                Coding.d23.neurULization.Processors.Readers.Deductive.IInstantiatesClassParameterSet,
                Coding.d23.neurULization.Processors.Writers.IInstantiatesClassProcessor
            >(
                new Coding.d23.neurULization.Processors.Readers.Deductive.InstantiatesClassParameterSet(
                    await ensembleRepository.GetExternalReferenceAsync(
                        appUserId,
                        cortexLibraryOutBaseUrl,
                        typeof(TValue)
                    )
                ),
                appUserId,
                identityAccessOutBaseUrl,
                cortexLibraryOutBaseUrl,
                queryResultLimit
                // TODO: add support for CancellationToken
            );

            AssertionConcern.AssertStateTrue(
                instantiatesClassResult.Item1,
                $"'Instantiates^Avatar' is required to invoke {nameof(IneurULizer.DeneurULizeAsync)}"
            );

            var instanceNeurons = value.GetPresynapticNeurons(instantiatesClassResult.Item2.Neuron.Id);

            foreach (var instanceNeuron in instanceNeurons)
            {
                if (instanceProcessor.TryParse(
                    value,
                    new Processors.Readers.Inductive.InstanceParameterSet(
                        instanceNeuron,
                        externalReferences[valueClassKey],
                        grannyProperties.Select(gp =>
                            Processors.Readers.Inductive.PropertyAssociationParameterSet.CreateWithoutGranny(
                                externalReferences[gp.Key],
                                externalReferences[gp.ClassKey]
                            )
                        )
                    ),
                    out IInstance instance
                ))
                {
                    var tempResult = new TValue();

                    foreach (var gp in grannyProperties)
                    {
                        var propAssoc = instance.PropertyAssociations.SingleOrDefault(
                            pa => pa.PropertyAssignment.Expression.Units
                                .AsEnumerable()
                                .GetValueUnitGranniesByTypeId(primitives.Unit.Id).SingleOrDefault().Value.Id == externalReferences[gp.Key].Id
                        );
                        object propValue = null;

                        var property = tempResult.GetType().GetProperty(gp.PropertyName);
                        var classAttribute = property.GetCustomAttributes<neurULClassAttribute>().SingleOrDefault();

                        if (classAttribute != null)
                        {
                            AssertionConcern.AssertArgumentValid(
                                t => property.PropertyType == typeof(Guid),
                                typeof(TValue),
                                $"Property '{property.Name}' has '{nameof(neurULClassAttribute)}' but its Type is not equal to 'Guid'.",
                                nameof(TValue)
                                );

                            propValue = propAssoc.PropertyAssignment.PropertyValueExpression.ValueExpression.Value.Neuron.Id;
                        }
                        else
                        {
                            AssertionConcern.Equals(gp.ClassKey, ExternalReference.ToKeyString(property.PropertyType));

                            var propValueString = propAssoc.PropertyAssignment.PropertyValueExpression.ValueExpression.Value.Neuron.Tag;
                            if (property.PropertyType == typeof(string))
                            {
                                propValue = propValueString;
                            }
                            else if (property.PropertyType == typeof(Guid))
                            {
                                propValue = Guid.Parse(propValueString);
                            }
                            else if (
                                Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateTimeOffset) ||
                                property.PropertyType == typeof(DateTimeOffset)
                                )
                            {
                                propValue = DateTimeOffset.Parse(propValueString);
                            }
                            // TODO: else use neurULConverterAttribute
                        }
                        property.SetValue(tempResult, propValue);
                    }

                    foreach (var np in neuronProperties)
                    {
                        var instanceNeuronProperty = instance.Neuron.GetType().GetProperty(
                            np.GetType().Name.Replace("Property", string.Empty)
                        );
                        object propertyValue = instanceNeuronProperty.GetValue(instance.Neuron);

                        tempResult.GetType().GetProperty(np.Name).SetValue(tempResult, propertyValue);
                    }

                    result.Add(tempResult);
                }
            }

            return result.AsEnumerable();
        }        
    }
}
