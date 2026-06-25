// EO2.cs
// 主 Mod 类，包含 Harmony 补丁、气体映射与枚举定义。
using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace EmitO2
{
    public class EO2 : UserMod2
    {
        // ---------- 气体枚举（与旧版完全一致） ----------
        public enum UserOption
        {
            O2氧气,
            Al气态铝,
            CO2二氧化碳,
            CarbonGas气态精炼碳,
            Cl氯气,
            ContaminatedO2污染氧,
            CopperGas气态铜,
            GoldGas气态金,
            H2氢气,
            IronGas气态铁,
            CoGas气态钴,
            LeadGas气态铅,
            MercuryGas气态汞,
            Methane天然气,
            NiobiumGas气态铌,
            PGas气态磷,
            RockGas气态岩,
            SaltGas气态盐,
            SourGas高硫天然气,
            Steam蒸汽,
            SteelGas气态钢,
            SulfurGas气态硫,
            SuperCoolantGas气态超冷剂,
            TungstenGas气态钨,
            EthanolGas气态乙醇
        }

        // ---------- 气体枚举 → SimHashes 整数映射 ----------
        private static readonly Dictionary<UserOption, int> GasHashLookup = new Dictionary<UserOption, int>
        {
            { UserOption.O2氧气, -1528777920 },
            { UserOption.Al气态铝, 100766521 },
            { UserOption.CO2二氧化碳, 1960575215 },
            { UserOption.CarbonGas气态精炼碳, -314016756 },
            { UserOption.Cl氯气, -1324664829 },
            { UserOption.ContaminatedO2污染氧, 721531317 },
            { UserOption.CopperGas气态铜, 1966552544 },
            { UserOption.GoldGas气态金, -805366663 },
            { UserOption.H2氢气, -1046145888 },
            { UserOption.IronGas气态铁, 1541626289 },
            { UserOption.CoGas气态钴, -1429687642 },
            { UserOption.LeadGas气态铅, 905042813 },
            { UserOption.MercuryGas气态汞, -839856666 },
            { UserOption.Methane天然气, -841236436 },
            { UserOption.NiobiumGas气态铌, -1616033402 },
            { UserOption.PGas气态磷, 1887387588 },
            { UserOption.RockGas气态岩, -432557516 },
            { UserOption.SaltGas气态盐, -1946026749 },
            { UserOption.SourGas高硫天然气, -927923200 },
            { UserOption.Steam蒸汽, -899515856 },
            { UserOption.SteelGas气态钢, -1406916018 },
            { UserOption.SulfurGas气态硫, -2120504832 },
            { UserOption.SuperCoolantGas气态超冷剂, -3376362 },
            { UserOption.TungstenGas气态钨, 431998133 },
            { UserOption.EthanolGas气态乙醇, -756961258 }
        };

        // 获取当前设置的气体 SimHashes 值
        private static int GetSelectedGasHash()
        {
            var cfg = SingletonOptions<EO2_UI>.Instance;
            if (cfg != null && GasHashLookup.TryGetValue(cfg.EmitGasType, out int hash))
                return hash;
            // 回退到氧气
            return -1528777920;
        }

        // ---------- Mod 加载入口 ----------
        public override void OnLoad(Harmony harmony)
        {
            PUtil.InitLibrary();                    // 初始化 PLib
            new POptions().RegisterOptions(this, typeof(EO2_UI)); // 注册配置 UI
            base.OnLoad(harmony);
        }

        // ---------- 补丁 1：修改复制人呼出气体质量与类型（OxygenBreather.Sim200ms） ----------
        [HarmonyPatch(typeof(OxygenBreather), "Sim200ms")]
        public class OxygenBreather_Sim200ms_Patch
        {
            /// <summary>
            /// Transpiler：替换 AddGasChunk 调用中的 SimHashes.CarbonDioxide 为用户选择的气体，
            /// 实现太空服存储的气体也变为自定义气体。
            /// </summary>
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                int targetHash = GetSelectedGasHash();
                var codes = new List<CodeInstruction>(instructions);

                for (int i = 0; i < codes.Count - 1; i++)
                {
                    // 查找 ldc.i4 加载二氧化碳常量，且下一条指令是 call（调用方法）
                    if (codes[i].opcode == OpCodes.Ldc_I4 &&
                        codes[i].operand is int hash && hash == 1960575215 && // CarbonDioxide
                        codes[i + 1].opcode == OpCodes.Call)
                    {
                        // 检查调用的方法是否为 Storage.AddGasChunk（通过方法名简单判断）
                        var method = codes[i + 1].operand as MethodInfo;
                        if (method != null && method.Name == "AddGasChunk")
                        {
                            codes[i].operand = targetHash; // 替换为玩家选择的气体
                            break; // 仅替换第一个匹配项，避免重复
                        }
                    }
                }
                return codes;
            }

            /// <summary>
            /// Postfix：修改 O2toCO2conversion 和 minCO2ToEmit，使呼出量固定为玩家设定值（克→千克）。
            /// </summary>
            [HarmonyPostfix]
            public static void Postfix(float dt, OxygenBreather __instance)
            {
                var cfg = SingletonOptions<EO2_UI>.Instance;
                if (cfg == null) return;

                // 将克转换为千克
                float massKg = cfg.EmitGasMass / 1000f;
                __instance.O2toCO2conversion = massKg;
                __instance.minCO2ToEmit = massKg;
            }
        }

        // ---------- 补丁 2：修改 CO2 粒子及实际排放物质（CO2Manager.Sim33ms） ----------
        [HarmonyPatch(typeof(CO2Manager), "Sim33ms")]
        public class CO2Manager_Sim33ms_Patch
        {
            /// <summary>
            /// Transpiler：将 ModifyMass 和 SpawnBubble 调用中的 CarbonDioxide 常量替换为目标气体。
            /// </summary>
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                int targetHash = GetSelectedGasHash();
                var codes = new List<CodeInstruction>(instructions);

                for (int i = 0; i < codes.Count - 1; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldc_I4 &&
                        codes[i].operand is int hash && hash == 1960575215 && // CarbonDioxide
                        codes[i + 1].opcode == OpCodes.Call)
                    {
                        var method = codes[i + 1].operand as MethodInfo;
                        if (method != null &&
                            (method.Name == "ModifyMass" || method.Name == "SpawnBubble"))
                        {
                            codes[i].operand = targetHash; // 替换气体类型
                            // 不 break，允许替换多处（ModifyMass 和 SpawnBubble 可能各自出现）
                        }
                    }
                }
                return codes;
            }
        }
    }
}