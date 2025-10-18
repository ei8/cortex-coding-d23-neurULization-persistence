using ei8.Cortex.Coding.Persistence;
using ei8.Cortex.Coding.Persistence.Wrappers;
using ei8.Cortex.Coding.Wrappers;
using ei8.EventSourcing.Client;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    /// <summary>
    /// Represents a StringWrapper (write-only) repository.
    /// </summary>
    public class StringWrapperWriteRepository :
        WriteRepositoryBase<StringWrapper>,
        IStringWrapperWriteRepository
    {
        /// <summary>
        /// Constructs a StringWrapper (write-only) repository.
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="networkTransactionService"></param>
        /// <param name="neurULizer"></param>
        public StringWrapperWriteRepository(
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
