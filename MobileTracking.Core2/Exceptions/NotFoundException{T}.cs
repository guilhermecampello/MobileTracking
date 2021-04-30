using System;
using System.Linq;

namespace MobileTracking.Core.Application
{
    public class NotFoundException<T> : NotFoundException
    {
        public NotFoundException()
            : base(typeof(T))
        {
        }

        public NotFoundException(string message)
            : base(typeof(T), message)
        {
        }

        public NotFoundException(string message, Exception innerException)
            : base(typeof(T), message, innerException)
        {
        }

        public static NotFoundException<T> ById(object id)
        {
            return new NotFoundException<T>(
                $"{typeof(T).Name} with id \"{id}\" not found");
        }

        public static NotFoundException<T> ByProperty(
            string propertyName, object value)
        {
            return new NotFoundException<T>(
                $"{typeof(T).Name} with {propertyName} \"{value}\" not found");
        }

        public static NotFoundException<T> ByProperties(
            params (string, object)[] properties)
        {
            var type = typeof(T).Name;

            if (properties.Length == 0)
            {
                return new NotFoundException<T>($"{type} not found");
            }

            if (properties.Length == 1)
            {
                var property = properties[0];

                return new NotFoundException<T>(
                    $"{type} with {property.Item1} " +
                    $"\"{property.Item2}\" not found");
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

            return new NotFoundException<T>(message);
        }
    }
}
