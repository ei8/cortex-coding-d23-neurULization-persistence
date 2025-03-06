using ei8.Cortex.Coding.d23.Grannies;
using ei8.Cortex.Coding.Persistence;
using ei8.Cortex.Coding.Reflection;
using neurUL.Common.Domain.Model;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    public static class neurULizerExtensions
    {
        /// <summary>
        /// Persistence-aware neurULize. Retrieves external references, idPropertyValueNeurons, and uniquifies resulting network.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="neurULizer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static async Task<Network> neurULizeAsync<TValue>(
            this IneurULizer neurULizer,
            TValue value
        )
            where TValue : class
        {
            var typeInfo = neurULizerTypeInfo.GetTypeInfo(value);
            var options = (neurULizerOptions) neurULizer.Options;

            // use key to retrieve external reference url from library
            var externalReferences = await options.ExternalReferenceRepository.GetByKeysAsync(
                typeInfo.Keys.Except(new[] { string.Empty }).ToArray()
            );

            var valueNeuronIds = typeInfo.GrannyProperties
                .Where(gp => gp.ValueMatchBy == ValueMatchBy.Id)
                .Select(gp => gp.Value);

            var idPropertyValueNeurons = (await options.NetworkRepository.GetByQueryAsync(
                    new Library.Common.NeuronQuery()
                    {
                        Id = valueNeuronIds
                    },
                    false
                ))
                .Network.GetItems<Neuron>()
                .ToDictionary(n => n.Id);

            foreach (var stn in options.TransactionData.SavedTransientNeurons)
                idPropertyValueNeurons.Add(stn.Id, stn);

            var result = neurULizer.neurULize(
                value,
                typeInfo,
                idPropertyValueNeurons,
                externalReferences
            );

            await options.NetworkRepository.UniquifyAsync(
                result,
                options.TransactionData,
                options.NetworkCache
            );

            return result;
        }

        /// <summary>
        /// Persistence-aware DeneurULize. Retrieves external references and obtains required grannies.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="neurULizer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<TValue>> DeneurULizeAsync<TValue>(
            this IneurULizer neurULizer,
            Network value
        )
            where TValue : class, new()
        {
            var typeInfo = neurULizerTypeInfo.GetTypeInfo<TValue>();
            var options = (neurULizerOptions) neurULizer.Options;

            // use key to retrieve external reference url from library
            var externalReferences = await options.ExternalReferenceRepository.GetByKeysAsync(
                typeInfo.Keys.Except(new[] { string.Empty }).ToArray()
            );

            // TODO: use GrannyCacheService
            var instantiatesClassResult = await options.GrannyService.TryGetParseBuildPersistAsync(
                new InstantiatesClassGrannyInfo(
                    new Coding.d23.neurULization.Processors.Readers.Deductive.InstantiatesClassParameterSet(
                        await options.ExternalReferenceRepository.GetByKeyAsync(
                            typeof(TValue)
                        )
                    )
                )
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
