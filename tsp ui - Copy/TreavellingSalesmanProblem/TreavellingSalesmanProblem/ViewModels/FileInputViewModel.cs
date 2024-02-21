using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSP.Models;

namespace TSP.ViewModels
{
    public class FileInputViewModel : InputViewModel
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
                OnPropertyChanged(nameof(SourceFilePath));
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
                OnPropertyChanged(nameof(ResultFilePath));
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
                OnPropertyChanged(nameof(OutputFolderPath));
            }
        }

        public FileInputViewModel(ResultsModel results) : base(results) { }
    }
}
