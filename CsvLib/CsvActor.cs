using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace CsvLibrary
{
    public class CsvActor<T>
    {
        // Holds type-level csv data
        internal CsvTypeAttribute _typeAttr;

        // Special construct to capture a property, its CsvFieldAttribute, and both setter and getter
        internal CsvPropertyInfo[] _fields;

        // Home for shared reflection code
        internal CsvActor()
        {
            // Get the type attribute for csvs on the type parameter
            _typeAttr = typeof(T).GetCustomAttribute<CsvTypeAttribute>(true);

            // Required, tho
            if (_typeAttr == null)
                throw new InvalidOperationException($"Type {nameof(T)} must be flagged as CsvType using CsvTypeAttribute.");

            // This is where reflective linq black magic happens. avert thine gaze
            _fields = ReflectOnType();
        }

        internal static CsvPropertyInfo[] ReflectOnType()
            => typeof(T)
                .GetProperties() // All properties for now
                .Select(prop => new CsvPropertyInfo(prop, // struct for important property info
                                                    prop.GetCustomAttribute<CsvFieldAttribute>(),
                                                    prop.GetGetMethod(),
                                                    prop.GetSetMethod()))
                .OrderBy(tpl => tpl.attr?.Index) // Order by index...
                .ThenBy(tpl => tpl.pi.Name) // then by property name
                .Where(tpl => tpl.attr != null || tpl.setter != null) // Either has the attribute or is public (has setter)
                .ToArray();
    }

    internal struct CsvPropertyInfo
    {
        public PropertyInfo pi;
        public CsvFieldAttribute attr;
        public MethodInfo getter;
        public MethodInfo setter;

        public CsvPropertyInfo(PropertyInfo pi, CsvFieldAttribute attr, MethodInfo getter, MethodInfo setter)
        {
            this.pi = pi;
            this.attr = attr;
            this.getter = getter;
            this.setter = setter;
        }

        public override bool Equals(object obj)
        {
            return obj is CsvPropertyInfo other &&
                   EqualityComparer<PropertyInfo>.Default.Equals(pi, other.pi) &&
                   EqualityComparer<CsvFieldAttribute>.Default.Equals(attr, other.attr) &&
                   EqualityComparer<MethodInfo>.Default.Equals(getter, other.getter) &&
                   EqualityComparer<MethodInfo>.Default.Equals(setter, other.setter);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(pi, attr, getter, setter);
        }

        public void Deconstruct(out PropertyInfo pi, out CsvFieldAttribute attr, out MethodInfo getter, out MethodInfo setter)
        {
            pi = this.pi;
            attr = this.attr;
            getter = this.getter;
            setter = this.setter;
        }

        public static implicit operator (PropertyInfo pi, CsvFieldAttribute attr, MethodInfo getter, MethodInfo setter)(CsvPropertyInfo value)
        {
            return (value.pi, value.attr, value.getter, value.setter);
        }

        public static implicit operator CsvPropertyInfo((PropertyInfo pi, CsvFieldAttribute attr, MethodInfo getter, MethodInfo setter) value)
        {
            return new CsvPropertyInfo(value.pi, value.attr, value.getter, value.setter);
        }
    }
}
