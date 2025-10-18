using neurUL.Common.Domain.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    /// <summary>
    /// Provides functionalities for InstanceNeuronsRetrievers that 
    /// retrieve Class instances.
    /// </summary>
    public class ClassInstanceNeuronsRetriever : IClassInstanceNeuronsRetriever
    {
        private readonly IGrannyService grannyService;
        private Neuron @class;

        /// <summary>
        /// Constructs a ClassInstanceNeuronsRetriever.
        /// </summary>
        /// <param name="grannyService"></param>
        public ClassInstanceNeuronsRetriever(IGrannyService grannyService)
        {
            AssertionConcern.AssertArgumentNotNull(grannyService, nameof(grannyService));

            this.grannyService = grannyService;
        }

        /// <summary>
        /// Gets Instance neurons.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="value"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Neuron>> GetInstanceNeuronsAsync<TValue>(
            Network value, 
            CancellationToken token = default
        )
            where TValue : class, new()
        {
            AssertionConcern.AssertArgumentNotNull(value, nameof(value));
            AssertionConcern.AssertStateTrue(this.@class != null, "'Class' need to be initialized prior to instance neurons retrieval.");

            // TODO: use GrannyCacheService
            var instantiatesClassResult = await this.grannyService.TryGetParseAsync(
                new InstantiatesClassGrannyInfo(new Coding.d23.neurULization.Processors.Readers.Deductive.InstantiatesClassParameterSet(this.@class))
            );

            AssertionConcern.AssertStateTrue(
                instantiatesClassResult.Success,
                $"InstantiatesClass Granny is required to invoke {nameof(IneurULizer.DeneurULize)}"
            );

            this.@class = null;

            return value.GetPresynapticNeurons(instantiatesClassResult.Granny.Neuron.Id);
        }

        /// <summary>
        /// Initializes the retriever.
        /// </summary>
        /// <param name="value"></param>
        public void Initialize(Neuron value)
        {
            this.@class = value;
        }
    }
}
