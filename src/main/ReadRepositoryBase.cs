using ei8.Cortex.Coding.Mirrors;
using ei8.Cortex.Library.Common;
using neurUL.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    /// <summary>
    /// Base class for Instance (read-only) Repositories.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ReadRepositoryBase<T, TRetriever>
        where T : class, new()
    {
        protected readonly INetworkRepository networkRepository;
        protected readonly IMirrorRepository mirrorRepository;
        protected readonly IneurULizer neurULizer;
        protected readonly IGrannyService grannyService;
        protected readonly INetworkDictionary<CacheKey> readWriteCache;
        protected readonly IInstanceNeuronsRetriever<TRetriever> instanceNeuronsRetriever;

        /// <summary>
        /// Constructs an Instance (read-only) Repository Base.
        /// </summary>
        /// <param name="networkRepository"></param>
        /// <param name="mirrorRepository"></param>
        /// <param name="neurULizer"></param>
        /// <param name="grannyService"></param>
        /// <param name="readWriteCache"></param>
        /// <param name="instanceNeuronsRetriever"></param>
        public ReadRepositoryBase(
            INetworkRepository networkRepository,
            IMirrorRepository mirrorRepository,
            IneurULizer neurULizer,
            IGrannyService grannyService,
            INetworkDictionary<CacheKey> readWriteCache,
            IInstanceNeuronsRetriever<TRetriever> instanceNeuronsRetriever
        )
        { 
            AssertionConcern.AssertArgumentNotNull(networkRepository, nameof(networkRepository));
            AssertionConcern.AssertArgumentNotNull(mirrorRepository, nameof(mirrorRepository));
            AssertionConcern.AssertArgumentNotNull(neurULizer, nameof(neurULizer));
            AssertionConcern.AssertArgumentNotNull(grannyService, nameof(grannyService));
            AssertionConcern.AssertArgumentNotNull(readWriteCache, nameof(readWriteCache));
            AssertionConcern.AssertArgumentNotNull(instanceNeuronsRetriever, nameof(instanceNeuronsRetriever));

            this.networkRepository = networkRepository;
            this.mirrorRepository = mirrorRepository;
            this.neurULizer = neurULizer;
            this.grannyService = grannyService;
            this.readWriteCache = readWriteCache;
            this.instanceNeuronsRetriever = instanceNeuronsRetriever;
        }

        protected virtual async Task<IEnumerable<T>> GetByIdsCore(
            IEnumerable<Guid> ids,
            Func<Guid, NeuronQuery> neuronQueryCreator,
            bool restrictQueryResultCount = true,
            CancellationToken token = default
        )
        {
            ids.ValidateIds();

            GrannyResult instantiatesInstanceResult = await ReadRepositoryBase<T, TRetriever>.GetInstantiates(
                this.grannyService,
                this.mirrorRepository,
                token
            );

            var query = neuronQueryCreator(instantiatesInstanceResult.Granny.Neuron.Id);

            return await this.GetCore(
                async () => (await this.networkRepository.GetByQueryAsync(
                    query,
                    restrictQueryResultCount
                )).Network,
                token
            );
        }

        /// <summary>
        /// Gets Instances using the specified IDs and IInstanceNeuronsRetriever.
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected virtual async Task<IEnumerable<T>> GetCore(
            Func<Task<Network>> networkRetriever,
            CancellationToken token = default
        )
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var network = await networkRetriever();

            var result = await this.neurULizer.DeneurULizeCacheAsync<T>(
                network,
                this.instanceNeuronsRetriever,
                this.readWriteCache[CacheKey.Read],
                token
            );

            watch.Stop();
            System.Diagnostics.Debug.WriteLine($"{typeof(T).Name}.GetByIds took (secs): {watch.Elapsed.TotalSeconds}");

            return result;
        }

        protected static async Task<GrannyResult> GetInstantiates(
            IGrannyService grannyService, 
            IMirrorRepository mirrorRepository, 
            CancellationToken token
        )
        {
            var instantiatesInstanceResult = await grannyService.TryGetParseAsync(
                new InstantiatesClassGrannyInfo(
                    new Coding.d23.neurULization.Processors.Readers.Deductive.InstantiatesClassParameterSet(
                        await mirrorRepository.GetByKeyAsync(
                            typeof(T)
                        )
                    )
                )
            );

            AssertionConcern.AssertStateTrue(
                instantiatesInstanceResult.Success,
                $"'Instantiates^{typeof(T).Name}' is required to invoke {nameof(ReadRepositoryBase<T, TRetriever>.GetCore)}"
            );

            return instantiatesInstanceResult;
        }
    }
}
