using System;
using HarmonyLib;

namespace ModifyPlantPulverizer
{
    public static class BuildPatch
    {
        [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
        public static class PlantPulverizer_1LoadGeneratedBuildings_Patch
        {
            public static void Prefix()
            {
                // 注册建筑
                // BuildingConfigManager.Instance.RegisterBuilding(new SoybeanMilkMachineConfig());
                // 将建筑添加到菜单
                ModUtil.AddBuildingToPlanScreen("Refining", SoybeanMilkMachineConfig.ID);
                // 使其可研究
                Db.Get().Techs.Get("FoodRepurposing").unlockedItemIDs.Add(SoybeanMilkMachineConfig.ID);
                // 调用上面的轮子,将字符串添加到游戏中
                StringUtils.Add_New_BuildStrings(SoybeanMilkMachineConfig.ID, BUILDING_MOD.NAME, BUILDING_MOD.DESC, BUILDING_MOD.EFFECT);
            }
        }
    }
}
