using ei8.Cortex.Coding.Persistence.Versioning;
using System.Threading.Tasks;
using System.Threading;
using ei8.Cortex.Coding.Persistence;
using ei8.EventSourcing.Client;
using neurUL.Common.Domain.Model;
using ei8.Cortex.Coding.Versioning;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence.Versioning
{
    public class SnapshotWriteRepository : ISnapshotWriteRepository
    {
        private readonly ITransaction transaction;
        private readonly INetworkTransactionService networkTransactionService;
        private readonly IneurULizer neurULizer;

        public SnapshotWriteRepository(
            ITransaction transaction,
            INetworkTransactionService networkTransactionService,
            IneurULizer neurULizer
        )
        {
            AssertionConcern.AssertArgumentNotNull(transaction, nameof(transaction));
            AssertionConcern.AssertArgumentNotNull(networkTransactionService, nameof(networkTransactionService));
            AssertionConcern.AssertArgumentNotNull(neurULizer, nameof(neurULizer));

            this.transaction = transaction;
            this.networkTransactionService = networkTransactionService;
            this.neurULizer = neurULizer;
        }

        public async Task Save(Snapshot value, CancellationToken token = default)
        {
            var me = await neurULizer.neurULizeAsync(
                value,
                token
            );

            await networkTransactionService.SaveAsync(transaction, me);
        }
    }
}
