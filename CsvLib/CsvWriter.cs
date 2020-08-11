using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CsvHelperTest
{
    public class CsvWriter<T> : CsvActor<T>, IDisposable where T : new()
    {
        private readonly StreamWriter _writer;

        public CsvWriter(string filePath, bool append = false)
        {
            _writer = new StreamWriter(filePath, append);
        }

        public void WriteRecord(T record)
        {
            var sb = new StringBuilder();

            sb.AppendJoin(_typeAttr.Delimiter, _fields.Select(f => f.getter.Invoke(record, null).ToString()));

            _writer.WriteLine(sb.ToString());
        }

        public async Task WriteRecordAsync(T record)
        {
            var sb = new StringBuilder();

            sb.AppendJoin(_typeAttr.Delimiter, _fields.Select(f => f.getter.Invoke(record, null).ToString()));

            await _writer.WriteLineAsync(sb.ToString());
        }

        public void WriteRecords(IEnumerable<T> records)
        {
            var sb = new StringBuilder();

            foreach (var record in records)
            {
                sb.AppendJoin(_typeAttr.Delimiter, _fields.Select(f => f.getter.Invoke(record, null).ToString()));

                _writer.WriteLine(sb.ToString());

                sb.Clear();
            }
        }

        public async Task WriteRecordsAsync(IAsyncEnumerable<T> records)
        {
            var sb = new StringBuilder();

            await foreach (var record in records)
            {
                sb.AppendJoin(_typeAttr.Delimiter, _fields.Select(f => f.getter.Invoke(record, null).ToString()));

                await _writer.WriteLineAsync(sb.ToString());
            }
        }

        public void Dispose()
        {
            _writer.Dispose();
        }
    }
}
