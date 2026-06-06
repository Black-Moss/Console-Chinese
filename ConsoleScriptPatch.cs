using System.Collections.Generic;
using HarmonyLib;

namespace ConsoleChinese;

[HarmonyPatch(typeof(ConsoleScript))]
public class ConsoleScriptPatch
{
    [HarmonyPatch("RegisterAllCommands")]
    [HarmonyPostfix]
    public static void Postfix_RegisterAllCommands()
    {
        var commandDescriptions = new Dictionary<string, string>
        {
            { "help", "显示所有可用命令的列表" },
            { "heal", "瞬间治愈" },
            { "coagulate", "停止所有出血" },
            { "kill", "将脑组织完整度设为0立即杀死玩家" },
            { "spawn", "在指定位置生成一个物品/环境物体" },
            { "spawncategory", "从指定的战利品池生成所有物品，并且无重力" },
            { "tp", "将玩家传送到指定位置。如果为空，则默认传送到光标位置" },
            { "skiplayer", "如果未提供层级索引，则会立即跳到下一层；如果提供了层级索引，则会直接跳到该层" },
            { "skiptext", "跳过当前的教程文本" },
            { "log", "在控制台历史中新增文本" },
            { "talk", "让玩家说出一些话" },
            { "framerate", "设置游戏的最大FPS。可能在构建版本中不起作用" },
            { "alert", "显示一条提醒" },
            { "saveandquit", "立即保存游戏并退出到主菜单。加载存档时，你将回到当前关卡的起点" },
            { "resetskills", "将所有技能设置为零" },
            { "fucklore", "立即跳过任何全屏设定文本" },
            { "timescale", "将时间刻度设置为所需的值" },
            { "setconsoleheight", "将控制台高度设置为所需的值" },
            { "setconsolecolor", "设置控制台某个元素的颜色" },
            { "copylog", "将整个控制台日志复制到剪贴板" },
            { "clear", "清除控制台日志" },
            { "addxp", "给予角色在选定技能上的经验" },
            { "loglocale", "获取一个本地化字符串并将其记录到控制台" },
            { "nukeplayerprefs", "删除PlayerPrefs设置" },
            { "openfolder", "打开游戏文件夹" },
            { "setbodyfield", "设置身体数值" },
            { "setlimbfield", "设置肢体数值" },
            { "amputate", "瞬间肢体断裂，不可逆转" },
            { "unchipped", "打开/关闭无芯片模式" },
            { "addcustomcommand", "向列表中增加自定义命令" },
            { "addliquid", "向主手持有的物品中增加指定量的液体" },
            { "locate", "搜索具有指定名称的任何物体，并传送到其中一个" },
            { "removecustomcommand", "从列表中移除已有的自定义命令" },
            { "music", "管理背景音乐。可以播放新曲目、跳过当前曲目的时间或打开MP3菜单" },
            { "bind", "管理自定义命令绑定，可以增加、移除或列出它们" },
            { "repeat", "以指定的次数运行指定的一系列命令，并带有延迟" },
            { "explode", "在指定位置发生爆炸，并可使用可选参数" },
            { "floodfill", "用指定的液体填充指定位置。如果尝试填充超过2048个瓦片则不会生效" },
            { "echo", "切换所有新命令日志" },
            { "ui", "切换主界面的显示" },
            { "freecam", "让摄像机通过方向键单独控制" },
            { "starterkit", "以随机套装生成，包含基础容器、工具和药品" },
            { "noclip", "切换无碰撞模式（启用飞行并禁用玩家碰撞）" },
            { "playsound", "播放指定ID的音频" },
            { "fullbright", "切换调试照明" },
            { "plushies", "按顺序生成一排游戏中所有类型的毛绒玩具" },
            { "errorlogging", "切换错误日志" }
        };

        if (ConsoleScript.Commands == null)
        {
            return;
        }

        foreach (var command in ConsoleScript.Commands)
        {
            if (commandDescriptions.TryGetValue(command.name, out string description))
            {
                command.description = description;
            }
        }
    }
}
