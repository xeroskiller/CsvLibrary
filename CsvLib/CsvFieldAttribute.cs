using System;

namespace CsvHelperTest
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class CsvFieldAttribute : Attribute
    {
        public int? Index { get; set; }

        public CsvFieldAttribute(int? index = null)
        {
            Index = index;
        }
    }


}
