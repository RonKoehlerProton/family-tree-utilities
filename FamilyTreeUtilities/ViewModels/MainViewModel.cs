using System.Windows;
using System.Windows.Input;
using FamilyTreeUtilities.Commands;
using FamilyTreeUtilities.Services;
using FamilyTreeUtilities.Services.Analyzers;

namespace FamilyTreeUtilities.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly GedcomParserService _gedcomParser;
        private readonly TitleCodeService _titleCodeService;
        private readonly OneNameAnalyzer _oneNameAnalyzer;
        private readonly TitleAnalyzer _titleAnalyzer;
        private readonly SameLastNameAnalyzer _sameLastNameAnalyzer;
        private readonly ChildNameMismatchAnalyzer _childNameMismatchAnalyzer;

        private string _displayText;
        private string _currentFilePath;

        public MainViewModel()
        {
            // Initialize services
            _gedcomParser = new GedcomParserService();
            _titleCodeService = new TitleCodeService();
            _oneNameAnalyzer = new OneNameAnalyzer();
            _titleAnalyzer = new TitleAnalyzer(_titleCodeService);
            _sameLastNameAnalyzer = new SameLastNameAnalyzer();
            _childNameMismatchAnalyzer = new ChildNameMismatchAnalyzer();

            // Initialize commands
            NewCommand = new RelayCommand(ExecuteNew);
            OpenCommand = new RelayCommand(ExecuteOpen);
            SaveCommand = new RelayCommand(ExecuteSave);
            ExitCommand = new RelayCommand(ExecuteExit);
            OneNameCommand = new RelayCommand(ExecuteOneName, CanExecuteFileCommands);
            TitleCommand = new RelayCommand(ExecuteTitle);
            TitleCodesCommand = new RelayCommand(ExecuteTitleCodes);
            SameLastNameCommand = new RelayCommand(ExecuteSameLastName, CanExecuteFileCommands);
            ChildNameMismatchCommand = new RelayCommand(ExecuteChildNameMismatch, CanExecuteFileCommands);

            DisplayText = "Welcome to Ron's Family Tree Utilities";
        }

        public ICommand NewCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand OneNameCommand { get; }
        public ICommand TitleCommand { get; }
        public ICommand TitleCodesCommand { get; }
        public ICommand SameLastNameCommand { get; }
        public ICommand ChildNameMismatchCommand { get; }

        public string DisplayText
        {
            get => _displayText;
            set => SetProperty(ref _displayText, value);
        }

        private void ExecuteNew(object parameter)
        {
            MessageBox.Show("New command executed", "Info");
        }

        private void ExecuteOpen(object parameter)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "GEDCOM Files (*.ged)|*.ged|All Files (*.*)|*.*",
                Title = "Open GEDCOM File",
                DefaultExt = ".ged"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _currentFilePath = openFileDialog.FileName;
                DisplayText = $"File loaded: {System.IO.Path.GetFileName(_currentFilePath)}\n\nClick a button to analyze the file.";
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void ExecuteSave(object parameter)
        {
            MessageBox.Show("Save command executed", "Info");
        }

        private void ExecuteExit(object parameter)
        {
            Application.Current.Shutdown();
        }

        private bool CanExecuteFileCommands(object parameter)
        {
            return !string.IsNullOrEmpty(_currentFilePath);
        }

        private void ExecuteOneName(object parameter)
        {
            if (string.IsNullOrEmpty(_currentFilePath))
                return;

            try
            {
                var gedcomData = _gedcomParser.Parse(_currentFilePath);
                var result = _oneNameAnalyzer.Analyze(gedcomData);
                DisplayText = result;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error analyzing file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteTitle(object parameter)
        {
            if (string.IsNullOrEmpty(_currentFilePath))
            {
                MessageBox.Show("Please open a GEDCOM file first.", "No File Loaded", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var gedcomData = _gedcomParser.Parse(_currentFilePath);
                var result = _titleAnalyzer.Analyze(gedcomData);
                DisplayText = result;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error analyzing file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteTitleCodes(object parameter)
        {
            _titleCodeService.OpenTitleCodesFile();
        }

        private void ExecuteSameLastName(object parameter)
        {
            if (string.IsNullOrEmpty(_currentFilePath))
                return;

            try
            {
                var gedcomData = _gedcomParser.Parse(_currentFilePath);
                var result = _sameLastNameAnalyzer.Analyze(gedcomData);
                DisplayText = result;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error analyzing file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteChildNameMismatch(object parameter)
        {
            if (string.IsNullOrEmpty(_currentFilePath))
                return;

            try
            {
                var gedcomData = _gedcomParser.Parse(_currentFilePath);
                var result = _childNameMismatchAnalyzer.Analyze(gedcomData);
                DisplayText = result;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error analyzing file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}