using ei8.Cortex.Coding.d23.neurULization.Processors.Readers.Inductive;
using ei8.Cortex.Coding.d23.neurULization.Processors.Writers;
using ei8.Cortex.Coding.Persistence;
using neurUL.Common.Domain.Model;
using System.Collections.Generic;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    public class neurULizerOptions : Id23neurULizerOptions
    {
        public neurULizerOptions(
            IExternalReferenceRepository externalReferenceRepository,
            INetworkRepository networkRepository, 
            Coding.d23.neurULization.Processors.Writers.IInstanceWriter instanceWriter,
            Coding.d23.neurULization.Processors.Readers.Inductive.IInstanceReader inductiveInstanceReader,
            Coding.d23.neurULization.Processors.Writers.IIdInstanceValueWriter idInstanceValueWriter,
            Coding.d23.neurULization.Processors.Readers.Inductive.IInstanceValueReader inductiveInstanceValueReader,
            IExternalReferenceSet externalReferences, 
            IDictionary<string, Network> networkCache,
            IGrannyService grannyService,
            INetworkTransactionData transactionData
        )
        {
            AssertionConcern.AssertArgumentNotNull(externalReferenceRepository, nameof(externalReferenceRepository));
            AssertionConcern.AssertArgumentNotNull(networkRepository, nameof(networkRepository));
            AssertionConcern.AssertArgumentNotNull(instanceWriter, nameof(instanceWriter));
            AssertionConcern.AssertArgumentNotNull(inductiveInstanceReader, nameof(inductiveInstanceReader));
            AssertionConcern.AssertArgumentNotNull(idInstanceValueWriter, nameof(idInstanceValueWriter));
            AssertionConcern.AssertArgumentNotNull(inductiveInstanceValueReader, nameof(inductiveInstanceValueReader));
            AssertionConcern.AssertArgumentNotNull(externalReferences, nameof(externalReferences));
            AssertionConcern.AssertArgumentNotNull(networkCache, nameof(networkCache));
            AssertionConcern.AssertArgumentNotNull(grannyService, nameof(grannyService));
            AssertionConcern.AssertArgumentNotNull(transactionData, nameof(transactionData));

            this.ExternalReferenceRepository = externalReferenceRepository;
            this.NetworkRepository = networkRepository;
            this.InstanceWriter = instanceWriter;
            this.InductiveInstanceReader = inductiveInstanceReader;
            this.IdInstanceValueWriter = idInstanceValueWriter;
            this.InductiveInstanceValueReader = inductiveInstanceValueReader;
            this.ExternalReferences = externalReferences;
            this.NetworkCache = networkCache;
            this.GrannyService = grannyService;
            this.TransactionData = transactionData;
        }

        public IExternalReferenceRepository ExternalReferenceRepository { get; }
        public INetworkRepository NetworkRepository { get; }
        public IInstanceWriter InstanceWriter { get; }
        public IInstanceReader InductiveInstanceReader { get; }
        public IIdInstanceValueWriter IdInstanceValueWriter { get; }
        public IInstanceValueReader InductiveInstanceValueReader { get; }
        public IExternalReferenceSet ExternalReferences { get; }
        public IDictionary<string, Network> NetworkCache { get; }
        public IGrannyService GrannyService { get; }
        public INetworkTransactionData TransactionData { get; }
    }
}
