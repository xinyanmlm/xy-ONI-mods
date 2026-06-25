
[b]复制人呼出气体自定义[/b]

[list]
[*] 可自由设置复制人呼出的气体种类与质量
[*] 支持 25 种气体：氧气、氢气、氯气、蒸汽、各种金属气体等
[*] 呼出量可调，范围 20 克 ～ 10 千克
[*] 视觉效果与实际物质保持一致
[*] 默认：呼出 20 克氧气（设定多少克，复制人就一次呼出多少克）
[/list]

[b]更新日志[/b]
[b]v0.3.6 (2026-06-25)[/b]
- 修复了点击游戏内 Mod 设置按钮导致游戏崩溃的 bug
- 优化了部分代码

[b]v0.2.0[/b]
- 添加了配置面板，现在可以自由选择复制人呼出的气体类型以及重量
- 默认呼出 2000 毫克/秒 的氧气，可在配置界面随意更改，例如改为 10 千克/秒，复制人鼓膜迅速破裂 :)
- 也可以将气体类型改为气态碳，源源不断获取精炼碳（只要复制人还在呼吸）

[hr]

[b]Duplicant Breath Customizer[/b]

[list]
[*] Customize the type and amount of gas exhaled by duplicants
[*] 25 gas types available: Oxygen, Hydrogen, Chlorine, Steam, metal gases, and more
[*] Exhalation mass adjustable from 20g to 10kg
[*] Visual particles match the actual gas type
[*] Default: exhales 20g of Oxygen(duplicants exhale exactly the amount you set per breath)
[/list]

[b]Changelog[/b]
[b]v0.3.6 (2026-06-25)[/b]
- Fixed a crash when clicking the in-game Mod Options button
- Code optimizations

[b]v0.2.0[/b]
- Added a configuration panel, now you can freely choose the exhaled gas type and weight
- Default exhaled oxygen mass is 2000mg/s, adjustable (e.g., 10kg/s will rupture duplicant eardrums quickly ;)
- Change gas type to Carbon Gas to get a constant supply of refined carbon (as long as the duplicant is breathing)

[b]气体类型代码对应表 Gas Type Code Table[/b]
[code]
+------+------------------------+------------------------+
| Code |      Chinese Name      |       English Name     |
+------+------------------------+------------------------+
| 0    | 氧气                   | Oxygen                 |
| 1    | 气态铝                 | Aluminum Gas           |
| 2    | 二氧化碳               | Carbon Dioxide         |
| 3    | 气态精炼碳             | Carbon Gas             |
| 4    | 氯气                   | Chlorine               |
| 5    | 污染氧                 | Contaminated Oxygen    |
| 6    | 气态铜                 | Copper Gas             |
| 7    | 气态金                 | Gold Gas               |
| 8    | 氢气                   | Hydrogen               |
| 9    | 气态铁                 | Iron Gas               |
| 10   | 气态钴                 | Cobalt Gas             |
| 11   | 气态铅                 | Lead Gas               |
| 12   | 气态汞                 | Mercury Gas            |
| 13   | 天然气                 | Methane                |
| 14   | 气态铌                 | Niobium Gas            |
| 15   | 气态磷                 | Phosphorus Gas         |
| 16   | 气态岩                 | Rock Gas               |
| 17   | 气态盐                 | Salt Gas               |
| 18   | 高硫天然气             | Sour Gas               |
| 19   | 蒸汽                   | Steam                  |
| 20   | 气态钢                 | Steel Gas              |
| 21   | 气态硫                 | Sulfur Gas             |
| 22   | 气态超冷剂             | Super Coolant Gas      |
| 23   | 气态钨                 | Tungsten Gas           |
| 24   | 气态乙醇               | Ethanol Gas            |
+------+------------------------+------------------------+
[/code]
