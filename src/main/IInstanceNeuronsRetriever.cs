using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    public interface IInstanceNeuronsRetriever
    {
        Task<IEnumerable<Neuron>> GetInstanceNeuronsAsync<TValue>(Network value, CancellationToken token = default)
            where TValue : class, new();
    }

    public interface IInstanceNeuronsRetriever<T> : IInstanceNeuronsRetriever
    {
        void Initialize(T value);
    }
}
