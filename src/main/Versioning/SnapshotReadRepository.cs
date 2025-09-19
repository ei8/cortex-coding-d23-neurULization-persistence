using ei8.Cortex.Coding.Persistence.Versioning;
using ei8.Cortex.Coding.Versioning;
using ei8.Cortex.Library.Common;
using neurUL.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence.Versioning
{
    /// <summary>
    /// Represents a Snapshot (read-only) repository.
    /// </summary>
    public class SnapshotReadRepository : ISnapshotReadRepository
    {
        private readonly IneurULizer neurULizer;
        private readonly INetworkRepository networkRepository;
        private readonly IIdInstanceNeuronsRetriever idInstanceNeuronsRetriever;
        private readonly INetworkDictionary<CacheKey> readWriteCache;

        /// <summary>
        /// Constructs a Snapshot (read-only) repository.
        /// </summary>
        /// <param name="neurULizer"></param>
        /// <param name="networkRepository"></param>
        /// <param name="idInstanceNeuronsRetriever"></param>
        /// <param name="readWriteCache"></param>
        public SnapshotReadRepository(
            IneurULizer neurULizer,
            INetworkRepository networkRepository,
            IIdInstanceNeuronsRetriever idInstanceNeuronsRetriever,
            INetworkDictionary<CacheKey> readWriteCache
        )
        {
            AssertionConcern.AssertArgumentNotNull(neurULizer, nameof(neurULizer));
            AssertionConcern.AssertArgumentNotNull(networkRepository, nameof(networkRepository));
            AssertionConcern.AssertArgumentNotNull(idInstanceNeuronsRetriever, nameof(idInstanceNeuronsRetriever));
            AssertionConcern.AssertArgumentNotNull(readWriteCache, nameof(readWriteCache));

            this.neurULizer = neurULizer;
            this.networkRepository = networkRepository;
            this.idInstanceNeuronsRetriever = idInstanceNeuronsRetriever;
            this.readWriteCache = readWriteCache;
        }

        /// <summary>
        /// Gets Snapshots using the specified IDs.
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Snapshot>> GetByIds(IEnumerable<Guid> ids, CancellationToken token = default)
        {
            AssertionConcern.AssertArgumentNotNull(ids, nameof(ids));
            AssertionConcern.AssertArgumentValid(i => i.Any(), ids, $"Specified value cannot be an empty array.", nameof(ids));

            var queryResult = await networkRepository.GetByQueryAsync(
                new NeuronQuery()
                {
                    Id = ids.Select(i => i.ToString()),
                    Depth = Constants.ValueToInstantiatesClassDepth,
                    DirectionValues = DirectionValues.Outbound
                }
            );

            queryResult.Network.ValidateIds(ids);

            idInstanceNeuronsRetriever.Initialize(ids);
            return await this.neurULizer.DeneurULizeCacheAsync<Snapshot>(
                queryResult.Network,
                this.idInstanceNeuronsRetriever,
                this.readWriteCache[CacheKey.Read],
                token
            );
        }
    }
}
