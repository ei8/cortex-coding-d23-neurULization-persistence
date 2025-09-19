using ei8.Cortex.Coding.d23.Grannies;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    public interface IGrannyService
    {
        /// <summary>
        /// Tries retrieving grannies from persistence, parsing them, 
        /// and building and persisting them if necessary, 
        /// using the specified granny parameters.
        /// </summary>
        /// <typeparam name="TGranny"></typeparam>
        /// <typeparam name="TDeductiveReader"></typeparam>
        /// <typeparam name="TParameterSet"></typeparam>
        /// <typeparam name="TWriter"></typeparam>
        /// <param name="grannyInfo"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<GrannyResult> TryGetParseBuildPersistAsync<
            TGranny,
            TDeductiveReader,
            TParameterSet,
            TWriter
        >(
            IGrannyInfo<TGranny, TDeductiveReader, TParameterSet, TWriter> grannyInfo,
            CancellationToken token = default
        )
            where TGranny : IGranny
            where TDeductiveReader : Coding.d23.neurULization.Processors.Readers.Deductive.IGrannyReader<TGranny, TParameterSet>
            where TParameterSet : Coding.d23.neurULization.Processors.Readers.Deductive.IDeductiveParameterSet
            where TWriter : Cortex.Coding.d23.neurULization.Processors.Writers.IGrannyWriter<TGranny, TParameterSet>;

        /// <summary>
        /// Tries retrieving grannies from persistence, and parsing them using the specified granny parameters.
        /// </summary>
        /// <typeparam name="TGranny"></typeparam>
        /// <typeparam name="TDeductiveReader"></typeparam>
        /// <typeparam name="TParameterSet"></typeparam>
        /// <typeparam name="TWriter"></typeparam>
        /// <param name="grannyInfo"></param>
        /// <returns></returns>
        Task<GrannyResult> TryGetParseAsync<
            TGranny, 
            TDeductiveReader, 
            TParameterSet, 
            TWriter
        >(
            IGrannyInfo<TGranny, TDeductiveReader, TParameterSet, TWriter> grannyInfo
        )
            where TGranny : IGranny
            where TDeductiveReader : Processors.Readers.Deductive.IGrannyReader<TGranny, TParameterSet>
            where TParameterSet : Processors.Readers.Deductive.IDeductiveParameterSet
            where TWriter : Cortex.Coding.d23.neurULization.Processors.Writers.IGrannyWriter<TGranny, TParameterSet>;

        /// <summary>
        /// Tries parsing Grannies using the specified granny parameters.
        /// </summary>
        /// <typeparam name="TGranny"></typeparam>
        /// <typeparam name="TDeductiveReader"></typeparam>
        /// <typeparam name="TParameterSet"></typeparam>
        /// <typeparam name="TWriter"></typeparam>
        /// <param name="grannyInfos"></param>
        /// <param name="network"></param>
        /// <returns></returns>
        IEnumerable<GrannyResult> TryParse<
            TGranny,
            TDeductiveReader,
            TParameterSet,
            TWriter
        >(
            IEnumerable<IGrannyInfo<TGranny, TDeductiveReader, TParameterSet, TWriter>> grannyInfos,
            Network network
        )
            where TGranny : IGranny
            where TDeductiveReader : Processors.Readers.Deductive.IGrannyReader<TGranny, TParameterSet>
            where TParameterSet : Processors.Readers.Deductive.IDeductiveParameterSet
            where TWriter : Cortex.Coding.d23.neurULization.Processors.Writers.IGrannyWriter<TGranny, TParameterSet>;
    }
}
