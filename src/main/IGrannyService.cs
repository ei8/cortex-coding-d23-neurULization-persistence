using ei8.Cortex.Coding.d23.Grannies;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    public interface IGrannyService
    {
        Task<Tuple<bool, TGranny>> TryObtainPersistAsync<
            TGranny,
            TDeductiveReaderProcessor,
            TParameterSet,
            TWriterProcessor
        >(
            TParameterSet parameters,
            string appUserId,
            string identityAccessOutBaseUrl,
            string cortexLibraryOutBaseUrl,
            int queryResultLimit,
            CancellationToken token = default
        )
            where TGranny : IGranny
            where TDeductiveReaderProcessor : Coding.d23.neurULization.Processors.Readers.Deductive.IGrannyReadProcessor<TGranny, TParameterSet>
            where TParameterSet : Coding.d23.neurULization.Processors.Readers.Deductive.IDeductiveParameterSet
            where TWriterProcessor : Cortex.Coding.d23.neurULization.Processors.Writers.IGrannyWriteProcessor<TGranny, TParameterSet>;
    }
}
