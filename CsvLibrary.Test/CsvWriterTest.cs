using CsvLibrary;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace CsvLibrary.Test
{
    public class CsvWriterTest
    {
        internal const string _DATETIME_FORMAT = "yyyyMMdd";
        public static IEnumerable<object[]> Data =>
            new List<object[]>
            {
                    new object[] { new DateTime(1900, 1, 1).ToString(_DATETIME_FORMAT) }
                ,   new object[] { new DateTime(2020, 8, 10).ToString(_DATETIME_FORMAT) }
            };

        [Fact]
        public void CsvWriter_WritesTypeAsExpected_Simple()
        {
            var testData = Enumerable.Range(1, 2).Select(n => new TestCsvType(n));
            var outputFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            using (var writer = new CsvWriter<TestCsvType>(outputFile))
                writer.WriteRecords(testData);

            var actual = File.ReadAllLines(outputFile);

            var expected = new[]
            {
                "test_bool,test_datetime,test_decimal,test_double,test_int,test_long,test_string",
                "False,19000102,11,7,1,2,5",
                "True,19000103,22,14,2,4,10"
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void CsvWriter_ConvertsDateTimeProperty_FollowsTypeLevelFormat(DateTime dt)        
        {

            var str = dt.ToString(_DATETIME_FORMAT);
            // var dt = DateTime.ParseExact(str, _DATETIME_FORMAT, null);
            var outputFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            var writer = new CsvWriter<TempTypeFormat>(outputFile);

            var prp = typeof(TempTypeFormat).GetProperty("test_datetime");
            CsvPropertyInfo testPrp = (prp, null, prp.GetGetMethod(), prp.GetSetMethod());

            var obj = new TestCsvType() { test_datetime = dt };

            var result = writer._FieldToString(testPrp, obj);

            Assert.Equal(result, str);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void CsvWriter_ConvertsDateTimeProperty_FollowsFieldLevelFormat(DateTime dt)
        {

            var str = dt.ToString(_DATETIME_FORMAT);
            // var dt = DateTime.ParseExact(str, _DATETIME_FORMAT, null);
            var outputFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            var writer = new CsvWriter<TempTypeFormat>(outputFile);

            var prp = typeof(TempTypeFormat).GetProperty("test_datetime");
            CsvPropertyInfo testPrp = (prp, null, prp.GetGetMethod(), prp.GetSetMethod());

            var obj = new TestCsvType() { test_datetime = dt };

            var result = writer._FieldToString(testPrp, obj);

            Assert.Equal(result, str);
        }

        [CsvType(DateTimeFormatString = _DATETIME_FORMAT)]
        class TempTypeFormat { public DateTime test_datetime { get; set; } }

        [CsvType] class TempFieldFormat { [CsvField(FormatString = "yyyy-MM-dd")] public DateTime test_datetime { get; set; } }
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
