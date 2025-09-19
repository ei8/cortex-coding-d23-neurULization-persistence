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
    /// <summary>
    /// Represents a StringWrapper repository.
    /// </summary>
    public class StringWrapperRepository : IStringWrapperRepository
    {
        private readonly ITransaction transaction;
        private readonly INetworkTransactionService networkTransactionService;
        private readonly IneurULizer neurULizer;
        private readonly INetworkRepository networkRepository;
        private readonly IIdInstanceNeuronsRetriever idInstanceNeuronsRetriever;
        private readonly INetworkDictionary<CacheKey> readWriteCache;

        /// <summary>
        /// Constructs a StringWrapper repository.
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="networkTransactionService"></param>
        /// <param name="neurULizer"></param>
        /// <param name="networkRepository"></param>
        /// <param name="idInstanceNeuronsRetriever"></param>
        /// <param name="readWriteCache"></param>
        public StringWrapperRepository(
            ITransaction transaction,
            INetworkTransactionService networkTransactionService,
            IneurULizer neurULizer,
            INetworkRepository networkRepository,
            IIdInstanceNeuronsRetriever idInstanceNeuronsRetriever,
            INetworkDictionary<CacheKey> readWriteCache
        )
        {
            AssertionConcern.AssertArgumentNotNull(transaction, nameof(transaction));
            AssertionConcern.AssertArgumentNotNull(networkTransactionService, nameof(networkTransactionService));
            AssertionConcern.AssertArgumentNotNull(neurULizer, nameof(neurULizer));
            AssertionConcern.AssertArgumentNotNull(networkRepository, nameof(networkRepository));
            AssertionConcern.AssertArgumentNotNull(idInstanceNeuronsRetriever, nameof(idInstanceNeuronsRetriever));
            AssertionConcern.AssertArgumentNotNull(readWriteCache, nameof(readWriteCache));

            this.transaction = transaction;
            this.networkTransactionService = networkTransactionService;
            this.neurULizer = neurULizer;
            this.networkRepository = networkRepository;
            this.idInstanceNeuronsRetriever = idInstanceNeuronsRetriever;
            this.readWriteCache = readWriteCache;
        }

        /// <summary>
        /// Gets StringWrappers using the specified IDs.
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="token"></param>
        /// <returns></returns>
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
            return await this.neurULizer.DeneurULizeCacheAsync<StringWrapper>(
                queryResult.Network,
                this.idInstanceNeuronsRetriever,
                this.readWriteCache[CacheKey.Read],
                token
            );
        }

        /// <summary>
        /// Saves the specified StringWrapper.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task Save(StringWrapper value, CancellationToken token = default)
        {
            var me = await this.neurULizer.neurULizeAsync(
                value,
                token
            );

            await this.networkTransactionService.SaveAsync(this.transaction, me);
        }
    }
}
