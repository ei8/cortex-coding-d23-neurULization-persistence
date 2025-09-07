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
    public class CreationReadRepository : ICreationReadRepository
    {
        private readonly IneurULizer neurULizer;
        private readonly INetworkRepository networkRepository;
        private readonly IMirrorRepository mirrorRepository;
        private readonly IGrannyService grannyService;
        private readonly IDictionary<string, IGranny> propertyAssociationCache;
        private readonly IClassInstanceNeuronsRetriever classInstanceNeuronsRetriever;
        private readonly Network readNetworkCache;

        public CreationReadRepository(
            IneurULizer neurULizer,
            INetworkRepository networkRepository,
            IMirrorRepository mirrorRepository,
            IGrannyService grannyService,
            IDictionary<string, IGranny> propertyAssociationCache,
            IClassInstanceNeuronsRetriever classInstanceNeuronsRetriever,
            Network readNetworkCache
        )
        {
            AssertionConcern.AssertArgumentNotNull(neurULizer, nameof(neurULizer));
            AssertionConcern.AssertArgumentNotNull(networkRepository, nameof(networkRepository));
            AssertionConcern.AssertArgumentNotNull(mirrorRepository, nameof(mirrorRepository));
            AssertionConcern.AssertArgumentNotNull(grannyService, nameof(grannyService));
            AssertionConcern.AssertArgumentNotNull(propertyAssociationCache, nameof(propertyAssociationCache));
            AssertionConcern.AssertArgumentNotNull(classInstanceNeuronsRetriever, nameof(classInstanceNeuronsRetriever));
            AssertionConcern.AssertArgumentNotNull(readNetworkCache, nameof(readNetworkCache));

            this.neurULizer = neurULizer;
            this.networkRepository = networkRepository;
            this.mirrorRepository = mirrorRepository;
            this.grannyService = grannyService;
            this.propertyAssociationCache = propertyAssociationCache;
            this.classInstanceNeuronsRetriever = classInstanceNeuronsRetriever;
            this.readNetworkCache = readNetworkCache;
        }

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
                instantiatesCreationResult.Item1,
                $"'Instantiates^Creation' is required to invoke {nameof(GetBySubjectId)}"
            );

            var hasSubjectIdResult = await grannyService.TryGetPropertyValueAssociationFromCacheOrDb<Creation>(
                mirrorRepository,
                networkRepository,
                nameof(Creation.SubjectId),
                subjectId,
                propertyAssociationCache
            );

            if (hasSubjectIdResult.Success)
            {
                var queryResult = await networkRepository.GetByQueryAsync(
                    new NeuronQuery()
                    {
                        Postsynaptic = new[] {
                            instantiatesCreationResult.Item2.Neuron.Id.ToString(),
                            hasSubjectIdResult.Granny.Neuron.Id.ToString()
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
                var dnResult = await neurULizer.DeneurULizeAsync<Creation>(
                    queryResult.Network,
                    classInstanceNeuronsRetriever,
                    token
                );
                result = dnResult.Select(dm => dm.Result);
                dnResult
                    .Where(dnr => dnr.Success)
                    .ToList()
                    .ForEach(dnr => readNetworkCache.AddReplace(dnr.InstanceNeuron)
                    );
            }

            return result;
        }
    }
}
