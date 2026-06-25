using System;

namespace ModifyPlantPulverizer
{
    public static class StringUtils
    {
        public static void Add_New_BuildStrings(string plantId, string name, string description, string effect)
        {
            Strings.Add(new string[]
            {
                "STRINGS.BUILDINGS.PREFABS." + plantId.ToUpperInvariant() + ".NAME",
                name
            });
            Strings.Add(new string[]
            {
                "STRINGS.BUILDINGS.PREFABS." + plantId.ToUpperInvariant() + ".DESC",
                description
            });
            Strings.Add(new string[]
            {
                "STRINGS.BUILDINGS.PREFABS." + plantId.ToUpperInvariant() + ".EFFECT",
                effect
            });
        }
    }
}
