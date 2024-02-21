﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSP.Models;

namespace TSP.ViewModels
{
    public class FolderInputViewModel : InputViewModel
    {
        private string _sourceFolderPath;
        private string _resultFolderPath;
        private string _outputFolderPath;

        public string SourceFolderPath
        {
            get
            {
                return _sourceFolderPath;
            }
            set
            {
                _sourceFolderPath = value;
                OnPropertyChanged(nameof(SourceFolderPath));
            }
        }


        public string ResultFolderPath
        {
            get
            {
                return _resultFolderPath;
            }
            set
            {
                _resultFolderPath = value;
                OnPropertyChanged(nameof(ResultFolderPath));
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

        public FolderInputViewModel(ResultsModel results) : base(results) { }
    }
}
