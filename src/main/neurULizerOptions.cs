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
            IEnsembleRepository ensembleRepository, 
            Coding.d23.neurULization.Processors.Writers.IInstanceWriter instanceWriter,
            Coding.d23.neurULization.Processors.Readers.Inductive.IInstanceReader inductiveInstanceReader,
            IPrimitiveSet primitives, 
            IDictionary<string, Ensemble> ensembleCache,
            IGrannyService grannyService,
            IEnsembleTransactionData transactionData
        )
        {
            AssertionConcern.AssertArgumentNotNull(ensembleRepository, nameof(ensembleRepository));
            AssertionConcern.AssertArgumentNotNull(instanceWriter, nameof(instanceWriter));
            AssertionConcern.AssertArgumentNotNull(inductiveInstanceReader, nameof(inductiveInstanceReader));
            AssertionConcern.AssertArgumentNotNull(primitives, nameof(primitives));
            AssertionConcern.AssertArgumentNotNull(ensembleCache, nameof(ensembleCache));
            AssertionConcern.AssertArgumentNotNull(grannyService, nameof(grannyService));
            AssertionConcern.AssertArgumentNotNull(transactionData, nameof(transactionData));

            this.EnsembleRepository = ensembleRepository;
            this.InstanceWriter = instanceWriter;
            this.InductiveInstanceReader = inductiveInstanceReader;
            this.Primitives = primitives;
            this.EnsembleCache = ensembleCache;
            this.GrannyService = grannyService;
            this.TransactionData = transactionData;
        }

        public IEnsembleRepository EnsembleRepository { get; }
        public IInstanceWriter InstanceWriter { get; }
        public IInstanceReader InductiveInstanceReader { get; }
        public IPrimitiveSet Primitives { get; }
        public IDictionary<string, Ensemble> EnsembleCache { get; }
        public IGrannyService GrannyService { get; }
        public IEnsembleTransactionData TransactionData { get; }
    }
}
