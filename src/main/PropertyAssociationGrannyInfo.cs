using ei8.Cortex.Coding.d23.Grannies;
using ei8.Cortex.Coding.d23.neurULization.Processors.Readers.Deductive;
using ei8.Cortex.Coding.d23.neurULization.Processors.Writers;
using ei8.Cortex.Coding.Persistence;
using ei8.Cortex.Coding.Properties;
using ei8.Cortex.Library.Common;
using neurUL.Common.Domain.Model;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    public class PropertyAssociationGrannyInfo : IGrannyInfo<
        IPropertyAssociation,
        IPropertyAssociationReader,
        IPropertyAssociationParameterSet,
        IPropertyAssociationWriter
    >
    {
        public static async Task<PropertyAssociationGrannyInfo> CreateById<TAggregate>(
            string propertyName,
            IEnsembleRepository ensembleRepository,
            Guid valueId
        )
        {
            AssertionConcern.AssertArgumentNotEmpty(propertyName, "Specified parameter cannot be null or empty.",
                nameof(propertyName));
            AssertionConcern.AssertArgumentNotNull(ensembleRepository, nameof(ensembleRepository));
            var property = typeof(TAggregate).GetProperty(propertyName);
            var classAttribute = property.GetCustomAttributes<neurULClassAttribute>().SingleOrDefault();
            AssertionConcern.AssertArgumentValid(ca => ca != null, classAttribute, $"Specified property should have a '{nameof(neurULClassAttribute)}'.", nameof(propertyName));

            return new PropertyAssociationGrannyInfo(
                new Coding.d23.neurULization.Processors.Readers.Deductive.PropertyAssociationParameterSet(
                    await ensembleRepository.GetExternalReferenceAsync(
                        property
                    ),
                    (await ensembleRepository.GetByQueryAsync(
                        new NeuronQuery()
                        {
                            Id = new string[] { valueId.ToString() }
                        },
                        false
                    )).Ensemble.GetItems<Coding.Neuron>().Single(),
                    await ensembleRepository.GetExternalReferenceAsync(
                        classAttribute.Type
                    ),
                    ValueMatchBy.Id
                )
            );
        }

        public PropertyAssociationGrannyInfo(IPropertyAssociationParameterSet parameters)
        {
            this.Parameters = parameters;
        }

        public IPropertyAssociationParameterSet Parameters { get; }
    }
}
