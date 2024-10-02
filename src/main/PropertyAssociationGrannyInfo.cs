using ei8.Cortex.Coding.d23.Grannies;
using ei8.Cortex.Coding.d23.neurULization.Processors.Readers.Deductive;
using ei8.Cortex.Coding.d23.neurULization.Processors.Writers;
using ei8.Cortex.Library.Common;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    public class PropertyAssociationGrannyInfo : IGrannyInfo<
        IPropertyAssociation,
        IPropertyAssociationReader,
        IPropertyAssociationParameterSet,
        IPropertyAssociationWriter
    >
    {
        public static async Task<PropertyAssociationGrannyInfo> CreateById<TAggregate, TProperty>(
            string propertyName,
            IEnsembleRepository ensembleRepository,
            Guid valueId,
            string appUserId,
            string userId
        ) => new PropertyAssociationGrannyInfo(
            // TODO: create extensionmethod to create PropAssocParams from PropertyInfo and minimal parameters, place in d23.neurULization.Persistence
            new Coding.d23.neurULization.Processors.Readers.Deductive.PropertyAssociationParameterSet(
                await ensembleRepository.GetExternalReferenceAsync(
                    appUserId,
                    typeof(TAggregate).GetProperty(propertyName)
                ),
                (await ensembleRepository.GetByQueryAsync(
                    userId,
                    new NeuronQuery()
                    {
                        Id = new string[] { valueId.ToString() }
                    },
                    int.MaxValue
                )).GetItems<Coding.Neuron>().Single(),
                // TODO: can't this be retrieved from the property granny as it is specified
                // as the first parameter of PropertyAssociationParameterSet
                // or, rather from the [neurULClass(typeof(Avatar))] attribute of the Message class
                await ensembleRepository.GetExternalReferenceAsync(
                    appUserId,
                    typeof(TProperty)
                ),
                ValueMatchBy.Id
            )
        );

        public PropertyAssociationGrannyInfo(IPropertyAssociationParameterSet parameters)
        {
            this.Parameters = parameters;
        }

        public IPropertyAssociationParameterSet Parameters { get; }
    }
}
