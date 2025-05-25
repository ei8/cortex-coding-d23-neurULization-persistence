using neurUL.Common.Domain.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    public class ClassInstanceNeuronsRetriever : IClassInstanceNeuronsRetriever
    {
        private readonly IGrannyService grannyService;
        private Neuron @class;

        public ClassInstanceNeuronsRetriever(IGrannyService grannyService)
        {
            AssertionConcern.AssertArgumentNotNull(grannyService, nameof(grannyService));

            this.grannyService = grannyService;
        }

        public async Task<IEnumerable<Neuron>> GetInstanceNeuronsAsync<TValue>(
            Network value, 
            CancellationToken token = default
        )
            where TValue : class, new()
        {
            AssertionConcern.AssertArgumentNotNull(value, nameof(value));
            AssertionConcern.AssertStateTrue(this.@class != null, "'Class' need to be initialized prior to instance neurons retrieval.");

            // TODO: use GrannyCacheService
            var instantiatesClassResult = await this.grannyService.TryGetParseBuildPersistAsync(
                new InstantiatesClassGrannyInfo(new Coding.d23.neurULization.Processors.Readers.Deductive.InstantiatesClassParameterSet(this.@class)),
                token
            );

            AssertionConcern.AssertStateTrue(
                instantiatesClassResult.Item1,
                $"InstantiatesClass Granny is required to invoke {nameof(IneurULizer.DeneurULize)}"
            );

            this.@class = null;

            return value.GetPresynapticNeurons(instantiatesClassResult.Item2.Neuron.Id);
        }

        public void Initialize(Neuron value)
        {
            this.@class = value;
        }
    }
}
