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
            IMirrorRepository mirrorRepository,
            INetworkRepository networkRepository, 
            IInstanceWriter instanceWriter,
            IInstanceReader inductiveInstanceReader,
            IIdInstanceValueWriter idInstanceValueWriter,
            IInstanceValueReader inductiveInstanceValueReader,
            IMirrorSet mirrors,
            IDictionary<string, Network> networkCache,
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
            AssertionConcern.AssertArgumentNotNull(networkCache, nameof(networkCache));
            AssertionConcern.AssertArgumentNotNull(grannyService, nameof(grannyService));
            AssertionConcern.AssertArgumentNotNull(transactionData, nameof(transactionData));
            
            this.MirrorRepository = mirrorRepository;
            this.NetworkRepository = networkRepository;
            this.InstanceWriter = instanceWriter;
            this.InductiveInstanceReader = inductiveInstanceReader;
            this.IdInstanceValueWriter = idInstanceValueWriter;
            this.InductiveInstanceValueReader = inductiveInstanceValueReader;
            this.Mirrors = mirrors;
            this.NetworkCache = networkCache;
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
        public IDictionary<string, Network> NetworkCache { get; }
        public IGrannyService GrannyService { get; }
        public INetworkTransactionData TransactionData { get; }
    }
}
