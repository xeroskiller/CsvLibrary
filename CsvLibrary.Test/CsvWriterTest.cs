using CsvLibrary;
using FluentAssertions;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace CsvLibrary.Test
{
    public class CsvWriterTest
    {
        internal const string _DATETIME_FORMAT = "yyyyMMdd";

        [Fact]
        public void CsvWriter_WritesTypeAsExpected_Simple()
        {
            var testData = Enumerable.Range(1, 1).Select(n => new TestCsvType(n));
            var outputFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            using (var writer = new CsvWriter<TestCsvType>(outputFile))
                writer.WriteRecords(testData);

            var actual = File.ReadAllLines(outputFile);

            var expected = new[]
            {
                "test_bool,test_datetime,test_decimal,test_double,test_int,test_long,test_string",
                "False,19000102,11,7,1,2,5"
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("19000101")]
        [InlineData("20200810")]
        public void Test(string str)        
        {
            var dt = DateTime.ParseExact(str, _DATETIME_FORMAT, null);
            var outputFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            var writer = new CsvWriter<TestCsvType>(outputFile);

            var prp = typeof(TestCsvType).GetProperty("test_datetime");
            CsvPropertyInfo testPrp = (prp, null, prp.GetGetMethod(), prp.GetSetMethod());

            var obj = new TestCsvType() { test_datetime = dt };

            var result = writer._FieldToString(testPrp, obj);

            Assert.Equal(result, str);
        }
    }

    [CsvType(DateTimeFormatString = CsvWriterTest._DATETIME_FORMAT)]
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
