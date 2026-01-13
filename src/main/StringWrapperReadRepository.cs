using ei8.Cortex.Coding.Mirrors;
using ei8.Cortex.Coding.Model.Wrappers;
using ei8.Cortex.Coding.Persistence.Wrappers;
using ei8.Cortex.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    /// <summary>
    /// Represents a StringWrapper (read-only) repository.
    /// </summary>
    public class StringWrapperReadRepository :
        IdReadRepositoryBase<StringWrapper>,
        IStringWrapperReadRepository
    {
        /// <summary>
        /// Constructs a StringWrapper (read-only) repository.
        /// </summary>
        /// <param name="networkRepository"></param>
        /// <param name="mirrorRepository"></param>
        /// <param name="neurULizer"></param>
        /// <param name="grannyService"></param>
        /// <param name="readWriteCache"></param>
        /// <param name="idInstanceNeuronsRetriever"></param>
        public StringWrapperReadRepository(
            INetworkRepository networkRepository,
            IMirrorRepository mirrorRepository,
            IneurULizer neurULizer,
            IGrannyService grannyService,
            INetworkDictionary<CacheKey> readWriteCache,
            IIdInstanceNeuronsRetriever idInstanceNeuronsRetriever
        ) : base(
            networkRepository,
            mirrorRepository,
            neurULizer,
            grannyService,
            readWriteCache,
            idInstanceNeuronsRetriever
        )
        {
        }

        /// <summary>
        /// Gets StringWrappers using the specified IDs.
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IEnumerable<StringWrapper>> GetByIds(
            IEnumerable<Guid> ids,
            CancellationToken token = default
        ) => await this.GetByIdsCore(
            ids,
            (g) => new NeuronQuery()
            {
                Id = ids.Select(i => i.ToString()),
                Depth = Coding.d23.neurULization.Constants.ValueToInstantiatesClassDepth,
                DirectionValues = DirectionValues.Outbound
            },
            false,
            token
        );
    }
}
