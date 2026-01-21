using System.Collections.Generic;
using System.IO;
using FamilyTreeUtilities.Models;

namespace FamilyTreeUtilities.Services
{
    public class GedcomParserService
    {
        public GedcomData Parse(string filePath)
        {
            var gedcomData = new GedcomData();
            var lines = File.ReadAllLines(filePath);

            string currentId = null;
            string currentType = null;
            PersonInfo currentPerson = null;
            FamilyInfo currentFamily = null;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                // Check for individual record
                if (line.StartsWith("0") && line.Contains("INDI"))
                {
                    currentId = line.Split(' ')[1].Replace("@", "");
                    currentPerson = new PersonInfo { Id = currentId };
                    gedcomData.Individuals[currentId] = currentPerson;
                    currentType = "INDI";
                    currentFamily = null;
                }
                // Check for family record
                else if (line.StartsWith("0") && line.Contains("FAM"))
                {
                    currentId = line.Split(' ')[1].Replace("@", "");
                    currentFamily = new FamilyInfo { Id = currentId };
                    gedcomData.Families.Add(currentFamily);
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
                    else if (line.StartsWith("1 CHIL"))
                    {
                        string childId = line.Split('@')[1];
                        currentFamily.ChildrenIds.Add(childId);
                    }
                }
            }

            return gedcomData;
        }
    }
}