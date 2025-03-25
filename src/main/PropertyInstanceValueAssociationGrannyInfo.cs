using ei8.Cortex.Coding.d23.Grannies;
using ei8.Cortex.Coding.d23.neurULization.Processors.Readers.Deductive;
using ei8.Cortex.Coding.d23.neurULization.Processors.Writers;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    public class PropertyInstanceValueAssociationGrannyInfo : IGrannyInfo<
        IPropertyInstanceValueAssociation,
        IPropertyInstanceValueAssociationReader,
        IPropertyInstanceValueAssociationParameterSet,
        IPropertyInstanceValueAssociationWriter
    >
    {
        public PropertyInstanceValueAssociationGrannyInfo(IPropertyInstanceValueAssociationParameterSet parameters)
        {
            this.Parameters = parameters;
        }

        public IPropertyInstanceValueAssociationParameterSet Parameters { get; }
    }
}
