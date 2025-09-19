using ei8.Cortex.Coding.d23.Grannies;
using ei8.Cortex.Coding.d23.neurULization.Processors.Readers.Deductive;
using ei8.Cortex.Coding.d23.neurULization.Processors.Writers;
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
    /// <summary>
    /// Represents GrannyService extension methods.
    /// </summary>
    public static class GrannyServiceExtensions
    {
        private static string CreatePropertyCacheId<TAggregate>(string propertyName, Guid valueId)
        {
            AssertionConcern.AssertArgumentNotEmpty(propertyName, "Parameter cannot be null or empty.", nameof(propertyName));
            AssertionConcern.AssertArgumentValid(id => id != Guid.Empty, valueId, $"Parameter cannot be equal to '{Guid.Empty}", nameof(valueId));

            return $"{typeof(TAggregate).FullName}-{propertyName}-{valueId}";
        }

        /// <summary>
        /// Tries to obtain PropertyValueAssociations from the specified cache or from persistence.
        /// </summary>
        /// <typeparam name="TAggregate"></typeparam>
        /// <param name="grannyService"></param>
        /// <param name="mirrorRepository"></param>
        /// <param name="networkRepository"></param>
        /// <param name="propertyName"></param>
        /// <param name="valueIds"></param>
        /// <param name="propertyCache"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<GrannyResult>> TryObtainPropertyValueAssociations<TAggregate>(
            this IGrannyService grannyService,
            IMirrorRepository mirrorRepository,
            INetworkRepository networkRepository,
            string propertyName,
            IEnumerable<Guid> valueIds,
            IDictionary<string, IGranny> propertyCache
        )
        {
            var valueNeurons = (await networkRepository.GetByQueryAsync(
                new NeuronQuery()
                {
                    Id = valueIds.Select(vi => vi.ToString())
                },
                false
            )).Network.GetItems<Coding.Neuron>();

            var results = new List<GrannyResult>();

            foreach (var vn in valueNeurons)
            {
                results.Add(await GrannyServiceExtensions.TryObtainPropertiesCore<
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
                    vn.Id,
                    async (propertyKey) => new PropertyValueAssociationGrannyInfo(
                        new PropertyValueAssociationParameterSet(
                            await mirrorRepository.GetByKeyAsync(propertyKey),
                            vn
                        )
                    ),
                    propertyCache
                ));
            }

            return results;
        }

        // TODO:1 See if possible to transfer this method inside DeneurULize
        // so it can be called instead of this method
        private static async Task<GrannyResult> TryObtainPropertiesCore<TAggregate, TGrannyInfo, TGranny, TDeductiveReader, TParameterSet, TWriter>(
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
            var result = new GrannyResult(valueId, false, null);

            var propertyCacheId = GrannyServiceExtensions.CreatePropertyCacheId<TAggregate>(propertyName, valueId);
            if (!propertyCache.ContainsKey(propertyCacheId))
            {
                var hasPropertyResult = await grannyService.TryGetParseAsync(await grannyInfoCreator(property));

                if (hasPropertyResult.Success)
                    propertyCache.Add(propertyCacheId, hasPropertyResult.Granny);
            }

            if (propertyCache.ContainsKey(propertyCacheId))
                result = new GrannyResult(valueId, true, propertyCache[propertyCacheId]);

            return result;
        }

        /// <summary>
        /// Tries to obtain PropertyInstanceValueAssociations from the specified cache or from persistence.
        /// </summary>
        /// <typeparam name="TAggregate"></typeparam>
        /// <param name="grannyService"></param>
        /// <param name="mirrorRepository"></param>
        /// <param name="networkRepository"></param>
        /// <param name="propertyName"></param>
        /// <param name="valueId"></param>
        /// <param name="propertyCache"></param>
        /// <returns></returns>
        public static async Task<GrannyResult> TryObtainPropertyInstanceValueAssociations<TAggregate>(
            this IGrannyService grannyService,
            IMirrorRepository mirrorRepository,
            INetworkRepository networkRepository,
            string propertyName,
            Guid valueId,
            IDictionary<string, IGranny> propertyCache
        )
        {
            return await GrannyServiceExtensions.TryObtainPropertiesCore<
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
