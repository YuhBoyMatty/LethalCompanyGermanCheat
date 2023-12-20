using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using static IngamePlayerSettings;
using UnityEngine;

namespace badcheats
{
	[HarmonyPatch]
	public static class AssignNewNodesPatch
	{
		[HarmonyPatch(typeof(HUDManager), "AssignNewNodes")]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var codes = new List<CodeInstruction>(instructions);

			for (int i = 0; i < codes.Count; i++)
			{
				if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 20f)
				{
					if (Settings.Cheat_ScanRange)
						codes[i].operand = 500f;
					else
						codes[i].operand = 20f;
				}
			}
			return codes;
		}
	}


	[HarmonyPatch]
	public static class SyncBatteryServerRpcPatch
	{
		[HarmonyPatch(typeof(GrabbableObject), "SyncBatteryServerRpc")]
		public static bool Prefix(GrabbableObject __instance, ref int charge)
		{
			if (Settings.Cheat_ItemPower)
			{
				if (__instance.itemProperties.requiresBattery)
				{
					__instance.insertedBattery.empty = false;
					__instance.insertedBattery.charge = 1f;
					charge = 100;
				}
				
			}

			return true;
		}
	}

    [HarmonyPatch(typeof(GameNetworkManager), "Disconnect")]
    public static class GameNetworkManagerDisconnectPatch
    {
        public static bool Prefix(GameNetworkManager __instance)
        {
            if (Settings.Cheat_AntiDisconnect)
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(NetworkConnectionManager), "OnClientDisconnectFromServer")]
    public static class OnClientDisconnectPatch
    {
        public static bool Prefix(ulong clientId)
        {
            if (Settings.Cheat_AntiDisconnect)
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(StartOfRound), "OnPlayerDC")]
    public static class OnPlayerDCPatch
    {
        public static bool Prefix(int playerObjectNumber, ulong clientId, StartOfRound __instance)
        {
            if (Settings.Cheat_AntiDisconnect)
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(GameNetworkManager), "Singleton_OnClientDisconnectCallback")]
    public static class Singleton_OnClientDisconnectCallbackPatch
    {
        public static bool Prefix(ulong clientId, GameNetworkManager __instance)
        {
            if (Settings.Cheat_AntiDisconnect)
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(NetworkConnectionManager), "DisconnectEventHandler")]
    public static class DisconnectEventHandlerPatch
    {
        public static bool Prefix(ulong transportClientId, GameNetworkManager __instance)
        {
            if (Settings.Cheat_AntiDisconnect)
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(KillLocalPlayer), "KillPlayer")]
    public static class KillPlayerPatch
    {
        public static bool Prefix(PlayerControllerB playerWhoTriggered)
        {
            if (playerWhoTriggered.isPlayerControlled && Settings.Cheat_Health)
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch]
	public static class DamagePlayerPatch
	{
		[HarmonyPatch(typeof(PlayerControllerB), "DamagePlayer")]
		public static bool Prefix(PlayerControllerB __instance)
		{
			if (__instance.actualClientId == GameNetworkManager.Instance.localPlayerController.actualClientId)
			{
				if (Settings.Cheat_Health)
				{
					return false;
				}
			}

			return true;
		}
	}

    [HarmonyPatch]
    public static class SetInsideLightingDimnessPatch
    {
        // Token: 0x06000023 RID: 35 RVA: 0x00003260 File Offset: 0x00001460
        [HarmonyPatch(typeof(TimeOfDay), "SetInsideLightingDimness")]
        public static void Postfix()
        {
            if (Settings.Cheat_ShowClock)
            {
                HUDManager.Instance.SetClockVisible(true);
            }
        }
    }

    [HarmonyPatch]
    public static class ShipBuildModeManager_PlayerMeetsConditionsToBuild_Patch
    {
        [HarmonyPatch(typeof(ShipBuildModeManager), "PlayerMeetsConditionsToBuild")]
        private static void Postfix(ShipBuildModeManager __instance, PlaceableShipObject ___placingObject, PlayerControllerB ___player, ref bool __result)
        {
            if (Settings.Cheat_GrabShipItem)
            {
                bool flag2 = __instance.InBuildMode && (___placingObject == null || (___placingObject.inUse) || StartOfRound.Instance.unlockablesList.unlockables[___placingObject.unlockableID].inStorage);
                if (flag2)
                {
                    __result = false;
                }
                bool isTypingChat = GameNetworkManager.Instance.localPlayerController.isTypingChat;
                if (isTypingChat)
                {
                    __result = false;
                }
                bool flag3 = ___player.isPlayerDead || ___player.inSpecialInteractAnimation || ___player.activatingItem;
                if (flag3)
                {
                    __result = false;
                }
                bool flag4 = ___player.disablingJetpackControls || ___player.jetpackControls;
                if (flag4)
                {
                    __result = false;
                }
                bool flag5 =  !___player.isInHangarShipRoom;
                if (flag5)
                {
                    __result = false;
                }
                bool flag6 = StartOfRound.Instance.fearLevel > 0.4f;
                if (flag6)
                {
                    __result = false;
                }
                bool flag7 = StartOfRound.Instance.shipAnimator.GetCurrentAnimatorStateInfo(0).tagHash != Animator.StringToHash("ShipIdle");
                if (flag7)
                {
                    __result = false;
                }
                __result = true;
            }
        }
    }

    [HarmonyPatch]
	public static class ShipBuildModeManager_Update_Patch
	{
        [HarmonyPatch(typeof(ShipBuildModeManager), "Update")]
        private static void Postfix(ref ShipBuildModeManager __instance, ref bool ___CanConfirmPosition)
        {
            if (Settings.Cheat_GrabShipItem)
            {
                ___CanConfirmPosition = true;
                __instance.ghostObjectRenderer.sharedMaterial = __instance.ghostObjectGreen;
            }
        }
    }

    [HarmonyPatch]
    public static class Shotgun_Update_Patch
    {
        [HarmonyPatch(typeof(ShotgunItem), "Update")]
        private static void Postfix(ref ShotgunItem __instance)
        {
            if (__instance.isHeld == true)
            {
                if (Settings.Cheat_ShotgunAmmo)
                {
                    __instance.shellsLoaded = 2;
                }
            }
        }
    }

    [HarmonyPatch]
	public static class LateUpdatePostfixPatch
	{
		[HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
		public static void Postfix(PlayerControllerB __instance)
		{
            if (GameNetworkManager.Instance.localPlayerController.actualClientId != __instance.actualClientId) 
				return; 

			if (Settings.Cheat_Stammina)
			{
                __instance.sprintMeter = 1f;
                if (__instance.sprintMeterUI != null)
                {
                    __instance.sprintMeterUI.fillAmount = 1f;
                }
            }

			if (Settings.Cheat_NightVision)
			{
                __instance.nightVision.enabled = true;
                __instance.nightVision.intensity = 3000f;
                __instance.nightVision.range = 10000f;
            }
			else
			{
                __instance.nightVision.enabled = false;
                __instance.nightVision.intensity = 366.9317f;
                __instance.nightVision.range = 12f;
            }

            if (Settings.Cheat_LootItemsThroughWalls | Settings.Cheat_OpenDoorThroughWalls)
            {
                __instance.grabDistance = 10000f;
                LayerMask layerMask = LayerMask.GetMask(new string[] { "Props" });
                if (Settings.Cheat_LootItemsThroughWalls)
                {
                    layerMask = LayerMask.GetMask(new string[] { "Props" });
                }
                if (Settings.Cheat_OpenDoorThroughWalls)
                {
                    layerMask = LayerMask.GetMask(new string[] { "InteractableObject" });
                }
                if (Settings.Cheat_OpenDoorThroughWalls && Settings.Cheat_LootItemsThroughWalls)
                {
                    layerMask = LayerMask.GetMask(new string[] { "Props", "InteractableObject" });
                }
                BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
                typeof(PlayerControllerB).GetField("interactableObjectsMask", bindingFlags).SetValue(__instance, layerMask.value);
            }
            else
            {
                BindingFlags bindingFlags2 = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
                typeof(PlayerControllerB).GetField("interactableObjectsMask", bindingFlags2).SetValue(__instance, 832);
                __instance.grabDistance = 5f;
            }
        }
	}
}
