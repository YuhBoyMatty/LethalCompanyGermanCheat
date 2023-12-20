using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace badcheats
{
    public class Settings
    {
        public static bool EnglishVersion = true;

        public static bool Cheat_Health = false;
        public static bool Cheat_Stammina = false;
        public static bool Cheat_ScanRange = false;
        public static bool Cheat_ItemPower = false;
        public static bool Cheat_ShotgunAmmo = false;
        public static bool Cheat_FastClimb = false;
        public static bool Cheat_SpeedHack = false;
        public static bool Cheat_JumpHack = false;
        public static bool Cheat_NoClip = false;
        public static bool Cheat_Vacum = false;
        public static bool Cheat_OpenDoorThroughWalls = false;
        public static bool Cheat_LootItemsThroughWalls = false;
        public static bool Cheat_NightVision = false;
        public static bool Cheat_ShowClock = false;
        public static bool Cheat_AntiDisconnect = false;
        public static bool Cheat_GrabShipItem = false;
        public static string Cheat_NoClipReadSpeed = "0.1";
        public static float Cheat_NoClipSpeed = 0.1f;
        public static string Cheat_MoneyReadValue = "200";
        public static int Cheat_MoneyValue = 200;
        public static float Cheat_PlayerSpeed = 8f;
        public static float Cheat_PlayerJump = 10f;
        public static float Cheat_ClimbSpeed = 5f;
        public static int Cheat_XPValue = 100;

        public static int Lobby_Slots = 4;

        public static bool Overlay_GUI = false;
        public static bool Overlay_Crosshair = true;
        public static byte[] UIDesign;
        public static Color crosshairCol = Color.red;
        public static float crosshairScale = 14f;
        public static float lineThickness = 1.75f;

        public static float ESP_RefreshTime = 0.2f;

        public static bool ESP_Turret = false;
        public static bool ESP_Turret_Chams = false;

        public static bool ESP_Mine = false;
        public static bool ESP_Mine_Chams = false;

        public static bool ESP_Player = false;
        public static bool ESP_Player_Name = false;
        public static bool ESP_Player_Chams = false;
        public static int ESP_Dead_Count = 0;

        public static bool ESP_Loot = false;
        public static bool ESP_Loot_Name = false;
        public static bool ESP_Loot_Chams = false;
        public static int ESP_Loot_Count = 0;

        public static bool ESP_Enemy = false;
        public static bool ESP_Enemy_Name = false;
        public static bool ESP_Enemy_Chams = false;
        public static int ESP_Enemy_Count = 0;

        public static bool TAB_Player = false;
        public static bool TAB_Visual = false;
        public static bool TAB_Exploits = false;
        public static bool TAB_Misc = false;
        public static bool TAB_Host = false;
        public static bool TAB_Others = false;
        public static bool TAB_Cheat = false;
        public static bool TAB_Draw = false;
    }
}
