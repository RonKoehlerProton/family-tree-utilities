using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace FamilyTreeUtilities.Services
{
    public class TitleCodeService
    {
        private readonly string _titleCodesFilePath;

        public TitleCodeService()
        {
            string appRoot = AppDomain.CurrentDomain.BaseDirectory;
            _titleCodesFilePath = Path.Combine(appRoot, "TitleCodesList.txt");
        }

        public List<string> GetTitleCodes()
        {
            var titleCodes = new List<string>();

            if (!File.Exists(_titleCodesFilePath))
            {
                return titleCodes;
            }

            foreach (var line in File.ReadAllLines(_titleCodesFilePath))
            {
                string trimmed = line.Trim();
                // Skip empty lines and comments
                if (!string.IsNullOrWhiteSpace(trimmed) && !trimmed.StartsWith("#"))
                {
                    titleCodes.Add(trimmed);
                }
            }

            return titleCodes;
        }

        public void OpenTitleCodesFile()
        {
            try
            {
                // Create the file if it doesn't exist
                if (!File.Exists(_titleCodesFilePath))
                {
                    File.WriteAllText(_titleCodesFilePath, "# Title Codes - One per line\n# Examples:\nDr.\nMr.\nMrs.\nMs.\nProf.\nRev.\n");
                }

                // Open the file with the default text editor
                var process = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo(_titleCodesFilePath)
                    {
                        UseShellExecute = true
                    }
                };
                process.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Title Codes file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}