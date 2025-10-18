using ei8.Cortex.Coding.d23.Grannies;
using ei8.Cortex.Library.Common;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    /// <summary>
    /// Base class for Instance (read-only) Repositories 
    /// that use an IClassInstanceNeuronsRetriever.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ClassReadRepositoryBase<T> : ReadRepositoryBase<T, Neuron>
        where T : class, new()
    {
        /// <summary>
        /// Constructs a base class for Instance (read-only) Repositories
        /// that use an IClassInstanceNeuronsRetriever.
        /// </summary>
        /// <param name="networkRepository"></param>
        /// <param name="mirrorRepository"></param>
        /// <param name="neurULizer"></param>
        /// <param name="grannyService"></param>
        /// <param name="readWriteCache"></param>
        /// <param name="classInstanceNeuronsRetriever"></param>
        public ClassReadRepositoryBase(
            INetworkRepository networkRepository,
            IMirrorRepository mirrorRepository,
            IneurULizer neurULizer,
            IGrannyService grannyService,
            INetworkDictionary<CacheKey> readWriteCache,
            IClassInstanceNeuronsRetriever classInstanceNeuronsRetriever
        ) : base(
            networkRepository,
            mirrorRepository,
            neurULizer,
            grannyService,
            readWriteCache,
            classInstanceNeuronsRetriever
        )
        {
        }

        /// <summary>
        /// Gets all Instances.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAll(CancellationToken token = default)
        {
            var instantiatesInstanceResult = await ReadRepositoryBase<T, Neuron>.GetInstantiates(
                this.grannyService,
                this.mirrorRepository,
                token
            );

            return await this.GetCore(
                async () => (await this.networkRepository.GetByQueryAsync(
                    new NeuronQuery()
                    {
                        Postsynaptic = new string[] { instantiatesInstanceResult.Granny.Neuron.Id.ToString() },
                        SortBy = SortByValue.NeuronExternalReferenceUrl,
                        SortOrder = SortOrderValue.Ascending,
                        Depth = Coding.d23.neurULization.Constants.InstanceToValueInstantiatesClassDepth,
                        DirectionValues = DirectionValues.Outbound
                    }
                )).Network,
                token
            );
        }

        protected async Task<IEnumerable<T>> GetByPropertyAssociationValueIdsCore(
            IEnumerable<Guid> ids,
            IDictionary<string, IGranny> propertyAssociationCache,
            string propertyName,
            string methodName,
            bool restrictQueryResultCount = true,
            CancellationToken token = default
        )
        {
            ids.ValidateIds();

            var instantiatesInstanceResult = await ReadRepositoryBase<T, Neuron>.GetInstantiates(
                this.grannyService,
                this.mirrorRepository,
                token
            );

            var instanceHasPropertyAssociationValueIdsResults = await ClassReadRepositoryBase<T>.GetPropertyValueAssociationByValueIds(
                this.grannyService,
                this.mirrorRepository,
                this.networkRepository,
                propertyAssociationCache,
                ids,
                propertyName,
                methodName,
                token
            );

            return await this.GetCore(
                async () => {
                    Network cns = new Network();

                    // TODO:0 loop is not needed if NeuronQuery.Postsynaptic parameter accepts nested parameters
                    // eg. "x AND (y OR z)"
                    // Id = 1 AND (PropertyId = 2 or PropertyId = 3)
                    foreach (var chpavir in instanceHasPropertyAssociationValueIdsResults)
                    {
                        if (chpavir.Success)
                        {
                            cns.AddReplaceItems(
                                (await this.networkRepository.GetByQueryAsync(
                                    new NeuronQuery()
                                    {
                                        Postsynaptic = new[] {
                                    instantiatesInstanceResult.Granny.Neuron.Id.ToString(),
                                    chpavir.Granny.Neuron.Id.ToString()
                                        },
                                        SortBy = SortByValue.NeuronCreationTimestamp,
                                        SortOrder = SortOrderValue.Descending,
                                        Depth = Coding.d23.neurULization.Constants.InstanceToValueInstantiatesClassDepth,
                                        DirectionValues = Library.Common.DirectionValues.Outbound
                                    },
                                    restrictQueryResultCount
                                )).Network
                            );
                        }
                    }

                    return cns;
                },
                token
            );
        }

        protected override async Task<IEnumerable<T>> GetCore(Func<Task<Network>> networkRetriever, CancellationToken token = default)
        {
            ((IClassInstanceNeuronsRetriever)this.instanceNeuronsRetriever).Initialize(
                await this.mirrorRepository.GetByKeyAsync(typeof(T))
            );

            return await base.GetCore(networkRetriever, token);
        }

        protected static async Task<IEnumerable<GrannyResult>> GetPropertyValueAssociationByValueIds(
            IGrannyService grannyService,
            IMirrorRepository mirrorRepository,
            INetworkRepository networkRepository,
            IDictionary<string, IGranny> propertyAssociationCache,
            IEnumerable<Guid> valueIds,
            string propertyName,
            string methodName,
            CancellationToken token
        )
        {
            var results = await grannyService.TryObtainPropertyValueAssociations<T>(
                mirrorRepository,
                networkRepository,
                propertyName,
                valueIds,
                propertyAssociationCache
            );

            return results;
        }
    }
}
