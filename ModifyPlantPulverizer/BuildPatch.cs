using System;
using HarmonyLib;

namespace ModifyPlantPulverizer
{
    // Token: 0x02000004 RID: 4
    public static class BuildPatch
    {
        // Token: 0x02000007 RID: 7
        [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
        public static class PlantPulverizer_1LoadGeneratedBuildings_Patch
        {
            // Token: 0x0600000E RID: 14 RVA: 0x0000260C File Offset: 0x0000080C
            public static void Prefix()
            {
                ModUtil.AddBuildingToPlanScreen("Refining", "SoybeanMilkMachine");
                Db.Get().Techs.Get("FoodRepurposing").unlockedItemIDs.Add("SoybeanMilkMachine");
                StringUtils.Add_New_BuildStrings("SoybeanMilkMachine", BUILDING_MOD.NAME, BUILDING_MOD.DESC, BUILDING_MOD.EFFECT);
            }
        }
    }
}
