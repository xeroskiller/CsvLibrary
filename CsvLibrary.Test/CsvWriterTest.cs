using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace CsvLibrary.Test
{
    public class CsvWriterTest : TestBase
    {
        internal const string _DATETIME_FORMAT = "yyyyMMdd";
        public static IEnumerable<object[]> Data =>
            new List<object[]>
            {
                    new object[] { new DateTime(1900, 1, 1) }
                ,   new object[] { new DateTime(2020, 8, 10) }
            };

        [Fact]
        public void CsvWriter_WritesTypeAsExpected_Simple()
        {
            var testData = Enumerable.Range(1, 2).Select(n => new TestCsvType(n));
            var outputFile = GetTempRandomFileName();

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

            var outputFile = GetTempRandomFileName();

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
            var outputFile = GetTempRandomFileName();

            var writer = new CsvWriter<TempTypeFormat>(outputFile);

            var prp = typeof(TempTypeFormat).GetProperty("test_datetime");
            CsvPropertyInfo testPrp = (prp, null, prp.GetGetMethod(), prp.GetSetMethod());

            var obj = new TestCsvType() { test_datetime = dt };

            var result = writer._FieldToString(testPrp, obj);

            Assert.Equal(result, str);
        }

        [CsvType(dateTimeFormatString: _DATETIME_FORMAT)]
        class TempTypeFormat { public DateTime test_datetime { get; set; } }

        [CsvType] 
        class TempFieldFormat 
        { 
            [CsvField(FormatString = "yyyy-MM-dd")] 
            public DateTime test_datetime { get; set; } 
        }
    }
}
