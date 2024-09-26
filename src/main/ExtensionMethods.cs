﻿using ei8.Cortex.Coding.d23.Grannies;
using ei8.Cortex.Coding.d23.neurULization.Processors.Readers.Deductive;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    public static class ExtensionMethods
    {
        public static async Task<TGranny> GetGranny<TGranny, TDeductiveReaderProcessor, TParameterSet>(
            this TDeductiveReaderProcessor icProc,
            IEnsembleRepository ensembleRepository,
            TParameterSet parameters,
            string userId,
            string cortexLibraryOutBaseUrl, 
            int queryResultLimit
        )
            where TGranny : IGranny
            where TDeductiveReaderProcessor : Coding.d23.neurULization.Processors.Readers.Deductive.IGrannyReadProcessor<TGranny, TParameterSet>
            where TParameterSet : Coding.d23.neurULization.Processors.Readers.Deductive.IDeductiveParameterSet
        {
            var icPqs = icProc.GetQueries(parameters);
            var ensemble = new Ensemble();
            await icPqs.Process(
                ensembleRepository,
                ensemble,
                new List<IGranny>(),
                userId,
                cortexLibraryOutBaseUrl,
                queryResultLimit
            );

            icProc.TryParse(
                ensemble,
                parameters,
                out TGranny granny
            );

            return granny;
        }
    }
}
