using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

namespace ModifyPlantPulverizer
{
    public class SoybeanMilkMachineConfig : IBuildingConfig
    {
        public const string ID = "SoybeanMilkMachine";

        private static readonly List<Storage.StoredItemModifier> RefineryStoredItemModifiers = new List<Storage.StoredItemModifier>
        {
            Storage.StoredItemModifier.Hide,
            Storage.StoredItemModifier.Preserve,
            Storage.StoredItemModifier.Insulate,
            Storage.StoredItemModifier.Seal
        };

        // 移除过时的API
        // public override string[] GetDlcIds()
        // {
        //     return DlcManager.AVAILABLE_ALL_VERSIONS;
        // }

        public override string[] GetRequiredDlcIds()
        {
            // 该建筑在所有DLC版本下都可用，无需强制安装任何DLC
            return Array.Empty<string>();
        }

        public override string[] GetForbiddenDlcIds()
        {
            // 不禁止任何DLC
            return Array.Empty<string>();
        }

        public override BuildingDef CreateBuildingDef()
        {
            string id = ID;
            int width = 2;
            int height = 3;
            string anim = "milkpress_kanim";
            int hitpoints = 100;
            float construction_time = 30f;
            float[] tier = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
            string[] all_MINERALS = MATERIALS.ALL_MINERALS;
            float melting_point = 1600f;
            BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
            EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER4;

            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
                id, width, height, anim, hitpoints, construction_time,
                tier, all_MINERALS, melting_point, build_location_rule,
                TUNING.BUILDINGS.DECOR.PENALTY.TIER1, tier2, 0.2f);

            buildingDef.RequiresPowerInput = true;
            buildingDef.PowerInputOffset = new CellOffset(1, 0);
            buildingDef.EnergyConsumptionWhenActive = 240f;
            buildingDef.SelfHeatKilowattsWhenActive = 2f;
            buildingDef.OutputConduitType = ConduitType.Liquid;
            buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
            buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(1, 0));
            buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.AudioSize = "Metal";
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.AddOrGet<LogicOperationalController>();
            go.AddOrGet<DropAllWorkable>();
            // 本项存疑，不知道是控制什么的，原版与mod设置的都是true
            go.AddOrGet<BuildingComplete>().isManuallyOperated = true; 

            ComplexFabricator complexFabricator = go.AddOrGet<ComplexFabricator>();
            complexFabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
            complexFabricator.duplicantOperated = false;          // 改为电力自动

            go.AddOrGet<FabricatorIngredientStatusManager>();
            go.AddOrGet<CopyBuildingSettings>();

