using System.Collections.Generic;
using System.Linq;
using FamilyTreeUtilities.Models;

namespace FamilyTreeUtilities.Services.Analyzers
{
    public class SameLastNameAnalyzer
    {
        public string Analyze(GedcomData gedcomData)
        {
            var results = new List<string>();

            foreach (var family in gedcomData.Families)
            {
                if (string.IsNullOrEmpty(family.HusbandId) ||
                    string.IsNullOrEmpty(family.WifeId) ||
                    !gedcomData.Individuals.ContainsKey(family.HusbandId) ||
                    !gedcomData.Individuals.ContainsKey(family.WifeId))
                    continue;

                var husband = gedcomData.Individuals[family.HusbandId];
                var wife = gedcomData.Individuals[family.WifeId];

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
                    string husbandDisplay = FormatName(husband.AllNames[0]);
                    var wifeNames = wife.AllNames.Select(FormatName);
                    string wifeDisplay = string.Join(", ", wifeNames);

                    results.Add($"{wife.Id}: {wifeDisplay} (married to {husband.Id}: {husbandDisplay})");
                }
            }

            if (results.Count > 0)
            {
                return $"Women with Same Last Name as Husband (All Names Match) ({results.Count} found):\n\n" + string.Join("\n", results);
            }
            else
            {
                return "No women found where all their last names match their husband's last name.";
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
