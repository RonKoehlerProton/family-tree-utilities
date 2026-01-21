using System.Collections.Generic;
using System.Windows;
using FamilyTreeUtilities.Models;

namespace FamilyTreeUtilities.Services.Analyzers
{
    public class TitleAnalyzer
    {
        private readonly TitleCodeService _titleCodeService;

        public TitleAnalyzer(TitleCodeService titleCodeService)
        {
            _titleCodeService = titleCodeService;
        }

        public string Analyze(GedcomData gedcomData)
        {
            var titleCodes = _titleCodeService.GetTitleCodes();

            if (titleCodes.Count == 0)
            {
                MessageBox.Show("No title codes found in TitleCodesList.txt. Please add some title codes.",
                    "No Codes", MessageBoxButton.OK, MessageBoxImage.Information);
                return "No title codes configured.";
            }

            var peopleWithTitles = new List<string>();

            foreach (var person in gedcomData.Individuals.Values)
            {
                foreach (var fullName in person.AllNames)
                {
                    // Replace slashes with spaces for searching
                    string searchName = fullName.Replace("/", " ").Trim();
                    while (searchName.Contains("  "))
                    {
                        searchName = searchName.Replace("  ", " ");
                    }

                    // Check if name contains any title codes
                    foreach (var title in titleCodes)
                    {
                        // Use word boundary matching to avoid partial matches
                        bool isWholeWord = false;

                        // Split name into words
                        var words = searchName.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

                        foreach (var word in words)
                        {
                            if (word.Equals(title, System.StringComparison.OrdinalIgnoreCase))
                            {
                                isWholeWord = true;
                                break;
                            }
                        }

                        if (isWholeWord)
                        {
                            peopleWithTitles.Add($"{person.Id}: {searchName} (contains '{title}')");
                            break; // Only add once per person
                        }
                    }
                }
            }

            if (peopleWithTitles.Count > 0)
            {
                return $"People with Titles in Their Names ({peopleWithTitles.Count} found):\n\n" + string.Join("\n", peopleWithTitles);
            }
            else
            {
                return "No people found with title codes in their names.";
            }
        }
    }
}