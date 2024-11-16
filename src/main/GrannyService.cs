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
        private readonly IEnsembleRepository ensembleRepository;
        private readonly IDictionary<string, Ensemble> ensembleCache;
        private readonly ITransaction transaction;
        private readonly IEnsembleTransactionService ensembleTransactionService;
        private readonly IValidationClient validationClient;
        private readonly string identityAccessOutBaseUrl;
        private readonly string appUserId;

        public GrannyService(
            IServiceProvider serviceProvider,
            IEnsembleRepository ensembleRepository,
            IDictionary<string, Ensemble> ensembleCache,
            ITransaction transaction,
            IEnsembleTransactionService ensembleTransactionService,
            IValidationClient validationClient,
            string identityAccessOutBaseUrl,
            string appUserId
        )
        {
            AssertionConcern.AssertArgumentNotNull(ensembleRepository, nameof(ensembleRepository));
            AssertionConcern.AssertArgumentNotNull(ensembleCache, nameof(ensembleCache));
            AssertionConcern.AssertArgumentNotNull(transaction, nameof(transaction));
            AssertionConcern.AssertArgumentNotNull(ensembleTransactionService, nameof(ensembleTransactionService));
            AssertionConcern.AssertArgumentNotNull(validationClient, nameof(validationClient));
            AssertionConcern.AssertArgumentNotEmpty(identityAccessOutBaseUrl, "Parameter cannot be null or empty.", nameof(identityAccessOutBaseUrl));
            AssertionConcern.AssertArgumentNotEmpty(appUserId, "Parameter cannot be null or empty.", nameof(appUserId));

            this.serviceProvider = serviceProvider;
            this.ensembleRepository = ensembleRepository;
            this.ensembleCache = ensembleCache;
            this.transaction = transaction;
            this.ensembleTransactionService = ensembleTransactionService;
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
            var grannyResult = (TGranny) tryGetGrannyResult.Granny;

            if (!boolResult)
            {
                var instantiatesEnsemble = new Ensemble();
                grannyResult = this.serviceProvider.GetRequiredService<TWriter>().ParseBuild<
                    TGranny,
                    TWriter,
                    TParameterSet
                >(
                    instantiatesEnsemble,
                    grannyInfo.Parameters
                );

                if (instantiatesEnsemble.AnyTransient())
                {
                    var iaeNeurons = instantiatesEnsemble.GetItems<Coding.Neuron>()
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
                        await this.ensembleRepository.UniquifyAsync(
                            instantiatesEnsemble,
                            cache: this.ensembleCache
                        );
                        await this.transaction.BeginAsync(appUserGuid.Value);

                        await this.ensembleTransactionService.SaveAsync(
                            this.transaction,
                            instantiatesEnsemble
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
            IGrannyInfo<TGranny, TDeductiveReader, TParameterSet, TWriter> grannyInfo,
            string userId = default
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
                this.ensembleRepository,
                grannyInfo.Parameters,
                userId
            );

            return new GrannyResult(result.Item1, result.Item2);
        }

        public IEnumerable<GrannyResult> TryParse<TGranny, TDeductiveReader, TParameterSet, TWriter>(
            IEnumerable<IGrannyInfo<TGranny, TDeductiveReader, TParameterSet, TWriter>> grannyInfos, 
            Ensemble ensemble
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
                    ensemble,
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
    }
}
