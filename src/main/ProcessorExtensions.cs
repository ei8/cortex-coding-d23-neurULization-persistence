using ei8.Cortex.Coding.d23.Grannies;
using ei8.Cortex.Coding.d23.neurULization.Processors.Readers.Deductive;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    public static class ProcessorExtensions
    {
        public static async Task<Tuple<bool, TGranny>> TryGetParseAsync<TGranny, TDeductiveReader, TParameterSet>(
            this TDeductiveReader processor,
            INetworkRepository networkRepository,
            TParameterSet parameters
        )
            where TGranny : IGranny
            where TDeductiveReader : Coding.d23.neurULization.Processors.Readers.Deductive.IGrannyReader<TGranny, TParameterSet>
            where TParameterSet : Coding.d23.neurULization.Processors.Readers.Deductive.IDeductiveParameterSet
        {
            var network = new Network();
            var queries = processor.GetQueries(network, parameters);

            TGranny grannyResult = default;
            bool result = await queries.Process(
                    networkRepository,
                    network,
                    new List<IGranny>()
                );
            
            result = result && processor.TryParse(
                    network,
                    parameters,
                    out grannyResult
                );

            return Tuple.Create(result, grannyResult);
        }
    }
}
