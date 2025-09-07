using ei8.Cortex.Coding.Persistence;
using ei8.Cortex.Coding.Persistence.Versioning;
using ei8.Cortex.Coding.Versioning;
using ei8.EventSourcing.Client;
using System;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence.Versioning
{
    public class InProcessCreationWriteRepository : InProcessWriteRepositoryBase<Creation>, ICreationWriteRepository
    {
        public InProcessCreationWriteRepository(
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

        protected override Guid GetId(Creation value) => value.Id;
    }
}
