using ei8.Cortex.Coding.d23.Grannies;
using ei8.Cortex.Coding.d23.neurULization.Processors.Readers.Deductive;
using ei8.Cortex.Coding.d23.neurULization.Processors.Writers;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    public class InstanceValueGrannyInfo : IGrannyInfo<
        IInstanceValue,
        IInstanceValueReader,
        IInstanceValueParameterSet,
        IInstanceValueWriter
    >
    {
        public InstanceValueGrannyInfo(IInstanceValueParameterSet parameters)
        {
            this.Parameters = parameters;
        }

        public IInstanceValueParameterSet Parameters { get; }
    }
}
