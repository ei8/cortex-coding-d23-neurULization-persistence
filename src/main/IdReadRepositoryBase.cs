using ei8.Cortex.Coding.Mirrors;
using ei8.Cortex.Library.Common;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    /// <summary>
    /// Base class for Instance (read-only) Repositories 
    /// that use an IIdInstanceNeuronsRetriever.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IdReadRepositoryBase<T> : ReadRepositoryBase<T, IEnumerable<Guid>>
        where T : class, new()
    {
        /// <summary>
        /// Constructs a base class for Instance (read-only) Repositories 
        /// that use an IIdInstanceNeuronsRetriever.
        /// </summary>
        /// <param name="networkRepository"></param>
        /// <param name="mirrorRepository"></param>
        /// <param name="neurULizer"></param>
        /// <param name="grannyService"></param>
        /// <param name="readWriteCache"></param>
        /// <param name="idInstanceNeuronsRetriever"></param>
        public IdReadRepositoryBase(
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

        protected override Task<IEnumerable<T>> GetByIdsCore(
            IEnumerable<Guid> ids, 
            Func<Guid, NeuronQuery> neuronQueryCreator, 
            bool restrictQueryResultCount = true, 
            CancellationToken token = default
        )
        {
            ((IIdInstanceNeuronsRetriever)this.instanceNeuronsRetriever).Initialize(ids);

            return base.GetByIdsCore(ids, neuronQueryCreator, restrictQueryResultCount, token);
        }
    }
}
