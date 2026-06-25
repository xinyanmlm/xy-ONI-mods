using System;

namespace ModifyPlantPulverizer
{
    // Token: 0x02000003 RID: 3
    public static class StringUtils
    {
        // Token: 0x06000008 RID: 8 RVA: 0x000024D8 File Offset: 0x000006D8
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
