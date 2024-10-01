using ei8.Cortex.Coding.d23.Grannies;
using ei8.Cortex.Coding.d23.neurULization.Processors.Readers.Deductive;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    public class InstantiatesClassGrannyInfo : IGrannyInfo<
        IInstantiatesClass,
        Coding.d23.neurULization.Processors.Readers.Deductive.IInstantiatesClassReader,
        Coding.d23.neurULization.Processors.Readers.Deductive.IInstantiatesClassParameterSet,
        Coding.d23.neurULization.Processors.Writers.IInstantiatesClassWriter
    >
    {
        public InstantiatesClassGrannyInfo(IInstantiatesClassParameterSet parameters)
        {
            this.Parameters = parameters;
        }

        public IInstantiatesClassParameterSet Parameters { get; }
    }
}
