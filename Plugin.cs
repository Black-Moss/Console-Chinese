using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace ConsoleChinese
{
    [BepInPlugin(Guid, Name, Version)]
    public class Plugin : BaseUnityPlugin
    {
        internal new static ManualLogSource Logger;
        public const string Guid = "blackmoss.consolechinese";
        public const string Name = "控制台汉化";
        public const string Version = "1.2.0";
        private readonly Harmony _harmony = new(Guid);

        public void Awake()
        {
            Logger = base.Logger;

            _harmony.PatchAll();
            Logger.LogInfo("控制台指令汉化补丁已启动！");
        }
    }
}