using ei8.Cortex.Coding.d23.Grannies;
using ei8.Cortex.Coding.d23.neurULization.Processors.Readers.Deductive;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    public static class ProcessorExtensions
    {
        public static async Task<Tuple<bool, TGranny>> TryGetGrannyAsync<TGranny, TDeductiveReader, TParameterSet>(
            this TDeductiveReader processor,
            IEnsembleRepository ensembleRepository,
            TParameterSet parameters,
            string userId,
            string cortexLibraryOutBaseUrl, 
            int queryResultLimit
        )
            where TGranny : IGranny
            where TDeductiveReader : Coding.d23.neurULization.Processors.Readers.Deductive.IGrannyReader<TGranny, TParameterSet>
            where TParameterSet : Coding.d23.neurULization.Processors.Readers.Deductive.IDeductiveParameterSet
        {
            var icPqs = processor.GetQueries(parameters);
            var ensemble = new Ensemble();

            TGranny grannyResult = default;
            bool result = await icPqs.Process(
                    ensembleRepository,
                    ensemble,
                    new List<IGranny>(),
                    userId,
                    cortexLibraryOutBaseUrl,
                    queryResultLimit
                ) && processor.TryParse(
                    ensemble,
                    parameters,
                    out grannyResult
                );

            return Tuple.Create(result, grannyResult);
        }
    }
}
