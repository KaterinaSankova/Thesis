using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP.Models
{
    public class FileInputModel : InputModel
    {
        private string _sourceFilePath;
        private string _resultFilePath;
        private string _outputFolderPath;

        public string SourceFilePath
        {
            get
            {
                return _sourceFilePath;
            }
            set
            {
                _sourceFilePath = value;
            }
        }


        public string ResultFilePath
        {
            get
            {
                return _resultFilePath;
            }
            set
            {
                _resultFilePath = value;
            }
        }

        public string OutputFolderPath
        {
            get
            {
                return _outputFolderPath;
            }
            set
            {
                _outputFolderPath = value;
            }
        }

        public FileInputModel() { }
    }
}
