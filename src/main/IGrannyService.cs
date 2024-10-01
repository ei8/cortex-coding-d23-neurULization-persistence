using ei8.Cortex.Coding.d23.Grannies;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    public interface IGrannyService
    {
        Task<Tuple<bool, TGranny>> TryGetBuildPersistAsync<
            TGranny,
            TDeductiveReader,
            TParameterSet,
            TWriter
        >(
            IGrannyInfo<TGranny, TDeductiveReader, TParameterSet, TWriter> grannyInfo,
            string appUserId,
            string identityAccessOutBaseUrl,
            string cortexLibraryOutBaseUrl,
            int queryResultLimit,
            CancellationToken token = default
        )
            where TGranny : IGranny
            where TDeductiveReader : Coding.d23.neurULization.Processors.Readers.Deductive.IGrannyReader<TGranny, TParameterSet>
            where TParameterSet : Coding.d23.neurULization.Processors.Readers.Deductive.IDeductiveParameterSet
            where TWriter : Cortex.Coding.d23.neurULization.Processors.Writers.IGrannyWriter<TGranny, TParameterSet>;

        Task<Tuple<bool, TGranny>> TryGetGrannyAsync<
            TGranny, 
            TDeductiveReader, 
            TParameterSet, 
            TWriter
        >(
            IGrannyInfo<TGranny, TDeductiveReader, TParameterSet, TWriter> grannyInfo, 
            string appUserId, 
            string cortexLibraryOutBaseUrl, 
            int queryResultLimit
        )
            where TGranny : IGranny
            where TDeductiveReader : Processors.Readers.Deductive.IGrannyReader<TGranny, TParameterSet>
            where TParameterSet : Processors.Readers.Deductive.IDeductiveParameterSet
            where TWriter : Cortex.Coding.d23.neurULization.Processors.Writers.IGrannyWriter<TGranny, TParameterSet>;
    }
}
