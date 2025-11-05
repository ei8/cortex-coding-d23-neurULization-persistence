using ei8.Cortex.Coding.Model.Versioning;
using ei8.Cortex.Coding.Persistence.Versioning;
using ei8.Cortex.Library.Common;
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
    public class SnapshotReadRepository : 
        ClassReadRepositoryBase<Snapshot>, 
        ISnapshotReadRepository
    {
        /// <summary>
        /// Constructs a Snapshot (read-only) repository.
        /// </summary>
        /// <param name="networkRepository"></param>
        /// <param name="mirrorRepository"></param>
        /// <param name="neurULizer"></param>
        /// <param name="grannyService"></param>
        /// <param name="readWriteCache"></param>
        /// <param name="classInstanceNeuronsRetriever"></param>
        public SnapshotReadRepository(
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
        /// Gets Snapshots using the specified IDs.
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Snapshot>> GetByIds(
            IEnumerable<Guid> ids, 
            CancellationToken token = default
        ) => await this.GetByIdsCore(
            ids,
            (g) => new NeuronQuery()
            {
                Postsynaptic = new[] { g.ToString() },
                Id = ids.Select(i => i.ToString()),
                Depth = Coding.d23.neurULization.Constants.InstanceToValueInstantiatesClassDepth,
                DirectionValues = DirectionValues.Outbound
            },
            false,
            token
        );
    }
}
