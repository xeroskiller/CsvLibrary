using System;

namespace CsvLibrary
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class CsvFieldAttribute : Attribute
    {
        public int? Index { get; set; }
        public string FormatString { get; set; }

        public CsvFieldAttribute(int? index = null, string formatString = null)
        {
            Index = index;
            FormatString = formatString;
        }
    }


}
