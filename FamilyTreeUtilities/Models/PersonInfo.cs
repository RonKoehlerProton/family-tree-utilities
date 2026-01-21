using System.Collections.Generic;

namespace FamilyTreeUtilities.Models
{
    public class PersonInfo
    {
        public string Id { get; set; }
        public List<string> AllNames { get; set; } = new List<string>();
        public List<string> LastNames { get; set; } = new List<string>();
        public string Sex { get; set; }
    }
}