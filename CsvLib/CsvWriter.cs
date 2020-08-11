using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvLibrary
{
    public class CsvWriter<T> : CsvActor<T>, IDisposable where T : new()
    {
        // Stream writer for text output
        private readonly StreamWriter _writer;
        private bool _haveWrittenHeader = false;

        /// <summary>
        /// CsvWrite to reflect on type parameter and write delimited files
        /// </summary>
        /// <param name="filePath">Path to output file</param>
        /// <param name="append">bool to indicate appending to the given file</param>
        public CsvWriter(string filePath, bool append = false)
        {
            // Check if the file exists already - store as we need it more than once
            var fileExists = File.Exists(filePath);

            // Throw if it exists and were not supposed to append
            // TODO :: Maybe just make it overwrite? maybe dangerous
            if (fileExists && !append) throw new InvalidOperationException("Cannot overwrite existing file");

            // Open a new streamwriter
            _writer = new StreamWriter(filePath, append);

            // This ensures we write a header, if needed... i think
            _haveWrittenHeader = !_typeAttr.HasHeader || fileExists || !append;
        }

        /// <summary>
        /// Writes the headers as defined on attributes for the type
        /// </summary>
        public void WriteHeader()
        {
            // Have we / should we write header?
            if (!_haveWrittenHeader)
            {
                // Use reflected data to get and append-join header on the delimiter
                _writer.Write(
                    new StringBuilder().AppendJoin(_typeAttr.Delimiter, _fields.Select(fld => fld.pi.Name)).ToString());

                // Flip the flag to make sure we dont write it again
                _haveWrittenHeader = true;
            }
        }

        /// <summary>
        /// Writes a single record to CSV file using attribute-specified
        /// parameters, specifically <see cref="CsvTypeAttribute"/> for type and file
        /// level parameters, and <see cref="CsvFieldAttribute"/> for field parameters.
        /// </summary>
        /// <param name="record">Record of given type to write to file</param>
        public void WriteRecord(T record)
        {
            // Write header if it hasnt already been written
            if (!_haveWrittenHeader) WriteHeader();

            // Stringbuilder to avoid tons of allocations
            var sb = new StringBuilder();
            
            // Append fields across the delimiter
            sb.AppendJoin(_typeAttr.Delimiter, _fields.Select(f => _FieldToString(f, record)));

            // Write line to file
            _writer.WriteLine(sb.ToString());
        }

        public async Task WriteRecordAsync(T record)
        {
            // Write header if it hasnt already been written
            if (!_haveWrittenHeader) WriteHeader();

            // Stringbuilder to avoid tons of allocations
            var sb = new StringBuilder();

            // Append fields across the delimiter
            sb.AppendJoin(_typeAttr.Delimiter, _fields.Select(f => _FieldToString(f, record)));

            // Write line to file
            await _writer.WriteLineAsync(sb.ToString());
        }

        public void WriteRecords(IEnumerable<T> records)
        {
            // Write header if it hasnt already been written
            if (!_haveWrittenHeader) WriteHeader();

            // Stringbuilder to avoid tons of allocations
            var sb = new StringBuilder();

            // Loop over given records
            foreach (var record in records)
            {
                // Append fields across the delimiter
                sb.AppendJoin(_typeAttr.Delimiter, _fields.Select(f => _FieldToString(f, record)));

                // Write line to file
                _writer.WriteLine(sb.ToString());

                // Clear the stringbuilder
                sb.Clear();
            }
        }

        public async Task WriteRecordsAsync(IAsyncEnumerable<T> records)
        {
            // Write header if it hasnt already been written
            if (!_haveWrittenHeader) WriteHeader();

            // Stringbuilder to avoid tons of allocations
            var sb = new StringBuilder();
            
            // Loop over given records
            await foreach (var record in records)
            {
                // Append fields across the delimiter
                sb.AppendJoin(_typeAttr.Delimiter, _fields.Select(f => _FieldToString(f, record)));

                // Write line to file
                await _writer.WriteLineAsync(sb.ToString());

                // Clear the stringbuilder
                sb.Clear();
            }
        }

        public void Dispose()
        {
            // Close the writer too
            _writer.Dispose();
        }

        // Custom mathod to handle formatting
        private string _FieldToString(CsvPropertyInfo f, object record)
            => f.pi.PropertyType == typeof(DateTime)
                ? (f.getter.Invoke(record, null) as DateTime?)?.ToString(f.attr.FormatString) ?? string.Empty
                : f.getter.Invoke(record, null).ToString();
    }
}
