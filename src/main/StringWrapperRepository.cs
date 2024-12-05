using ei8.Cortex.Coding.Persistence;
using ei8.Cortex.Coding.Persistence.Wrappers;
using ei8.Cortex.Library.Common;
using ei8.EventSourcing.Client;
using neurUL.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    public class StringWrapperRepository : IStringWrapperRepository
    {
        private readonly ITransaction transaction;
        private readonly IEnsembleTransactionService ensembleTransactionService;
        private readonly IneurULizer neurULizer;
        private readonly IExternalReferenceRepository externalReferenceRepository;
        private readonly IEnsembleRepository ensembleRepository;
        private readonly IGrannyService grannyService;

        public StringWrapperRepository(
            ITransaction transaction,
            IEnsembleTransactionService ensembleTransactionService,
            IneurULizer neurULizer,
            IExternalReferenceRepository externalReferenceRepository,
            IEnsembleRepository ensembleRepository,
            IGrannyService grannyService
        )
        {
            AssertionConcern.AssertArgumentNotNull(transaction, nameof(transaction));
            AssertionConcern.AssertArgumentNotNull(ensembleTransactionService, nameof(ensembleTransactionService));
            AssertionConcern.AssertArgumentNotNull(neurULizer, nameof(neurULizer));
            AssertionConcern.AssertArgumentNotNull(externalReferenceRepository, nameof(externalReferenceRepository));
            AssertionConcern.AssertArgumentNotNull(ensembleRepository, nameof(ensembleRepository));
            AssertionConcern.AssertArgumentNotNull(grannyService, nameof(grannyService));

            this.transaction = transaction;
            this.ensembleTransactionService = ensembleTransactionService;
            this.neurULizer = neurULizer;
            this.externalReferenceRepository = externalReferenceRepository;
            this.ensembleRepository = ensembleRepository;
            this.grannyService = grannyService;
        }

        public async Task<IEnumerable<StringWrapper>> GetByIds(IEnumerable<Guid> ids, string userId, CancellationToken token = default)
        {
            var neurons = new QueryResult<Library.Common.Neuron>();

            var instantiatesStringResult = await this.grannyService.TryGetParseBuildPersistAsync(
                new InstantiatesClassGrannyInfo(
                    new Coding.d23.neurULization.Processors.Readers.Deductive.InstantiatesClassParameterSet(
                        await this.externalReferenceRepository.GetByKeyAsync(
                            typeof(string)
                        )
                    )
                ),
                token
            );

            AssertionConcern.AssertStateTrue(
                instantiatesStringResult.Item1,
                $"'Instantiates^String' is required to invoke {nameof(StringWrapperRepository.GetByIds)}"
            );

            var queryResult = await ensembleRepository.GetByQueryAsync(
                new NeuronQuery()
                {
                    Id = ids.Select(i => i.ToString()),
                    Postsynaptic = new string[] {
                        instantiatesStringResult.Item2.Neuron.Id.ToString()
                    },
                    SortBy = SortByValue.NeuronCreationTimestamp,
                    SortOrder = SortOrderValue.Descending,
                    Depth = Constants.InstanceToValueInstantiatesClassDepth,
                    DirectionValues = DirectionValues.Outbound
                },
                userId
            );

            return await this.neurULizer.DeneurULizeAsync<StringWrapper>(queryResult.Ensemble);
        }

        public async Task Save(StringWrapper stringValue, CancellationToken token = default)
        {
            // TODO: handle updates - message.Version == 0 ? WriteMode.Create : WriteMode.Update
            var me = await this.neurULizer.neurULizeAsync(stringValue);

            await this.ensembleTransactionService.SaveAsync(this.transaction, me);
        }
    }
}
