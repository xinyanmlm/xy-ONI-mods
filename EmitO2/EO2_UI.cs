// EO2_UI.cs
// 配置选项类，通过 PLib 提供 Mod 菜单中的滑块和下拉列表。
// 必须为 public，否则 PLib 无法生成 UI。
using EmitO2;
using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace EmitO2
{
    [JsonObject]
    [ConfigFile("Config.json", true, false)]   // 缩进 true，写入默认值 false
    [RestartRequired]                          // 修改后需重启游戏
    public class EO2_UI : SingletonOptions<EO2_UI>
    {
        // 呼出气体量，单位：克，默认 20g，范围 20~10000g
        [Option("呼出气体量", "每次呼出的气体质量（克），游戏默认值 20 克。", null)]
        [Limit(20.0, 10000.0)]
        [JsonProperty]
        public float EmitGasMass { get; set; } = 20f;

        // 气体类型，使用枚举，下拉选择
        [Option("气体类型", "复制人呼出的气体种类。", null)]
        [JsonProperty]
        public EO2.UserOption EmitGasType { get; set; } = EO2.UserOption.O2氧气;
    }
}