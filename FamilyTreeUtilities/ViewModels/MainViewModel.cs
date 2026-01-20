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
            SameLastNameCommand = new RelayCommand(ExecuteSameLastName, CanExecuteFileCommands);

            DisplayText = "Welcome to Ron's Family Tree Utilities";
        }

        public ICommand NewCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand OneNameCommand { get; }
        public ICommand TitleCommand { get; }
        public ICommand SameLastNameCommand { get; }

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

        private void ExecuteSameLastName(object parameter)
        {
            if (string.IsNullOrEmpty(_currentFilePath))
                return;

            try
            {
                var individuals = new System.Collections.Generic.Dictionary<string, PersonInfo>();
                var families = new System.Collections.Generic.List<FamilyInfo>();
                var lines = System.IO.File.ReadAllLines(_currentFilePath);

                string currentId = null;
                string currentType = null;
                PersonInfo currentPerson = null;
                FamilyInfo currentFamily = null;

                // First pass: collect all individuals and families
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();

                    // Check for individual record
                    if (line.StartsWith("0") && line.Contains("INDI"))
                    {
                        currentId = line.Split(' ')[1].Replace("@", "");
                        currentPerson = new PersonInfo { Id = currentId };
                        individuals[currentId] = currentPerson;
                        currentType = "INDI";
                        currentFamily = null;
                    }
                    // Check for family record
                    else if (line.StartsWith("0") && line.Contains("FAM"))
                    {
                        currentId = line.Split(' ')[1].Replace("@", "");
                        currentFamily = new FamilyInfo { Id = currentId };
                        families.Add(currentFamily);
                        currentType = "FAM";
                        currentPerson = null;
                    }
                    // Process individual data
                    else if (currentType == "INDI" && currentPerson != null)
                    {
                        if (line.StartsWith("1 NAME"))
                        {
                            string fullName = line.Substring(7).Trim();
                            currentPerson.AllNames.Add(fullName);

                            // Extract surname from between slashes
                            int firstSlash = fullName.IndexOf('/');
                            int lastSlash = fullName.LastIndexOf('/');

                            if (firstSlash >= 0 && lastSlash > firstSlash)
                            {
                                string lastName = fullName.Substring(firstSlash + 1, lastSlash - firstSlash - 1).Trim();
                                if (!string.IsNullOrWhiteSpace(lastName))
                                {
                                    currentPerson.LastNames.Add(lastName);
                                }
                            }
                        }
                        else if (line.StartsWith("1 SEX"))
                        {
                            currentPerson.Sex = line.Substring(6).Trim();
                        }
                    }
                    // Process family data
                    else if (currentType == "FAM" && currentFamily != null)
                    {
                        if (line.StartsWith("1 HUSB"))
                        {
                            currentFamily.HusbandId = line.Split('@')[1];
                        }
                        else if (line.StartsWith("1 WIFE"))
                        {
                            currentFamily.WifeId = line.Split('@')[1];
                        }
                    }
                }

                // Second pass: find wives where ALL last names match husband's last name
                var results = new System.Collections.Generic.List<string>();

                foreach (var family in families)
                {
                    if (!string.IsNullOrEmpty(family.HusbandId) &&
                        !string.IsNullOrEmpty(family.WifeId) &&
                        individuals.ContainsKey(family.HusbandId) &&
                        individuals.ContainsKey(family.WifeId))
                    {
                        var husband = individuals[family.HusbandId];
                        var wife = individuals[family.WifeId];

                        // Husband must have at least one last name
                        if (husband.LastNames.Count == 0 || wife.LastNames.Count == 0)
                            continue;

                        // Get husband's last name (use first one if multiple)
                        string husbandLastName = husband.LastNames[0];

                        // Check if ALL of wife's last names match the husband's last name
                        bool allNamesMatch = wife.LastNames.All(ln =>
                            ln.Equals(husbandLastName, System.StringComparison.OrdinalIgnoreCase));

                        if (allNamesMatch)
                        {
                            string husbandDisplay = husband.AllNames[0].Replace("/", " ").Trim();
                            while (husbandDisplay.Contains("  "))
                                husbandDisplay = husbandDisplay.Replace("  ", " ");

                            // Display all wife's names
                            var wifeNames = new System.Collections.Generic.List<string>();
                            foreach (var name in wife.AllNames)
                            {
                                string display = name.Replace("/", " ").Trim();
                                while (display.Contains("  "))
                                    display = display.Replace("  ", " ");
                                wifeNames.Add(display);
                            }

                            string wifeDisplay = string.Join(", ", wifeNames);
                            results.Add($"{wife.Id}: {wifeDisplay} (married to {husband.Id}: {husbandDisplay})");
                        }
                    }
                }

                // Display results
                if (results.Count > 0)
                {
                    DisplayText = $"Women with Same Last Name as Husband (All Names Match) ({results.Count} found):\n\n";
                    DisplayText += string.Join("\n", results);
                }
                else
                {
                    DisplayText = "No women found where all their last names match their husband's last name.";
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error reading GEDCOM file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private class PersonInfo
        {
            public string Id { get; set; }
            public System.Collections.Generic.List<string> AllNames { get; set; } = new System.Collections.Generic.List<string>();
            public System.Collections.Generic.List<string> LastNames { get; set; } = new System.Collections.Generic.List<string>();
            public string Sex { get; set; }
        }

        private class FamilyInfo
        {
            public string Id { get; set; }
            public string HusbandId { get; set; }
            public string WifeId { get; set; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}