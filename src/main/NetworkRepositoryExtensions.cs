using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    public static class NetworkRepositoryExtensions
    {
        /// <summary>
        /// Retrieves identical neurUL network from specified NetworkRepository based on specified tag and postsynaptic Ids.
        /// </summary>
        /// <param name="networkRepository"></param>
        /// <param name="tag"></param>
        /// <param name="postsynapticIds"></param>
        /// <returns></returns>
        public static async Task<Network> GetPersistentIdenticalNeuron(
            this INetworkRepository networkRepository,
            string tag,
            IEnumerable<Guid> postsynapticIds
        ) => (await networkRepository.GetByQueryAsync(
            new Library.Common.NeuronQuery()
            {
                Tag = !string.IsNullOrEmpty(tag) ? new[] { tag } : null,
                Postsynaptic = postsynapticIds.Select(pi => pi.ToString()),
                DirectionValues = Library.Common.DirectionValues.Outbound,
                Depth = 1
            }
        )).Network;

        /// <summary>
        /// Retrieves identical neurUL network from specified NetworkRepository based on specified presynaptic and postsynaptic neuron Ids.
        /// </summary>
        /// <param name="networkRepository"></param>
        /// <param name="presynapticNeuronId"></param>
        /// <param name="postsynapticNeuronId"></param>
        /// <returns></returns>
        public static async Task<Network> GetPersistentIdenticalNeuronTerminal(
            this INetworkRepository networkRepository,
            Guid presynapticNeuronId,
            Guid postsynapticNeuronId
        ) => (await networkRepository.GetByQueryAsync(
            new Library.Common.NeuronQuery()
            {
                Id = new[] { presynapticNeuronId.ToString() },
                Postsynaptic = new[] { postsynapticNeuronId.ToString() },
                // TODO: how should this be handled
                NeuronActiveValues = Library.Common.ActiveValues.All,
                TerminalActiveValues = Library.Common.ActiveValues.All
            },
            false
        )).Network;
    }
}
