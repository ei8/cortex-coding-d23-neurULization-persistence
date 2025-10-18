using System;
using System.Collections.Generic;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    /// <summary>
    /// Provides functionalities for InstanceNeuronsRetrievers 
    /// that retrieve Id-based instances.
    /// </summary>
    public interface IIdInstanceNeuronsRetriever : IInstanceNeuronsRetriever<IEnumerable<Guid>>
    {
    }
}
