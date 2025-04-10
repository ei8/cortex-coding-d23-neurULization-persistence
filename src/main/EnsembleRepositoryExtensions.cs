using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ei8.Cortex.Coding.d23.neurULization.Processors.Readers.Deductive;
using ei8.Cortex.Library.Common;
using System.Linq;
using ei8.Cortex.Coding.Persistence;
using neurUL.Common.Domain.Model;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    public static class NetworkRepositoryExtensions
    {
        public static async Task<IEnumerable<GrannyResult>> GetStringValues(
            this INetworkRepository networkRepository, 
            IExternalReferenceRepository externalReferenceRepository,
            IGrannyService grannyService, 
            Network network, 
            IEnumerable<Guid> ids, 
            string userId
        )
        {
            AssertionConcern.AssertArgumentNotNull(ids, nameof(ids));
            AssertionConcern.AssertArgumentValid(i => i.Any(), ids, $"Specified value cannot be an empty array.", nameof(ids));

            var idsQueryResult = await networkRepository.GetByQueryAsync(
                new NeuronQuery()
                {
                    Id = ids.Select(i => i.ToString()),
                    Depth = Coding.d23.neurULization.Constants.ValueToInstantiatesClassDepth,
                    DirectionValues = DirectionValues.Outbound
                },
                userId
            );
            var stringNeuron = await externalReferenceRepository.GetByKeyAsync(typeof(string));
            return grannyService.TryParse(
                ids.Select(i =>
                {
                    network.TryGetById(i, out Coding.Neuron valueNeuron);
                    return new InstanceValueGrannyInfo(
                        new InstanceValueParameterSet(
                            valueNeuron,
                            stringNeuron,
                            ValueMatchBy.Tag
                        )
                    );
                }
                ),
                idsQueryResult.Network
            );
        }
    }
}
