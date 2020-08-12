using System;

namespace CsvLibrary
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class CsvFieldAttribute : Attribute
    {
        public int Index { get; set; }
        public string FormatString { get; set; }

        public CsvFieldAttribute(int index = 0, string formatString = null)
        {
            Index = index <= 0 ? int.MaxValue : index;
            FormatString = formatString;
        }
    }


}
