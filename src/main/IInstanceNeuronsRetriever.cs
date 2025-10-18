using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    /// <summary>
    /// Provides functionalities for retrieving Instance Neurons.
    /// </summary>
    public interface IInstanceNeuronsRetriever
    {
        /// <summary>
        /// Retrieves Instance neurons from the specified Network.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="value"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<Neuron>> GetInstanceNeuronsAsync<TValue>(Network value, CancellationToken token = default)
            where TValue : class, new();
    }

    /// <summary>
    /// Provides functionalities InstanceNeuronsRetrievers
    /// that require initialization.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IInstanceNeuronsRetriever<T> : IInstanceNeuronsRetriever
    {
        /// <summary>
        /// Initializes the retriever with the specified value.
        /// </summary>
        /// <param name="value"></param>
        void Initialize(T value);
    }
}
