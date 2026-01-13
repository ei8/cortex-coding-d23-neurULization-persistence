using ei8.Cortex.Coding.d23.Grannies;
using ei8.Cortex.Coding.Mirrors;
using ei8.Cortex.Coding.Model.Versioning;
using ei8.Cortex.Coding.Persistence.Versioning;
using neurUL.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence.Versioning
{
    /// <summary>
    /// Represents a base class for an Operation (read-only) Repository.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OperationReadRepositoryBase<T> :
        ClassReadRepositoryBase<Creation>,
        ICreationReadRepository
    {
        private readonly IDictionary<string, IGranny> propertyAssociationCache;

        /// <summary>
        /// Constructs a base class for an Operation (read-only) Repository.
        /// </summary>
        /// <param name="networkRepository"></param>
        /// <param name="mirrorRepository"></param>
        /// <param name="neurULizer"></param>
        /// <param name="grannyService"></param>
        /// <param name="readWriteCache"></param>
        /// <param name="propertyAssociationCache"></param>
        /// <param name="classInstanceNeuronsRetriever"></param>
        public OperationReadRepositoryBase(
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
            classInstanceNeuronsRetriever
        )
        {
            AssertionConcern.AssertArgumentNotNull(propertyAssociationCache, nameof(propertyAssociationCache));

            this.propertyAssociationCache = propertyAssociationCache;
        }

        /// <summary>
        /// Gets Creations using the specified Subject Id.
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Creation>> GetBySubjectId(
            Guid subjectId,
            CancellationToken token = default
        ) => await base.GetByPropertyAssociationValueIdsCore(
            new[] { subjectId },
            this.propertyAssociationCache,
            nameof(Creation.SubjectId),
            nameof(CreationReadRepository.GetBySubjectId),
            false,
            token
        );
    }
}
