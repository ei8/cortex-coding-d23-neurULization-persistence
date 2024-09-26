using ei8.Cortex.Coding.d23.Grannies;
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

        public GrannyService(
            IServiceProvider serviceProvider,
            IEnsembleRepository ensembleRepository,
            IDictionary<string, Ensemble> ensembleCache,
            ITransaction transaction,
            IEnsembleTransactionService ensembleTransactionService,
            IValidationClient validationClient
        )
        {
            AssertionConcern.AssertArgumentNotNull(ensembleRepository, nameof(ensembleRepository));
            AssertionConcern.AssertArgumentNotNull(ensembleCache, nameof(ensembleCache));
            AssertionConcern.AssertArgumentNotNull(transaction, nameof(transaction));
            AssertionConcern.AssertArgumentNotNull(ensembleTransactionService, nameof(ensembleTransactionService));
            AssertionConcern.AssertArgumentNotNull(validationClient, nameof(validationClient));

            this.serviceProvider = serviceProvider;
            this.ensembleRepository = ensembleRepository;
            this.ensembleCache = ensembleCache;
            this.transaction = transaction;
            this.ensembleTransactionService = ensembleTransactionService;
            this.validationClient = validationClient;
        }

        // TODO: encapsulate in a GrannyCacheService with methods such as: GetInstantiatesClass<T>() etc.
        public async Task<Tuple<bool, TGranny>> TryObtainPersistAsync<
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
            where TWriterProcessor : Cortex.Coding.d23.neurULization.Processors.Writers.IGrannyWriteProcessor<TGranny, TParameterSet>
        {
            TGranny grannyResult = await this.serviceProvider.GetRequiredService<TDeductiveReaderProcessor>().GetGranny<
                TGranny,
                TDeductiveReaderProcessor, 
                TParameterSet
            >(
                this.ensembleRepository,
                parameters,
                // use appUserId since calls to this function relate to app required grannies
                appUserId,
                cortexLibraryOutBaseUrl,
                queryResultLimit
            );

            bool result = grannyResult != null;
            
            if (!result)
            {
                var instantiatesAvatarEnsemble = new Ensemble();
                grannyResult = await this.serviceProvider.GetRequiredService<TWriterProcessor>().ObtainAsync<
                    TGranny,
                    TWriterProcessor,
                    TParameterSet
                >(
                    instantiatesAvatarEnsemble,
                    parameters
                );

                if (instantiatesAvatarEnsemble.AnyTransient())
                {
                    var iaeNeurons = instantiatesAvatarEnsemble.GetItems<Coding.Neuron>()
                        .Where(n => n.IsTransient);

                    Guid? appUserGuid = null;
                    foreach (var n in iaeNeurons)
                    {
                        var avr = await this.validationClient.CreateNeuron(
                            identityAccessOutBaseUrl,
                            n.Id,
                            n.RegionId,
                            appUserId,
                            token
                        );
                        if (!avr.HasErrors)
                            appUserGuid = avr.UserNeuronId;
                    }

                    if (appUserGuid != null)
                    {
                        await this.ensembleRepository.UniquifyAsync(
                            appUserId,
                            instantiatesAvatarEnsemble,
                            cortexLibraryOutBaseUrl,
                            queryResultLimit,
                            this.ensembleCache
                        );
                        await this.ensembleTransactionService.SaveAsync(
                            transaction,
                            instantiatesAvatarEnsemble,
                            appUserGuid.Value
                        );
                        result = true;
                    }
                }
                else
                    result = true;
            }

            return Tuple.Create(result, grannyResult);
        }
    }
}
