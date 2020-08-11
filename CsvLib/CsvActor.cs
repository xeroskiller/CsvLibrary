using System;
using System.Data;
using System.Linq;
using System.Reflection;

namespace CsvHelperTest
{
    public class CsvActor<T>
    {
        // Holds type-level csv data
        internal CsvTypeAttribute _typeAttr;

        // Special construct to capture a property, its CsvFieldAttribute, and both setter and getter
        internal (PropertyInfo pi, CsvFieldAttribute attr, MethodInfo getter, MethodInfo setter)[] _fields;

        internal CsvActor()
        {
            _typeAttr = typeof(T).GetCustomAttribute<CsvTypeAttribute>(true);

            if (_typeAttr == null)
                throw new InvalidOperationException($"Type {nameof(T)} must be flagged as CsvType using CsvTypeAttribute.");

            _fields = ReflectOnType();
        }

        internal static (PropertyInfo pi, CsvFieldAttribute attr, MethodInfo getter, MethodInfo setter)[] ReflectOnType()
            => typeof(T)
                .GetProperties()
                .Select(prop => (pi: prop, attr: prop.GetCustomAttribute<CsvFieldAttribute>(), getter: prop.GetGetMethod(), setter: prop.GetSetMethod()))
                .OrderBy(tpl => tpl.attr?.Index ?? int.MaxValue)
                .ThenBy(tpl => tpl.pi.Name)
                .Where(tpl => tpl.attr != null || tpl.setter != null) // Either has the attribute or is public
                .ToArray();
    }
}
