using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Helpers
{
    public class TmpFileHelper
    {
        private const string _fileName = @"tmp.jpg";
        public TmpFileHelper() :
          this(Path.GetTempPath())
        { }

        public TmpFileHelper(string directory)
        {
            Create(Path.Combine(directory, _fileName));
        }

        ~TmpFileHelper()
        {
            Delete();
        }

        public void Dispose()
        {
            Delete();
            GC.SuppressFinalize(this);
        }

        public string FilePath { get; private set; }

        private void Create(string path)
        {
            FilePath = path;
            using (File.Create(FilePath)) { };
        }

        private void Delete()
        {
            if (FilePath == null) return;
            File.Delete(FilePath);
            FilePath = null;
        }
    }
}

