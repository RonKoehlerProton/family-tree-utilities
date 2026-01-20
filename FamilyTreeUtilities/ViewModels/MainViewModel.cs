using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using FamilyTreeUtilities.Commands;

namespace FamilyTreeUtilities.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _displayText;
        private string _currentFilePath;

        public MainViewModel()
        {
            NewCommand = new RelayCommand(ExecuteNew);
            OpenCommand = new RelayCommand(ExecuteOpen);
            SaveCommand = new RelayCommand(ExecuteSave);
            ExitCommand = new RelayCommand(ExecuteExit);
            OneNameCommand = new RelayCommand(ExecuteOneName, CanExecuteFileCommands);
            TitleCommand = new RelayCommand(ExecuteTitle, CanExecuteFileCommands);

            DisplayText = "Welcome to Ron's Family Tree Utilities";
        }

        public ICommand NewCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand OneNameCommand { get; }
        public ICommand TitleCommand { get; }

        public string DisplayText
        {
            get => _displayText;
            set
            {
                _displayText = value;
                OnPropertyChanged();
            }
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
                DisplayText = $"File loaded: {System.IO.Path.GetFileName(_currentFilePath)}\n\nClick 'One Name' or 'Title' to analyze the file.";
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
                var oneNamePersons = new System.Collections.Generic.List<string>();
                var lines = System.IO.File.ReadAllLines(_currentFilePath);

                string currentPerson = null;
                string currentName = null;

                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();

                    // Check for individual record
                    if (line.StartsWith("0") && line.Contains("INDI"))
                    {
                        currentPerson = line;
                        currentName = null;
                    }
                    // Check for NAME tag
                    else if (line.StartsWith("1 NAME") && currentPerson != null)
                    {
                        currentName = line.Substring(7).Trim();

                        // Replace slashes with spaces, then clean up extra spaces
                        currentName = currentName.Replace("/", " ").Trim();
                        while (currentName.Contains("  "))
                        {
                            currentName = currentName.Replace("  ", " ");
                        }

                        // Check if name has only one word (no spaces after cleanup)
                        if (!string.IsNullOrWhiteSpace(currentName) && !currentName.Contains(" "))
                        {
                            // Extract ID from person line
                            string id = currentPerson.Split(' ')[1].Replace("@", "");
                            oneNamePersons.Add($"{id}: {currentName}");
                        }
                    }
                }

                // Display results
                if (oneNamePersons.Count > 0)
                {
                    DisplayText = $"Persons with One Name Only ({oneNamePersons.Count} found):\n\n";
                    DisplayText += string.Join("\n", oneNamePersons);
                }
                else
                {
                    DisplayText = "No persons with only one name were found in the GEDCOM file.";
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error reading GEDCOM file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void ExecuteTitle(object parameter)
        {
            MessageBox.Show("Title command executed", "Info");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}