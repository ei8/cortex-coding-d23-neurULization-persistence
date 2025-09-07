using ei8.Cortex.Coding.Persistence;
using ei8.Cortex.Coding.Reflection;
using ei8.EventSourcing.Client;
using neurUL.Common.Domain.Model;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    public abstract class InProcessWriteRepositoryBase<T> where T : class
    {
        private readonly IMirrorRepository mirrorRepository;
        private readonly ITransaction transaction;
        private readonly INetworkTransactionData networkTransactionData;
        private readonly INetworkTransactionService networkTransactionService;
        private readonly IneurULizer neurULizer;

        public InProcessWriteRepositoryBase(
            IMirrorRepository mirrorRepository,
            ITransaction transaction,
            INetworkTransactionData networkTransactionData,
            INetworkTransactionService networkTransactionService,
            IneurULizer neurULizer
        )
        {
            AssertionConcern.AssertArgumentNotNull(mirrorRepository, nameof(mirrorRepository));
            AssertionConcern.AssertArgumentNotNull(transaction, nameof(transaction));
            AssertionConcern.AssertArgumentNotNull(networkTransactionData, nameof(networkTransactionData));
            AssertionConcern.AssertArgumentNotNull(networkTransactionService, nameof(networkTransactionService));
            AssertionConcern.AssertArgumentNotNull(neurULizer, nameof(neurULizer));

            this.mirrorRepository = mirrorRepository;
            this.transaction = transaction;
            this.networkTransactionData = networkTransactionData;
            this.networkTransactionService = networkTransactionService;
            this.neurULizer = neurULizer;
        }

        public async Task Save(T value, CancellationToken token = default)
        {
            // In Process version of neurULizeAsync
            var typeInfo = neurULizerTypeInfo.GetTypeInfo(value);

            // use key to retrieve external reference url from library
            var mirrors = await mirrorRepository.GetByKeysAsync(
                typeInfo.Keys.Except(new[] { string.Empty }).ToArray()
            );

            var valueNeuronIds = typeInfo.GrannyProperties
                .Where(gp => gp.ValueMatchBy == ValueMatchBy.Id)
                .Select(gp => gp.Value);

            var idPropertyValueNeurons = networkTransactionData.SavedTransientNeurons;

            var nn = neurULizer.neurULize(
                value,
                idPropertyValueNeurons,
                typeInfo,
                mirrors
            );

            var cachedNeuron = networkTransactionData.SavedTransientNeurons.SingleOrDefault(n => n.Id == this.GetId(value));
            if (nn.TryGetById(this.GetId(value), out Neuron result) &&
                cachedNeuron != null)
            {
                Coding.Persistence.NetworkExtensions.ReplaceWithIdentical(
                    nn,
                    networkTransactionData,
                    result.Id,
                    cachedNeuron
                );
            }

            await nn.UniquifyAsync(networkTransactionData);

            await networkTransactionService.SaveAsync(transaction, nn);
        }

        protected abstract Guid GetId(T value);
    }
}
