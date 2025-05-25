using ei8.Cortex.Coding.Persistence;
using ei8.Cortex.Coding.Persistence.Wrappers;
using ei8.Cortex.Coding.Wrappers;
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
        private readonly INetworkTransactionService networkTransactionService;
        private readonly IneurULizer neurULizer;
        private readonly INetworkRepository networkRepository;
        private readonly IIdInstanceNeuronsRetriever idInstanceNeuronsRetriever;

        public StringWrapperRepository(
            ITransaction transaction,
            INetworkTransactionService networkTransactionService,
            IneurULizer neurULizer,
            INetworkRepository networkRepository,
            IIdInstanceNeuronsRetriever idInstanceNeuronsRetriever
        )
        {
            AssertionConcern.AssertArgumentNotNull(transaction, nameof(transaction));
            AssertionConcern.AssertArgumentNotNull(networkTransactionService, nameof(networkTransactionService));
            AssertionConcern.AssertArgumentNotNull(neurULizer, nameof(neurULizer));
            AssertionConcern.AssertArgumentNotNull(networkRepository, nameof(networkRepository));
            AssertionConcern.AssertArgumentNotNull(idInstanceNeuronsRetriever, nameof(idInstanceNeuronsRetriever));

            this.transaction = transaction;
            this.networkTransactionService = networkTransactionService;
            this.neurULizer = neurULizer;
            this.networkRepository = networkRepository;
            this.idInstanceNeuronsRetriever = idInstanceNeuronsRetriever;
        }

        public async Task<IEnumerable<StringWrapper>> GetByIds(IEnumerable<Guid> ids, CancellationToken token = default)
        {
            AssertionConcern.AssertArgumentNotNull(ids, nameof(ids));
            AssertionConcern.AssertArgumentValid(i => i.Any(), ids, $"Specified value cannot be an empty array.", nameof(ids));

            var queryResult = await this.networkRepository.GetByQueryAsync(
                new NeuronQuery()
                {
                    Id = ids.Select(i => i.ToString()),
                    Depth = Coding.d23.neurULization.Constants.ValueToInstantiatesClassDepth,
                    DirectionValues = DirectionValues.Outbound
                }
            );

            queryResult.Network.ValidateIds(ids);

            this.idInstanceNeuronsRetriever.Initialize(ids);
            return (await this.neurULizer.DeneurULizeAsync<StringWrapper>(
                queryResult.Network, 
                this.idInstanceNeuronsRetriever,
                token
            )).Select(nr => nr.Result);
        }

        public async Task Save(StringWrapper stringValue, CancellationToken token = default)
        {
            // TODO: handle updates - message.Version == 0 ? WriteMode.Create : WriteMode.Update
            var me = await this.neurULizer.neurULizeAsync(
                stringValue,
                token
            );

            await this.networkTransactionService.SaveAsync(this.transaction, me);
        }
    }
}