            ComplexFabricatorWorkable workable = go.AddOrGet<ComplexFabricatorWorkable>();
            BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);

            workable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_milkpress_kanim") };
            workable.workingPstComplete = new HashedString[] { "working_pst_complete" };

            // 显式设置 storeProduced 为 false（与原版一致）
            // 原版在 ConfigureBuildingTemplate 最后设置了 complexFabricator.storeProduced = false;，而Mod未设置（默认为 true）。
            // 虽然 ConduitDispenser 仍会排出产物，但设为 false 可以避免产物堆积在存储中，与游戏原版行为一致。
            complexFabricator.storeProduced = false;

            // 为所有存储设置相同的 StoredItemModifiers
            complexFabricator.inStorage.SetDefaultStoredItemModifiers(RefineryStoredItemModifiers);
            complexFabricator.buildStorage.SetDefaultStoredItemModifiers(RefineryStoredItemModifiers);
            complexFabricator.outStorage.SetDefaultStoredItemModifiers(RefineryStoredItemModifiers);

            // 管道输出
            ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
            conduitDispenser.conduitType = ConduitType.Liquid;
            conduitDispenser.alwaysDispense = true;
            conduitDispenser.elementFilter = null;
            conduitDispenser.storage = complexFabricator.outStorage;

            AddRecipes(go);

            Prioritizable.AddRef(go);
        }

        private void AddRecipes(GameObject go)
        {
            // ---------- 配方 1：冰霜麦粒→咸乳（Water 或 Mucus）----------
            ComplexRecipe.RecipeElement[] inputs1 = new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement("ColdWheatSeed", 10f),
                new ComplexRecipe.RecipeElement(new Tag[] { SimHashes.Water.CreateTag(), SimHashes.Mucus.CreateTag() }, 15f)
            };
            ComplexRecipe.RecipeElement[] outputs1 = new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement(SimHashes.Milk.CreateTag(), 20f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
            };
            ComplexRecipe recipe1 = new ComplexRecipe(
                ComplexRecipeManager.MakeRecipeID(ID, inputs1, outputs1),
                inputs1, outputs1, 0, 0);
            recipe1.time = 20f;
            recipe1.description = string.Format(STRINGS.BUILDINGS.PREFABS.MILKPRESS.WHEAT_MILK_RECIPE_DESCRIPTION,
                STRINGS.ITEMS.FOOD.COLDWHEATSEED.NAME, SimHashes.Milk.CreateTag().ProperName());
            recipe1.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
            recipe1.fabricators = new List<Tag> { TagManager.Create(ID) };

            // ---------- 配方 2：火椒粒→咸乳（Water 或 Mucus）----------
            ComplexRecipe.RecipeElement[] inputs2 = new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement(SpiceNutConfig.ID, 3f),
                new ComplexRecipe.RecipeElement(new Tag[] { SimHashes.Water.CreateTag(), SimHashes.Mucus.CreateTag() }, 17f)
            };
            ComplexRecipe.RecipeElement[] outputs2 = new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement(SimHashes.Milk.CreateTag(), 20f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
            };
            ComplexRecipe recipe2 = new ComplexRecipe(
                ComplexRecipeManager.MakeRecipeID(ID, inputs2, outputs2),
                inputs2, outputs2, 0, 0);
            recipe2.time = 20f;
            recipe2.description = string.Format(STRINGS.BUILDINGS.PREFABS.MILKPRESS.NUT_MILK_RECIPE_DESCRIPTION,
                STRINGS.ITEMS.FOOD.SPICENUT.NAME, SimHashes.Milk.CreateTag().ProperName());
            recipe2.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
            recipe2.fabricators = new List<Tag> { TagManager.Create(ID) };

            // ---------- 配方 3：小吃豆→咸乳（Water 或 Mucus）----------
            ComplexRecipe.RecipeElement[] inputs3 = new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement("BeanPlantSeed", 2f),
                new ComplexRecipe.RecipeElement(new Tag[] { SimHashes.Water.CreateTag(), SimHashes.Mucus.CreateTag() }, 18f)
            };
            ComplexRecipe.RecipeElement[] outputs3 = new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement(SimHashes.Milk.CreateTag(), 20f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
            };
            ComplexRecipe recipe3 = new ComplexRecipe(
                ComplexRecipeManager.MakeRecipeID(ID, inputs3, outputs3),
                inputs3, outputs3, 0, 0);
            recipe3.time = 20f;
            recipe3.description = string.Format(STRINGS.BUILDINGS.PREFABS.MILKPRESS.NUT_MILK_RECIPE_DESCRIPTION,
                STRINGS.ITEMS.FOOD.BEANPLANTSEED.NAME, SimHashes.Milk.CreateTag().ProperName());
            recipe3.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
            recipe3.fabricators = new List<Tag> { TagManager.Create(ID) };

            // ---------- 配方 4（DLC4）：露珠 -> 咸乳 ----------
            if (DlcManager.IsContentSubscribed("DLC4_ID"))
            {
                ComplexRecipe.RecipeElement[] inputs4 = new ComplexRecipe.RecipeElement[]
                {
                    new ComplexRecipe.RecipeElement(DewDripConfig.ID, 2f)
                };
                ComplexRecipe.RecipeElement[] outputs4 = new ComplexRecipe.RecipeElement[]
                {
                    new ComplexRecipe.RecipeElement(SimHashes.Milk.CreateTag(), 20f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
                };
                ComplexRecipe recipe4 = new ComplexRecipe(
                    ComplexRecipeManager.MakeRecipeID(ID, inputs4, outputs4),
                    inputs4, outputs4, 0, 0);
                recipe4.time = 20f;
                recipe4.description = GameUtil.SafeStringFormat(STRINGS.BUILDINGS.PREFABS.MILKPRESS.DEWDRIPPER_MILK_RECIPE_DESCRIPTION,
                    STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.DEWDRIP.NAME, SimHashes.Milk.CreateTag().ProperName());
                recipe4.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
                recipe4.fabricators = new List<Tag> { TagManager.Create(ID) };
            }

            // ---------- 配方 5：菌泥 -> 植物润滑油 + 泥土 ----------
            {
                ComplexRecipe.RecipeElement[] inputs5 = new ComplexRecipe.RecipeElement[]
                {
                    new ComplexRecipe.RecipeElement(SimHashes.SlimeMold.CreateTag(), 100f)
                };
                ComplexRecipe.RecipeElement[] outputs5 = new ComplexRecipe.RecipeElement[]
                {
                    new ComplexRecipe.RecipeElement(SimHashes.PhytoOil.CreateTag(), 70f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false),
                    new ComplexRecipe.RecipeElement(SimHashes.Dirt.CreateTag(), 30f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
                };
                ComplexRecipe recipe5 = new ComplexRecipe(
                    ComplexRecipeManager.MakeRecipeID(ID, inputs5, outputs5),
                    inputs5, outputs5, 0, 0);
                recipe5.time = 40f;
                recipe5.description = string.Format(STRINGS.BUILDINGS.PREFABS.MILKPRESS.PHYTO_OIL_RECIPE_DESCRIPTION,
                    ELEMENTS.SLIMEMOLD.NAME, SimHashes.PhytoOil.CreateTag().ProperName(), SimHashes.Dirt.CreateTag().ProperName());
                recipe5.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
                recipe5.fabricators = new List<Tag> { TagManager.Create(ID) };
            }

            // ---------- 配方 6（DLC4）：海梳蕨叶 + 水 -> 植物润滑油 ----------
            if (DlcManager.IsContentSubscribed("DLC4_ID"))
            {
                float totalMass = 100f;
                ComplexRecipe.RecipeElement[] inputs6 = new ComplexRecipe.RecipeElement[]
                {
                    new ComplexRecipe.RecipeElement(KelpConfig.ID, totalMass * 0.25f),
                    new ComplexRecipe.RecipeElement(SimHashes.Water.CreateTag(), totalMass * 0.75f)
                };
                ComplexRecipe.RecipeElement[] outputs6 = new ComplexRecipe.RecipeElement[]
                {
                    new ComplexRecipe.RecipeElement(SimHashes.PhytoOil.CreateTag(), totalMass, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
                };
                ComplexRecipe recipe6 = new ComplexRecipe(
                    ComplexRecipeManager.MakeRecipeID(ID, inputs6, outputs6),
                    inputs6, outputs6, 0, 0, DlcManager.DLC4);
                recipe6.time = 40f;
                recipe6.description = GameUtil.SafeStringFormat(STRINGS.BUILDINGS.PREFABS.MILKPRESS.KELP_TO_PHYTO_OIL_RECIPE_DESCRIPTION,
                    STRINGS.ITEMS.INGREDIENTS.KELP.NAME, SimHashes.PhytoOil.CreateTag().ProperName());
                recipe6.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
                recipe6.fabricators = new List<Tag> { TagManager.Create(ID) };
            }

            // ---------- 配方 7（DLC4）：琥珀 -> 树脂 + 化石 + 沙子 ----------
            if (DlcManager.IsContentSubscribed("DLC4_ID"))
            {
                float totalMass = 100f;
                ComplexRecipe.RecipeElement[] inputs7 = new ComplexRecipe.RecipeElement[]
                {
                    new ComplexRecipe.RecipeElement(SimHashes.Amber.CreateTag(), totalMass)
                };
                ComplexRecipe.RecipeElement[] outputs7 = new ComplexRecipe.RecipeElement[]
                {
                    new ComplexRecipe.RecipeElement(SimHashes.NaturalResin.CreateTag(), totalMass * 0.5f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false),
                    new ComplexRecipe.RecipeElement(SimHashes.Fossil.CreateTag(), totalMass * 0.25f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false),
                    new ComplexRecipe.RecipeElement(SimHashes.Sand.CreateTag(), totalMass * 0.25f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
                };
                ComplexRecipe recipe7 = new ComplexRecipe(
                    ComplexRecipeManager.MakeRecipeID(ID, inputs7, outputs7),
                    inputs7, outputs7, 0, 0, DlcManager.DLC4);
                recipe7.time = 40f;
                recipe7.description = GameUtil.SafeStringFormat(STRINGS.BUILDINGS.PREFABS.MILKPRESS.RESIN_FROM_AMBER_RECIPE_DESCRIPTION,
                    SimHashes.Amber.CreateTag().ProperName(),
                    SimHashes.NaturalResin.CreateTag().ProperName(),
                    SimHashes.Fossil.CreateTag().ProperName(),
                    SimHashes.Sand.CreateTag().ProperName());
                recipe7.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
                recipe7.fabricators = new List<Tag> { TagManager.Create(ID) };
            }

            // ---------- 配方 8（DLC5）：粘胶木材 -> 胶乳 ----------
            if (DlcManager.IsContentSubscribed("DLC5_ID"))
            {
                float amount = 100f;
                float outputAmount = 50f;
                ComplexRecipe.RecipeElement[] inputs8 = new ComplexRecipe.RecipeElement[]
                {
                    new ComplexRecipe.RecipeElement(SimHashes.PalmWood.CreateTag(), amount)
                };
                ComplexRecipe.RecipeElement[] outputs8 = new ComplexRecipe.RecipeElement[]
                {
                    new ComplexRecipe.RecipeElement(SimHashes.Latex.CreateTag(), outputAmount, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
                };
                ComplexRecipe recipe8 = new ComplexRecipe(
                    ComplexRecipeManager.MakeRecipeID(ID, inputs8, outputs8),
                    inputs8, outputs8, 0, 0, DlcManager.DLC5);
                recipe8.time = 40f;
                recipe8.description = GameUtil.SafeStringFormat(STRINGS.BUILDINGS.PREFABS.MILKPRESS.PALMWOOD_TO_LATEX_RECIPE_DESCRIPTION,
                    SimHashes.PalmWood.CreateTag().ProperName(), SimHashes.Latex.CreateTag().ProperName());
                recipe8.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
                recipe8.fabricators = new List<Tag> { TagManager.Create(ID) };
            }
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            SymbolOverrideControllerUtil.AddToPrefab(go);
            go.GetComponent<KPrefabID>().prefabSpawnFn += (game_object) =>
            {
                ComplexFabricatorWorkable workable = game_object.GetComponent<ComplexFabricatorWorkable>();
                workable.WorkerStatusItem = Db.Get().DuplicantStatusItems.Processing;
                workable.AttributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
                workable.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
                workable.SkillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
                workable.SkillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
            };
        }
    }
}