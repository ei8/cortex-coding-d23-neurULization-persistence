using ei8.Cortex.Coding.Model.Reflection;
using ei8.Cortex.Coding.Persistence;
using ei8.Cortex.Library.Common;
using neurUL.Common.Domain.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    /// <summary>
    /// Represents neurULizer extension methods.
    /// </summary>
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
            TValue value,            
            CancellationToken token = default
        )
            where TValue : class
        {
            var typeInfo = neurULizerTypeInfo.GetTypeInfo(value);
            var options = (neurULizerOptions) neurULizer.Options;

            // use key to retrieve external reference url from library
            var mirrors = await options.MirrorRepository.GetByKeysAsync(
                typeInfo.Keys.Except(new[] { string.Empty }).ToArray()
            );

            var valueNeuronIds = typeInfo.GrannyProperties
                .Where(gp => gp.ValueMatchBy == ValueMatchBy.Id)
                .Select(gp => gp.Value);

            var idPropertyValueNeurons = (await options.NetworkRepository.GetByQueryAsync(
                    new NeuronQuery()
                    {
                        Id = valueNeuronIds
                    },
                    false
                ))
                .Network.GetItems<Neuron>()
                .ToList();

            idPropertyValueNeurons.AddRange(
                options.TransactionData.SavedTransientNeurons
            );

            var result = neurULizer.neurULize(
                value,
                idPropertyValueNeurons,
                typeInfo,
                mirrors
            );

            await result.UniquifyAsync(
                options.TransactionData,
                async (ct, cpis) => await options.NetworkRepository.GetPersistentIdenticalNeuron(ct, cpis),
                async (pre, post) => await options.NetworkRepository.GetPersistentIdenticalNeuronTerminal(pre, post),
                options.UniquifyCache
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
        public static async Task<IEnumerable<neurULizationResult<TValue>>> DeneurULizeAsync<TValue>(
            this IneurULizer neurULizer,
            Network value,
            IInstanceNeuronsRetriever instanceNeuronsRetriever,
            CancellationToken token = default
        )
            where TValue : class, new()
        {
            var typeInfo = neurULizerTypeInfo.GetTypeInfo<TValue>();
            var options = (neurULizerOptions) neurULizer.Options;

            // use key to retrieve external reference url from library
            var mirrors = await options.MirrorRepository.GetByKeysAsync(
                typeInfo.Keys.Except(new[] { string.Empty }).ToArray()
            );

            var instanceNeurons = await instanceNeuronsRetriever.GetInstanceNeuronsAsync<TValue>(value, token);

            var result = neurULizer.DeneurULize<TValue>(
                value, 
                instanceNeurons,
                typeInfo,
                mirrors
            );

            var failedInstances = result.Where(r => !r.Success);

            AssertionConcern.AssertStateTrue(
                !failedInstances.Any(),
                $"Failed deneurULizing instances with IDs: " +
                $"'{string.Join(", ", failedInstances.Select(i => i.InstanceNeuron.Id))}'."
            );

            return result;
        }

        /// <summary>
        /// Persistence-aware DeneurULize. Retrieves mirrors, obtains required grannies, and caches
        /// neurons in specified cache.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="neurULizer"></param>
        /// <param name="value"></param>
        /// <param name="instanceNeuronsRetriever"></param>
        /// <param name="readCache"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async static Task<IEnumerable<TValue>> DeneurULizeCacheAsync<TValue>(
            this IneurULizer neurULizer,
            Network value,
            IInstanceNeuronsRetriever instanceNeuronsRetriever,
            Network readCache,
            CancellationToken token = default
        )
            where TValue : class, new()
        {
            var dnResult = await neurULizer.DeneurULizeAsync<TValue>(
                value,
                instanceNeuronsRetriever,
                token
            );

            dnResult
                .Where(dnr => dnr.Success)
                .ToList()
                .ForEach(dnr => readCache.AddReplace(dnr.InstanceNeuron)
                );

            return dnResult.Select(dm => dm.Result);
        }
    }
}
