using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

namespace ModifyPlantPulverizer
{
    // Token: 0x02000002 RID: 2
    public class SoybeanMilkMachineConfig : IBuildingConfig
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        public override string[] GetDlcIds()
        {
            return DlcManager.AVAILABLE_ALL_VERSIONS;
        }

        // Token: 0x06000002 RID: 2 RVA: 0x00002058 File Offset: 0x00000258
        public override BuildingDef CreateBuildingDef()
        {
            string id = "SoybeanMilkMachine";
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
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tier, all_MINERALS, melting_point, build_location_rule, TUNING.BUILDINGS.DECOR.PENALTY.TIER1, tier2, 0.2f);
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

        // Token: 0x06000003 RID: 3 RVA: 0x00002130 File Offset: 0x00000330
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.AddOrGet<LogicOperationalController>();
            go.AddOrGet<DropAllWorkable>();
            go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
            ComplexFabricator complexFabricator = go.AddOrGet<ComplexFabricator>();
            complexFabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
            complexFabricator.duplicantOperated = false;
            go.AddOrGet<FabricatorIngredientStatusManager>();
            go.AddOrGet<CopyBuildingSettings>();
            ComplexFabricatorWorkable complexFabricatorWorkable = go.AddOrGet<ComplexFabricatorWorkable>();
            BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);
            complexFabricatorWorkable.overrideAnims = new KAnimFile[]
            {
                Assets.GetAnim("anim_interacts_milkpress_kanim")
            };
            complexFabricatorWorkable.workingPstComplete = new HashedString[]
            {
                "working_pst_complete"
            };
            complexFabricator.storeProduced = true;
            complexFabricator.inStorage.SetDefaultStoredItemModifiers(SoybeanMilkMachineConfig.RefineryStoredItemModifiers);
            complexFabricator.outStorage.SetDefaultStoredItemModifiers(SoybeanMilkMachineConfig.RefineryStoredItemModifiers);
            ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
            conduitDispenser.conduitType = ConduitType.Liquid;
            conduitDispenser.alwaysDispense = true;
            conduitDispenser.elementFilter = null;
            conduitDispenser.storage = go.GetComponent<ComplexFabricator>().outStorage;
            this.AddRecipes(go);
            Prioritizable.AddRef(go);
        }

        // Token: 0x06000004 RID: 4 RVA: 0x00002224 File Offset: 0x00000424
        private void AddRecipes(GameObject go)
        {
            ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement("ColdWheatSeed", 10f),
                new ComplexRecipe.RecipeElement(SimHashes.Water.CreateTag(), 15f)
            };
            ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement(SimHashes.Milk.CreateTag(), 20f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
            };
            ComplexRecipe complexRecipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("SoybeanMilkMachine", array, array2), array, array2, 0, 0);
            complexRecipe.time = 20f;
            complexRecipe.description = string.Format(STRINGS.BUILDINGS.PREFABS.MILKPRESS.WHEAT_MILK_RECIPE_DESCRIPTION, STRINGS.ITEMS.FOOD.COLDWHEATSEED.NAME, SimHashes.Milk.CreateTag().ProperName());
            complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
            complexRecipe.fabricators = new List<Tag>
            {
                TagManager.Create("SoybeanMilkMachine")
            };
            ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement(SpiceNutConfig.ID, 3f),
                new ComplexRecipe.RecipeElement(SimHashes.Water.CreateTag(), 17f)
            };
            ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement(SimHashes.Milk.CreateTag(), 20f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
            };
            ComplexRecipe complexRecipe2 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("SoybeanMilkMachine", array3, array4), array3, array4, 0, 0);
            complexRecipe2.time = 20f;
            complexRecipe2.description = string.Format(STRINGS.BUILDINGS.PREFABS.MILKPRESS.NUT_MILK_RECIPE_DESCRIPTION, STRINGS.ITEMS.FOOD.SPICENUT.NAME, SimHashes.Milk.CreateTag().ProperName());
            complexRecipe2.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
            complexRecipe2.fabricators = new List<Tag>
            {
                TagManager.Create("SoybeanMilkMachine")
            };
            ComplexRecipe.RecipeElement[] array5 = new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement("BeanPlantSeed", 2f),
                new ComplexRecipe.RecipeElement(SimHashes.Water.CreateTag(), 18f)
            };
            ComplexRecipe.RecipeElement[] array6 = new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement(SimHashes.Milk.CreateTag(), 20f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
            };
            ComplexRecipe complexRecipe3 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("SoybeanMilkMachine", array5, array6), array5, array6, 0, 0);
            complexRecipe3.time = 20f;
            complexRecipe3.description = string.Format(STRINGS.BUILDINGS.PREFABS.MILKPRESS.NUT_MILK_RECIPE_DESCRIPTION, STRINGS.ITEMS.FOOD.BEANPLANTSEED.NAME, SimHashes.Milk.CreateTag().ProperName());
            complexRecipe3.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
            complexRecipe3.fabricators = new List<Tag>
            {
                TagManager.Create("SoybeanMilkMachine")
            };
        }

        // Token: 0x06000005 RID: 5 RVA: 0x00002474 File Offset: 0x00000674
        public override void DoPostConfigureComplete(GameObject go)
        {
            SymbolOverrideControllerUtil.AddToPrefab(go);
            go.GetComponent<KPrefabID>().prefabSpawnFn += delegate (GameObject game_object)
            {
                ComplexFabricatorWorkable component = game_object.GetComponent<ComplexFabricatorWorkable>();
                component.WorkerStatusItem = Db.Get().DuplicantStatusItems.Processing;
                component.AttributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
                component.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
                component.SkillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
                component.SkillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
            };
        }

        // Token: 0x04000001 RID: 1
        public const string ID = "SoybeanMilkMachine";

        // Token: 0x04000002 RID: 2
        private static readonly List<Storage.StoredItemModifier> RefineryStoredItemModifiers = new List<Storage.StoredItemModifier>
        {
            Storage.StoredItemModifier.Hide,
            Storage.StoredItemModifier.Preserve,
            Storage.StoredItemModifier.Insulate,
            Storage.StoredItemModifier.Seal
        };
    }
}
