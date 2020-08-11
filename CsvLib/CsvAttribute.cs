using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace CsvLibrary
{
    public class CsvReader<T> : CsvActor<T>, IDisposable where T : new()
    {
        private readonly StreamReader _reader;

        public Action<string> ActionOnFieldcountMismatch { get; set; }
            = s => throw new InvalidOperationException("Field count mismatch.");

        public CsvReader(string filePath)
        {
            _reader = new StreamReader(filePath);
        }

        public T GetRecord()
        {
            T result = new T();

            var line = _reader.ReadLine();
            if (line == null) return default;

            var values = line.Split();

            if (values.Length != _fields.Length)
            {
                ActionOnFieldcountMismatch(line);
                return default;
            }

            for (int i = 0; i < _fields.Length; i++)
            {
                if (_fields[i].pi.PropertyType == typeof(decimal))
                    _fields[i].pi.SetValue(result, decimal.Parse(values[i]));

                else if (_fields[i].pi.PropertyType == typeof(float))
                    _fields[i].pi.SetValue(result, float.Parse(values[i]));

                else if (_fields[i].pi.PropertyType == typeof(int))
                    _fields[i].pi.SetValue(result, int.Parse(values[i]));

                else _fields[i].pi.SetValue(result, values[i]);
            }

            return result;
        }

        public async Task<T> GetRecordAsync()
        {
            T result = new T();

            var line = await _reader.ReadLineAsync();
            if (line == null) return default;

            var values = line.Split();

            if (values.Length != _fields.Length)
            {
                await Task.Run(() => ActionOnFieldcountMismatch(line));
                return default;
            }

            for (int i = 0; i < _fields.Length; i++)
            {
                if (_fields[i].pi.PropertyType == typeof(decimal))
                    _fields[i].pi.SetValue(result, decimal.Parse(values[i]));

                else if (_fields[i].pi.PropertyType == typeof(float))
                    _fields[i].pi.SetValue(result, float.Parse(values[i]));

                else if (_fields[i].pi.PropertyType == typeof(int))
                    _fields[i].pi.SetValue(result, int.Parse(values[i]));

                else _fields[i].pi.SetValue(result, values[i]);
            }

            return result;
        }

        public IEnumerable<T> GetRecords(bool continueOnError = true)
        {
            string line;

            while ((line = _reader.ReadLine()) != null)
            {
                T result = new T();

                if (string.IsNullOrEmpty(line)) break;

                var values = line.Split(_typeAttr.Delimiter);

                if (values.Length != _fields.Length)
                {
                    ActionOnFieldcountMismatch(line);
                    if (continueOnError) yield return default;
                    else break;
                }

                for (int i = 0; i < _fields.Length; i++)
                {
                    if (_fields[i].pi.PropertyType == typeof(decimal))
                        _fields[i].pi.SetValue(result, decimal.Parse(values[i]));

                    else if (_fields[i].pi.PropertyType == typeof(float))
                        _fields[i].pi.SetValue(result, float.Parse(values[i]));

                    else if (_fields[i].pi.PropertyType == typeof(int))
                        _fields[i].pi.SetValue(result, int.Parse(values[i]));

                    else _fields[i].pi.SetValue(result, values[i]);
                }

                yield return result;
            }
        }

        public async IAsyncEnumerable<T> GetRecordsAsync([EnumeratorCancellation] CancellationToken cancelToken, bool continueOnError = true)
        {
            T result = new T();
            string line;

            while ((line = await _reader.ReadLineAsync()) != null && !cancelToken.IsCancellationRequested)
            {
                result = new T();

                if (string.IsNullOrEmpty(line)) break;

                var values = line.Split();

                if (values.Length != _fields.Length)
                {
                    await Task.Run(() => ActionOnFieldcountMismatch(line));

                    if (continueOnError) yield return default;
                    else break;
                }

                for (int i = 0; i < _fields.Length; i++)
                {
                    if (_fields[i].pi.PropertyType == typeof(decimal))
                        _fields[i].pi.SetValue(result, decimal.Parse(values[i]));

                    else if (_fields[i].pi.PropertyType == typeof(float))
                        _fields[i].pi.SetValue(result, float.Parse(values[i]));

                    else if (_fields[i].pi.PropertyType == typeof(int))
                        _fields[i].pi.SetValue(result, int.Parse(values[i]));

                    else _fields[i].pi.SetValue(result, values[i]);
                }

                yield return result;
            }
        }

        public void Dispose()
        {
            _reader.Dispose();
        }
    }


}
