using neurUL.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    /// <summary>
    /// Provides functionalities for InstanceNeuronsRetrievers that 
    /// retrieve Id-based instances. 
    /// </summary>
    public class IdInstanceNeuronsRetriever : IIdInstanceNeuronsRetriever
    {
        private IEnumerable<Guid> ids;

        /// <summary>
        /// Constructs an InstanceNeuronsRetriever that retrieves 
        /// Id-based instances.
        /// </summary>
        public IdInstanceNeuronsRetriever()
        {
            this.ids = null;
        }

        /// <summary>
        /// Gets Instance neurons.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="value"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public Task<IEnumerable<Neuron>> GetInstanceNeuronsAsync<TValue>(Network value, CancellationToken token = default) where TValue : class, new()
        {
            AssertionConcern.AssertArgumentNotNull(value, nameof(value));
            AssertionConcern.AssertStateTrue(this.ids != null, "'IDs' need to be initialized prior to instance neurons retrieval.");

            var result = ids.Select(
                id => value.TryGetById(id, out Coding.Neuron instanceValueGrannyNeuron) ? 
                    instanceValueGrannyNeuron : 
                    throw new InvalidOperationException($"Neuron with Id '{id}' was not found in specified network.")
            );

            this.ids = null;

            return Task.FromResult(result);
        }

        /// <summary>
        /// Initializes the retriever.
        /// </summary>
        /// <param name="value"></param>
        public void Initialize(IEnumerable<Guid> value)
        {
            this.ids = value;
        }
    }
}
