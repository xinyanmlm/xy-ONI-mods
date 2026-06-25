由于游戏底层框架变动和mod依赖文件发生冲突，现在[b]点击游戏内本mod设置按钮，会导致游戏崩溃（如果不点就不会崩溃）[/b]，作者在更新新mod之前，给大家提供一种应急解决方案，方案如下：

1.手动打开你的电脑文档路径，如果没有修改过文档路径，那么可以直接复制下面目录到文件夹的地址栏，回车打开：
%USERPROFILE%\Documents\Klei\OxygenNotIncluded\mods\Steam\3343067865
如果修改过文档路径，请打开如下路径：
文档\Klei\OxygenNotIncluded\mods\Steam\3343067865

2.用记事本打开该文件夹内的Config.json文件，你将会看到如下格式的内容：
{
  "EmitGasMass": 20.0,
  "EmitGasType": 0
}

其中，第一行的"EmitGasMass"是指呼出的气体质量，游戏默认为20g，你可以设置为20~10000的值；
第二行的"EmitGasType"是呼出的气体类型代码，0表示呼出类型为氧气，本Mod设置的气体类型表如下，可以按照你的需要进行设置成对应的代码，设置完成后保存该文件，重启游戏即可生效：


[b]本mod使用的气体类型代码对应表[/b]：
+------+------------------------+------------------------+
| 代码 |       中文名称         |       英文名称         |
+------+------------------------+------------------------+
| 0    | 氧气                     | O2                     |
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


[i]-------------------v0.2.0 更新----------------------[/i]

[b]更新内容[/b]：添加了配置面板。现在，你可以自由选择复制人呼出的气体类型以及重量了，默认是呼出2000毫克的氧气/秒。当然，可以在配置界面将重量更改为10kg/秒，这样在封闭环境下复制人将很快鼓膜破裂 : ) 又或者将气体类型更改为气态碳，从而获取源源不断的精炼碳（只要复制人还在呼吸），希望没有bug……

Added configuration panel. Now, you can freely choose the type and weight of gas that the Duplicants exhales, and the default is to exhale 2000 milligrams of oxygen per second. Of course, you can change the weight to 10kg/s in the configuration interface, so that the Duplicant's eardrum will pop quickly in a closed environment. Alternatively, you can change the gas type to gaseous carbon to obtain a continuous supply of refined carbon (as long as the Duplicant is still breathing). Hopefully, there are no bugs...


复制人不是人。

现在复制人[b]不再呼出二氧化碳[/b]了，他们[b]呼出氧气[/b]。

Duplicants are not humans.

Now Duplicants no longer exhale carbon dioxide, they exhale oxygen.

