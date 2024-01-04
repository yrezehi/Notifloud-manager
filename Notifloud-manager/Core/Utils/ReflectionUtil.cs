using Core.Models.Abstracts.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Reflection;

namespace Core.Utils
{
    public static class ReflectionUtil
    {
        public static object GetValueOf(object targetObject, string propertyName) =>
        targetObject.GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)!.GetValue(targetObject, null)!;

        public static void SetValueOf(object targetObject, string propertyName, object value) =>
            targetObject.GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)!.SetValue(targetObject, value);

        public static bool ContainsProperty(object targetObject, string propertyName) =>
            targetObject.GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) != null;

        public static IEnumerable<PropertyInfo> GetInterfacedObjectProperties(Type type) =>
            (new Type[] { type })
                .Concat(type.GetInterfaces())
                    .SelectMany(interfaceProperty => interfaceProperty.GetProperties());

        public static IEnumerable<PropertyInfo> GetObjectProperties(Type type) =>
            (new Type[] { type })
                .SelectMany(interfaceProperty => interfaceProperty.GetProperties());

        public static IEntity MapEntity<T>(IEntity entity, IEntity entityToUpdate)
        {
            var dtoProperties = GetInterfacedObjectProperties(entity.GetType());

            foreach (var property in dtoProperties)
            {
                var dtoPropertyValue = GetValueOf(entity, property.Name);

                if (!IsNullOrDefault(dtoPropertyValue))
                {
                    if (ContainsProperty(entityToUpdate, property.Name))
                        SetValueOf(entityToUpdate, property.Name, dtoPropertyValue);
                }
            }

            return entityToUpdate;
        }

        public static bool IsNullOrDefault<T>(T argument)
        {
            if (argument == null)
                return true;

            if (Equals(argument, default(T)))
                return true;

            if (argument is int && Convert.ToInt32(argument) == 0)
                return true;

            Type methodType = typeof(T);
            if (Nullable.GetUnderlyingType(methodType) != null)
                return false;

            // deal with boxed value types
            Type argumentType = argument.GetType();
            if (argumentType.IsValueType && argumentType != methodType)
            {
                object objectInstance = Activator.CreateInstance(argument.GetType())!;
                return objectInstance.Equals(argument);
            }

            return false;
        }

        public static PropertyInfo[] GetEntityProperties<TEntity>(DbContext context) where TEntity : class
        {
            var entityType = context.Model.FindEntityType(typeof(TEntity));

            var navigationProperties = GetNavigationProperties(entityType!).ToList();

            var secondLevelNavigationProperties = new List<INavigation>();

            foreach (var navigationProperty in navigationProperties)
            {
                var targetEntityType = navigationProperty.TargetEntityType;

                var secondLevelNavigationProps = GetNavigationProperties(targetEntityType);

                secondLevelNavigationProperties.AddRange(secondLevelNavigationProps);
            }

            navigationProperties.AddRange(secondLevelNavigationProperties);

            var properties = navigationProperties.Select(p => p.PropertyInfo).ToArray();

            return properties;
        }

        private static IEnumerable<INavigation> GetNavigationProperties(IEntityType entityType)
        {
            return entityType.GetNavigations();
        }
    }
}
