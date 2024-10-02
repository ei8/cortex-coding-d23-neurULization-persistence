using ei8.Cortex.Coding.d23.neurULization.Processors.Writers;
using neurUL.Common.Domain.Model;
using System.Collections.Generic;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    public class neurULizerOptions : Id23neurULizerOptions
    {
        // TODO: see if possible to remove ensembleRepository, 
        // if so, neurULizer can be returned to d23.neurULization
        // and thus make it persistence unaware
        private readonly IEnsembleRepository ensembleRepository;
        private readonly IInstanceWriter instanceWriter;
        private readonly Processors.Readers.Inductive.IInstanceReader inductiveInstanceReader;
        private readonly IPrimitiveSet primitives;
        private readonly IDictionary<string, Ensemble> ensembleCache;
        private readonly IGrannyService grannyService;
        private readonly string appUserId;

        public neurULizerOptions(
            IEnsembleRepository ensembleRepository, 
            Coding.d23.neurULization.Processors.Writers.IInstanceWriter instanceWriter,
            Coding.d23.neurULization.Processors.Readers.Inductive.IInstanceReader inductiveInstanceReader,
            IPrimitiveSet primitives, 
            IDictionary<string, Ensemble> ensembleCache,
            IGrannyService grannyService,
            string appUserId
        )
        {
            AssertionConcern.AssertArgumentNotNull(ensembleRepository, nameof(ensembleRepository));
            AssertionConcern.AssertArgumentNotNull(instanceWriter, nameof(instanceWriter));
            AssertionConcern.AssertArgumentNotNull(inductiveInstanceReader, nameof(inductiveInstanceReader));
            AssertionConcern.AssertArgumentNotNull(primitives, nameof(primitives));
            AssertionConcern.AssertArgumentNotNull(ensembleCache, nameof(ensembleCache));
            AssertionConcern.AssertArgumentNotNull(grannyService, nameof(grannyService));
            string emptyMessage = "Specified value cannot be null or empty.";
            AssertionConcern.AssertArgumentNotEmpty(appUserId, emptyMessage, nameof(appUserId));

            this.ensembleRepository = ensembleRepository;
            this.instanceWriter = instanceWriter;
            this.inductiveInstanceReader = inductiveInstanceReader;
            this.primitives = primitives;
            this.ensembleCache = ensembleCache;
            this.grannyService = grannyService;
            this.appUserId = appUserId;
        }

        public IEnsembleRepository EnsembleRepository => this.ensembleRepository;

        public IInstanceWriter InstanceWriter => this.instanceWriter;

        public Processors.Readers.Inductive.IInstanceReader InductiveInstanceReader => this.inductiveInstanceReader;

        public IPrimitiveSet Primitives => this.primitives;

        public IDictionary<string, Ensemble> EnsembleCache => this.ensembleCache;

        public IGrannyService GrannyService => this.grannyService;

        public string AppUserId => this.appUserId;
    }
}
