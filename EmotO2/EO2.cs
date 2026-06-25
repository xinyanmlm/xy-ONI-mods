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

        // ---------- 气体枚举 → SimHashes 整数映射，来自SimHashes.cs ----------
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
                var codes = new List<CodeInstruction>(instructions);
                for (int i = 0; i < codes.Count - 2; i++)
                {
                    // 原来的匹配模式，已通过 IL 验证非常准确
                    if (codes[i].opcode == OpCodes.Ldloc_S &&
                        codes[i + 1].opcode == OpCodes.Callvirt &&
                        codes[i + 2].opcode == OpCodes.Ldc_I4)
                    {
                        // 确保替换的是 AddGasChunk 的参数（可选的安全检查）
                        var method = codes[i + 1].operand as MethodInfo;
                        if (method != null && method.Name == "GetComponent")
                        {
                            codes[i + 2].operand = GetSelectedGasHash();
                            break; // 仅替换第一个匹配点即可
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
            /// Transpiler：将 SimMessages.ModifyMass 调用中的 SimHashes.CarbonDioxide
            /// 替换为用户选择的气体类型，从而改变粒子落地后实际添加的元素。
            /// 匹配逻辑：ldc.i4 1960575215 后紧跟 call ModifyMass，精准匹配唯一目标，
            /// 避开元素 ID 比较（ldc.i4 + beq.s），不会影响呼吸判定。
            /// </summary>
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);
                for (int i = 0; i < codes.Count - 1; i++)
                {
                    // 匹配 ldc.i4 加载 CarbonDioxide 常量，且下一条指令是 call
                    if (codes[i].opcode == OpCodes.Ldc_I4 &&
                        codes[i + 1].opcode == OpCodes.Call)
                    {
                        // 额外安全检查：确保调用的方法是 ModifyMass
                        var method = codes[i + 1].operand as MethodInfo;
                        if (method != null &&
                            (method.Name == "ModifyMass"))
                        {
                            // 替换为玩家选择的气体哈希值
                            codes[i].operand = GetSelectedGasHash();
                            // 方法内只有一处调用 ModifyMass，找到即可停止
                            break;
                        }
                    }
                }
                return codes;
            }
        }
    }
}