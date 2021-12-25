using System;

namespace CsvLibrary.Test
{
    [CsvType(dateTimeFormatString: CsvWriterTest._DATETIME_FORMAT)]
    internal class TestCsvType
    {
        public int test_int { get; set; }
        public long test_long { get; set; }
        public string test_string { get; set; }
        public double test_double { get; set; }
        public decimal test_decimal { get; set; }
        public DateTime? test_datetime { get; set; }
        public bool test_bool { get; set; }

        public TestCsvType() { }

        public TestCsvType(int n)
        {
            test_int = n;
            test_long = n * 2;
            test_string = (n * 5).ToString();
            test_double = n * 7;
            test_decimal = n * 11;
            test_datetime = (new DateTime(1900, 1, 1)).AddDays(n);
            test_bool = n % 2 == 0;
        }
    }
}
