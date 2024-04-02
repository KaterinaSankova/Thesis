using System.Windows.Input;
using TSP.Stores;
using System.ComponentModel;
using System.Collections.ObjectModel;
using TSP.Commands;

namespace TSP.ViewModels
{
    public class ResultsViewModel : ViewModelBase
    {
        private NavigationStore _navigationStore;
        private ObservableCollection<AlgorithmResultViewModel> _algoResults = new();
        private string _message = "";
        private bool _isInputValid = false;
        private bool _calculationsFinished = true;
        private bool _canStartCalculations = false;

        public NavigationStore NavigationStore { get => _navigationStore; set => _navigationStore = value; }

        public ObservableCollection<AlgorithmResultViewModel> AlgoResults
        {
            get
            {
                return _algoResults;
            }
            set
            {
                _algoResults = value;
                OnPropertyChanged(nameof(AlgoResults));
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public bool IsInputValid
        {
            get => _isInputValid;
            set
            {
                _isInputValid = value;
                CanStartCalculations = _isInputValid && CalculationsFinished;
                OnPropertyChanged(nameof(IsInputValid));
            }
        }


        public bool CalculationsFinished
        {
            get
            {
                return _calculationsFinished;
            }
            set
            {
                _calculationsFinished = value;
                CanStartCalculations = IsInputValid && _calculationsFinished;
                OnPropertyChanged(nameof(CalculationsFinished));
                OnPropertyChanged(nameof(CalculationsStarted));
            }
        }

        public bool CalculationsStarted => !CalculationsFinished;

        public bool CanStartCalculations
        {
            get
            {
                return _canStartCalculations;
            }
            set
            {
                _canStartCalculations = value;
                OnPropertyChanged(nameof(CanStartCalculations));
            }
        }

        public ICommand StartCalculations { get; }

        public ICommand CancelCalculations { get; }

        public ResultsViewModel(NavigationStore navigationStore, CancellationTokenStore cancelationTokenStore)
        {
            this._navigationStore = navigationStore;
            StartCalculations = new StartCalculationsCommand(this, cancelationTokenStore);
            CancelCalculations = new CancelCalculationsCommand(this, cancelationTokenStore);

            _navigationStore.CurrentViewModel.PropertyChanged += OnInputChanged;
            navigationStore.CurrentViewModelChanged += OnInputChanged;
        }

        public void WriteMessage(string message) => Message = message;

        public void AddMessage(string message) => Message += message;

        public void RemoveLastMessage()
        {
            int indexOfNewLine = Message.IndexOf('\n');
            if (indexOfNewLine != -1)
                Message = Message[..indexOfNewLine];
            else
                Message = "";
        }

        private void OnInputChanged(object? sender, PropertyChangedEventArgs e) => IsInputValid = ValidateInput();

        private bool ValidateInput()
        {
            if (NavigationStore.CurrentViewModel.GetType().Name == nameof(FileInputViewModel))
            {
                if (string.IsNullOrEmpty(((FileInputViewModel)NavigationStore.CurrentViewModel).SourceFilePath))
                    return false;
                else
                    return true;
            }
            else if (NavigationStore.CurrentViewModel.GetType().Name == nameof(FolderInputViewModel))
            {
                if (string.IsNullOrEmpty(((FolderInputViewModel)NavigationStore.CurrentViewModel).SourceFolderPath))
                    return false;
                else
                    return true;
            }
            else
            {
                return true;
            }
        }

        private void OnInputChanged()
        {
            _navigationStore.CurrentViewModel.PropertyChanged += OnInputChanged;
            IsInputValid = ValidateInput();
        }
    }
}