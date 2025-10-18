using ei8.Cortex.Coding.d23.Grannies;
using ei8.Cortex.Coding.Persistence.Versioning;
using ei8.Cortex.Coding.Versioning;
using System.Collections.Generic;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence.Versioning
{
    /// <summary>
    /// Represents a Creation (read-only) Repository.
    /// </summary>
    public class CreationReadRepository : 
        OperationReadRepositoryBase<Creation>,
        ICreationReadRepository
    {
        /// <summary>
        /// Constructs sssa Creation (read-only) Repository.
        /// </summary>
        /// <param name="networkRepository"></param>
        /// <param name="mirrorRepository"></param>
        /// <param name="neurULizer"></param>
        /// <param name="grannyService"></param>
        /// <param name="readWriteCache"></param>
        /// <param name="propertyAssociationCache"></param>
        /// <param name="classInstanceNeuronsRetriever"></param>s
        public CreationReadRepository(
            INetworkRepository networkRepository,
            IMirrorRepository mirrorRepository,
            IneurULizer neurULizer,
            IGrannyService grannyService,
            INetworkDictionary<CacheKey> readWriteCache,
            IDictionary<string, IGranny> propertyAssociationCache,
            IClassInstanceNeuronsRetriever classInstanceNeuronsRetriever
        ) : base(
            networkRepository,
            mirrorRepository,
            neurULizer,
            grannyService,
            readWriteCache,
            propertyAssociationCache,
            classInstanceNeuronsRetriever
        )
        {
        }
    }
}
