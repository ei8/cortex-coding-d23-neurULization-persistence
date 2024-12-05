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
            IEnsembleRepository ensembleRepository, 
            Coding.d23.neurULization.Processors.Writers.IInstanceWriter instanceWriter,
            Coding.d23.neurULization.Processors.Readers.Inductive.IInstanceReader inductiveInstanceReader,
            IExternalReferenceSet externalReferences, 
            IDictionary<string, Ensemble> ensembleCache,
            IGrannyService grannyService,
            IEnsembleTransactionData transactionData
        )
        {
            AssertionConcern.AssertArgumentNotNull(externalReferenceRepository, nameof(externalReferenceRepository));
            AssertionConcern.AssertArgumentNotNull(ensembleRepository, nameof(ensembleRepository));
            AssertionConcern.AssertArgumentNotNull(instanceWriter, nameof(instanceWriter));
            AssertionConcern.AssertArgumentNotNull(inductiveInstanceReader, nameof(inductiveInstanceReader));
            AssertionConcern.AssertArgumentNotNull(externalReferences, nameof(externalReferences));
            AssertionConcern.AssertArgumentNotNull(ensembleCache, nameof(ensembleCache));
            AssertionConcern.AssertArgumentNotNull(grannyService, nameof(grannyService));
            AssertionConcern.AssertArgumentNotNull(transactionData, nameof(transactionData));

            this.ExternalReferenceRepository = externalReferenceRepository;
            this.EnsembleRepository = ensembleRepository;
            this.InstanceWriter = instanceWriter;
            this.InductiveInstanceReader = inductiveInstanceReader;
            this.ExternalReferences = externalReferences;
            this.EnsembleCache = ensembleCache;
            this.GrannyService = grannyService;
            this.TransactionData = transactionData;
        }

        public IExternalReferenceRepository ExternalReferenceRepository { get; }
        public IEnsembleRepository EnsembleRepository { get; }
        public IInstanceWriter InstanceWriter { get; }
        public IInstanceReader InductiveInstanceReader { get; }
        public IExternalReferenceSet ExternalReferences { get; }
        public IDictionary<string, Ensemble> EnsembleCache { get; }
        public IGrannyService GrannyService { get; }
        public IEnsembleTransactionData TransactionData { get; }
    }
}
