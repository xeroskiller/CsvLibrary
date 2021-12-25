using System;
using System.Collections.Generic;
using System.Reflection;

namespace CsvLibrary
{
    /// <summary>
    /// Pseudo-record type to hold any available info about csv field serialization
    /// </summary>
    internal struct CsvPropertyInfo
    {
        // PropertyInfo object for this specific field
        public PropertyInfo propertyInfo;

        // Attribute applied to this field, if any
        public CsvFieldAttribute csvFieldAttribute;

        // Getter for this property
        public MethodInfo fieldGetter;

        // Setter for this property
        public MethodInfo fieldSetter;

        public CsvPropertyInfo(PropertyInfo pi, CsvFieldAttribute attr, MethodInfo getter, MethodInfo setter)
        {
            propertyInfo = pi;
            csvFieldAttribute = attr;
            fieldGetter = getter;
            fieldSetter = setter;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) throw new ArgumentNullException(nameof(obj));

            return obj is CsvPropertyInfo other &&
                   EqualityComparer<PropertyInfo>.Default.Equals(propertyInfo, other.propertyInfo) &&
                   EqualityComparer<CsvFieldAttribute>.Default.Equals(csvFieldAttribute, other.csvFieldAttribute) &&
                   EqualityComparer<MethodInfo>.Default.Equals(fieldGetter, other.fieldGetter) &&
                   EqualityComparer<MethodInfo>.Default.Equals(fieldSetter, other.fieldSetter);
        }

        public override int GetHashCode() 
            => HashCode.Combine(propertyInfo, csvFieldAttribute, fieldGetter, fieldSetter);


        // The following methods are convenient for treating these four values as a tuple
        public void Deconstruct(out PropertyInfo pi, out CsvFieldAttribute attr, out MethodInfo getter, out MethodInfo setter)
        {
            pi = propertyInfo;
            attr = csvFieldAttribute;
            getter = fieldGetter;
            setter = fieldSetter;
        }

        public static implicit operator (PropertyInfo pi, CsvFieldAttribute attr, MethodInfo getter, MethodInfo setter)
            (CsvPropertyInfo value) 
            => (value.propertyInfo, value.csvFieldAttribute, value.fieldGetter, value.fieldSetter);

        public static implicit operator CsvPropertyInfo(
            (PropertyInfo pi, CsvFieldAttribute attr, MethodInfo getter, MethodInfo setter) value) 
            => new CsvPropertyInfo(value.pi, value.attr, value.getter, value.setter);
    }
}
