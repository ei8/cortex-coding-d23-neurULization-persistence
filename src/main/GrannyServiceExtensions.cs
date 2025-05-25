using ei8.Cortex.Coding.d23.Grannies;
using ei8.Cortex.Coding.d23.neurULization.Processors.Readers.Deductive;
using ei8.Cortex.Coding.d23.neurULization.Processors.Writers;
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
        private static string CreatePropertyCacheId<TAggregate>(string propertyName, Guid valueId)
        {
            AssertionConcern.AssertArgumentNotEmpty(propertyName, "Parameter cannot be null or empty.", nameof(propertyName));
            AssertionConcern.AssertArgumentValid(id => id != Guid.Empty, valueId, $"Parameter cannot be equal to '{Guid.Empty}", nameof(valueId));

            return $"{typeof(TAggregate).FullName}-{propertyName}-{valueId}";
        }

        public static async Task<GrannyResult> TryGetPropertyValueAssociationFromCacheOrDb<TAggregate>(
            this IGrannyService grannyService,
            IMirrorRepository mirrorRepository,
            INetworkRepository networkRepository,
            string propertyName,
            Guid valueId,
            IDictionary<string, IGranny> propertyCache
        )
        {
            return await TryGetPropertyFromCacheOrDbCore<
                TAggregate,
                PropertyValueAssociationGrannyInfo,
                IPropertyValueAssociation,
                IPropertyValueAssociationReader,
                IPropertyValueAssociationParameterSet,
                IPropertyValueAssociationWriter
            >
            (
                grannyService, 
                propertyName, 
                valueId, 
                async (propertyKey) => new PropertyValueAssociationGrannyInfo(
                    new PropertyValueAssociationParameterSet(
                        await mirrorRepository.GetByKeyAsync(propertyKey),
                        (await networkRepository.GetByQueryAsync(
                            new NeuronQuery()
                            {
                                Id = new[] { valueId.ToString() }
                            },
                            false
                        )).Network.GetItems<Coding.Neuron>().Single()
                    )
                ),
                propertyCache
            );
        }

        // TODO:1 See if possible to transfer this method inside DeneurULize
        // so it can be called instead of this method
        private static async Task<GrannyResult> TryGetPropertyFromCacheOrDbCore<TAggregate, TGrannyInfo, TGranny, TDeductiveReader, TParameterSet, TWriter>(
            IGrannyService grannyService, 
            string propertyName, 
            Guid valueId, 
            Func<PropertyInfo, Task<TGrannyInfo>> grannyInfoCreator,
            IDictionary<string, IGranny> propertyCache
        )
            where TGrannyInfo : IGrannyInfo<TGranny, TDeductiveReader, TParameterSet, TWriter>
            where TGranny : IGranny
            where TDeductiveReader : Processors.Readers.Deductive.IGrannyReader<TGranny, TParameterSet>
            where TParameterSet : Processors.Readers.Deductive.IDeductiveParameterSet
            where TWriter : Cortex.Coding.d23.neurULization.Processors.Writers.IGrannyWriter<TGranny, TParameterSet>
        {
            AssertionConcern.AssertArgumentNotEmpty(propertyName, "Specified parameter cannot be null or empty.",
                            nameof(propertyName));
            var property = typeof(TAggregate).GetProperty(propertyName);
            var propertyCacheId = GrannyServiceExtensions.CreatePropertyCacheId<TAggregate>(propertyName, valueId);
            var result = new GrannyResult(false, null);

            if (!propertyCache.ContainsKey(propertyCacheId))
            {
                var hasPropertyResult = await grannyService.TryGetParseAsync(await grannyInfoCreator(property));

                if (hasPropertyResult.Success)
                    propertyCache.Add(propertyCacheId, hasPropertyResult.Granny);
            }

            if (propertyCache.ContainsKey(propertyCacheId))
                result = new GrannyResult(true, propertyCache[propertyCacheId]);

            return result;
        }

        public static async Task<GrannyResult> TryGetPropertyInstanceValueAssociationFromCacheOrDb<TAggregate>(
            this IGrannyService grannyService,
            IMirrorRepository mirrorRepository,
            INetworkRepository networkRepository,
            string propertyName,
            Guid valueId,
            IDictionary<string, IGranny> propertyCache
        )
        {
            return await TryGetPropertyFromCacheOrDbCore<
                TAggregate,
                PropertyInstanceValueAssociationGrannyInfo,
                IPropertyInstanceValueAssociation,
                IPropertyInstanceValueAssociationReader,
                IPropertyInstanceValueAssociationParameterSet,
                IPropertyInstanceValueAssociationWriter
            >
            (
                grannyService,
                propertyName,
                valueId,
                async (propertyKey) => {
                    var classAttribute = propertyKey.GetCustomAttributes<neurULClassAttribute>().SingleOrDefault();
                    AssertionConcern.AssertArgumentValid(ca => ca != null, classAttribute, $"Specified property should have a '{nameof(neurULClassAttribute)}'.", nameof(propertyName));
                    return new PropertyInstanceValueAssociationGrannyInfo(
                        new PropertyInstanceValueAssociationParameterSet(
                            await mirrorRepository.GetByKeyAsync(propertyKey),
                            (await networkRepository.GetByQueryAsync(
                                new NeuronQuery()
                                {
                                    Id = new[] { valueId.ToString() }
                                },
                                false
                            )).Network.GetItems<Coding.Neuron>().Single(),
                            await mirrorRepository.GetByKeyAsync(classAttribute.Type),
                            ValueMatchBy.Id
                        )
                    );
                },
                propertyCache
            );
        }
    }
}
