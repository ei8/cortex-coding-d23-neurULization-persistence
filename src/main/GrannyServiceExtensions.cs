using ei8.Cortex.Coding.d23.Grannies;
using ei8.Cortex.Coding.d23.neurULization.Processors.Readers.Deductive;
using ei8.Cortex.Coding.Persistence;
using ei8.Cortex.Coding.Properties;
using ei8.Cortex.Library.Common;
using neurUL.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    public static class GrannyServiceExtensions
    {
        private static string CreateCacheId<TAggregate>(string propertyName, Guid valueId)
        {
            AssertionConcern.AssertArgumentNotEmpty(propertyName, "Parameter cannot be null or empty.", nameof(propertyName));
            AssertionConcern.AssertArgumentValid(id => id != Guid.Empty, valueId, $"Parameter cannot be equal to '{Guid.Empty}", nameof(valueId));

            return $"{typeof(TAggregate).FullName}-{propertyName}-{valueId}";
        }

        public static async Task<IGranny> GetPropertyAssociationFromCacheOrDb<TAggregate>(
            this IGrannyService grannyService,
            IEnsembleRepository ensembleRepository,
            string propertyName,
            Guid valueId,
            IDictionary<string, IGranny> propertyAssociationCache
        )
        {
            AssertionConcern.AssertArgumentNotEmpty(propertyName, "Specified parameter cannot be null or empty.",
                nameof(propertyName));
            AssertionConcern.AssertArgumentNotNull(ensembleRepository, nameof(ensembleRepository));
            var property = typeof(TAggregate).GetProperty(propertyName);
            var classAttribute = property.GetCustomAttributes<neurULClassAttribute>().SingleOrDefault();
            AssertionConcern.AssertArgumentValid(ca => ca != null, classAttribute, $"Specified property should have a '{nameof(neurULClassAttribute)}'.", nameof(propertyName));

            var propertyCacheId = GrannyServiceExtensions.CreateCacheId<TAggregate>(propertyName, valueId);

            if (!propertyAssociationCache.ContainsKey(propertyCacheId))
            {
                var hasPropertyResult = await grannyService.TryGetParseAsync(
                    new PropertyAssociationGrannyInfo(
                        new PropertyAssociationParameterSet(
                            await ensembleRepository.GetExternalReferenceAsync(property),
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
                    )
                );

                if (hasPropertyResult.Success)
                    propertyAssociationCache.Add(propertyCacheId, hasPropertyResult.Granny);
            }

            return propertyAssociationCache[propertyCacheId];
        }
    }
}
