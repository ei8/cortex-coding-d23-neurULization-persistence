using ei8.Cortex.Coding.d23.Grannies;
using ei8.Cortex.Coding.d23.neurULization.Processors.Readers.Deductive;
using ei8.Cortex.Coding.d23.neurULization.Processors.Writers;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    public class PropertyValueAssociationGrannyInfo : IGrannyInfo<
        IPropertyValueAssociation,
        IPropertyValueAssociationReader,
        IPropertyValueAssociationParameterSet,
        IPropertyValueAssociationWriter
    >
    {
        public PropertyValueAssociationGrannyInfo(IPropertyValueAssociationParameterSet parameters)
        {
            this.Parameters = parameters;
        }

        public IPropertyValueAssociationParameterSet Parameters { get; }
    }
}
