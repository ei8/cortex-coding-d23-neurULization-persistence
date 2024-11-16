using ei8.Cortex.Coding.d23.Grannies;
using ei8.Cortex.Coding.d23.neurULization.Processors.Readers.Deductive;
using ei8.Cortex.Coding.d23.neurULization.Processors.Writers;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    public class PropertyAssociationGrannyInfo : IGrannyInfo<
        IPropertyAssociation,
        IPropertyAssociationReader,
        IPropertyAssociationParameterSet,
        IPropertyAssociationWriter
    >
    {
        public PropertyAssociationGrannyInfo(IPropertyAssociationParameterSet parameters)
        {
            this.Parameters = parameters;
        }

        public IPropertyAssociationParameterSet Parameters { get; }
    }
}
