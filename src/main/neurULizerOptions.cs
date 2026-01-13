using ei8.Cortex.Coding.d23.neurULization.Implementation;
using ei8.Cortex.Coding.d23.neurULization.Processors.Readers.Inductive;
using ei8.Cortex.Coding.d23.neurULization.Processors.Writers;
using ei8.Cortex.Coding.Mirrors;
using ei8.Cortex.Coding.Persistence;
using neurUL.Common.Domain.Model;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    /// <summary>
    /// Represens an option set for a d# neurULizer.
    /// </summary>
    public class neurULizerOptions : Id23neurULizerOptions
    {
        /// <summary>
        /// Constructs a d# neurULizer option set.
        /// </summary>
        /// <param name="mirrorRepository"></param>
        /// <param name="networkRepository"></param>
        /// <param name="instanceWriter"></param>
        /// <param name="inductiveInstanceReader"></param>
        /// <param name="idInstanceValueWriter"></param>
        /// <param name="inductiveInstanceValueReader"></param>
        /// <param name="mirrors"></param>
        /// <param name="uniquifyCache"></param>
        /// <param name="grannyService"></param>
        /// <param name="transactionData"></param>
        public neurULizerOptions(
            IMirrorRepository mirrorRepository,
            INetworkRepository networkRepository, 
            IInstanceWriter instanceWriter,
            IInstanceReader inductiveInstanceReader,
            IIdInstanceValueWriter idInstanceValueWriter,
            IInstanceValueReader inductiveInstanceValueReader,
            IMirrorSet mirrors,
            INetworkDictionary<string> uniquifyCache,
            IGrannyService grannyService,
            INetworkTransactionData transactionData
        )
        {
            AssertionConcern.AssertArgumentNotNull(mirrorRepository, nameof(mirrorRepository));
            AssertionConcern.AssertArgumentNotNull(networkRepository, nameof(networkRepository));
            AssertionConcern.AssertArgumentNotNull(instanceWriter, nameof(instanceWriter));
            AssertionConcern.AssertArgumentNotNull(inductiveInstanceReader, nameof(inductiveInstanceReader));
            AssertionConcern.AssertArgumentNotNull(idInstanceValueWriter, nameof(idInstanceValueWriter));
            AssertionConcern.AssertArgumentNotNull(inductiveInstanceValueReader, nameof(inductiveInstanceValueReader));
            AssertionConcern.AssertArgumentNotNull(mirrors, nameof(mirrors));
            AssertionConcern.AssertArgumentNotNull(uniquifyCache, nameof(uniquifyCache));
            AssertionConcern.AssertArgumentNotNull(grannyService, nameof(grannyService));
            AssertionConcern.AssertArgumentNotNull(transactionData, nameof(transactionData));
            
            this.MirrorRepository = mirrorRepository;
            this.NetworkRepository = networkRepository;
            this.InstanceWriter = instanceWriter;
            this.InductiveInstanceReader = inductiveInstanceReader;
            this.IdInstanceValueWriter = idInstanceValueWriter;
            this.InductiveInstanceValueReader = inductiveInstanceValueReader;
            this.Mirrors = mirrors;
            this.UniquifyCache = uniquifyCache;
            this.GrannyService = grannyService;
            this.TransactionData = transactionData;
        }

        public IMirrorRepository MirrorRepository { get; }
        public INetworkRepository NetworkRepository { get; }
        public IInstanceWriter InstanceWriter { get; }
        public IInstanceReader InductiveInstanceReader { get; }
        public IIdInstanceValueWriter IdInstanceValueWriter { get; }
        public IInstanceValueReader InductiveInstanceValueReader { get; }
        public IMirrorSet Mirrors { get; }
        public INetworkDictionary<string> UniquifyCache { get; }
        public IGrannyService GrannyService { get; }
        public INetworkTransactionData TransactionData { get; }
    }
}
