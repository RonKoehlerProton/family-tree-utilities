using System.Collections.Generic;

namespace FamilyTreeUtilities.Models
{
    public class GedcomData
    {
        public Dictionary<string, PersonInfo> Individuals { get; set; } = new Dictionary<string, PersonInfo>();
        public List<FamilyInfo> Families { get; set; } = new List<FamilyInfo>();
    }
}