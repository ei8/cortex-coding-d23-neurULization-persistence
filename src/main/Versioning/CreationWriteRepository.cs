using ei8.Cortex.Coding.Model.Versioning;
using ei8.Cortex.Coding.Persistence;
using ei8.Cortex.Coding.Persistence.Versioning;
using ei8.EventSourcing.Client;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence.Versioning
{
    /// <summary>
    /// Represents a Creation (write-only) Repository.
    /// </summary>
    public class CreationWriteRepository : 
        WriteRepositoryBase<Creation>, 
        ICreationWriteRepository
    {
        /// <summary>
        /// Constructs a Creation (write-only) Repository.
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="networkTransactionService"></param>
        /// <param name="neurULizer"></param>
        public CreationWriteRepository(
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
