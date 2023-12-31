using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Sentinel.Validator.POC.Repo
{
    public static class PropertyInfoHelpers
    {
        public static PropertyInfo GetKeyProperty<T>()
        {

            var keyProp = typeof(T).GetProperties().SingleOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Any());

            if (keyProp == null)
            {
                throw new ArgumentException("KeyAttribute is mising for " + typeof(T).ToString());
            }
            return keyProp;
        }

        public static TKey? GetKeyValue<TKey, TValue>(TValue item)
        {
            var keyProp = GetKeyProperty<TValue>();

            var key = keyProp.GetValue(item);
            return (TKey?)key;
        }

        internal static PropertyInfo[] GetProperties<T>()
        {
            return typeof(T).GetProperties();
        }
    }
}