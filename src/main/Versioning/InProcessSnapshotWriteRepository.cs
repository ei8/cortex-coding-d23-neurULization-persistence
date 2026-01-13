using ei8.Cortex.Coding.Mirrors;
using ei8.Cortex.Coding.Model.Versioning;
using ei8.Cortex.Coding.Persistence;
using ei8.Cortex.Coding.Persistence.Versioning;
using ei8.EventSourcing.Client;
using System;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence.Versioning
{
    /// <summary>
    /// Represents an In-process Snapshot (write-only) Repository.
    /// </summary>
    public class InProcessSnapshotWriteRepository : InProcessWriteRepositoryBase<Snapshot>, ISnapshotWriteRepository
    {
        /// <summary>
        /// Constructs an In-process Snapshot (write-only) Repository.
        /// </summary>
        /// <param name="mirrorRepository"></param>
        /// <param name="transaction"></param>
        /// <param name="networkTransactionData"></param>
        /// <param name="networkTransactionService"></param>
        /// <param name="neurULizer"></param>
        public InProcessSnapshotWriteRepository(
            IMirrorRepository mirrorRepository,
            ITransaction transaction,
            INetworkTransactionData networkTransactionData,
            INetworkTransactionService networkTransactionService,
            IneurULizer neurULizer
        ) : base(
            mirrorRepository,
            transaction,
            networkTransactionData,
            networkTransactionService,
            neurULizer
        )
        {
        }

        protected override Guid GetId(Snapshot value) => value.Id;
    }
}
