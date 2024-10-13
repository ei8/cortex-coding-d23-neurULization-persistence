using ei8.Cortex.Coding.d23.Grannies;
using ei8.Cortex.Coding.d23.neurULization.Processors.Readers.Deductive;
using ei8.Cortex.Coding.d23.neurULization.Processors.Writers;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    public class ValueGrannyInfo : IGrannyInfo<
        IValue,
        IValueReader,
        IValueParameterSet,
        IValueWriter
    >
    {
        public ValueGrannyInfo(IValueParameterSet parameters)
        {
            this.Parameters = parameters;
        }

        public IValueParameterSet Parameters { get; }
    }
}
