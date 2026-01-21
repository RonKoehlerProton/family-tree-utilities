using System.Collections.Generic;
using System.Linq;
using FamilyTreeUtilities.Models;

namespace FamilyTreeUtilities.Services.Analyzers
{
    public class ChildNameMismatchAnalyzer
    {
        private readonly SoundexService _soundexService;

        public ChildNameMismatchAnalyzer()
        {
            _soundexService = new SoundexService();
        }

        public string Analyze(GedcomData gedcomData)
        {
            var results = new List<string>();

            foreach (var family in gedcomData.Families)
            {
                // Skip families without a father
                if (string.IsNullOrEmpty(family.HusbandId) || !gedcomData.Individuals.ContainsKey(family.HusbandId))
                    continue;

                var father = gedcomData.Individuals[family.HusbandId];

                // Father must have at least one last name
                if (father.LastNames.Count == 0)
                    continue;

                string fatherLastName = father.LastNames[0];

                // Check each child in the family
                foreach (var childId in family.ChildrenIds)
                {
                    if (!gedcomData.Individuals.ContainsKey(childId))
                        continue;

                    var child = gedcomData.Individuals[childId];

                    // Skip if child has no last names
                    if (child.LastNames.Count == 0)
                        continue;

                    // Check if ANY of the child's last names match the father's (exact or soundex)
                    bool hasMatchingName = child.LastNames.Any(ln =>
                        ln.Equals(fatherLastName, System.StringComparison.OrdinalIgnoreCase) ||
                        _soundexService.GetSoundex(ln) == _soundexService.GetSoundex(fatherLastName));

                    if (!hasMatchingName)
                    {
                        string fatherDisplay = FormatName(father.AllNames[0]);
                        var childNames = child.AllNames.Select(FormatName);
                        string childDisplay = string.Join(", ", childNames);

                        results.Add($"{child.Id}: {childDisplay} (father: {father.Id}: {fatherDisplay})");
                    }
                }
            }

            if (results.Count > 0)
            {
                return $"Children with Last Name Different from Father ({results.Count} found):\n\n" + string.Join("\n", results);
            }
            else
            {
                return "No children found with last names different from their father.";
            }
        }

        private string FormatName(string name)
        {
            string display = name.Replace("/", " ").Trim();
            while (display.Contains("  "))
                display = display.Replace("  ", " ");
            return display;
        }
    }
}
