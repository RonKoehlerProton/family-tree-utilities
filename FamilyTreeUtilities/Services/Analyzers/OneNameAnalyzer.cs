using System.Collections.Generic;
using FamilyTreeUtilities.Models;

namespace FamilyTreeUtilities.Services.Analyzers
{
    public class OneNameAnalyzer
    {
        public string Analyze(GedcomData gedcomData)
        {
            var oneNamePersons = new List<string>();

            foreach (var person in gedcomData.Individuals.Values)
            {
                foreach (var fullName in person.AllNames)
                {
                    // Replace slashes with spaces, then clean up extra spaces
                    string currentName = fullName.Replace("/", " ").Trim();
                    while (currentName.Contains("  "))
                    {
                        currentName = currentName.Replace("  ", " ");
                    }

                    // Check if name has only one word (no spaces after cleanup)
                    if (!string.IsNullOrWhiteSpace(currentName) && !currentName.Contains(" "))
                    {
                        oneNamePersons.Add($"{person.Id}: {currentName}");
                        break; // Only add once per person
                    }
                }
            }

            if (oneNamePersons.Count > 0)
            {
                return $"Persons with One Name Only ({oneNamePersons.Count} found):\n\n" + string.Join("\n", oneNamePersons);
            }
            else
            {
                return "No persons with only one name were found in the GEDCOM file.";
            }
        }
    }
}