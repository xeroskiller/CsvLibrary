using System;

namespace CsvLibrary
{
    /// <summary>
    /// Represents field-level CSV serialization spec. Applies to a field or property, allows
    /// simple serialization/deserialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class CsvFieldAttribute : Attribute
    {
        /// <summary>
        /// Required position of this value in a record, if applicable. Negative value are
        /// disallowed, and treated as int.MaxValue
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Required Header for this value in a record, if applicable.
        /// </summary>
        public string FieldName { get; private set; }

        /// <summary>
        /// Format string to be applied when written or read.
        /// </summary>
        public string FormatString { get; set; }

        public CsvFieldAttribute(int index = 0, string formatString = null)
        {
            Index = index < 0 ? int.MaxValue : index;
            FormatString = formatString;
        }
    }


}
