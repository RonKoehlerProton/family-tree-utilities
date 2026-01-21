using System.Collections.Generic;

namespace FamilyTreeUtilities.Models
{
    public class FamilyInfo
    {
        public string Id { get; set; }
        public string HusbandId { get; set; }
        public string WifeId { get; set; }
        public List<string> ChildrenIds { get; set; } = new List<string>();
    }
}