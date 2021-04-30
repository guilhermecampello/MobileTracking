using System;
using System.Linq;

namespace MobileTracking.Core.Application
{
    public class NotFoundException : Exception
    {
        public NotFoundException(Type type)
            : base()
        {
            this.Type = type;
        }

        public NotFoundException(Type type, string message)
            : base(message)
        {
            this.Type = type;
        }

        public NotFoundException(Type type, string message, Exception innerException)
            : base(message, innerException)
        {
            this.Type = type;
        }

        public Type Type { get; }

        public static NotFoundException ById<T>(object id)
        {
            return new NotFoundException(
                typeof(T),
                $"{typeof(T).Name} with id \"{id}\" not found");
        }

        public static NotFoundException ByProperty<T>(
            string propertyName, object value)
        {
            return new NotFoundException(
                typeof(T),
                $"{typeof(T).Name} with {propertyName} \"{value}\" not found");
        }

        public static NotFoundException ByProperties<T>(
            params (string, object)[] properties)
        {
            var type = typeof(T);

            if (properties.Length == 0)
            {
                return new NotFoundException(type, $"{type.Name} not found");
            }

            if (properties.Length == 1)
            {
                var property = properties[0];

                return new NotFoundException(
                    type,
                    $"{type.Name} with {property.Item1} \"{property.Item2}\" not found");
            }

            var message = $"{typeof(T).Name} with ";

            message += string.Join(
                ", ",
                properties
                    .Take(properties.Length - 1)
                    .Select(property =>
                        $"{property.Item1} \"{property.Item2}\""));

            var lastProperty = properties.Last();

            message +=
                $" and {lastProperty.Item1} \"{lastProperty.Item2}\" not found";

            return new NotFoundException(type, message);
        }
    }
}
