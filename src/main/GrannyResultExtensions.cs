using System.Collections.Generic;
using System;
using System.Linq;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    public static class GrannyResultExtensions
    {
        public static string GetTagById(this IEnumerable<GrannyResult> grannyResults, Guid id) =>
            grannyResults.Single(gr => gr.Granny.Neuron.Id == id).Granny.Neuron.Tag;
    }
}
