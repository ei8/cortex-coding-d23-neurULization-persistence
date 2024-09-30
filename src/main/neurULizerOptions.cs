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
        private readonly string cortexLibraryOutBaseUrl;
        private readonly string identityAccessOutBaseUrl;
        private readonly int queryResultLimit;

        public neurULizerOptions(
            IEnsembleRepository ensembleRepository, 
            Coding.d23.neurULization.Processors.Writers.IInstanceWriter instanceWriter,
            Coding.d23.neurULization.Processors.Readers.Inductive.IInstanceReader inductiveInstanceReader,
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
            AssertionConcern.AssertArgumentNotNull(instanceWriter, nameof(instanceWriter));
            AssertionConcern.AssertArgumentNotNull(inductiveInstanceReader, nameof(inductiveInstanceReader));
            AssertionConcern.AssertArgumentNotNull(primitives, nameof(primitives));
            AssertionConcern.AssertArgumentNotNull(ensembleCache, nameof(ensembleCache));
            AssertionConcern.AssertArgumentNotNull(grannyService, nameof(grannyService));
            string emptyMessage = "Specified value cannot be null or empty.";
            AssertionConcern.AssertArgumentNotEmpty(appUserId, emptyMessage, nameof(appUserId));
            AssertionConcern.AssertArgumentNotEmpty(cortexLibraryOutBaseUrl, emptyMessage, nameof(cortexLibraryOutBaseUrl));
            AssertionConcern.AssertArgumentNotEmpty(identityAccessOutBaseUrl, emptyMessage, nameof(identityAccessOutBaseUrl));
            AssertionConcern.AssertArgumentRange(queryResultLimit, 0, int.MaxValue, nameof(queryResultLimit));

            this.ensembleRepository = ensembleRepository;
            this.instanceWriter = instanceWriter;
            this.inductiveInstanceReader = inductiveInstanceReader;
            this.primitives = primitives;
            this.ensembleCache = ensembleCache;
            this.grannyService = grannyService;
            this.appUserId = appUserId;
            this.cortexLibraryOutBaseUrl = cortexLibraryOutBaseUrl;
            this.identityAccessOutBaseUrl = identityAccessOutBaseUrl;
            this.queryResultLimit = queryResultLimit;
        }

        public IEnsembleRepository EnsembleRepository => this.ensembleRepository;

        public IInstanceWriter InstanceWriter => this.instanceWriter;

        public Processors.Readers.Inductive.IInstanceReader InductiveInstanceReader => this.inductiveInstanceReader;

        public IPrimitiveSet Primitives => this.primitives;

        public IDictionary<string, Ensemble> EnsembleCache => this.ensembleCache;

        public IGrannyService GrannyService => this.grannyService;

        public string AppUserId => this.appUserId;

        public string CortexLibraryOutBaseUrl => this.cortexLibraryOutBaseUrl;

        public string IdentityAccessOutBaseUrl => this.identityAccessOutBaseUrl;

        public int QueryResultLimit => this.queryResultLimit;
    }
}
