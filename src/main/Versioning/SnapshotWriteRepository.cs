using ei8.Cortex.Coding.Persistence.Versioning;
using System.Threading.Tasks;
using System.Threading;
using ei8.Cortex.Coding.Persistence;
using ei8.EventSourcing.Client;
using neurUL.Common.Domain.Model;
using ei8.Cortex.Coding.Versioning;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence.Versioning
{
    /// <summary>
    /// Represents a Snapshot (write-only) repository.
    /// </summary>
    public class SnapshotWriteRepository : 
        WriteRepositoryBase<Snapshot>,
        ISnapshotWriteRepository
    {
        /// <summary>
        /// Constructs a Snapshot (write-only) repository.
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="networkTransactionService"></param>
        /// <param name="neurULizer"></param>
        public SnapshotWriteRepository(
            ITransaction transaction,
            INetworkTransactionService networkTransactionService,
            IneurULizer neurULizer
        ) : base(
            transaction,
            networkTransactionService,
            neurULizer
        )
        {
        }
    }
}
