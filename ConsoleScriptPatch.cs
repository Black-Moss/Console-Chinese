using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HarmonyLib;
using TMPro;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace ConsoleChinese;

[HarmonyPatch(typeof(ConsoleScript))]
public class ConsoleScriptPatch
{
    [HarmonyPatch("RegisterAllCommands")]
    [HarmonyPostfix]
    public static void RegisterAllCommandsPostfix()
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
            { "talk", "说话" },
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
            { "nukeplayerprefs", "删除玩家预设置" },
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
            { "floodfill", "用指定的液体填充指定位置。如果尝试填充超过2048个物块则不会生效" },
            { "echo", "切换所有新命令日志" },
            { "ui", "切换主界面的显示" },
            { "freecam", "让摄像机通过方向键单独控制" },
            { "starterkit", "以随机套装生成，包含基础容器、工具和药品" },
            { "noclip", "切换无碰撞模式（启用飞行并禁用玩家碰撞）" },
            { "playsound", "播放指定ID的音频" },
            { "fullbright", "切换全局照明" },
            { "plushies", "按顺序生成一排游戏中所有类型的毛绒玩具" },
            { "errorlogging", "切换错误日志" }
        };

        if (ConsoleScript.Commands == null)
        {
            return;
        }

        var argLongDescTranslations = new Dictionary<string, string>
        {
            { "The ID of the entity to spawn", "要生成的实体ID" },
            { "Where to spawn the item", "生成物品的位置" },
            { "Optional item condition, ranging 0-1, if applicable", "可选的物品状态，范围0-1（如适用）" },
            { "Optional amount of the entity to spawn", "可选的要生成实体数量" },
            { "The ID of the category to spawn from", "要从中生成的类别ID" },
            { "Where to teleport to", "传送目标位置" },
            { "The layer index to go to.", "要前往的层索引" },
            { "The text to log.", "要记录的文本" },
            { "The text make the character speak", "让角色说的话" },
            { "The framerate cap to set.", "要设置的帧率上限" },
            { "If true, displays the alert in the center of the screen.", "如果为true，在屏幕中央显示提醒" },
            { "The text make the character speak.", "让角色说的话" },
            { "The application volume. WARNING: this should be a value ranging 0-1.", "应用音量。警告：值应在0-1之间" },
            { "The speed to set the timescale to.", "要设置的时间刻度速度" },
            { "(Default: 300) the height of the console log window", "（默认：300）控制台日志窗口的高度" },
            { "Which part of the console to color", "要着色的控制台部分" },
            { "Hex color code", "十六进制颜色代码" },
            { "The skill to affect", "要影响的技能" },
            { "How much experience to give", "要给予的经验值数量" },
            { "The locale file section to look in", "要查找的语言文件区域" },
            { "The locale key to look for", "要查找的语言键" },
            { "Which folder type to go to", "要前往的文件夹类型" },
            { "The field you want to set", "要设置的字段" },
            { "Value to set the field to", "要设置的字段值" },
            { "Name of your desired limb", "目标肢体的名称" },
            { "Enable unchipped?", "启用无芯片模式？" },
            { "Enable the pixel filter?", "启用像素滤镜？" },
            { "Name of the command, no spaces", "命令名称，不能有空格" },
            { "Description of the command, spaces replaced by _", "命令描述，空格用_代替" },
            { "Commands to run, separated by ; and spaces replaced by _", "要运行的命令，用;分隔，空格用_代替" },
            { "ID of the liquid to add", "要添加的液体ID" },
            { "mL amount of the liquid to add", "要添加的液体毫升数" },
            {
                "Name of object to search for. Object name has to contain this string to be found",
                "要搜索的对象名称。对象名称必须包含此字符串"
            },
            { "Index of the object to teleport to", "要传送到的对象索引" },
            { "Name of the command to remove", "要移除的命令名称" },
            {
                "play to play a new track, skiptime to skip time in the current track, menu to open the MP3 menu",
                "play播放新曲目，skiptime跳过当前曲目时间，menu打开MP3菜单"
            },
            { "(if skipping time) How much time to skip by", "（如果跳过时间）要跳过的时间量" },
            {
                "add to add new binds, clear to clear all binds and list to list all binds",
                "add添加新绑定，clear清除所有绑定，list列出所有绑定"
            },
            { "(if adding) Keyboard button to bind", "（如果添加）要绑定的键盘按键" },
            { "(if adding) Commands to run, separated by ; and spaces replaced by _", "（如果添加）要运行的命令，用;分隔，空格用_代替" },
            { "How many times to repeat the command", "重复命令的次数" },
            { "How long to wait between each repeat, in seconds", "每次重复之间的等待时间（秒）" },
            { "Where to make the explosion", "爆炸发生的位置" },
            { "Limb muscle damage range (0-60)", "肢体肌肉伤害范围（0-60）" },
            { "Limb skin damage range (0-75)", "肢体皮肤伤害范围（0-75）" },
            { "Chance to do skin damage (0.2)", "皮肤伤害概率（0.2）" },
            { "Chance to fracture (0.06)", "骨折概率（0.06）" },
            { "Chance to dislocate (0.135)", "脱臼概率（0.135）" },
            { "Chance to disfigure (0.34)", "毁容概率（0.34）" },
            { "Chance to bleed (0.15)", "出血概率（0.15）" },
            { "Bleeding range (4-30)", "出血量范围（4-30）" },
            { "Damage to deal to structures (500)", "对结构造成的伤害（500）" },
            { "Explosion range (12)", "爆炸范围（12）" },
            { "How hard to send things flying away (60)", "物体被炸飞的力度（60）" },
            { "Chance to inflict shrapnel (0.5)", "造成弹片伤害的概率（0.5）" },
            { "Sound to play (explosion)", "要播放的音效（爆炸）" },
            { "Enable logging?", "启用日志记录？" },
            { "ID of the sound to play", "要播放的音效ID" },
        };

        var argShortDescTranslations = new Dictionary<string, string>
        {
            { "position", "位置" },
            { "int index", "整数 索引" },
            { "int amount", "整数 数量" },
            { "int times", "整数 次数" },
            { "int", "整数" },
            { "float condition", "小数 状态" },
            { "float experience", "小数 经验" },
            { "float height", "小数 高度" },
            { "float amount", "小数 数量" },
            { "float delay", "小数 延迟" },
            { "float", "小数" },
            { "bool important", "布尔 重要程度" },
            { "bool", "布尔" },
            { "string id", "文本 ID" },
            { "string text", "文本 内容" },
            { "string type", "文本 类型" },
            { "string key", "文本 键" },
            { "string field", "文本 字段" },
            { "string limb", "文本 肢体" },
            { "string name", "文本 名称" },
            { "string description", "文本 描述" },
            { "string action", "文本 操作" },
            { "string command", "文本 命令" },
            { "string sound", "文本 音效" },
            { "object value", "对象 值" },
            { "element", "元素" },
            { "color", "颜色" },
            { "type", "类型" },
            { "keycode", "键码" },
            { "range muscledmg", "范围 肌肉伤害" },
            { "range skindmg", "范围 皮肤伤害" },
            { "range bleed", "范围 出血" },
            { "float skinchance", "小数 皮肤概率" },
            { "float breakchance", "小数 骨折概率" },
            { "float dislchance", "小数 脱臼概率" },
            { "float disfchance", "小数 毁容概率" },
            { "float bleedchance", "小数 出血概率" },
            { "float structuredmg", "小数 结构伤害" },
            { "float range", "小数 范围" },
            { "float velocity", "小数 速度" },
            { "float shrapnelchance", "小数 弹片概率" },
            { "(if skipping time)", "（如果跳过时间）" },
            { "(if adding)", "（如果添加）" },
        };

        foreach (var command in ConsoleScript.Commands)
        {
            if (commandDescriptions.TryGetValue(command.name, out var description))
            {
                command.description = description;
            }

            if (command.argDescription is not { Length: > 0 }) continue;
            var argDescription = command.argDescription;
            for (var i = 0; i < argDescription.Length; i++)
            {
                var d = argDescription[i];
                var newShortDesc = d.shortDesc;
                var newLongDesc = d.longDesc;

                if (argShortDescTranslations.TryGetValue(d.shortDesc, out var shortCn))
                    newShortDesc = shortCn;

                if (argLongDescTranslations.TryGetValue(d.longDesc, out var longCn))
                    newLongDesc = longCn;

                argDescription[i] = (newShortDesc, newLongDesc);
            }
        }
    }

    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    public static void StartPostfix(ConsoleScript __instance)
    {
        if (__instance.input == null || __instance.input.placeholder == null) return;
        var placeholderText = __instance.input.placeholder.GetComponent<TextMeshProUGUI>();
        if (placeholderText != null)
            placeholderText.text = "输入命令 (输入 help 查看命令列表)";
    }

    [HarmonyPatch("RegisterSpawnEntities")]
    [HarmonyPrefix]
    public static bool RegisterSpawnEntitiesPrefix(ConsoleScript __instance)
    {
        if (Traverse.Create(__instance).Field<bool>("registeredSpawnEntities").Value)
            return false;

        var command = ConsoleScript.SearchExact("spawn");
        if (command == null) return false;

        var source = new List<GameObject>();
        source.AddRange(Resources.LoadAll<GameObject>(""));
        var list = source
            .Where(x => (bool)(UnityEngine.Object)x.GetComponent<Item>()
                        || (bool)(UnityEngine.Object)x.GetComponent<BuildingEntity>())
            .ToList();

        command.argAutofill ??= new Dictionary<int, List<string>>();

        command.argAutofill[0] = list.Select(x => x.name).ToList();

        Traverse.Create(__instance).Field<bool>("registeredSpawnEntities").Value = true;
        return false;
    }

    [HarmonyPatch("CheckArgumentCount")]
    [HarmonyPrefix]
    public static bool CheckArgumentCountPrefix(string[] args, int desired)
    {
        return args.Length <= desired
            ? throw new Exception($"预计至少有 {desired} 个参数, 但得到了 {args.Length - 1} 个")
            : false;
    }

    [HarmonyPatch("CheckForWorld")]
    [HarmonyPrefix]
    public static bool CheckForWorld()
    {
        if (!(bool)(UnityEngine.Object)PlayerCamera.main)
            throw new Exception("没有加载任何世界。尝试开始游戏？");

        return false;
    }

    [HarmonyPatch("ParseInt")]
    [HarmonyPrefix]
    public static bool ParseIntPrefix(string s, ref int __result)
    {
        if (!int.TryParse(s, out var result))
            throw new Exception($"\"{s}\" 不是一个有效的整数值！(0, 1, 5 等)");
        __result = result;
        return false;
    }

    [HarmonyPatch("ParseFloat")]
    [HarmonyPrefix]
    public static bool ParseFloatPrefix(string s, ref float __result)
    {
        if (!float.TryParse(s,
                System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands,
                System.Globalization.CultureInfo.InvariantCulture, out var result))
            throw new Exception($"\"{s}\" 不是一个有效的浮点数值！(2, 0.7, 14.1 等)");
        __result = result;
        return false;
    }

    [HarmonyPatch("ParseBool")]
    [HarmonyPrefix]
    public static bool ParseBoolPrefix(string s, ref bool __result)
    {
        if (!bool.TryParse(s, out var result))
            throw new Exception($"\"{s}\" 不是一个有效的布尔值！(true/false)");
        __result = result;
        return false;
    }

    [HarmonyPatch("ParseColor")]
    [HarmonyPrefix]
    public static bool ParseColorPrefix(string s, ref Color __result)
    {
        if (!ColorUtility.TryParseHtmlString(s, out var color))
            throw new Exception($"\"{s}\" 不是一个有效的颜色值！(#FFFFFF 等)");
        __result = color;
        return false;
    }

    [HarmonyPatch("ParseKeycode")]
    [HarmonyPrefix]
    public static bool ParseKeycodePrefix(string s, ref KeyCode __result)
    {
        if (!Enum.TryParse<KeyCode>(s, out var result))
            throw new Exception($"\"{s}\" 不是一个有效的按键代码！");
        __result = result;
        return false;
    }

    [HarmonyPatch("ParsePosition")]
    [HarmonyPrefix]
    public static bool ParsePositionPrefix(string s, ref Vector2 __result)
    {
        switch (s)
        {
            case "cursor":
                Debug.Assert(Camera.main != null, "Camera.main != null");
                __result = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                return false;
            case "player":
                __result = PlayerCamera.main.body.transform.position;
                return false;
            case "random":
                __result = new Vector2(
                    UnityEngine.Random.Range(-1f, 1f) * WorldGeneration.world.halfWidth,
                    UnityEngine.Random.Range(-1f, 1f) * WorldGeneration.world.halfHeight);
                return false;
            default:
                var strArray = s.Split([','], StringSplitOptions.RemoveEmptyEntries);
                if (strArray.Length != 2
                    || !float.TryParse(strArray[0], System.Globalization.NumberStyles.Float
                                                    | System.Globalization.NumberStyles.AllowThousands,
                        System.Globalization.CultureInfo.InvariantCulture, out var x)
                    || !float.TryParse(strArray[1], System.Globalization.NumberStyles.Float
                                                    | System.Globalization.NumberStyles.AllowThousands,
                        System.Globalization.CultureInfo.InvariantCulture, out var y))
                    throw new Exception($"\"{s}\" 不是一个有效的位置！(cursor/player/random/#,#)");
                __result = new Vector2(x, y);
                return false;
        }
    }

    [HarmonyPatch("ParseRangeF")]
    [HarmonyPrefix]
    public static bool ParseRangeFPrefix(string s, ref RangeF __result)
    {
        var strArray = s.Split(['-'], StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length != 2
            || !float.TryParse(strArray[0], System.Globalization.NumberStyles.Float
                                            | System.Globalization.NumberStyles.AllowThousands,
                System.Globalization.CultureInfo.InvariantCulture, out var min)
            || !float.TryParse(strArray[1], System.Globalization.NumberStyles.Float
                                            | System.Globalization.NumberStyles.AllowThousands,
                System.Globalization.CultureInfo.InvariantCulture, out var max))
            throw new Exception($"\"{s}\" 不是一个有效的范围值！(#-#)");

        __result = new RangeF(min, max);
        return false;
    }

    [HarmonyPatch("HandleDescriptionText")]
    [HarmonyPostfix]
    public static void HandleDescriptionTextPostfix(ConsoleScript __instance)
    {
        var traverse = Traverse.Create(__instance);
        var descriptionText = traverse.Field("descriptionText").GetValue<TextMeshProUGUI>();
        if (descriptionText == null) return;

        if (descriptionText.text.Contains("Invalid command"))
        {
            descriptionText.text = descriptionText.text.Replace("Invalid command", "无效命令");
        }
    }

    private static readonly Dictionary<string, string> StaticLogTranslations = new()
    {
        { "Saved console config to disk.", "已将控制台配置保存到磁盘" },
        { "Scroll to see all commands.", "滚动以查看所有命令" },
        { "Healed the player.", "已治愈玩家" },
        { "Stopped bleeding on player.", "已停止玩家出血" },
        { "Killed the player.", "已杀死玩家" },
        { "Saved the game and quit to menu.", "已保存游戏并退出到主菜单" },
        { "Reset all player skills.", "已重置所有玩家技能" },
        { "Copied console log to clipboard", "已将控制台日志复制到剪贴板" },
        { "Deleted all PlayerPrefs keys.", "已删除所有PlayerPrefs键" },
        { "Opened saves folder.", "已打开存档文件夹" },
        { "Opened locale folder.", "已打开本地化文件夹" },
        { "Opened custom music folder.", "已打开自定义音乐文件夹" },
        { "Playing new biome track.", "正在播放新的生物群系曲目" },
        { "Opened music menu.", "已打开音乐菜单" },
        { "Cleared all custom binds.", "已清除所有自定义绑定" },
        { "No command binds active.", "没有激活的命令绑定" },
        { "Echo enabled.", "已启用回显" },
        { "Toggled UI.", "已切换UI显示" },
        { "Toggled freecam. Remember to use the arrow keys!", "已切换自由摄像机。记住使用方向键！" },
        { "Enabled noclip.", "已启用无碰撞模式" },
        { "Disabled noclip.", "已禁用无碰撞模式" },
        { "Enabled fullbright.", "已启用全局照明" },
        { "Disabled fullbright.", "已禁用全局照明" },
        { "Enabled error logging.", "已启用错误日志记录" },
        { "Disabled error logging.", "已禁用错误日志记录" },
    };

    private static string TranslateLogText(string text)
    {
        if (StaticLogTranslations.TryGetValue(text, out string staticTranslation))
        {
            return staticTranslation;
        }

        if (text.Contains("<color=orange>Invalid command"))
        {
            return text.Replace("Invalid command", "无效命令");
        }

        if (text.Contains("Scroll to see all commands."))
        {
            return text.Replace("Scroll to see all commands.", "滚动以查看所有命令");
        }

        if (text.Contains("failed:"))
        {
            return Regex.Replace(text, "failed:", "失败:");
        }

        var match = Regex.Match(text, """^Created "(.+)" at \(?(-?\d+(?:\.\d+)?), ?(-?\d+(?:\.\d+)?)\)?\.$""");
        if (match.Success)
        {
            return $"已在 ({match.Groups[2].Value}, {match.Groups[3].Value}) 处创建 \"{match.Groups[1].Value}\"";
        }

        match = Regex.Match(text,
            """^Spawned all items from category "(.+)" at \(?(-?\d+(?:\.\d+)?), ?(-?\d+(?:\.\d+)?)\)?\.$""");
        if (match.Success)
        {
            return $"已从类别 \"{match.Groups[1].Value}\" 中生成所有物品在 ({match.Groups[2].Value}, {match.Groups[3].Value})";
        }

        match = Regex.Match(text, @"^Teleported player to \(?(-?\d+(?:\.\d+)?), ?(-?\d+(?:\.\d+)?)\)?\.$");
        if (match.Success)
        {
            return $"已将玩家传送到 ({match.Groups[1].Value}, {match.Groups[2].Value})";
        }

        match = Regex.Match(text, @"^Going to layer (\d+) \((.+)\)\.$");
        if (match.Success)
        {
            return $"正在前往第 {match.Groups[1].Value} 层 ({match.Groups[2].Value})";
        }

        match = Regex.Match(text, """^Made player say "(.+)"$""");
        if (match.Success)
        {
            return $"已让玩家说出 \"{match.Groups[1].Value}\"";
        }

        match = Regex.Match(text, """^Set max framerate to "(\d+)"\.$""");
        if (match.Success)
        {
            return $"已将最大FPS设置为 \"{match.Groups[1].Value}\"";
        }

        match = Regex.Match(text, """^Displayed alert "(.+)"$""");
        if (match.Success)
        {
            return $"已显示提醒 \"{match.Groups[1].Value}\"";
        }

        match = Regex.Match(text, @"^Set timescale to (.+)\.$");
        if (match.Success)
        {
            return $"已将时间刻度设置为 {match.Groups[1].Value}";
        }

        match = Regex.Match(text, @"^Console height set to (.+)\.$");
        if (match.Success)
        {
            return $"已将控制台高度设置为 {match.Groups[1].Value}";
        }

        match = Regex.Match(text, @"^Console (.+) color set to (.+)\.$");
        if (match.Success)
        {
            var element = match.Groups[1].Value;
            var elementCn = element == "text" ? "文本" : "背景";
            return $"已将控制台{elementCn}颜色设置为 {match.Groups[2].Value}";
        }

        match = Regex.Match(text, @"^Gave the player (\d+) strength experience\.$");
        if (match.Success)
        {
            return $"已给予玩家 {match.Groups[1].Value} 点力量经验";
        }

        match = Regex.Match(text, @"^Gave the player (\d+) resilience experience\.$");
        if (match.Success)
        {
            return $"已给予玩家 {match.Groups[1].Value} 点韧性经验";
        }

        match = Regex.Match(text, @"^Gave the player (\d+) intelligence experience\.$");
        if (match.Success)
        {
            return $"已给予玩家 {match.Groups[1].Value} 点智力经验";
        }

        match = Regex.Match(text, """^Deleted PlayerPrefs key "(.+)"\.$""");
        if (match.Success)
        {
            return $"已删除PlayerPrefs键 \"{match.Groups[1].Value}\"";
        }

        match = Regex.Match(text, """^Set player body field "(.+)" to "(.+)"\.$""");
        if (match.Success)
        {
            return $"已将玩家身体字段 \"{match.Groups[1].Value}\" 设置为 \"{match.Groups[2].Value}\"";
        }

        match = Regex.Match(text, """^Set "(.+)" field "(.+)" to "(.+)"\.$""");
        if (match.Success)
        {
            return $"已将 \"{match.Groups[1].Value}\" 的字段 \"{match.Groups[2].Value}\" 设置为 \"{match.Groups[3].Value}\"";
        }

        match = Regex.Match(text, """^Dismembered "(.+)"\.$""");
        if (match.Success)
        {
            return $"已截断 \"{match.Groups[1].Value}\"";
        }

        match = Regex.Match(text, @"^Set unchipped to (True|False)\.$");
        if (match.Success)
        {
            return $"已将无芯片模式设置为 {(match.Groups[1].Value == "True" ? "开启" : "关闭")}";
        }

        match = Regex.Match(text, """^Added command "(.+)"$""");
        if (match.Success)
        {
            return $"已添加命令 \"{match.Groups[1].Value}\"";
        }

        match = Regex.Match(text, @"^Added ([\d.]+)mL of (.+) to (.+) \(([\d.]+)mL\/([\d.]+)mL\)\.$");
        if (match.Success)
        {
            return
                $"已向 {match.Groups[3].Value} 添加了 {match.Groups[1].Value}mL 的 {match.Groups[2].Value}（{match.Groups[4].Value}mL/{match.Groups[5].Value}mL）";
        }

        match = Regex.Match(text, """^Couldn't find any objects named "(.+)"$""");
        if (match.Success)
        {
            return $"找不到名称为 \"{match.Groups[1].Value}\" 的任何对象";
        }

        match = Regex.Match(text, """^Found (\d+) objects? named "(.+)". Teleported to "(.+)", index (\d+)\.$""");
        if (match.Success)
        {
            return
                $"找到了 {match.Groups[1].Value} 个名称为 \"{match.Groups[2].Value}\" 的对象。已传送到 \"{match.Groups[3].Value}\"，索引 {match.Groups[4].Value}";
        }

        match = Regex.Match(text, """^Removed command "(.+)"$""");
        if (match.Success)
        {
            return $"已移除命令 \"{match.Groups[1].Value}\"";
        }

        match = Regex.Match(text, @"^Skipped playing track by ([\d.]+) seconds\.$");
        if (match.Success)
        {
            return $"已将曲目快进 {match.Groups[1].Value} 秒";
        }

        match = Regex.Match(text, """^Bound "(.+)" to "(.+)"\.$""");
        if (match.Success)
        {
            return $"已将 \"{match.Groups[1].Value}\" 绑定到 \"{match.Groups[2].Value}\"";
        }

        match = Regex.Match(text, @"^Created explosion at \(?(-?\d+(?:\.\d+)?), ?(-?\d+(?:\.\d+)?)\)?\.$");
        if (match.Success)
        {
            return $"已在 ({match.Groups[1].Value}, {match.Groups[2].Value}) 处创建爆炸";
        }

        match = Regex.Match(text, @"^Floodfilled (.+) at \(?(-?\d+(?:\.\d+)?), ?(-?\d+(?:\.\d+)?)\)?\.$");
        if (match.Success)
        {
            return $"已在 ({match.Groups[2].Value}, {match.Groups[3].Value}) 处填充了 {match.Groups[1].Value}";
        }

        match = Regex.Match(text, """^Playing sound "(.+)"$""");
        if (match.Success)
        {
            return $"正在播放音效 \"{match.Groups[1].Value}\"";
        }

        match = Regex.Match(text, @"^Instantiated (\d+) different plushies at player\.$");
        return match.Success
            ? $"已在玩家位置生成了 {match.Groups[1].Value} 种不同的毛绒玩具"
            : text;
    }

    [HarmonyPatch("LogToConsole")]
    [HarmonyPrefix]
    public static bool LogToConsolePrefix(ConsoleScript __instance, string text)
    {
        var t = Traverse.Create(__instance);
        var echo = t.Field("echo").GetValue<bool>();
        if (!echo) return false;

        var translated = TranslateLogText(text);

        var logs = t.Field("logs").GetValue<List<string>>();
        logs.Add(
            $"[<alpha=#55>{TimeSpan.FromSeconds(Time.realtimeSinceStartup):mm\\:ss}<alpha=#FF>] {translated}");

        if (logs.Count > 100)
            logs.RemoveAt(0);

        var active = t.Field("active").GetValue<bool>();
        if (!active) return false;
        var logText = t.Field("logText").GetValue<TextMeshProUGUI>();
        if (logText != null)
            logText.text = string.Join("\n", logs);

        return false;
    }
}