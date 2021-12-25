using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CsvLibrary.Test
{
    public abstract class TestBase : IDisposable
    {
        private List<string> _disposeFiles = new List<string>();

        internal string GetTempRandomFileName() 
            => Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        public void Dispose()
        {
            
        }
    }
}
