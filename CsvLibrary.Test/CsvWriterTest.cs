using CsvLibrary;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace CsvLibrary.Test
{
    public class CsvWriterTest
    {
        [Fact]
        public void Test1()
        {
            var testData = Enumerable.Range(1, 100).Select(n => new TestCsvType(n));
            var outputFile = Path.GetTempFileName();

            using (var writer = new CsvWriter<TestCsvType>(outputFile))
                writer.WriteRecords(testData);
        }
    }

    [CsvType]
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
            test_datetime = DateTime.Now.AddDays(n);
            test_bool = n % 2 == 0;
        }
    }
}
