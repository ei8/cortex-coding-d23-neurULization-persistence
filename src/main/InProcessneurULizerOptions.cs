using ei8.Cortex.Coding.d23.neurULization.Implementation;
using ei8.Cortex.Coding.d23.neurULization.Processors.Readers.Inductive;
using ei8.Cortex.Coding.d23.neurULization.Processors.Writers;
using neurUL.Common.Domain.Model;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    public class InProcessneurULizerOptions : Id23neurULizerOptions
    {
        public IInstanceWriter InstanceWriter { get; }

        public IInstanceReader InductiveInstanceReader { get; }

        public IIdInstanceValueWriter IdInstanceValueWriter { get; }

        public IInstanceValueReader InductiveInstanceValueReader { get; }

        public IMirrorSet Mirrors { get; }

        public InProcessneurULizerOptions(
            IInstanceWriter instanceWriter,
            IInstanceReader inductiveInstanceReader,
            IIdInstanceValueWriter idInstanceValueWriter,
            IInstanceValueReader inductiveInstanceValueReader,
            IMirrorSet mirrors
        )
        {
            AssertionConcern.AssertArgumentNotNull(instanceWriter, nameof(instanceWriter));
            AssertionConcern.AssertArgumentNotNull(inductiveInstanceReader, nameof(inductiveInstanceReader));
            AssertionConcern.AssertArgumentNotNull(idInstanceValueWriter, nameof(idInstanceValueWriter));
            AssertionConcern.AssertArgumentNotNull(inductiveInstanceValueReader, nameof(inductiveInstanceValueReader));
            AssertionConcern.AssertArgumentNotNull(mirrors, nameof(mirrors));

            this.InstanceWriter = instanceWriter;
            this.InductiveInstanceReader = inductiveInstanceReader;
            this.IdInstanceValueWriter = idInstanceValueWriter;
            this.InductiveInstanceValueReader = inductiveInstanceValueReader;
            this.Mirrors = mirrors;
        }
    }
}
