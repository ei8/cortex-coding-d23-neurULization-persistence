using ei8.Cortex.Coding.d23.Grannies;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    public interface IGrannyInfo<TGranny, TDeductiveReader, TParameterSet, TWriter>
        where TGranny : IGranny
        where TDeductiveReader : Processors.Readers.Deductive.IGrannyReader<TGranny, TParameterSet>
        where TParameterSet : Processors.Readers.Deductive.IDeductiveParameterSet
        where TWriter : Cortex.Coding.d23.neurULization.Processors.Writers.IGrannyWriter<TGranny, TParameterSet>
    {
        TParameterSet Parameters { get; }
    }
}
