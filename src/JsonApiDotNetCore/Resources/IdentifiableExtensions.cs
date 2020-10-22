using System;
using System.Reflection;

namespace JsonApiDotNetCore.Resources
{
    public static class IdentifiableExtensions
    {
        internal static object GetTypedId(this IIdentifiable identifiable)
        {
            if (identifiable == null) throw new ArgumentNullException(nameof(identifiable));
            
            PropertyInfo property = identifiable.GetType().GetProperty(nameof(Identifiable.Id));
            
            if (property == null)
            {
                throw new InvalidOperationException($"Resource of type '{identifiable.GetType()}' does not have an Id property.");
            }

            return property.GetValue(identifiable);
        }
    }
}
