using ei8.Cortex.Coding.d23.neurULization.Processors.Writers;
using neurUL.Common.Domain.Model;
using System.Collections.Generic;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    public class neurULizerOptions : IneurULizerOptions
    {
        private readonly IEnsembleRepository ensembleRepository;
        private readonly IInstanceProcessor writersInstanceProcessor;
        private readonly Processors.Readers.Inductive.IInstanceProcessor readersInductiveInstanceProcessor;
        private readonly IPrimitiveSet primitives;
        private readonly IDictionary<string, Ensemble> ensembleCache;
        private readonly IGrannyService grannyService;
        private readonly string appUserId;
        private readonly string cortexLibraryOutBaseUrl;
        private readonly string identityAccessOutBaseUrl;
        private readonly int queryResultLimit;

        public neurULizerOptions(
            IEnsembleRepository ensembleRepository, 
            Coding.d23.neurULization.Processors.Writers.IInstanceProcessor writersInstanceProcessor,
            Coding.d23.neurULization.Processors.Readers.Inductive.IInstanceProcessor readersInductiveInstanceProcessor,
            IPrimitiveSet primitives, 
            IDictionary<string, Ensemble> ensembleCache,
            IGrannyService grannyService,
            string appUserId,
            string cortexLibraryOutBaseUrl,
            string identityAccessOutBaseUrl,
            int queryResultLimit
        )
        {
            AssertionConcern.AssertArgumentNotNull(ensembleRepository, nameof(ensembleRepository));
            AssertionConcern.AssertArgumentNotNull(writersInstanceProcessor, nameof(writersInstanceProcessor));
            AssertionConcern.AssertArgumentNotNull(readersInductiveInstanceProcessor, nameof(readersInductiveInstanceProcessor));
            AssertionConcern.AssertArgumentNotNull(primitives, nameof(primitives));
            AssertionConcern.AssertArgumentNotNull(ensembleCache, nameof(ensembleCache));
            AssertionConcern.AssertArgumentNotNull(grannyService, nameof(grannyService));
            string emptyMessage = "Specified value cannot be null or empty.";
            AssertionConcern.AssertArgumentNotEmpty(appUserId, emptyMessage, nameof(appUserId));
            AssertionConcern.AssertArgumentNotEmpty(cortexLibraryOutBaseUrl, emptyMessage, nameof(cortexLibraryOutBaseUrl));
            AssertionConcern.AssertArgumentNotEmpty(identityAccessOutBaseUrl, emptyMessage, nameof(identityAccessOutBaseUrl));
            AssertionConcern.AssertArgumentRange(queryResultLimit, 0, int.MaxValue, nameof(queryResultLimit));

            this.ensembleRepository = ensembleRepository;
            this.writersInstanceProcessor = writersInstanceProcessor;
            this.readersInductiveInstanceProcessor = readersInductiveInstanceProcessor;
            this.primitives = primitives;
            this.ensembleCache = ensembleCache;
            this.grannyService = grannyService;
            this.appUserId = appUserId;
            this.cortexLibraryOutBaseUrl = cortexLibraryOutBaseUrl;
            this.identityAccessOutBaseUrl = identityAccessOutBaseUrl;
            this.queryResultLimit = queryResultLimit;
        }

        public IEnsembleRepository EnsembleRepository => this.ensembleRepository;

        public IInstanceProcessor WritersInstanceProcessor => this.writersInstanceProcessor;

        public Processors.Readers.Inductive.IInstanceProcessor ReadersInductiveInstanceProcessor => this.readersInductiveInstanceProcessor;

        public IPrimitiveSet Primitives => this.primitives;

        public IDictionary<string, Ensemble> EnsembleCache => this.ensembleCache;

        public IGrannyService GrannyService => this.grannyService;

        public string AppUserId => this.appUserId;

        public string CortexLibraryOutBaseUrl => this.cortexLibraryOutBaseUrl;

        public string IdentityAccessOutBaseUrl => this.identityAccessOutBaseUrl;

        public int QueryResultLimit => this.queryResultLimit;
    }
}
