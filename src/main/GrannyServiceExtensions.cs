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

        public static async Task<GrannyResult> TryGetPropertyInstanceValueAssociationFromCacheOrDb<TAggregate>(
            this IGrannyService grannyService,
            IExternalReferenceRepository externalReferenceRepository,
            string propertyName,
            Guid valueId,
            IDictionary<string, IGranny> propertyAssociationCache
        )
        {
            AssertionConcern.AssertArgumentNotEmpty(propertyName, "Specified parameter cannot be null or empty.",
                nameof(propertyName));
            var property = typeof(TAggregate).GetProperty(propertyName);
            var classAttribute = property.GetCustomAttributes<neurULClassAttribute>().SingleOrDefault();
            AssertionConcern.AssertArgumentValid(ca => ca != null, classAttribute, $"Specified property should have a '{nameof(neurULClassAttribute)}'.", nameof(propertyName));

            var propertyCacheId = GrannyServiceExtensions.CreateCacheId<TAggregate>(propertyName, valueId);
            GrannyResult result = new GrannyResult(false, null);

            if (!propertyAssociationCache.ContainsKey(propertyCacheId))
            {
                var hasPropertyResult = await grannyService.TryGetParseAsync(
                    new PropertyInstanceValueAssociationGrannyInfo(
                        new PropertyInstanceValueAssociationParameterSet(
                            await externalReferenceRepository.GetByKeyAsync(property),
                            (await grannyService.NetworkRepository.GetByQueryAsync(
                                new NeuronQuery()
                                {
                                    Id = new string[] { valueId.ToString() }
                                },
                                false
                            )).Network.GetItems<Coding.Neuron>().Single(),
                            await externalReferenceRepository.GetByKeyAsync(classAttribute.Type),
                            ValueMatchBy.Id
                        )
                    )
                );

                if (hasPropertyResult.Success)
                    propertyAssociationCache.Add(propertyCacheId, hasPropertyResult.Granny);
            }

            if (propertyAssociationCache.ContainsKey(propertyCacheId))
                result = new GrannyResult(true, propertyAssociationCache[propertyCacheId]);

            return result;
        }
    }
}
