using ei8.Cortex.Coding.Persistence;
using ei8.Cortex.Coding.Persistence.Versioning;
using ei8.Cortex.Coding.Versioning;
using ei8.Cortex.Library.Common;
using ei8.EventSourcing.Client;
using neurUL.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SQLite.SQLite3;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence.Versioning
{
    public class SnapshotReadRepository : ISnapshotReadRepository
    {
        private readonly IneurULizer neurULizer;
        private readonly INetworkRepository networkRepository;
        private readonly IIdInstanceNeuronsRetriever idInstanceNeuronsRetriever;
        private readonly Network readNetworkCache;

        public SnapshotReadRepository(
            IneurULizer neurULizer,
            INetworkRepository networkRepository,
            IIdInstanceNeuronsRetriever idInstanceNeuronsRetriever,
            Network readNetworkCache
        )
        {
            AssertionConcern.AssertArgumentNotNull(neurULizer, nameof(neurULizer));
            AssertionConcern.AssertArgumentNotNull(networkRepository, nameof(networkRepository));
            AssertionConcern.AssertArgumentNotNull(idInstanceNeuronsRetriever, nameof(idInstanceNeuronsRetriever));
            AssertionConcern.AssertArgumentNotNull(readNetworkCache, nameof(readNetworkCache));

            this.neurULizer = neurULizer;
            this.networkRepository = networkRepository;
            this.idInstanceNeuronsRetriever = idInstanceNeuronsRetriever;
            this.readNetworkCache = readNetworkCache;
        }

        public async Task<IEnumerable<Snapshot>> GetByIds(IEnumerable<Guid> ids, CancellationToken token = default)
        {
            AssertionConcern.AssertArgumentNotNull(ids, nameof(ids));
            AssertionConcern.AssertArgumentValid(i => i.Any(), ids, $"Specified value cannot be an empty array.", nameof(ids));

            var queryResult = await networkRepository.GetByQueryAsync(
                new NeuronQuery()
                {
                    Id = ids.Select(i => i.ToString()),
                    Depth = Constants.ValueToInstantiatesClassDepth,
                    DirectionValues = DirectionValues.Outbound
                }
            );

            queryResult.Network.ValidateIds(ids);

            idInstanceNeuronsRetriever.Initialize(ids);
            var drs = (await neurULizer.DeneurULizeAsync<Snapshot>(
                queryResult.Network,
                idInstanceNeuronsRetriever,
                token
            )).Where(dnr => dnr.Success);
            readNetworkCache.AddReplaceItems(drs.Select(dr => dr.InstanceNeuron));
            return drs.Select(nr => nr.Result);
        }
    }
}
