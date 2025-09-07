using ei8.Cortex.Coding.Persistence;
using ei8.Cortex.Coding.Persistence.Versioning;
using ei8.Cortex.Coding.Reflection;
using ei8.Cortex.Coding.Versioning;
using ei8.EventSourcing.Client;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence.Versioning
{
    public class InProcessSnapshotWriteRepository : InProcessWriteRepositoryBase<Snapshot>, ISnapshotWriteRepository
    {
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
