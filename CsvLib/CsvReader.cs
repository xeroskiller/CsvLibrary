using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace CsvLibrary
{
    /// <summary>
    /// Used to deserialize specced types and their collections to a CSV file.
    /// </summary>
    /// <typeparam name="T">Class specced for deserialization</typeparam>
    public class CsvReader<T> : CsvActor<T>, IDisposable where T : new()
    {
        public static string DefaultFieldDelimiter = ",";

        // Stream for the file being read
        private readonly StreamReader _reader;
        // FieldDelimiter so we only have to figure it out once
        private readonly string _fieldDelimiter;

        // Allows injecting custom action on error
        public Action<string> ActionOnFieldcountMismatch { get; set; }
            = s => throw new InvalidOperationException("Field count mismatch.");

        // Constructors
        public CsvReader(string filePath)
        {
            _reader = new StreamReader(filePath);
            _fieldDelimiter = _typeAttr?.FieldDelimiter ?? DefaultFieldDelimiter;
        }
        public CsvReader(Stream stream)
        {
            _reader = new StreamReader(stream);
            _fieldDelimiter = _typeAttr?.FieldDelimiter ?? DefaultFieldDelimiter;
        }

        // 'Pop' a record off the CSV file
        public T GetRecord()
        {
            // Buffer value
            T result = new T();

            // Read our line in
            var line = _reader.ReadLine();

            // Null on null or empty
            // TODO :: Reconsider?
            if (string.IsNullOrEmpty(line)) return result;

            // Split the line on the appropriate delimiter
            var values = line.Split(_fieldDelimiter);

            // Make sure we have an appropriate field count 
            if (values.Length != _fields.Length)
            {
                // Call error action
                ActionOnFieldcountMismatch(line);

                // Return an empty object
                // TODO :: Maybe add error actions? Bail/Partial/Default?
                return default;
            }

            // Uses the values split out to construct a single object
            _SetValues(result, values);

            // Return the constructed object
            return result;
        }

        public async Task<T> GetRecordAsync()
        {
            // Buffer value
            T result = new T();

            // Read our line in
            var line = await _reader.ReadLineAsync();

            // Null on null or empty
            if (string.IsNullOrEmpty(line)) return default;

            // Split the line on the appropriate delimiter
            var values = line.Split(_fieldDelimiter);

            // Make sure we have an appropriate field count 
            if (values.Length != _fields.Length)
            {
                // Call error action
                await Task.Run(() => ActionOnFieldcountMismatch(line));

                // Return an empty object
                return default;
            }

            // Uses the values split out to construct a single object
            _SetValues(result, values);

            // Return the constructed object
            return result;
        }

        public IEnumerable<T> GetRecords(bool continueOnError = true)
        {
            string line;

            while ((line = _reader.ReadLine()) != null)
            {
                // Buffer value
                T result = new T();

                // Null on null or empty
                if (string.IsNullOrEmpty(line)) break;

                // Split the line on the appropriate delimiter
                var values = line.Split(_fieldDelimiter);

                // Make sure we have an appropriate field count 
                if (values.Length != _fields.Length)
                {
                    // Call error action
                    ActionOnFieldcountMismatch(line);

                    // Return an empty object if were supposed to continue on error
                    if (continueOnError) yield return default;
                    else break;
                }

                // Uses the values split out to construct a single object
                _SetValues(result, values);

                // Yield the constructed object
                yield return result;
            }
        }

        public async IAsyncEnumerable<T> GetRecordsAsync([EnumeratorCancellation] CancellationToken cancelToken, bool continueOnError = true)
        {
            T result;
            string line;

            while ((line = await _reader.ReadLineAsync()) != null && !cancelToken.IsCancellationRequested)
            {
                // Buffer value
                result = new T();

                // Null on null or empty
                if (string.IsNullOrEmpty(line)) break;

                // Split the line on the appropriate delimiter
                var values = line.Split(_fieldDelimiter);

                // Make sure we have an appropriate field count 
                if (values.Length != _fields.Length)
                {
                    // Call error action
                    await Task.Run(() => ActionOnFieldcountMismatch(line));

                    // Return an empty object if were supposed to continue on error
                    if (continueOnError) yield return default;
                    else break;
                }

                // Uses the values split out to construct a single object
                _SetValues(result, values);

                // Yield the constructed object
                yield return result;
            }
        }

        public void Dispose()
        {
            _reader.Dispose();
        }

        private void _SetValues(T result, string[] values)
        {
            for (int i = 0; i < _fields.Length; i++)
            {
                // Look for decimal fields
                if (_fields[i].propertyInfo.PropertyType == typeof(decimal))
                    _fields[i].propertyInfo.SetValue(result, decimal.Parse(values[i]));

                // Float fields
                else if (_fields[i].propertyInfo.PropertyType == typeof(float))
                    _fields[i].propertyInfo.SetValue(result, float.Parse(values[i]));

                // Int fields
                else if (_fields[i].propertyInfo.PropertyType == typeof(int))
                    _fields[i].propertyInfo.SetValue(result, int.Parse(values[i]));

                // Everything else
                else _fields[i].propertyInfo.SetValue(result, values[i]);
            }
        }
    }
}
