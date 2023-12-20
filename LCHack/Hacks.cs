using GameNetcodeStuff;
using HarmonyLib;
using LCHack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Screen = UnityEngine.Screen;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace badcheats
{
	internal class Hacks : MonoBehaviour
	{
		private Dictionary<Type, List<Component>> objectCache = new Dictionary<Type, List<Component>>();
        private GUISkin H4cksUI;

		private bool cursorIsLocked = true;
		private bool insertKeyWasPressed = false;
		private float lastToggleTime = 0f;
		private const float toggleCooldown = 0.5f; 

        private static List<Collider> disabledColliders = new List<Collider>();

        public static void Noclip(bool noclipStatus)
        {
            if (noclipStatus)
            {
                Collider[] array = UnityEngine.Object.FindObjectsOfType<Collider>();
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i] != null && array[i].enabled)
                    {
                        array[i].enabled = false;
                        disabledColliders.Add(array[i]);
                    }
                }
                Collider component = GameNetworkManager.Instance.localPlayerController.GetComponent<Collider>();
                if (component != null && component.enabled)
                {
                    component.enabled = false;
                    disabledColliders.Add(component);
                }
                Vector3 vector = default(Vector3);
                if (Keyboard.current.wKey.isPressed)
                {
                    vector += GameNetworkManager.Instance.localPlayerController.transform.forward;
                }
                if (Keyboard.current.sKey.isPressed)
                {
                    vector -= GameNetworkManager.Instance.localPlayerController.transform.forward;
                }
                if (Keyboard.current.aKey.isPressed)
                {
                    vector -= GameNetworkManager.Instance.localPlayerController.transform.right;
                }
                if (Keyboard.current.dKey.isPressed)
                {
                    vector += GameNetworkManager.Instance.localPlayerController.transform.right;
                }
                if (Keyboard.current.spaceKey.isPressed)
                {
                    vector += GameNetworkManager.Instance.localPlayerController.transform.up;
                }
                if (Keyboard.current.ctrlKey.isPressed)
                {
                    vector -= GameNetworkManager.Instance.localPlayerController.transform.up;
                }
                GameNetworkManager.Instance.localPlayerController.transform.position += vector * Settings.Cheat_NoClipSpeed;
                return;
            }
            foreach (Collider collider in disabledColliders)
            {
                if (collider != null)
                {
                    collider.enabled = true;
                }
            }
            disabledColliders.Clear();
        }


        #region Keypress logic

        private const int VK_INSERT = 0x2D;

		[DllImport("user32.dll")]
		private static extern short GetAsyncKeyState(int vKey);

		private bool IsKeyDown(int keyCode)
		{
			return (GetAsyncKeyState(keyCode) & 0x8000) > 0;
		}

		#endregion

		#region Create singleton
		private static Hacks instance;
		public void Awake()
		{
			if (Hacks.instance == null)
			{
				Hacks.instance = this;
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
				return;
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
		public static Hacks Instance
		{
			get
			{
				if (Hacks.instance == null)
				{
					Hacks.instance = UnityEngine.Object.FindObjectOfType<Hacks>();
					if (Hacks.instance == null)
					{
						Hacks.instance = new GameObject("HacksSingleton").AddComponent<Hacks>();
					}
				}
				return Hacks.instance;
			}
		}
        #endregion

        [Obsolete]
        public void Start()
		{
			try
			{
				var harmony = new Harmony("H4cks");
				harmony.PatchAll();

                StartCoroutine(CacheRefreshRoutine());
                StartCoroutine(CacheRefreshOthers());
                StartCoroutine(CacheRefreshChams());

                HUDManager hudshake = (HUDManager)UnityEngine.Object.FindObjectOfType(typeof(HUDManager));
                DestroyObject(hudshake.playerScreenShakeAnimator);
            }
			catch
			{

			}
        }
		
		#region Cache
		IEnumerator CacheRefreshRoutine()
		{
			while (true)
			{
                if (StartOfRound.Instance == null)
                {
                    objectCache.Clear();
                }

                CacheObjects<EntranceTeleport>();
                CacheObjects<GrabbableObject>();
                CacheObjects<Terminal>();
                CacheObjects<Landmine>();
                CacheObjects<Turret>();
                CacheObjects<PlayerControllerB>();
                CacheObjects<SteamValveHazard>();
                CacheObjects<EnemyAI>();

                yield return new WaitForSeconds(Settings.ESP_RefreshTime);
			}
		}

        IEnumerator CacheRefreshOthers()
        {
            while (true)
            {
                if (is_KeyDown)
                {
                    ShipLights shipLights = (ShipLights)UnityEngine.Object.FindObjectOfType(typeof(ShipLights));
                    shipLights.SetShipLightsServerRpc(!shipLights.areLightsOn);
                }

                yield return new WaitForSeconds(0.009f);
            }
        }

        IEnumerator CacheRefreshChams()
        {
            while (true)
            {
                if (Settings.ESP_Enemy_Chams)
                {
                    foreach (EnemyAI Enemy in FindObjectsOfType<EnemyAI>())
                    {
                        foreach (Renderer renderer in Enemy.GetComponentsInChildren<Renderer>())
                        {
                            renderer.material = chamsMaterial;
                            renderer.material.SetColor(_Color, Color.red);
                        }
                    }
                }

                if (Settings.ESP_Loot_Chams)
                {
                    foreach (GrabbableObject Grabbable in FindObjectsOfType<GrabbableObject>())
                    {
                        foreach (Renderer renderer in Grabbable.GetComponentsInChildren<Renderer>())
                        {
                            renderer.material = chamsMaterial;
                            renderer.material.SetColor(_Color, Color.green);
                        }
                    }
                }

                if (Settings.ESP_Turret_Chams)
                {
                    foreach (Turret Grabbable in FindObjectsOfType<Turret>())
                    {
                        foreach (Renderer renderer in Grabbable.GetComponentsInChildren<Renderer>())
                        {
                            renderer.material = chamsMaterial;
                            renderer.material.SetColor(_Color, Color.red);
                        }
                    }
                }

                if (Settings.ESP_Mine_Chams)
                {
                    foreach (Landmine Grabbable in FindObjectsOfType<Landmine>())
                    {
                        foreach (Renderer renderer in Grabbable.GetComponentsInChildren<Renderer>())
                        {
                            renderer.material = chamsMaterial;
                            renderer.material.SetColor(_Color, Color.red);
                        }
                    }
                }

                yield return new WaitForSeconds(4f);
            }
        }

		void UpdateEnemyCount()
		{
			if (objectCache.TryGetValue(typeof(EnemyAI), out var enemies_counter))
			{
				Settings.ESP_Enemy_Count = enemies_counter.Count;
            }

            if (objectCache.TryGetValue(typeof(GrabbableObject), out var items_counter))
            {
                Settings.ESP_Loot_Count = items_counter.Count;
            }

            if (objectCache.TryGetValue(typeof(DeadBodyInfo), out var player_dead))
            {
                Settings.ESP_Loot_Count = player_dead.Count;
            }
        }

		void CacheObjects<T>() where T : Component
		{
			objectCache[typeof(T)] = new List<Component>(FindObjectsOfType<T>());
		}
		#endregion

		#region ESP Drawing

		public static bool WorldToScreen(Camera camera, Vector3 world, out Vector3 screen)
		{
			screen = camera.WorldToViewportPoint(world);

			screen.x *= Screen.width;
			screen.y *= Screen.height;

			screen.y = Screen.height - screen.y;

			return screen.z > 0;
		}

        private void ProcessObjects<T>(Func<T, Vector3, string> labelBuilder) where T : Component
		{
			if (!objectCache.TryGetValue(typeof(T), out var cachedObjects))
				return;

			foreach (T obj in cachedObjects.Cast<T>())
			{
                if (obj is GrabbableObject GO && (GO.isPocketed || GO.isHeld))
				{
                    continue;
				}

				if (obj is GrabbableObject GO2 && GO2.itemProperties.itemName is "clipboard" or "Sticky note")
				{
                    continue;
				}

				if (obj is SteamValveHazard valve && valve.triggerScript.interactable == false)
				{
					continue;
				}

				Vector3 screen;

                if (WorldToScreen(GameNetworkManager.Instance.localPlayerController.gameplayCamera,
						obj.transform.position, out screen))
				{
                    if (Settings.ESP_Loot_Name)
                    {
                        string label = labelBuilder(obj, screen);

                        float distance = Vector3.Distance(GameNetworkManager.Instance.localPlayerController.gameplayCamera.transform.position, obj.transform.position);
                        distance = (float)Math.Round(distance);
                        ESPUtils.DrawString(new Vector2(screen.x, screen.y + 8f), $"[Distance: {distance}] " + label, GetColorForObject<T>(), true, 11, FontStyle.Normal, 1);
                    }
				}
            }
		}

        private void ProcessPlayers()
		{
			if (!objectCache.TryGetValue(typeof(PlayerControllerB), out var cachedPlayers))
				return;

			foreach (PlayerControllerB player in cachedPlayers.Cast<PlayerControllerB>())
			{
				if (player.IsLocalPlayer || player.playerUsername == GameNetworkManager.Instance.localPlayerController.playerUsername || player.disconnectedMidGame)
				{
					continue;
				}

                Vector3 screen;
				if (WorldToScreen(GameNetworkManager.Instance.localPlayerController.gameplayCamera,
						player.transform.position, out screen))
				{
                    if (Settings.ESP_Player_Name)
                    {
                        string label;

                        if (player.IsClient)
                        {
                            label = player.playerUsername;
                        }
                        else
                        {
                            label = player.playerUsername;
                        }

                        float distance = Vector3.Distance(GameNetworkManager.Instance.localPlayerController.gameplayCamera.transform.position, player.transform.position);
                        distance = (float)Math.Round(distance);
                        ESPUtils.DrawString(new Vector2(screen.x, screen.y + 8f), $"[Distance: {distance}] " + label + $" : {player.health}", Color.green, true, 11, FontStyle.Normal, 1, distance);
                    }
                }
			}
		}

        Camera camera = Camera.main;

        private void ProcessEnemies()
		{
            if (!objectCache.TryGetValue(typeof(EnemyAI), out var cachedEnemies))
				return;

			foreach (EnemyAI enemyAI in cachedEnemies.Cast<EnemyAI>())
			{
                Vector3 screen;
                if (WorldToScreen(GameNetworkManager.Instance.localPlayerController.gameplayCamera, enemyAI.eye.transform.position, out screen))
                {
                    if (Settings.ESP_Enemy_Name)
					{
                        string label;
                        if (string.IsNullOrWhiteSpace(enemyAI.enemyType.enemyName))
                        {
                            label = "Unkown Enemy";
                        }
                        else
                        {
                            label = enemyAI.enemyType.enemyName;
                        }

                        float distance = Vector3.Distance(GameNetworkManager.Instance.localPlayerController.gameplayCamera.transform.position, enemyAI.eye.transform.position);
                        distance = (float)Math.Round(distance);

                        ESPUtils.DrawString(new Vector2(screen.x, screen.y + 8f), $"[Distance: {distance}] " + label + $" : {enemyAI.enemyHP}", Color.red, true, 11, FontStyle.Normal, 1, distance);
                    }
                }

                UpdateEnemyCount();
            }
		}

        private Material chamsMaterial;
        private int _Color;

        private Color GetColorForObject<T>()
		{
			switch (typeof(T).Name)
			{
				case "EntranceTeleport":
					return Color.cyan;
				case "GrabbableObject":
					return Color.yellow;
				case "Landmine":
					return Color.red;
				case "Turret":
					return Color.red;
				case "Terminal":
					return Color.magenta;
				default:
					return Color.white;
			}
		}

        #endregion

        public void OnGUI()
        {
            string title;

            if (Settings.Overlay_GUI)
			{
                GUI.skin = H4cksUI;
                Hacks.dbgWindow = GUILayout.Window(0, Hacks.dbgWindow, new GUI.WindowFunction(DrawMenuWindow), "Any Suggestions?, Discord: _jannik_2008");
            }

            if (Settings.Overlay_Crosshair)
            {
                Vector2 lineHorizontalStart = new Vector2(Screen.width / 2 - Settings.crosshairScale, Screen.height / 2);
                Vector2 lineHorizontalEnd = new Vector2(Screen.width / 2 + Settings.crosshairScale, Screen.height / 2);

                Vector2 lineVerticalStart = new Vector2(Screen.width / 2, Screen.height / 2 - Settings.crosshairScale);
                Vector2 lineVerticalEnd = new Vector2(Screen.width / 2, Screen.height / 2 + Settings.crosshairScale);

                ESPUtils.DrawLine(lineHorizontalStart, lineHorizontalEnd, Settings.crosshairCol, Settings.lineThickness);
                ESPUtils.DrawLine(lineVerticalStart, lineVerticalEnd, Settings.crosshairCol, Settings.lineThickness);
            }

            if (StartOfRound.Instance != null)
			{
                title = $"[BadCheats - Lethal Company]" +
                    $"\nEnemy Count: {Settings.ESP_Enemy_Count}" +
                    $"\nItems Count: {Settings.ESP_Loot_Count}" +
                    $"\nDead Count: {Settings.ESP_Dead_Count}";
                ESPUtils.DrawString(new Vector2(Screen.width / 2 - 700, Screen.height / 2 - 50), title, Color.magenta, true, 18, FontStyle.Normal, 1);

                if (Settings.ESP_Mine)
                {
                    ProcessObjects<Landmine>((landmine, vector) => "Landmine");
                }

                if (Settings.ESP_Turret)
                {
                    ProcessObjects<Turret>((turret, vector) => "Turret");
                }

                ProcessObjects<EntranceTeleport>((entrance, vector) => entrance.isEntranceToBuilding ? " Entrance" : " Exit");
                ProcessObjects<Terminal>((terminal, vector) => "Terminal");
                ProcessObjects<SteamValveHazard>((valve, vector) => "Steam Valve");

                if (Settings.ESP_Player)
                {
                    ProcessPlayers();
                }

                if (Settings.ESP_Loot)
                {
                    ProcessObjects<GrabbableObject>((grabbableObject, vector) => grabbableObject.itemProperties.itemName);
                }

                if (Settings.ESP_Enemy)
                {
                    ProcessEnemies();
                }

                if (Settings.Cheat_ItemPower)
                {
                    if (GameNetworkManager.Instance.localPlayerController.currentlyHeldObjectServer != null)
                    {
                        if (GameNetworkManager.Instance.localPlayerController.IsServer)
                            GameNetworkManager.Instance.localPlayerController.currentlyHeldObjectServer.insertedBattery.charge = 1f;
                    }
                }
            }	
			else
			{
                title = $"[BadCheats - Lethal Company]";
                ESPUtils.DrawString(new Vector2(Screen.width / 2 - 700, Screen.height / 2 - 50), title, Color.magenta, true, 18, FontStyle.Normal, 1);

                Settings.Cheat_Health = false;
                Settings.ESP_Loot = false;
                Settings.ESP_Enemy = false;
                Settings.Cheat_Stammina = false;
                Settings.Cheat_ScanRange = false;
                Settings.Cheat_ItemPower = false;
                Settings.ESP_Enemy_Chams = false;
                Settings.ESP_Loot_Chams = false;
                Settings.Cheat_OpenDoorThroughWalls = false;
				Settings.Cheat_LootItemsThroughWalls = false;
				Settings.Cheat_NightVision = false;
                Settings.Cheat_GrabShipItem = false;
                Settings.Cheat_ShowClock = false;
                Settings.Cheat_AntiDisconnect = false;
            }
		}

        void Update()
		{
            bool isKeyDown = IsKeyDown(VK_INSERT);

			if (isKeyDown && !insertKeyWasPressed && Time.time - lastToggleTime > toggleCooldown)
			{
                Settings.Overlay_GUI = !Settings.Overlay_GUI;
				lastToggleTime = Time.time;
			}

			if (StartOfRound.Instance != null)
			{
                PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
                GrabbableObject[] objects = new GrabbableObject[Settings.ESP_Loot_Count];

                if (Settings.Overlay_GUI)
				{
					// Menu opened, unlock cursor
					Cursor.visible = true;
					Cursor.lockState = CursorLockMode.None;
					cursorIsLocked = false;
				}
				else if (!cursorIsLocked)
				{
					// To prevent not being able to use ESC menu. We only free up the cursor once
					Cursor.visible = false;
					Cursor.lockState = CursorLockMode.Locked;
					cursorIsLocked = true;
				}

                if (Settings.Cheat_FastClimb)
                {
                    localPlayerController.climbSpeed = Settings.Cheat_ClimbSpeed;
                }

                if (Settings.Cheat_NoClip)
                {
                    Noclip(true);
                    return;
                }
                else
                {
                    Noclip(false);
                }

                if (Settings.Cheat_SpeedHack)
                {
                    localPlayerController.movementSpeed = Settings.Cheat_PlayerSpeed;
                }

                if (Settings.Cheat_JumpHack)
                {
                    localPlayerController.jumpForce = Settings.Cheat_PlayerJump;
                }

                if (Settings.Cheat_Vacum)
                {
                    //for (int i = 0; i < objects.Length; i++)
                    //{
                    //    objects[i].parentObject.transform.position = new Vector3(localPlayerController.transform.position.x, localPlayerController.transform.position.y, localPlayerController.transform.position.z + 1f);
                    //    objects[i].parentObject.transform.localPosition = new Vector3(localPlayerController.transform.position.x, localPlayerController.transform.position.y, localPlayerController.transform.position.z + 1f);
                    //    objects[i].transform.localPosition = new Vector3(localPlayerController.transform.position.x, localPlayerController.transform.position.y, localPlayerController.transform.position.z + 1f);
                    //}
                }
            }

            insertKeyWasPressed = isKeyDown;
		}

        private static Rect dbgWindow = new Rect(300, 300, 280f, 0f);
        private static bool is_KeyDown = false;

        void DrawMenuWindow(int id)
		{
            if (StartOfRound.Instance != null)
            {
                GUI.skin = H4cksUI;

                if (GUILayout.Button("<color=#ffff00> • Player Options </color>"))
                {
                    Settings.TAB_Player = !Settings.TAB_Player;
                    dbgWindow.height = 0f;
                }

                if (Settings.TAB_Player)
                {
                    Settings.Cheat_Health = GUILayout.Toggle(Settings.Cheat_Health, "➧ Unlimited Heatlh");
                    Settings.Cheat_Stammina = GUILayout.Toggle(Settings.Cheat_Stammina, "➧ Unlimited Stammina");
                    Settings.Cheat_ScanRange = GUILayout.Toggle(Settings.Cheat_ScanRange, "➧ Unlimited Scan Range");
                    Settings.Cheat_ItemPower = GUILayout.Toggle(Settings.Cheat_ItemPower, "➧ Unlimited Item Power");
                    Settings.Cheat_ShotgunAmmo = GUILayout.Toggle(Settings.Cheat_ShotgunAmmo, "➧ Unlimited Ammo (Shotgun)");
                    Settings.Cheat_AntiDisconnect = GUILayout.Toggle(Settings.Cheat_AntiDisconnect, "➧ Anti Disconnect");
                    Settings.Cheat_FastClimb = GUILayout.Toggle(Settings.Cheat_FastClimb, $"➧ Climb Ladder Speed {Settings.Cheat_ClimbSpeed}");
                    if (Settings.Cheat_FastClimb)
                    {
                        Settings.Cheat_ClimbSpeed = (float)GUILayout.HorizontalSlider(Settings.Cheat_ClimbSpeed, 1f, 30f);
                    }
                    Settings.Cheat_SpeedHack = GUILayout.Toggle(Settings.Cheat_SpeedHack, $"➧ Player Speed {Settings.Cheat_PlayerSpeed}");
                    if (Settings.Cheat_SpeedHack)
                    {
                        Settings.Cheat_PlayerSpeed = (float)GUILayout.HorizontalSlider(Settings.Cheat_PlayerSpeed, 8f, 200f);
                    }
                    Settings.Cheat_JumpHack = GUILayout.Toggle(Settings.Cheat_JumpHack, $"➧ Player JumpHigh {Settings.Cheat_PlayerJump}");
                    if (Settings.Cheat_JumpHack)
                    {
                        Settings.Cheat_PlayerJump = (float)GUILayout.HorizontalSlider(Settings.Cheat_PlayerJump, 10f, 200f);
                    }
                    if (GUILayout.Button("Revive Me"))
                    {
                        StartOfRound StartOfRound = GameObject.FindObjectOfType<StartOfRound>();
                        if (StartOfRound != null)
                        {
                            StartOfRound.PlayerHasRevivedServerRpc();
                        }
                    }
                    Settings.Cheat_XPValue = (int)GUILayout.HorizontalSlider(Settings.Cheat_XPValue, -1000, 1000);
                    if (GUILayout.Button($"Set XP {Settings.Cheat_XPValue}"))
                    {
                        HUDManager instance = HUDManager.Instance;
                        instance.localPlayerXP += Settings.Cheat_XPValue;
                    }
                }

                if (GUILayout.Button("<color=#ffff00> • Visual Options </color>"))
                {
                    Settings.TAB_Visual = !Settings.TAB_Visual;
                    dbgWindow.height = 0f;
                }

                if (Settings.TAB_Visual)
                {
                    Settings.Cheat_ShowClock = GUILayout.Toggle(Settings.Cheat_ShowClock, "➧ Draw Clock");
                    Settings.Cheat_NightVision = GUILayout.Toggle(Settings.Cheat_NightVision, "➧ Draw Night Vision");
                    Settings.Overlay_Crosshair = GUILayout.Toggle(Settings.Overlay_Crosshair, "➧ Draw Crosshair");
                    if (Settings.Overlay_Crosshair)
                    {
                        GUILayout.Label("➧ Custom Crosshair");
                        Settings.crosshairScale = (float)GUILayout.HorizontalSlider(Settings.crosshairScale, 0.1f, 50f);
                        Settings.lineThickness = (float)GUILayout.HorizontalSlider(Settings.lineThickness, 0.1f, 50f);
                    }
                    GUILayout.Label("➧ Enemy List");
                    Settings.ESP_Enemy = GUILayout.Toggle(Settings.ESP_Enemy, "➧ Draw Enemy ESP");
                    if (Settings.ESP_Enemy)
                    {
                        Settings.ESP_Enemy_Name = GUILayout.Toggle(Settings.ESP_Enemy_Name, "➧ Draw Enemy Name");
                        Settings.ESP_Enemy_Chams = GUILayout.Toggle(Settings.ESP_Enemy_Chams, "➧ Draw Enemy Chams");
                    }
                    GUILayout.Label("➧ Loot List");
                    Settings.ESP_Loot = GUILayout.Toggle(Settings.ESP_Loot, "➧ Draw Loot ESP");
                    if (Settings.ESP_Loot)
                    {
                        Settings.ESP_Loot_Name = GUILayout.Toggle(Settings.ESP_Loot_Name, "➧ Draw Loot Name");
                        Settings.ESP_Loot_Chams = GUILayout.Toggle(Settings.ESP_Loot_Chams, "➧ Draw Loot Chams");
                    }
                    GUILayout.Label("➧ Turret List");
                    Settings.ESP_Turret = GUILayout.Toggle(Settings.ESP_Turret, "➧ Draw Turret ESP");
                    if (Settings.ESP_Turret)
                    {
                        Settings.ESP_Turret_Chams = GUILayout.Toggle(Settings.ESP_Turret_Chams, "➧ Draw Turret Chams");
                    }
                    GUILayout.Label("➧ Landmine List");
                    Settings.ESP_Mine = GUILayout.Toggle(Settings.ESP_Mine, "➧ Draw Landmine ESP");
                    if (Settings.ESP_Mine)
                    {
                        Settings.ESP_Mine_Chams = GUILayout.Toggle(Settings.ESP_Mine_Chams, "➧ Draw Landmine Chams");
                    }
                }

                if (GUILayout.Button("<color=#ffff00> • Exploits Options </color>"))
                {
                    Settings.TAB_Exploits = !Settings.TAB_Exploits;
                    dbgWindow.height = 0f;
                }

                if (Settings.TAB_Exploits)
                {
                    Settings.Cheat_GrabShipItem = GUILayout.Toggle(Settings.Cheat_GrabShipItem, "➧ Build Anywhere");
                    Settings.Cheat_NoClip = GUILayout.Toggle(Settings.Cheat_NoClip, $"➧ NoClip [Speed: {Settings.Cheat_NoClipSpeed}]");
                    if (Settings.Cheat_NoClip)
                    {
                        Settings.Cheat_NoClipSpeed = (float)GUILayout.HorizontalSlider(Settings.Cheat_NoClipSpeed, 0.1f, 1f);
                    }
                    Settings.Cheat_OpenDoorThroughWalls = GUILayout.Toggle(Settings.Cheat_OpenDoorThroughWalls, "➧ Open Doors Through Walls");
                    Settings.Cheat_LootItemsThroughWalls = GUILayout.Toggle(Settings.Cheat_LootItemsThroughWalls, "➧ Loot Items Through Walls");
                    if (GUILayout.Button("Insta Tentacle near Players"))
                    {
                        DepositItemsDesk depositItemsDesk = (DepositItemsDesk)UnityEngine.Object.FindObjectOfType(typeof(DepositItemsDesk));
                        depositItemsDesk.AttackPlayersServerRpc();
                    }
                    if (GUILayout.Button("Turn Ship Light Loop ON/OFF"))
                    {
                        is_KeyDown = !is_KeyDown;
                    }
                    if (GUILayout.Button("End Game"))
                    {
                        ((StartOfRound)UnityEngine.Object.FindObjectOfType(typeof(StartOfRound))).EndGameServerRpc(0);
                    }
                }

                if (GUILayout.Button("<color=#ffff00> • Misc Options </color>"))
                {
                    Settings.TAB_Misc = !Settings.TAB_Misc;
                    dbgWindow.height = 0f;
                }

                if (Settings.TAB_Misc)
                {
                    Settings.Cheat_Vacum = GUILayout.Toggle(Settings.Cheat_Vacum, "➧ Vacum Items");

                    Settings.Cheat_MoneyValue = (int)GUILayout.HorizontalSlider(Settings.Cheat_MoneyValue, -500, 500);
                    if (GUILayout.Button($"Add Money {Settings.Cheat_MoneyValue}"))
                    {
                        UnityEngine.Object.FindObjectOfType<Terminal>().groupCredits += Settings.Cheat_MoneyValue;
                    }
                }

                if (GUILayout.Button("<color=#ffff00> • Host Options </color>"))
                {
                    Settings.TAB_Host = !Settings.TAB_Host;
                    dbgWindow.height = 0f;
                }

                if (Settings.TAB_Host)
                {
                    Settings.Lobby_Slots = (int)GUILayout.HorizontalSlider(Settings.Lobby_Slots, 4, 16);
                    if (GUILayout.Button($"Set Lobby Slots " + Settings.Lobby_Slots))
                    {
                        GameNetworkManager.Instance.SetLobbyJoinable(true);
                        GameNetworkManager.Instance.maxAllowedPlayers = Settings.Lobby_Slots;
                    }
                }

                if (GUILayout.Button("<color=#ffff00> • Others Options </color>"))
                {
                    Settings.TAB_Others = !Settings.TAB_Others;
                    dbgWindow.height = 0f;
                }

                if (Settings.TAB_Others)
                {

                }
            }
            else
            {
                GUILayout.Label("<color=#ffff00> • Works only ingame </color>");
            }

            if (GUILayout.Button("<color=#ffff00> • Cheat Options </color>"))
            {
                Settings.TAB_Cheat = !Settings.TAB_Cheat;
                dbgWindow.height = 0f;
            }

            if (Settings.TAB_Cheat)
            {
                GUILayout.Label($"<color=#ffff00> • Refresh Time Cache {Settings.ESP_RefreshTime} </color>");
                Settings.ESP_RefreshTime = (float)GUILayout.HorizontalSlider(Settings.ESP_RefreshTime, 0.001f, 1f);

                if (GUILayout.Button("SteamID + Name Changer Test"))
                {
                    System.Random random = new System.Random();
                    PlayerControllerB playerController = new PlayerControllerB();
                    playerController.playerSteamId = (ulong)random.Next(1000, 99999999);
                    playerController.playerUsername = H4cks_Class.RandomString(16);
                }
            }

            GUI.DragWindow();
		}
    }
}	
