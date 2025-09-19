using ei8.Cortex.Coding.d23.Grannies;
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
    /// Represents a Creation (read-only) Repository.
    /// </summary>
    public class CreationReadRepository : ICreationReadRepository
    {
        private readonly IneurULizer neurULizer;
        private readonly INetworkRepository networkRepository;
        private readonly IMirrorRepository mirrorRepository;
        private readonly IGrannyService grannyService;
        private readonly IDictionary<string, IGranny> propertyAssociationCache;
        private readonly IClassInstanceNeuronsRetriever classInstanceNeuronsRetriever;
        private readonly INetworkDictionary<CacheKey> readWriteCache;

        /// <summary>
        /// Creates a Creation (read-only) Repository.
        /// </summary>
        /// <param name="neurULizer"></param>
        /// <param name="networkRepository"></param>
        /// <param name="mirrorRepository"></param>
        /// <param name="grannyService"></param>
        /// <param name="propertyAssociationCache"></param>
        /// <param name="classInstanceNeuronsRetriever"></param>
        /// <param name="readWriteCache"></param>
        public CreationReadRepository(
            IneurULizer neurULizer,
            INetworkRepository networkRepository,
            IMirrorRepository mirrorRepository,
            IGrannyService grannyService,
            IDictionary<string, IGranny> propertyAssociationCache,
            IClassInstanceNeuronsRetriever classInstanceNeuronsRetriever,
            INetworkDictionary<CacheKey> readWriteCache
        )
        {
            AssertionConcern.AssertArgumentNotNull(neurULizer, nameof(neurULizer));
            AssertionConcern.AssertArgumentNotNull(networkRepository, nameof(networkRepository));
            AssertionConcern.AssertArgumentNotNull(mirrorRepository, nameof(mirrorRepository));
            AssertionConcern.AssertArgumentNotNull(grannyService, nameof(grannyService));
            AssertionConcern.AssertArgumentNotNull(propertyAssociationCache, nameof(propertyAssociationCache));
            AssertionConcern.AssertArgumentNotNull(classInstanceNeuronsRetriever, nameof(classInstanceNeuronsRetriever));
            AssertionConcern.AssertArgumentNotNull(readWriteCache, nameof(readWriteCache));

            this.neurULizer = neurULizer;
            this.networkRepository = networkRepository;
            this.mirrorRepository = mirrorRepository;
            this.grannyService = grannyService;
            this.propertyAssociationCache = propertyAssociationCache;
            this.classInstanceNeuronsRetriever = classInstanceNeuronsRetriever;
            this.readWriteCache=readWriteCache;
        }

        /// <summary>
        /// Gets Creations using the specified Subject Id.
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Creation>> GetBySubjectId(Guid subjectId, CancellationToken token = default)
        {
            var result = Enumerable.Empty<Creation>();

            var instantiatesCreationResult = await grannyService.TryGetParseBuildPersistAsync(
                new InstantiatesClassGrannyInfo(
                    new Processors.Readers.Deductive.InstantiatesClassParameterSet(
                        await mirrorRepository.GetByKeyAsync(
                            typeof(Creation)
                        )
                    )
                ),
                token
            );

            AssertionConcern.AssertStateTrue(
                instantiatesCreationResult.Success,
                $"'Instantiates^Creation' is required to invoke {nameof(GetBySubjectId)}"
            );

            var hasSubjectIdResult = await grannyService.TryObtainPropertyValueAssociations<Creation>(
                mirrorRepository,
                networkRepository,
                nameof(Creation.SubjectId),
                new Guid[] { subjectId },
                propertyAssociationCache
            );

            if (hasSubjectIdResult.Single().Success)
            {
                var queryResult = await networkRepository.GetByQueryAsync(
                    new NeuronQuery()
                    {
                        Postsynaptic = new[] {
                            instantiatesCreationResult.Granny.Neuron.Id.ToString(),
                            hasSubjectIdResult.Single().Granny.Neuron.Id.ToString()
                        },
                        SortBy = SortByValue.NeuronCreationTimestamp,
                        SortOrder = SortOrderValue.Descending,
                        Depth = Constants.InstanceToValueInstantiatesClassDepth,
                        DirectionValues = DirectionValues.Outbound
                    },
                    false
                );

                classInstanceNeuronsRetriever.Initialize(
                    await mirrorRepository.GetByKeyAsync(
                        typeof(Creation)
                    )
                );

                result = await this.neurULizer.DeneurULizeCacheAsync<Creation>(
                    queryResult.Network,
                    this.classInstanceNeuronsRetriever,
                    this.readWriteCache[CacheKey.Read],
                    token
                );
            }

            return result;
        }
    }
}
