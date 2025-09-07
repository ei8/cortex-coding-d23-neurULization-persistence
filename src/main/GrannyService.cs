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
    public class GrannyService : IGrannyService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IDictionary<string, Network> networkCache;
        private readonly ITransaction transaction;
        private readonly INetworkTransactionService networkTransactionService;
        private readonly IValidationClient validationClient;
        private readonly string identityAccessOutBaseUrl;
        private readonly string appUserId;

        public GrannyService(
            IServiceProvider serviceProvider,
            INetworkRepository networkRepository,
            IDictionary<string, Network> networkCache,
            ITransaction transaction,
            INetworkTransactionService networkTransactionService,
            IValidationClient validationClient,
            string identityAccessOutBaseUrl,
            string appUserId
        )
        {
            AssertionConcern.AssertArgumentNotNull(serviceProvider, nameof(serviceProvider));
            AssertionConcern.AssertArgumentNotNull(networkRepository, nameof(networkRepository));
            AssertionConcern.AssertArgumentNotNull(networkCache, nameof(networkCache));
            AssertionConcern.AssertArgumentNotNull(transaction, nameof(transaction));
            AssertionConcern.AssertArgumentNotNull(networkTransactionService, nameof(networkTransactionService));
            AssertionConcern.AssertArgumentNotNull(validationClient, nameof(validationClient));
            AssertionConcern.AssertArgumentNotEmpty(identityAccessOutBaseUrl, "Parameter cannot be null or empty.", nameof(identityAccessOutBaseUrl));
            AssertionConcern.AssertArgumentNotEmpty(appUserId, "Parameter cannot be null or empty.", nameof(appUserId));

            this.serviceProvider = serviceProvider;
            this.NetworkRepository = networkRepository;
            this.networkCache = networkCache;
            this.transaction = transaction;
            this.networkTransactionService = networkTransactionService;
            this.validationClient = validationClient;
            this.identityAccessOutBaseUrl = identityAccessOutBaseUrl;
            this.appUserId = appUserId;
        }

        // TODO: encapsulate in a GrannyCacheService with methods such as: GetInstantiatesClass<T>() etc.
        public async Task<Tuple<bool, TGranny>> TryGetParseBuildPersistAsync<
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
            where TWriter : Cortex.Coding.d23.neurULization.Processors.Writers.IGrannyWriter<TGranny, TParameterSet>
        {
            var tryGetGrannyResult = await this.TryGetParseAsync(grannyInfo);

            var boolResult = tryGetGrannyResult.Success;
            var grannyResult = (TGranny)tryGetGrannyResult.Granny;

            if (!boolResult)
            {
                var instantiatesNetwork = new Network();
                boolResult = this.serviceProvider.GetRequiredService<TWriter>().TryParseBuild(
                    instantiatesNetwork,
                    grannyInfo.Parameters,
                    out grannyResult
                );

                if (instantiatesNetwork.AnyTransient())
                {
                    var iaeNeurons = instantiatesNetwork.GetItems<Coding.Neuron>()
                        .Where(n => n.IsTransient);

                    Guid? appUserGuid = null;
                    foreach (var n in iaeNeurons)
                    {
                        var avr = await this.validationClient.CreateNeuron(
                            identityAccessOutBaseUrl,
                            n.Id,
                            n.RegionId,
                            this.appUserId,
                            token
                        );
                        if (!avr.HasErrors)
                            appUserGuid = avr.UserNeuronId;
                    }

                    if (appUserGuid.HasValue)
                    {
                        await instantiatesNetwork.UniquifyAsync(
                            persistentIdenticalNeuronRetriever: (t, ps) => 
                                this.NetworkRepository.GetPersistentIdenticalNeuron(t, ps),
                            persistentIdenticalNeuronTerminalRetriever: (pre, post) => 
                                this.NetworkRepository.GetPersistentIdenticalNeuronTerminal(pre, post),
                            cache: this.networkCache
                        );
                        await this.transaction.BeginAsync(appUserGuid.Value);

                        await this.networkTransactionService.SaveAsync(
                            this.transaction,
                            instantiatesNetwork
                        );

                        await this.transaction.CommitAsync();
                        boolResult = true;
                    }
                }
                else
                    boolResult = true;
            }

            return Tuple.Create(boolResult, grannyResult);
        }

        public async Task<GrannyResult> TryGetParseAsync<TGranny, TDeductiveReader, TParameterSet, TWriter>(
            IGrannyInfo<TGranny, TDeductiveReader, TParameterSet, TWriter> grannyInfo
        )
            where TGranny : IGranny
            where TDeductiveReader : Processors.Readers.Deductive.IGrannyReader<TGranny, TParameterSet>
            where TParameterSet : Processors.Readers.Deductive.IDeductiveParameterSet
            where TWriter : Cortex.Coding.d23.neurULization.Processors.Writers.IGrannyWriter<TGranny, TParameterSet>
        {
            var result = await this.serviceProvider.GetRequiredService<TDeductiveReader>().TryGetParseAsync<
                TGranny,
                TDeductiveReader,
                TParameterSet
            >(
                this.NetworkRepository,
                grannyInfo.Parameters
            );

            return new GrannyResult(result.Item1, result.Item2);
        }

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
                        bResult,
                        gResult
                    )
                );
            }

            return result.AsEnumerable();
        }

        public INetworkRepository NetworkRepository { get; }
    }
}