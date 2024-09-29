using ei8.Cortex.Coding.d23.Grannies;
using ei8.Cortex.Coding.Reflection;
using neurUL.Common.Domain.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    public static class neurULizerExtensions
    {
        /// <summary>
        /// Persistence-aware neurULize. Retrieves external references, idPropertyValueNeurons, and uniquifies resulting ensemble.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="neurULizer"></param>
        /// <param name="value"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<Ensemble> neurULizeAsync<TValue>(
            this IneurULizer neurULizer,
            TValue value,
            string userId
        )
            where TValue : class
        {
            var typeInfo = neurULizerTypeInfo.GetTypeInfo(value);
            var options = (neurULizerOptions) neurULizer.Options;

            // use key to retrieve external reference url from library
            var externalReferences = await options.EnsembleRepository.GetExternalReferencesAsync(
                options.AppUserId,
                options.CortexLibraryOutBaseUrl,
                typeInfo.Keys.ToArray()
            );

            var valueNeuronIds = typeInfo.GrannyProperties
                .Where(gp => gp.ValueMatchBy == ValueMatchBy.Id)
                .Select(gp => gp.Value);

            var idPropertyValueNeurons = (await options.EnsembleRepository.GetByQueryAsync(
                    userId,
                    new Library.Common.NeuronQuery()
                    {
                        Id = valueNeuronIds
                    },
                    options.CortexLibraryOutBaseUrl,
                    int.MaxValue
                ))
                .GetItems<Neuron>()
                .ToDictionary(n => n.Id.ToString());

            var result = neurULizer.neurULize(
                value,
                typeInfo,
                idPropertyValueNeurons,
                externalReferences
            );

            await options.EnsembleRepository.UniquifyAsync(
                options.AppUserId,
                result,
                options.CortexLibraryOutBaseUrl,
                options.QueryResultLimit,
                options.EnsembleCache
            );

            return result;
        }

        /// <summary>
        /// Persistence-aware DeneurULize. Retrieves external references and obtains required grannies.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="neurULizer"></param>
        /// <param name="value"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<TValue>> DeneurULizeAsync<TValue>(
            this IneurULizer neurULizer,
            Ensemble value,
            string userId
        )
            where TValue : class, new()
        {
            var typeInfo = neurULizerTypeInfo.GetTypeInfo<TValue>();
            var options = (neurULizerOptions) neurULizer.Options;

            // use key to retrieve external reference url from library
            var externalReferences = await options.EnsembleRepository
                .GetExternalReferencesAsync(
                    userId,
                    options.CortexLibraryOutBaseUrl,
                    typeInfo.Keys.ToArray()
                );

            // TODO: use GrannyCacheService
            var instantiatesClassResult = await options.GrannyService.TryGetBuildPersistAsync<
                IInstantiatesClass,
                Coding.d23.neurULization.Processors.Readers.Deductive.IInstantiatesClassProcessor,
                Coding.d23.neurULization.Processors.Readers.Deductive.IInstantiatesClassParameterSet,
                Coding.d23.neurULization.Processors.Writers.IInstantiatesClassProcessor
            >(
                new Coding.d23.neurULization.Processors.Readers.Deductive.InstantiatesClassParameterSet(
                    await options.EnsembleRepository.GetExternalReferenceAsync(
                        options.AppUserId,
                        options.CortexLibraryOutBaseUrl,
                        typeof(TValue)
                    )
                ),
                options.AppUserId,
                options.IdentityAccessOutBaseUrl,
                options.CortexLibraryOutBaseUrl,
                options.QueryResultLimit
            // TODO: add support for CancellationToken
            );

            AssertionConcern.AssertStateTrue(
                instantiatesClassResult.Item1,
                $"'Instantiates^Avatar' is required to invoke {nameof(IneurULizer.DeneurULize)}"
            );

            var instanceNeurons = value.GetPresynapticNeurons(instantiatesClassResult.Item2.Neuron.Id);

            return neurULizer.DeneurULize<TValue>(
                value, 
                instanceNeurons,
                typeInfo,
                externalReferences
            );
        }
    }
}
