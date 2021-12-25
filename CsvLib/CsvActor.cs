using System;
using System.Data;
using System.Linq;
using System.Reflection;

namespace CsvLibrary
{
    /// <summary>
    /// Super-class for CsvWriter and CsvReader. Contains shared methods and fields.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CsvActor<T>
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

        // Query to turn a type into an array of CsvPropertyInfo objects respecting ordering
        //      and property specs.
        internal static CsvPropertyInfo[] ReflectOnType()
            => typeof(T)
                // All properties for now
                .GetProperties()

                // struct for important property info
                .Select(prop => new CsvPropertyInfo(prop, 
                                                    prop.GetCustomAttribute<CsvFieldAttribute>(),
                                                    prop.GetGetMethod(),
                                                    prop.GetSetMethod()))

                // Order by index...
                .OrderBy(tpl => tpl.csvFieldAttribute?.Index)

                // then by property name
                .ThenBy(tpl => tpl.propertyInfo.Name)

                // Either has the attribute or is public (has setter)
                .Where(tpl => tpl.csvFieldAttribute != null || tpl.fieldSetter != null) 
                .ToArray();
    }
}
