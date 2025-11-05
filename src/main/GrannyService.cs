using ei8.Cortex.Coding.d23.Grannies;
using ei8.Cortex.Coding.d23.neurULization.Processors.Readers.Deductive;
using ei8.Cortex.Coding.d23.neurULization.Processors.Writers;
using ei8.Cortex.Coding.Persistence;
using ei8.Cortex.IdentityAccess.Client.Out;
using ei8.EventSourcing.Client;
using Microsoft.Extensions.DependencyInjection;
using neurUL.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    /// <summary>
    /// Represents a Granny Service.
    /// </summary>
    public class GrannyService : IGrannyService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly INetworkRepository networkRepository;

        /// <summary>
        /// Constructs a Granny Service.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="networkRepository"></param>
        public GrannyService(
            IServiceProvider serviceProvider,
            INetworkRepository networkRepository
        )
        {
            AssertionConcern.AssertArgumentNotNull(serviceProvider, nameof(serviceProvider));
            AssertionConcern.AssertArgumentNotNull(networkRepository, nameof(networkRepository));

            this.serviceProvider = serviceProvider;
            this.networkRepository = networkRepository;
        }

        /// <summary>
        /// Tries retrieving grannies from persistence, 
        /// and parsing them using the specified granny parameters.
        /// </summary>
        /// <typeparam name="TGranny"></typeparam>
        /// <typeparam name="TDeductiveReader"></typeparam>
        /// <typeparam name="TParameterSet"></typeparam>
        /// <typeparam name="TWriter"></typeparam>
        /// <param name="grannyInfo"></param>
        /// <returns></returns>
        public async Task<GrannyResult> TryGetParseAsync<TGranny, TDeductiveReader, TParameterSet, TWriter>(
            IGrannyInfo<TGranny, TDeductiveReader, TParameterSet, TWriter> grannyInfo
        )
            where TGranny : IGranny
            where TDeductiveReader : Processors.Readers.Deductive.IGrannyReader<TGranny, TParameterSet>
            where TParameterSet : Processors.Readers.Deductive.IDeductiveParameterSet
            where TWriter : Cortex.Coding.d23.neurULization.Processors.Writers.IGrannyWriter<TGranny, TParameterSet>
        {
            var network = new Network();
            var processor = this.serviceProvider.GetRequiredService<TDeductiveReader>();
            var queries = processor.GetQueries(network, grannyInfo.Parameters);

            TGranny grannyResult = default;
            bool result = await queries.Process(
                this.networkRepository,
                network,
                new List<IGranny>()
            );

            result = result && processor.TryParse(
                network,
                grannyInfo.Parameters,
                out grannyResult
            );

            return new GrannyResult(grannyInfo.Parameters, result, grannyResult);
        }

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
        public IEnumerable<GrannyResult> TryParse<TGranny, TDeductiveReader, TParameterSet, TWriter>(
            IEnumerable<IGrannyInfo<TGranny, TDeductiveReader, TParameterSet, TWriter>> grannyInfos,
            Network network
        )
            where TGranny : IGranny
            where TDeductiveReader : IGrannyReader<TGranny, TParameterSet>
            where TParameterSet : IDeductiveParameterSet
            where TWriter : IGrannyWriter<TGranny, TParameterSet>
        {
            var result = new List<GrannyResult>();
            foreach (var gi in grannyInfos)
            {
                var bResult = this.serviceProvider.GetRequiredService<TDeductiveReader>().TryParse(
                    network,
                    gi.Parameters,
                    out TGranny gResult
                );

                result.Add(
                    new GrannyResult
                    (
                        gi.Parameters,
                        bResult,
                        gResult
                    )
                );
            }

            return result.AsEnumerable();
        }
    }
}