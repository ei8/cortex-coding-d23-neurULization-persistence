using System;
using System.Linq;
using System.Reflection;
using ei8.Cortex.Coding.Properties;
using ei8.Cortex.Coding.Properties.Neuron;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    internal static class ReflectionExtensions
    {
        internal static PropertyData ToPropertyData(this PropertyInfo property, object obj)
        {
            PropertyData result = null;
            var ignore = property.GetCustomAttributes<neurULIgnoreAttribute>().SingleOrDefault();
            if (ignore == null)
            {
                var neuronPropertyAttribute = property.GetCustomAttributes<neurULNeuronPropertyAttribute>().SingleOrDefault();
                if (neuronPropertyAttribute != null)
                {
                    result = ReflectionExtensions.GetNeuronPropertyData(
                        neuronPropertyAttribute, 
                        property.Name, 
                        property.GetValue(obj)
                    );
                }
                else
                {
                    var classAttribute = property.GetCustomAttributes<neurULClassAttribute>().SingleOrDefault();
                    // if property type is Guid and property is decorated by classAttribute
                    var matchBy = property.PropertyType == typeof(Guid) && classAttribute != null ?
                            // match by id
                            ValueMatchBy.Id :
                            // otherwise, match by tag
                            ValueMatchBy.Tag;
                    var propertyValue = property.GetValue(obj)?.ToString();
                    string propertyKey = ExternalReference.ToKeyString(property);

                    result = new PropertyData(
                        propertyKey,
                        // if classAttribute was specified
                        classAttribute?.Type != null ?
                            // use classAttribute type
                            ExternalReference.ToKeyString(classAttribute.Type) :
                            // otherwise, use property type
                            ExternalReference.ToKeyString(property.PropertyType),
                        propertyValue,
                        matchBy
                        );
                }
            }

            return result;
        }

        private static PropertyData GetNeuronPropertyData(neurULNeuronPropertyAttribute neuronPropertyAttribute, string propertyName, object propertyValue)
        {
            PropertyData result = null;

            var neuronPropertyName = neuronPropertyAttribute.PropertyName ?? propertyName;
            INeuronProperty neuronProperty;
            switch (neuronPropertyName)
            {
                case nameof(Neuron.Id):
                    neuronProperty = new IdProperty((Guid)propertyValue, propertyName);
                    break;
                case nameof(Neuron.Tag):
                    neuronProperty = new TagProperty((string)propertyValue, propertyName);
                    break;
                case nameof(Neuron.ExternalReferenceUrl):
                    neuronProperty = new ExternalReferenceUrlProperty((string)propertyValue, propertyName);
                    break;
                case nameof(Neuron.RegionId):
                    neuronProperty = new RegionIdProperty((Guid?)propertyValue, propertyName);
                    break;
                case nameof(Neuron.RegionTag):
                    neuronProperty = new RegionTagProperty((string)propertyValue, propertyName);
                    break;
                case nameof(Neuron.CreationTimestamp):
                    neuronProperty = new CreationTimestampProperty((DateTimeOffset?)propertyValue, propertyName);
                    break;
                case nameof(Neuron.CreationAuthorId):
                    neuronProperty = new CreationAuthorIdProperty((Guid)propertyValue, propertyName);
                    break;
                case nameof(Neuron.CreationAuthorTag):
                    neuronProperty = new CreationAuthorTagProperty((string)propertyValue, propertyName);
                    break;
                case nameof(Neuron.UnifiedLastModificationTimestamp):
                    neuronProperty = new UnifiedLastModificationTimestampProperty((DateTimeOffset?)propertyValue, propertyName);
                    break;
                case nameof(Neuron.UnifiedLastModificationAuthorId):
                    neuronProperty = new UnifiedLastModificationAuthorIdProperty((Guid?)propertyValue, propertyName);
                    break;
                case nameof(Neuron.UnifiedLastModificationAuthorTag):
                    neuronProperty = new UnifiedLastModificationAuthorTagProperty((string)propertyValue, propertyName);
                    break;
                case nameof(Neuron.Url):
                    neuronProperty = new UrlProperty((string)propertyValue, propertyName);
                    break;
                case nameof(Neuron.Version):
                    neuronProperty = new VersionProperty((int)propertyValue, propertyName);
                    break;
                default:
                    throw new NotImplementedException($"Neuron Property '{neuronPropertyName}' not yet implemented.");
            }

            if (neuronProperty != null)
                result = new PropertyData(neuronProperty);
        
            return result;
        }
    }
}
