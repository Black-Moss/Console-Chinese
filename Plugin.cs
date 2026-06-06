using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace ConsoleScriptZH
{
    [BepInPlugin(Guid, Name, Version)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;
        public const string Guid = "blackmoss.consolechinese";
        public const string Name = "控制台汉化";
        public const string Version = "1.1.0";
        private readonly Harmony _harmony = new(Guid);

        public void Awake()
        {
            Logger = base.Logger;

            _harmony.PatchAll();
            Logger.LogInfo("控制台指令汉化补丁已启动！");
        }
    }
}