







using Archipelago;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Packets;
using Harmony;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppSystem;
using Il2CppSystem.Collections.ObjectModel;
using Il2CppSystem.IO;
using Il2CppSystem.Runtime.Remoting.Messaging;
using Il2CppTMPro;
using MelonLoader;
using MelonLoader.TinyJSON;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Linq.Expressions;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

using static Il2CppSystem.Globalization.CultureInfo;
using static UnityEngine.GraphicsBuffer;


namespace PVZFusionArchipelago
{
    public class Class1 : MelonMod
    {
        public const int maxItems = 100; // holy shit good programming practice?\
        public const int maxLocations = 400;
        public int seedSlots = 0;
        public static ArchipelagoSession session;
        public static int currentPlayerSlot;
        public static bool[] unlockedArray = new bool[maxItems];
        public static bool[] checkedArray = new bool[maxLocations];


        const string page1 = "InGameUI(Clone)/Bottom/SeedLibrary/Grid/Pages/Page1/";
        const string page2 = "InGameUI(Clone)/Bottom/SeedLibrary/Grid/ColorfulCards/Page1/";
  
        const string advanturePage1 = "ChallengeMenu(Clone)/Levels/PageAdvantureLevel/Pages/Page1/";
        const string advanturePage2 = "ChallengeMenu(Clone)/Levels/PageAdvantureLevel/Pages/Page2/";
        const string advanturePage3 = "ChallengeMenu(Clone)/Levels/PageAdvantureLevel/Pages/Page3/";
        const string advanturePage4 = "ChallengeMenu(Clone)/Levels/PageNewAdvantureLevel/Pages/Page1/";

        const string showcasePage = "ExploreMenu(Clone)/Level/Line1/";
        const string challengePage = "ChallengeMenu(Clone)/Levels/PageUnlockChallenge/Page1/";
        const string slotPath = "InGameUI(Clone)/SeedBank/SeedGroup/";
        public int ringLinkMode = 0;
        public static int prevSun;
        public static bool sunExists = false;
        public static int goalType = 0;



        public AssetBundle bundle;
        public GameObject sunPrefab;
        readonly IConnectionInfoProvider connectionInfoProvider;


        public override void OnInitializeMelon()
        {
            for (int i = 0; i < maxItems; i++)
            {
                unlockedArray[i] = false;
            }
            string modDir = this.MelonAssembly.Location;
            string modFolder = System.IO.Path.GetDirectoryName(modDir);
            string jsonPath = System.IO.Path.Combine(modFolder, "config.json");

            string unity3dpath = System.IO.Path.Combine(Application.dataPath, "data.unity3d");

            bundle = AssetBundle.LoadFromFile(unity3dpath);
            if (bundle != null)
            {
                GameObject sunPrefab = bundle.LoadAsset<GameObject>("SunPrefab");
            }








            if (System.IO.File.Exists(jsonPath))
            {
                string jsonContent = System.IO.File.ReadAllText(jsonPath);
                // Deserialize if needed
                APData data = JsonConvert.DeserializeObject<APData>(jsonContent);
                MelonLogger.Msg("Loaded JSON data");
                session = ArchipelagoSessionFactory.CreateSession(data.serverAddress, data.serverPort);
                Connect("Plants Vs Zombies Fusion", data.slotName, "", session);
            }
            else
            {
                MelonLogger.Warning("config.json not found, place it in the same folder as PVZFusionArchipelago.dll");
            }

        }



        public override void OnLateUpdate()
        {
            AttachClickHandler("TrophyPrefab");
            if (session == null)
            {
                MelonLogger.Msg("didnt find archipelago session");
                return;
            }
            CheckForNewItems();
            try
            {
                if (ringLinkMode != 0)
                {


                    var canvas = GameObject.Find("Canvas");
                    if (canvas != null)
                    {
                        var levelUI = canvas.transform.Find("InGameUI(Clone)");



                        var board = GameObject.Find("Board");
                        if ((board != null) && (levelUI != null))
                        {

                            var boardComp = board.GetComponent(Il2CppType.Of<Board>());
                            var type = boardComp.GetIl2CppType();
                            var field = type.GetField("theSun");
                            Il2CppSystem.Object rawValue = field.GetValue(boardComp);
                            int intValue = Il2CppSystem.Convert.ToInt32(rawValue) / 10;
                            if (sunExists == false)
                            {
                                prevSun = intValue;
                                sunExists = true;
                            }

                            if (intValue != prevSun)
                            {

                                var dataDict = new Dictionary<string, JToken>
                        {
                            { "time", JToken.FromObject(System.DateTimeOffset.UtcNow.ToUnixTimeSeconds()) },
                            { "source", JToken.FromObject(session.Players.ActivePlayer.Slot) },
                            { "amount", JToken.FromObject((intValue)-(prevSun)) }
                        };

                                session.Socket.SendPacket(new BouncePacket()
                                {
                                    Tags = new List<string> { "RingLink" },
                                    Slots = new List<int> { session.Players.ActivePlayer.Slot },
                                    Data = dataDict
                                });
                                prevSun = intValue;
                            }


                        }
                        else
                        {
                            sunExists = false;

                        }
                    }
                    else
                    {
                        sunExists = false;

                    }

                }
            }
            catch (System.Exception e) {
                MelonLogger.Error("Exception when handling ring link, likely a thread error that Im not skilled enough to fix");
            }
       

            CheckConveyorBelt();


            if (unlockedArray[1] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "PeaShooter"); }
            if (unlockedArray[2] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "SunFlower"); }
            if (unlockedArray[3] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "CherryBomb"); }
            if (unlockedArray[4] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "WallNut"); }
            if (unlockedArray[5] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "PotatoMine"); }
            if (unlockedArray[6] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "Chomper"); }
            if (unlockedArray[7] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "Present"); }
            if (unlockedArray[8] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "TallNut"); }
            if (unlockedArray[9] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "EndoFlame"); }
            if (unlockedArray[10] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "SmallPuff"); }
            if (unlockedArray[11] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "FumeShroom"); }
            if (unlockedArray[12] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "HypnoShroom"); }
            if (unlockedArray[13] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "ScaredyShroom"); }
            if (unlockedArray[14] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "IceShroom"); }
            if (unlockedArray[15] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "DoomShroom"); }
            if (unlockedArray[16] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "PresentZombie"); }
            if (unlockedArray[17] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "GloomShroom"); }
            if (unlockedArray[18] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "GraveBust"); }
            if (unlockedArray[19] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "LilyPad"); }
            if (unlockedArray[20] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "Squash"); }
            if (unlockedArray[21] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "ThreePeater"); }
            if (unlockedArray[22] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "Tanglekelp"); }
            if (unlockedArray[23] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "Jalapeno"); }
            if (unlockedArray[24] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "Caltrop"); }
            if (unlockedArray[25] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "TorchWood"); }
            if (unlockedArray[26] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "Spike"); }
            if (unlockedArray[27] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "Wheat"); }
            if (unlockedArray[28] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "SeaShroom"); }
            if (unlockedArray[29] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "Plantern"); }
            if (unlockedArray[30] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "Cactus"); }
            if (unlockedArray[31] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "Blover"); }
            if (unlockedArray[32] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "StarFruit"); }
            if (unlockedArray[33] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "Pumpkin"); }
            if (unlockedArray[34] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "Magnetshroom"); }
            if (unlockedArray[35] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "Cattail"); }
            if (unlockedArray[36] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "Imitater"); }
            if (unlockedArray[37] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "Cabbage"); }
            if (unlockedArray[38] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "Pot"); }
            if (unlockedArray[39] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "Cornpult"); }
            if (unlockedArray[40] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "Garlic"); }
            if (unlockedArray[41] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "Umbrellaleaf"); }
            if (unlockedArray[42] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "Marigold"); }
            if (unlockedArray[43] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "Melonpult"); }
            if (unlockedArray[44] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "CobCannon"); }
            if (unlockedArray[45] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "MixBomb"); }
            if (unlockedArray[46] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "PineFurnace"); }
            if (unlockedArray[47] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "SpruceShooter"); }
            if (unlockedArray[48] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "ShulkFlower"); }
            if (unlockedArray[49] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "IceLotus"); }
            if (unlockedArray[50] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "WaterAloes"); }
            if (unlockedArray[51] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "Bamboo"); }
            if (unlockedArray[52] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "SnowPresent"); }
            if (unlockedArray[53] == false)
            { HideTargetObjectChildren("CanvasUp", page1 + "SpruceBallista"); }

            if (unlockedArray[55] == false)
            { HideTargetObjectChildren("CanvasUp", page2 + "CattailGirl"); }
            if (unlockedArray[56] == false)
            { HideTargetObjectChildren("CanvasUp", page2 + "SwordStar"); }
            if (unlockedArray[57] == false)
            { HideTargetObjectChildren("CanvasUp", page2 + "Squalour"); }
            if (unlockedArray[58] == false)
            { HideTargetObjectChildren("CanvasUp", page2 + "Hamburger"); }
            if (unlockedArray[59] == false)
            { HideTargetObjectChildren("CanvasUp", page2 + "EndoFlameGirl"); }
            if (unlockedArray[60] == false)
            { HideTargetObjectChildren("CanvasUp", page2 + "IceBean"); }
            if (unlockedArray[61] == false)
            { HideTargetObjectChildren("CanvasUp", page2 + "Prismflower"); }
            if (unlockedArray[62] == false)
            { HideTargetObjectChildren("CanvasUp", page2 + "SniperPea"); }
            if (unlockedArray[63] == false)
            { HideTargetObjectChildren("CanvasUp", page2 + "Chrysantheautumn"); }
            if (unlockedArray[64] == false)
            { HideTargetObjectChildren("CanvasUp", page2 + "IcePeach"); }
            if (unlockedArray[65] == false)
            { HideTargetObjectChildren("CanvasUp", page2 + "FrozenPear"); }
            if (unlockedArray[66] == false)
            { HideTargetObjectChildren("CanvasUp", page2 + "PassionFruit"); }





            if (unlockedArray[70] == false)//shovel
            { HideTargetObject("Canvas", "InGameUI(Clone)/ShovelBank");}

            if (unlockedArray[71] == false)//fertilizer
            {
                var board = GameObject.Find("Board");
                if (board != null)
                {
                    var fertilizerTransform = board.transform.Find("Ferilize(Clone)");

                    if (fertilizerTransform != null)
                    { GameObject.Destroy(fertilizerTransform.gameObject); }
                }
            }



            if (unlockedArray[72] == false)//gloves
            { HideTargetObject("Canvas", "InGameUI(Clone)/GloveBank");
                HideTargetObjectChildren("Canvas", "GardenUI(Clone)/Tools/GloveBank");
            }
            if (unlockedArray[74] == false)//mallet
            { HideTargetObject("Canvas", "InGameUI(Clone)/HammerBank"); }


            if (unlockedArray[76] == false)//watering can
            { HideTargetObjectChildren("Canvas", "GardenUI(Clone)/Tools/WaterBank"); }

            if (unlockedArray[77] == false)//gramophone
            { HideTargetObjectChildren("Canvas", "GardenUI(Clone)/Tools/PhonographBank"); }

            if (unlockedArray[78] == false)//bug spray
            { HideTargetObjectChildren("Canvas", "GardenUI(Clone)/Tools/BugSprayBank"); }

            if (unlockedArray[79] == false)//wheelbarrow
            { HideTargetObjectChildren("Canvas", "GardenUI(Clone)/Tools/WheelBarrowBank"); }








            if (unlockedArray[90] == false)
            {   HideTargetObject("Canvas", advanturePage1 + "Lv1");
                HideTargetObject("Canvas", advanturePage1 + "Lv2");
                HideTargetObject("Canvas", advanturePage1 + "Lv3");
                HideTargetObject("Canvas", advanturePage1 + "Lv4");
                HideTargetObject("Canvas", advanturePage1 + "Lv5");
                HideTargetObject("Canvas", advanturePage1 + "Lv6");
                HideTargetObject("Canvas", advanturePage1 + "Lv7");
                HideTargetObject("Canvas", advanturePage1 + "Lv8");
                HideTargetObject("Canvas", advanturePage1 + "Lv9");
            } else
            {   if (!checkedArray[1]){ HideTargetObject("Canvas", advanturePage1 + "Lv1/Window/Trophy"); }//definitely a way to automate this
                if (!checkedArray[2]){ HideTargetObject("Canvas", advanturePage1 + "Lv2/Window/Trophy"); }
                if (!checkedArray[3]){ HideTargetObject("Canvas", advanturePage1 + "Lv3/Window/Trophy"); }
                if (!checkedArray[4]){ HideTargetObject("Canvas", advanturePage1 + "Lv4/Window/Trophy"); }
                if (!checkedArray[5]){ HideTargetObject("Canvas", advanturePage1 + "Lv5/Window/Trophy"); }
                if (!checkedArray[6]){ HideTargetObject("Canvas", advanturePage1 + "Lv6/Window/Trophy"); }
                if (!checkedArray[7]){ HideTargetObject("Canvas", advanturePage1 + "Lv7/Window/Trophy"); }
                if (!checkedArray[8]){ HideTargetObject("Canvas", advanturePage1 + "Lv8/Window/Trophy"); }
                if (!checkedArray[9]){ HideTargetObject("Canvas", advanturePage1 + "Lv9/Window/Trophy"); }
            }
            if (unlockedArray[91] == false)
            {   HideTargetObject("Canvas", advanturePage1 + "Lv10");
                HideTargetObject("Canvas", advanturePage1 + "Lv11");
                HideTargetObject("Canvas", advanturePage1 + "Lv12");
                HideTargetObject("Canvas", advanturePage1 + "Lv13");
                HideTargetObject("Canvas", advanturePage1 + "Lv14");
                HideTargetObject("Canvas", advanturePage1 + "Lv15");
                HideTargetObject("Canvas", advanturePage1 + "Lv16");
                HideTargetObject("Canvas", advanturePage1 + "Lv17");
                HideTargetObject("Canvas", advanturePage1 + "Lv18");
            } else
            {   if (!checkedArray[10]){ HideTargetObject("Canvas", advanturePage1 + "Lv10/Window/Trophy"); }
                if (!checkedArray[11]){ HideTargetObject("Canvas", advanturePage1 + "Lv11/Window/Trophy"); }
                if (!checkedArray[12]){ HideTargetObject("Canvas", advanturePage1 + "Lv12/Window/Trophy"); }
                if (!checkedArray[13]){ HideTargetObject("Canvas", advanturePage1 + "Lv13/Window/Trophy"); }
                if (!checkedArray[14]){ HideTargetObject("Canvas", advanturePage1 + "Lv14/Window/Trophy"); }
                if (!checkedArray[15]){ HideTargetObject("Canvas", advanturePage1 + "Lv15/Window/Trophy"); }
                if (!checkedArray[16]){ HideTargetObject("Canvas", advanturePage1 + "Lv16/Window/Trophy"); }
                if (!checkedArray[17]){ HideTargetObject("Canvas", advanturePage1 + "Lv17/Window/Trophy"); }
                if (!checkedArray[18]){ HideTargetObject("Canvas", advanturePage1 + "Lv18/Window/Trophy"); }
            }
            if (unlockedArray[92] == false)
            {   HideTargetObject("Canvas", advanturePage2 + "Lv19");
                HideTargetObject("Canvas", advanturePage2 + "Lv20");
                HideTargetObject("Canvas", advanturePage2 + "Lv21");
                HideTargetObject("Canvas", advanturePage2 + "Lv22");
                HideTargetObject("Canvas", advanturePage2 + "Lv23");
                HideTargetObject("Canvas", advanturePage2 + "Lv24");
                HideTargetObject("Canvas", advanturePage2 + "Lv25");
                HideTargetObject("Canvas", advanturePage2 + "Lv26");
                HideTargetObject("Canvas", advanturePage2 + "Lv27");
            } else
            {   if (!checkedArray[19]) { HideTargetObject("Canvas", advanturePage2 + "Lv19/Window/Trophy"); }
                if (!checkedArray[20]) { HideTargetObject("Canvas", advanturePage2 + "Lv20/Window/Trophy"); }
                if (!checkedArray[21]) { HideTargetObject("Canvas", advanturePage2 + "Lv21/Window/Trophy"); }
                if (!checkedArray[22]) { HideTargetObject("Canvas", advanturePage2 + "Lv22/Window/Trophy"); }
                if (!checkedArray[23]) { HideTargetObject("Canvas", advanturePage2 + "Lv23/Window/Trophy"); }
                if (!checkedArray[24]) { HideTargetObject("Canvas", advanturePage2 + "Lv24/Window/Trophy"); }
                if (!checkedArray[25]) { HideTargetObject("Canvas", advanturePage2 + "Lv25/Window/Trophy"); }
                if (!checkedArray[26]) { HideTargetObject("Canvas", advanturePage2 + "Lv26/Window/Trophy"); }
                if (!checkedArray[27]) { HideTargetObject("Canvas", advanturePage2 + "Lv27/Window/Trophy"); }
            }
            if (unlockedArray[93] == false)
            {   HideTargetObject("Canvas", advanturePage2 + "Lv28");
                HideTargetObject("Canvas", advanturePage2 + "Lv29");
                HideTargetObject("Canvas", advanturePage2 + "Lv30");
                HideTargetObject("Canvas", advanturePage2 + "Lv31");
                HideTargetObject("Canvas", advanturePage2 + "Lv32");
                HideTargetObject("Canvas", advanturePage2 + "Lv33");
                HideTargetObject("Canvas", advanturePage2 + "Lv34");
                HideTargetObject("Canvas", advanturePage2 + "Lv35");
                HideTargetObject("Canvas", advanturePage2 + "Lv36");
            } else
            {   if (!checkedArray[28]) { HideTargetObject("Canvas", advanturePage2 + "Lv28/Window/Trophy"); }
                if (!checkedArray[29]) { HideTargetObject("Canvas", advanturePage2 + "Lv29/Window/Trophy"); }
                if (!checkedArray[30]) { HideTargetObject("Canvas", advanturePage2 + "Lv30/Window/Trophy"); }
                if (!checkedArray[31]) { HideTargetObject("Canvas", advanturePage2 + "Lv31/Window/Trophy"); }
                if (!checkedArray[32]) { HideTargetObject("Canvas", advanturePage2 + "Lv32/Window/Trophy"); }
                if (!checkedArray[33]) { HideTargetObject("Canvas", advanturePage2 + "Lv33/Window/Trophy"); }
                if (!checkedArray[34]) { HideTargetObject("Canvas", advanturePage2 + "Lv34/Window/Trophy"); }
                if (!checkedArray[35]) { HideTargetObject("Canvas", advanturePage2 + "Lv35/Window/Trophy"); }
                if (!checkedArray[36]) { HideTargetObject("Canvas", advanturePage2 + "Lv36/Window/Trophy"); }
            }
            if (unlockedArray[94] == false)
            {   HideTargetObject("Canvas", advanturePage3 + "Lv37");
                HideTargetObject("Canvas", advanturePage3 + "Lv38");
                HideTargetObject("Canvas", advanturePage3 + "Lv39");
                HideTargetObject("Canvas", advanturePage3 + "Lv40");
                HideTargetObject("Canvas", advanturePage3 + "Lv41");
                HideTargetObject("Canvas", advanturePage3 + "Lv42");
                HideTargetObject("Canvas", advanturePage3 + "Lv43");
                HideTargetObject("Canvas", advanturePage3 + "Lv44");
                HideTargetObject("Canvas", advanturePage3 + "Lv45");
                if (goalType == 0) { HideTargetObject("Canvas", "ChallengeMenu(Clone)/Levels/PageMiniGames/Pages/Page2/Lv77"); }
            } else
            {   if (!checkedArray[37]) { HideTargetObject("Canvas", advanturePage3 + "Lv37/Window/Trophy"); }
                if (!checkedArray[38]) { HideTargetObject("Canvas", advanturePage3 + "Lv38/Window/Trophy"); }
                if (!checkedArray[39]) { HideTargetObject("Canvas", advanturePage3 + "Lv39/Window/Trophy"); }
                if (!checkedArray[40]) { HideTargetObject("Canvas", advanturePage3 + "Lv40/Window/Trophy"); }
                if (!checkedArray[41]) { HideTargetObject("Canvas", advanturePage3 + "Lv41/Window/Trophy"); }
                if (!checkedArray[42]) { HideTargetObject("Canvas", advanturePage3 + "Lv42/Window/Trophy"); }
                if (!checkedArray[43]) { HideTargetObject("Canvas", advanturePage3 + "Lv43/Window/Trophy"); }
                if (!checkedArray[44]) { HideTargetObject("Canvas", advanturePage3 + "Lv44/Window/Trophy"); }
                if (!checkedArray[45]) { HideTargetObject("Canvas", advanturePage3 + "Lv45/Window/Trophy"); }
                if (goalType == 0) { HideTargetObject("Canvas", "ChallengeMenu(Clone)/Levels/PageMiniGames/Pages/Page2/Lv77/Window/Trophy"); }
            }

            if (unlockedArray[95] == false)
            {
                HideTargetObject("Canvas", advanturePage4 + "Lv1");
                HideTargetObject("Canvas", advanturePage4 + "Lv2");
                HideTargetObject("Canvas", advanturePage4 + "Lv3");
                HideTargetObject("Canvas", advanturePage4 + "Lv4");
                HideTargetObject("Canvas", advanturePage4 + "Lv5");
                HideTargetObject("Canvas", advanturePage4 + "Lv6");
                HideTargetObject("Canvas", advanturePage4 + "Lv7");
                HideTargetObject("Canvas", advanturePage4 + "Lv8");
                HideTargetObject("Canvas", advanturePage4 + "Lv9");
            }   else
            {   if (!checkedArray[46]) { HideTargetObject("Canvas", advanturePage4 + "Lv1/Window/Trophy"); }
                if (!checkedArray[47]) { HideTargetObject("Canvas", advanturePage4 + "Lv2/Window/Trophy"); }
                if (!checkedArray[48]) { HideTargetObject("Canvas", advanturePage4 + "Lv3/Window/Trophy"); }
                if (!checkedArray[49]) { HideTargetObject("Canvas", advanturePage4 + "Lv4/Window/Trophy"); }
                if (!checkedArray[50]) { HideTargetObject("Canvas", advanturePage4 + "Lv5/Window/Trophy"); }
                if (!checkedArray[51]) { HideTargetObject("Canvas", advanturePage4 + "Lv6/Window/Trophy"); }
                if (!checkedArray[52]) { HideTargetObject("Canvas", advanturePage4 + "Lv7/Window/Trophy"); }
                if (!checkedArray[53]) { HideTargetObject("Canvas", advanturePage4 + "Lv8/Window/Trophy"); }
                if (!checkedArray[54]) { HideTargetObject("Canvas", advanturePage4 + "Lv9/Window/Trophy"); }
            }
            if (unlockedArray[96] == false)
            {
                HideTargetObject("Canvas", "ChallengeMenu(Clone)/Levels/FirstBtns/UnlockChallenge");
            } else {
                if (!checkedArray[55]) { HideTargetObject("Canvas", challengePage + "LV7/Window/Trophy"); }
                if (!checkedArray[56]) { HideTargetObject("Canvas", challengePage + "LV10/Window/Trophy"); }
                if (!checkedArray[57]) { HideTargetObject("Canvas", challengePage + "Lv19/Window/Trophy"); }
                if (!checkedArray[58]) { HideTargetObject("Canvas", challengePage + "Lv22/Window/Trophy"); }
                if (!checkedArray[59]) { HideTargetObject("Canvas", challengePage + "Lv31/Window/Trophy"); }
                if (!checkedArray[60]) { HideTargetObject("Canvas", challengePage + "Lv32/Window/Trophy"); }
                if (!checkedArray[61]) { HideTargetObject("Canvas", challengePage + "Lv54/Window/Trophy"); }
                if (!checkedArray[62]) { HideTargetObject("Canvas", challengePage + "Lv55/Window/Trophy"); }
                if (!checkedArray[63]) { HideTargetObject("Canvas", challengePage + "Lv74/Window/Trophy"); }
                if (!checkedArray[64]) { HideTargetObject("Canvas", challengePage + "Lv75/Window/Trophy"); }
                if (!checkedArray[65]) { HideTargetObject("Canvas", challengePage + "Lv128/Window/Trophy"); }
            }
            if (unlockedArray[97] == false)
            {
                HideTargetObject("MainMenuCanvas", "MainMenu(Clone)/NewAdv");
            } else
            { 
                if (!checkedArray[66]) { HideTargetObject("Canvas", showcasePage + "Lv1/Window/Trophy"); }
                if (!checkedArray[67]) { HideTargetObject("Canvas", showcasePage + "Lv2/Window/Trophy"); }
                if (!checkedArray[68]) { HideTargetObject("Canvas", showcasePage + "Lv3/Window/Trophy"); }
                if (!checkedArray[69]) { HideTargetObject("Canvas", showcasePage + "Lv4/Window/Trophy"); }
                if (!checkedArray[70]) { HideTargetObject("Canvas", showcasePage + "Lv5/Window/Trophy"); }
                if (!checkedArray[71]) { HideTargetObject("Canvas", showcasePage + "Lv6/Window/Trophy"); }
                if (!checkedArray[72]) { HideTargetObject("Canvas", showcasePage + "Lv7/Window/Trophy"); }
                if (!checkedArray[73]) { HideTargetObject("Canvas", showcasePage + "Lv8/Window/Trophy"); }
                if (!checkedArray[74]) { HideTargetObject("Canvas", showcasePage + "Lv9/Window/Trophy"); }
                if (!checkedArray[75]) { HideTargetObject("Canvas", showcasePage + "Lv10/Window/Trophy"); }
                if (!checkedArray[76]) { HideTargetObject("Canvas", showcasePage + "Lv11/Window/Trophy"); }
                if (!checkedArray[77]) { HideTargetObject("Canvas", showcasePage + "Lv12/Window/Trophy"); }

            }

                //const string showcasePage = "ExploreMenu(Clone)/Level/Line1";
            //const string challengePage = "ChallengeMenu(Clone)/Levels/PageUnlockChallenge/Page1/";


            if (seedSlots < 9)
            { HideTargetObject("CanvasUp", slotPath + "seed13"); }
            if (seedSlots < 8)
            { HideTargetObject("CanvasUp", slotPath + "seed12"); }
            if (seedSlots < 7)
            { HideTargetObject("CanvasUp", slotPath + "seed11"); }
            if (seedSlots < 6)
            { HideTargetObject("CanvasUp", slotPath + "seed10"); }
            if (seedSlots < 5)
            { HideTargetObject("CanvasUp", slotPath + "seed9"); }
            if (seedSlots < 4)
            { HideTargetObject("CanvasUp", slotPath + "seed8"); }
            if (seedSlots < 3)
            { HideTargetObject("CanvasUp", slotPath + "seed7"); }
            if (seedSlots < 2)
            { HideTargetObject("CanvasUp", slotPath + "seed6"); }
            if (seedSlots < 1)
            { HideTargetObject("CanvasUp", slotPath + "seed5"); }



        }

        private void CheckConveyorBelt()
        {
            var canvas = GameObject.Find("Canvas");
            if (canvas == null)
            {
                //MelonLogger.Warning("Background not found");
                return;
            }
            var targetTransform = canvas.transform.Find("InGameUI(Clone)/ConeryorBelt/Content");
            if (targetTransform == null)
            {
                return;
            }

            if (ringLinkMode!=0)
            {
                var board = GameObject.Find("Board");
                if (board != null)
                {
                    var conveyorTransform = canvas.transform.Find("InGameUI(Clone)/ConeryorBelt");


                    if (conveyorTransform.gameObject.activeSelf)
                    {
                        var boardComp = board.GetComponent(Il2CppType.Of<Board>());

                        var type = boardComp.GetIl2CppType();
                        var field = type.GetField("theSun");
                        prevSun = 0;
                        field.SetValue(boardComp, 0);
                    }
                }

            }

            int childCount = targetTransform.childCount;
            for (int i = 0; i < childCount; i++)
            {

                var child = targetTransform.GetChild(i);

                var packet = child.GetComponent(Il2CppType.Of<DroppedCard>());

                //AHHHHHHHHHHHH
                
                if (packet == null)

                { continue; }
                var type = packet.GetIl2CppType();
                var field = type.GetField("thePlantType");
                Il2CppSystem.Object rawValue = field.GetValue(packet);
                int intValue = Il2CppSystem.Convert.ToInt32(rawValue);
                PlantType plantType = (PlantType)intValue;
                //what kind of scooby doo ass nonsense is il2cpp

                //MelonLogger.Msg(plantType.ToString());


                if (unlockedArray[1] == false && plantType == PlantType.Peashooter)
                {
                    GameObject.Destroy(child.gameObject);
                }
                if (unlockedArray[3] == false && plantType == PlantType.CherryBomb)
                {
                    GameObject.Destroy(child.gameObject);
                }
                if (unlockedArray[4] == false && plantType == PlantType.WallNut)
                {
                    GameObject.Destroy(child.gameObject);
                }
                if (unlockedArray[5] == false && plantType == PlantType.PotatoMine)
                {
                    GameObject.Destroy(child.gameObject);
                }
                if (unlockedArray[6] == false && plantType == PlantType.Chomper)
                {
                    GameObject.Destroy(child.gameObject);
                }



                if (unlockedArray[12] == false && plantType == PlantType.HypnoShroom)
                {
                    GameObject.Destroy(child.gameObject);
                }

                if (unlockedArray[14] == false && plantType == PlantType.IceShroom)
                {
                    GameObject.Destroy(child.gameObject);
                }

                if (unlockedArray[19] == false && plantType == PlantType.LilyPad)
                {
                    GameObject.Destroy(child.gameObject);
                }
                if (unlockedArray[20] == false && plantType == PlantType.Squash)
                {
                    GameObject.Destroy(child.gameObject);
                }
                if (unlockedArray[21] == false && plantType == PlantType.ThreePeater)
                {
                    GameObject.Destroy(child.gameObject);
                }
                if (unlockedArray[22] == false && plantType == PlantType.Tanglekelp)
                {
                    GameObject.Destroy(child.gameObject);
                }
                if (unlockedArray[23] == false && plantType == PlantType.Jalapeno)
                {
                    GameObject.Destroy(child.gameObject);
                }
                if (unlockedArray[24] == false && plantType == PlantType.Caltrop)
                {
                    GameObject.Destroy(child.gameObject);
                }
                if (unlockedArray[25] == false && plantType == PlantType.TorchWood)
                {
                    GameObject.Destroy(child.gameObject);
                }

                if (unlockedArray[28] == false && plantType == PlantType.SeaShroom)
                {
                    GameObject.Destroy(child.gameObject);
                }
                if (unlockedArray[29] == false && plantType == PlantType.Plantern)
                {
                    GameObject.Destroy(child.gameObject);
                }
                if (unlockedArray[30] == false && plantType == PlantType.Cactus)
                {
                    GameObject.Destroy(child.gameObject);
                }
                if (unlockedArray[31] == false && plantType == PlantType.Blover)
                {
                    GameObject.Destroy(child.gameObject);
                }
                if (unlockedArray[32] == false && plantType == PlantType.StarFruit)
                {
                    GameObject.Destroy(child.gameObject);
                }
                if (unlockedArray[33] == false && plantType == PlantType.Pumpkin)
                {
                    GameObject.Destroy(child.gameObject);
                }
                if (unlockedArray[34] == false && plantType == PlantType.Magnetshroom)
                {
                    GameObject.Destroy(child.gameObject);
                }
                if (unlockedArray[37] == false && plantType == PlantType.Cabbagepult)
                {
                    GameObject.Destroy(child.gameObject);
                }
                if (unlockedArray[38] == false && plantType == PlantType.Pot)
                {
                    GameObject.Destroy(child.gameObject);
                }
                if (unlockedArray[39] == false && plantType == PlantType.Cornpult)
                {
                    GameObject.Destroy(child.gameObject);
                }
                if (unlockedArray[41] == false && plantType == PlantType.Umbrellaleaf)
                {
                    GameObject.Destroy(child.gameObject);
                }
                if (unlockedArray[42] == false && plantType == PlantType.Marigold)
                {
                    GameObject.Destroy(child.gameObject);
                }
                if (unlockedArray[43] == false && plantType == PlantType.Melonpult)
                {
                    GameObject.Destroy(child.gameObject);
                }

                if (unlockedArray[46] == false && plantType == PlantType.PineFurnace)
                {
                    GameObject.Destroy(child.gameObject);
                }
                if (unlockedArray[47] == false && plantType == PlantType.SpruceShooter)
                {
                    GameObject.Destroy(child.gameObject);
                }
                if (unlockedArray[48] == false && plantType == PlantType.Shulkflower)
                {
                    GameObject.Destroy(child.gameObject);
                }
                if (unlockedArray[49] == false && plantType == PlantType.IceLotus)
                {
                    GameObject.Destroy(child.gameObject);
                }
                if (unlockedArray[50] == false && plantType == PlantType.WaterAloes)
                {
                    GameObject.Destroy(child.gameObject);
                }
                if (unlockedArray[51] == false && plantType == PlantType.Bamboo)
                {
                    GameObject.Destroy(child.gameObject);
                }






                //if (unlockedArray[41] == false)
                //{ HideTargetObjectChildren("CanvasUp", page1 + "Umbrellaleaf"); }





            }//AIMah


        }





        private void AttachClickHandler(string path)
        {

            

            var canvas = GameObject.Find("Board");
            if (canvas == null)
            {
                //MelonLogger.Warning("Background not found");
                return;
            }

            var targetTransform = canvas.transform.Find(path);
            if (targetTransform == null)
            {
                return;
            }

            GameObject obj = targetTransform.gameObject;

            if (obj.GetComponent<OnClickHandler2D>() == null)
            { 
                obj.AddComponent<OnClickHandler2D>();
                MelonLogger.Msg($"Attached OnClickHandler2D to '{obj.name}'");
                canvas = GameObject.Find("CanvasUp");
                //if (canvas == null)
                //{
                //    return;
                //}

                var leveltext = canvas.transform.Find("LevelName2");
                if (leveltext == null)
                {
                    leveltext = canvas.transform.Find("LevelName3");

                    if (leveltext == null)
                    { return; }
                }

                var textComponent = leveltext.GetComponent<TextMeshProUGUI>();
                //if (textComponent == null)//dumb error checking thats probably good practice
                //{
                //    return;
                //}
                string levelName = textComponent.text;


                switch (levelName)
                {
                    case "Adventure Mode: Classic | Level 1":
                        checkedArray[1] = true;
                        session.Locations.CompleteLocationChecks(1);//theres a way to do this in 1 cmd but i will actually die before i figure out what it is
                        session.Locations.CompleteLocationChecks(301);
                        break;
                    case "Adventure Mode: Classic | Level 2":
                        checkedArray[2] = true;
                        session.Locations.CompleteLocationChecks(2);
                        session.Locations.CompleteLocationChecks(302);
                        break;
                    case "Adventure Mode: Classic | Level 3":
                        checkedArray[3] = true;
                        session.Locations.CompleteLocationChecks(3);
                        session.Locations.CompleteLocationChecks(303);
                        break;
                    case "Adventure Mode: Classic | Level 4":
                        checkedArray[4] = true;
                        session.Locations.CompleteLocationChecks(4);
                        session.Locations.CompleteLocationChecks(304);
                        break;
                    case "Adventure Mode: Classic | Level 5":
                        checkedArray[5] = true;
                        session.Locations.CompleteLocationChecks(5);
                        session.Locations.CompleteLocationChecks(305);
                        break;
                    case "Adventure Mode: Classic | Level 6":
                        checkedArray[6] = true;
                        session.Locations.CompleteLocationChecks(6);
                        session.Locations.CompleteLocationChecks(306);
                        break;
                    case "Adventure Mode: Classic | Level 7":
                        checkedArray[7] = true;
                        session.Locations.CompleteLocationChecks(7);
                        session.Locations.CompleteLocationChecks(307);
                        break;
                    case "Adventure Mode: Classic | Level 8":
                        checkedArray[8] = true;
                        session.Locations.CompleteLocationChecks(8);
                        session.Locations.CompleteLocationChecks(308);
                        break;
                    case "Adventure Mode: Classic | Level 9":
                        checkedArray[9] = true;
                        session.Locations.CompleteLocationChecks(9);
                        session.Locations.CompleteLocationChecks(309);
                        CheckForGoalType1();
                        break;
                    case "Adventure Mode: Classic | Level 10":
                        checkedArray[10] = true;
                        session.Locations.CompleteLocationChecks(10);
                        session.Locations.CompleteLocationChecks(310);
                        break;
                    case "Adventure Mode: Classic | Level 11":
                        checkedArray[11] = true;
                        session.Locations.CompleteLocationChecks(11);
                        session.Locations.CompleteLocationChecks(311);
                        break;
                    case "Adventure Mode: Classic | Level 12":
                        checkedArray[12] = true;
                        session.Locations.CompleteLocationChecks(12);
                        session.Locations.CompleteLocationChecks(312);
                        break;
                    case "Adventure Mode: Classic | Level 13":
                        checkedArray[13] = true;
                        session.Locations.CompleteLocationChecks(13);
                        session.Locations.CompleteLocationChecks(313);
                        break;
                    case "Adventure Mode: Classic | Level 14":
                        checkedArray[14] = true;
                        session.Locations.CompleteLocationChecks(14);
                        session.Locations.CompleteLocationChecks(314);
                        break;
                    case "Adventure Mode: Classic | Level 15":
                        checkedArray[15] = true;
                        session.Locations.CompleteLocationChecks(15);
                        session.Locations.CompleteLocationChecks(315);
                        break;
                    case "Adventure Mode: Classic | Level 16":
                        checkedArray[16] = true;
                        session.Locations.CompleteLocationChecks(16);
                        session.Locations.CompleteLocationChecks(316);
                        break;
                    case "Adventure Mode: Classic | Level 17":
                        checkedArray[17] = true;
                        session.Locations.CompleteLocationChecks(17);
                        session.Locations.CompleteLocationChecks(317);
                        break;
                    case "Adventure Mode: Classic | Level 18":
                        checkedArray[18] = true;
                        session.Locations.CompleteLocationChecks(18);
                        session.Locations.CompleteLocationChecks(318);
                        CheckForGoalType1();
                        break;
                    case "Adventure Mode: Classic | Level 19":
                        checkedArray[19] = true;
                        session.Locations.CompleteLocationChecks(19);
                        session.Locations.CompleteLocationChecks(319);
                        break;
                    case "Adventure Mode: Classic | Level 20":
                        checkedArray[20] = true;
                        session.Locations.CompleteLocationChecks(20);
                        session.Locations.CompleteLocationChecks(320);
                        break;
                    case "Adventure Mode: Classic | Level 21":
                        checkedArray[21] = true;
                        session.Locations.CompleteLocationChecks(21);
                        session.Locations.CompleteLocationChecks(321);
                        break;
                    case "Adventure Mode: Classic | Level 22":
                        checkedArray[22] = true;
                        session.Locations.CompleteLocationChecks(22);
                        session.Locations.CompleteLocationChecks(322);
                        break;
                    case "Adventure Mode: Classic | Level 23":
                        checkedArray[23] = true;
                        session.Locations.CompleteLocationChecks(23);
                        session.Locations.CompleteLocationChecks(323);
                        break;
                    case "Adventure Mode: Classic | Level 24":
                        checkedArray[24] = true;
                        session.Locations.CompleteLocationChecks(24);
                        session.Locations.CompleteLocationChecks(324);
                        break;
                    case "Adventure Mode: Classic | Level 25":
                        checkedArray[25] = true;
                        session.Locations.CompleteLocationChecks(25);
                        session.Locations.CompleteLocationChecks(325);
                        break;
                    case "Adventure Mode: Classic | Level 26":
                        checkedArray[26] = true;
                        session.Locations.CompleteLocationChecks(26);
                        session.Locations.CompleteLocationChecks(326);
                        break;
                    case "Adventure Mode: Classic | Level 27":
                        checkedArray[27] = true;
                        session.Locations.CompleteLocationChecks(27);
                        session.Locations.CompleteLocationChecks(327);
                        CheckForGoalType1();
                        break;
                    case "Adventure Mode: Classic | Level 28":
                        checkedArray[28] = true;
                        session.Locations.CompleteLocationChecks(28);
                        session.Locations.CompleteLocationChecks(328);
                        break;
                    case "Adventure Mode: Classic | Level 29":
                        checkedArray[29] = true;
                        session.Locations.CompleteLocationChecks(29);
                        session.Locations.CompleteLocationChecks(329);
                        break;
                    case "Adventure Mode: Classic | Level 30":
                        checkedArray[30] = true;
                        session.Locations.CompleteLocationChecks(30);
                        session.Locations.CompleteLocationChecks(330);
                        break;
                    case "Adventure Mode: Classic | Level 31":
                        checkedArray[31] = true;
                        session.Locations.CompleteLocationChecks(31);
                        session.Locations.CompleteLocationChecks(331);
                        break;
                    case "Adventure Mode: Classic | Level 32":
                        checkedArray[32] = true;
                        session.Locations.CompleteLocationChecks(32);
                        session.Locations.CompleteLocationChecks(332);
                        break;
                    case "Adventure Mode: Classic | Level 33":
                        checkedArray[33] = true;
                        session.Locations.CompleteLocationChecks(33);
                        session.Locations.CompleteLocationChecks(333);
                        break;
                    case "Adventure Mode: Classic | Level 34":
                        checkedArray[34] = true;
                        session.Locations.CompleteLocationChecks(34);
                        session.Locations.CompleteLocationChecks(334);
                        break;
                    case "Adventure Mode: Classic | Level 35":
                        checkedArray[35] = true;
                        session.Locations.CompleteLocationChecks(35);
                        session.Locations.CompleteLocationChecks(335);
                        break;
                    case "Adventure Mode: Classic | Level 36":
                        checkedArray[36] = true;
                        session.Locations.CompleteLocationChecks(36);
                        session.Locations.CompleteLocationChecks(336);
                        CheckForGoalType1();
                        break;
                    case "Adventure Mode: Classic | Level 37":
                        checkedArray[37] = true;
                        session.Locations.CompleteLocationChecks(37);
                        session.Locations.CompleteLocationChecks(337);
                        break;
                    case "Adventure Mode: Classic | Level 38":
                        checkedArray[38] = true;
                        session.Locations.CompleteLocationChecks(38);
                        session.Locations.CompleteLocationChecks(338);
                        break;
                    case "Adventure Mode: Classic | Level 39":
                        checkedArray[39] = true;
                        session.Locations.CompleteLocationChecks(39);
                        session.Locations.CompleteLocationChecks(339);
                        break;
                    case "Adventure Mode: Classic | Level 40":
                        checkedArray[40] = true;
                        session.Locations.CompleteLocationChecks(40);
                        session.Locations.CompleteLocationChecks(340);
                        break;
                    case "Adventure Mode: Classic | Level 41":
                        checkedArray[41] = true;
                        session.Locations.CompleteLocationChecks(41);
                        session.Locations.CompleteLocationChecks(341);
                        break;
                    case "Adventure Mode: Classic | Level 42":
                        checkedArray[42] = true;
                        session.Locations.CompleteLocationChecks(42);
                        session.Locations.CompleteLocationChecks(342);
                        break;
                    case "Adventure Mode: Classic | Level 43":
                        checkedArray[43] = true;
                        session.Locations.CompleteLocationChecks(43);
                        session.Locations.CompleteLocationChecks(343);
                        break;
                    case "Adventure Mode: Classic | Level 44":
                        checkedArray[44] = true;
                        session.Locations.CompleteLocationChecks(44);
                        session.Locations.CompleteLocationChecks(344);
                        break;
                    case "Adventure Mode: Classic | Level 45":
                        checkedArray[45] = true;
                        session.Locations.CompleteLocationChecks(45);
                        session.Locations.CompleteLocationChecks(345);
                        CheckForGoalType1();
                        break;
                    case "Adventure Mode: Snow | Level 1":
                        checkedArray[46] = true;
                        session.Locations.CompleteLocationChecks(46);
                        session.Locations.CompleteLocationChecks(346);
                        break;
                    case "Adventure Mode: Snow | Level 2":
                        checkedArray[47] = true;
                        session.Locations.CompleteLocationChecks(47);
                        session.Locations.CompleteLocationChecks(347);
                        break;
                    case "Adventure Mode: Snow | Level 3":
                        checkedArray[48] = true;
                        session.Locations.CompleteLocationChecks(48);
                        session.Locations.CompleteLocationChecks(348);
                        break;
                    case "Adventure Mode: Snow | Level 4":
                        checkedArray[49] = true;
                        session.Locations.CompleteLocationChecks(49);
                        session.Locations.CompleteLocationChecks(349);
                        break;
                    case "Adventure Mode: Snow | Level 5":
                        checkedArray[50] = true;
                        session.Locations.CompleteLocationChecks(50);
                        session.Locations.CompleteLocationChecks(350);
                        break;
                    case "Adventure Mode: Snow | Level 6":
                        checkedArray[51] = true;
                        session.Locations.CompleteLocationChecks(51);
                        session.Locations.CompleteLocationChecks(351);
                        break;
                    case "Adventure Mode: Snow | Level 7":
                        checkedArray[52] = true;
                        session.Locations.CompleteLocationChecks(52);
                        session.Locations.CompleteLocationChecks(352);
                        break;
                    case "Adventure Mode: Snow | Level 8":
                        checkedArray[53] = true;
                        session.Locations.CompleteLocationChecks(53);
                        session.Locations.CompleteLocationChecks(353);
                        break;
                    case "Adventure Mode: Snow | Level 9":
                        checkedArray[54] = true;
                        session.Locations.CompleteLocationChecks(54);
                        session.Locations.CompleteLocationChecks(354);
                        CheckForGoalType1();
                        break;
                    case "Fusion Challenge: Explod-o-shooter":
                        checkedArray[55] = true;
                        session.Locations.CompleteLocationChecks(55);
                        session.Locations.CompleteLocationChecks(355);
                        break;
                    case "Fusion Challenge: Chompzilla":
                        checkedArray[56] = true;
                        session.Locations.CompleteLocationChecks(56);
                        session.Locations.CompleteLocationChecks(356);
                        break;
                    case "Fusion Challenge: Charm-shroom":
                        checkedArray[57] = true;
                        session.Locations.CompleteLocationChecks(57);
                        session.Locations.CompleteLocationChecks(357);
                        break;
                    case "Fusion Challenge: Doomspike-shroom":
                        checkedArray[58] = true;
                        session.Locations.CompleteLocationChecks(58);
                        session.Locations.CompleteLocationChecks(358);
                        break;
                    case "Fusion Challenge: Infernowood":
                        checkedArray[59] = true;
                        session.Locations.CompleteLocationChecks(59);
                        session.Locations.CompleteLocationChecks(359);
                        break;
                    case "Fusion Challenge: Krakerberus":
                        checkedArray[60] = true;
                        session.Locations.CompleteLocationChecks(60);
                        session.Locations.CompleteLocationChecks(360);
                        break;
                    case "Fusion Challenge: Stardrop":
                        checkedArray[61] = true;
                        session.Locations.CompleteLocationChecks(61);
                        session.Locations.CompleteLocationChecks(361);
                        break;
                    case "Fusion Challenge: Pump-karrier":
                        checkedArray[62] = true;
                        session.Locations.CompleteLocationChecks(62);
                        session.Locations.CompleteLocationChecks(362);
                        break;
                    case "Fusion Challenge: Salad-pult":
                        checkedArray[63] = true;
                        session.Locations.CompleteLocationChecks(63);
                        session.Locations.CompleteLocationChecks(363);
                        break;
                    case "Fusion Challenge: Alchemist Umbrella":
                        checkedArray[64] = true;
                        session.Locations.CompleteLocationChecks(64);
                        session.Locations.CompleteLocationChecks(364);
                        break;
                    case "Fusion Challenge: Spruce Supershooter":
                        checkedArray[65] = true;
                        session.Locations.CompleteLocationChecks(65);
                        session.Locations.CompleteLocationChecks(365);
                        break;
                    case "Titan Pea Turret":
                        checkedArray[66] = true;
                        session.Locations.CompleteLocationChecks(66);
                        session.Locations.CompleteLocationChecks(366);
                        break;
                    case "Explod-o-tato Mine":
                        checkedArray[67] = true;
                        session.Locations.CompleteLocationChecks(67);
                        session.Locations.CompleteLocationChecks(367);
                        break;
                    case "Pumpkin Bunker":
                        checkedArray[68] = true;
                        session.Locations.CompleteLocationChecks(68);
                        session.Locations.CompleteLocationChecks(368);
                        break;
                    case "Nugget-shroom":
                        checkedArray[69] = true;
                        session.Locations.CompleteLocationChecks(69);
                        session.Locations.CompleteLocationChecks(369);
                        break;
                    case "Spuddy-shroom":
                        checkedArray[70] = true;
                        session.Locations.CompleteLocationChecks(70);
                        session.Locations.CompleteLocationChecks(370);
                        break;
                    case "Foul-shroom":
                        checkedArray[71] = true;
                        session.Locations.CompleteLocationChecks(71);
                        session.Locations.CompleteLocationChecks(371);
                        break;
                    case "Mind-blover":
                        checkedArray[72] = true;
                        session.Locations.CompleteLocationChecks(72);
                        session.Locations.CompleteLocationChecks(372);
                        break;
                    case "Boomwood":
                        checkedArray[73] = true;
                        session.Locations.CompleteLocationChecks(73);
                        session.Locations.CompleteLocationChecks(373);
                        break;
                    case "Bamboom":
                        checkedArray[74] = true;
                        session.Locations.CompleteLocationChecks(74);
                        session.Locations.CompleteLocationChecks(374);
                        break;
                    case "Spike-nut":
                        checkedArray[75] = true;
                        session.Locations.CompleteLocationChecks(75);
                        session.Locations.CompleteLocationChecks(375);
                        break;
                    case "Leviathan-shroom":
                        checkedArray[76] = true;
                        session.Locations.CompleteLocationChecks(76);
                        session.Locations.CompleteLocationChecks(376);
                        break;




                    case "Dr. Zomboss' Revenge":
                        checkedArray[100] = true;
                        session.Locations.CompleteLocationChecks(100);
                        if (goalType == 0)
                        { session.Socket.SendPacket(new StatusUpdatePacket() { Status = ArchipelagoClientState.ClientGoal }); }
                        break;
                    default:
                        MelonLogger.Msg("Level not implemented");
                        break;
                }
            }
        }

        [RegisterTypeInIl2Cpp]
        public class OnClickHandler2D : MonoBehaviour
        {
            public OnClickHandler2D(System.IntPtr ptr) : base(ptr) { }

            void OnMouseDown()
            {
                MelonLogger.Msg($"Clicked: {gameObject.name}");



            }
        }

        public void OnPacketReceived(ArchipelagoPacketBase packet) //what the fuck is static and i might have to add it back when i discover some bs bug
        {
            switch (packet)
            {
                case BouncePacket:
                    //MelonLogger.Msg("received bounced packet");
                    JObject jObject = packet.ToJObject();


                    foreach (var tag in jObject["tags"])
                    {
                        if ((string)tag == "RingLink")

                        {


                            foreach (var slotNum in jObject["slots"])
                            { //MelonLogger.Msg($"slotnum: {(int)slotNum}");
                                if ((int)slotNum == session.Players.ActivePlayer.Slot)
                                { goto escape2loops; }//fuck you im using goto
                                    }

                                var board = GameObject.Find("Board");
                                if (board == null)
                                {
                                    continue;
                                }
                                var boardComp = board.GetComponent(Il2CppType.Of<Board>());
                                
                                var type = boardComp.GetIl2CppType();
                                var field = type.GetField("theSun");
                                Il2CppSystem.Object rawValue = field.GetValue(boardComp);
                                int intValue = Il2CppSystem.Convert.ToInt32(rawValue);

                                

                                var data = jObject["data"];
                                int amount = data.Value<int>("amount");

                                prevSun = (intValue/10) + (amount);
                                field.SetValue(boardComp, intValue + amount*10);





                            }

                    }
                escape2loops:
                    break;

                default:
                    break;
            }
        }

        private void CheckForNewItems()
        {





            while (session.Items.Any())
            {
                var networkItem = session.Items.DequeueItem();

                //if (networkItem.ItemId == null)
                //{ continue; }
                if (networkItem.ItemId < maxItems)
                { unlockedArray[networkItem.ItemId] = true; }

                if (networkItem.ItemId == 200)
                {
                    var board = GameObject.Find("Board");
                    if (board == null)
                    {
                        continue;
                    }
                    var boardComp = board.GetComponent(Il2CppType.Of<Board>());

                    var type = boardComp.GetIl2CppType();
                    var field = type.GetField("theSun");
                    Il2CppSystem.Object rawValue = field.GetValue(boardComp);
                    int intValue = Il2CppSystem.Convert.ToInt32(rawValue);

                    field.SetValue(boardComp, intValue + 250);

                }

                if (networkItem.ItemId == 201)
                {
                    seedSlots += 1;
                }
            }
        }


        private void HideTargetObjectChildren(string canvasname, string target)
        {

            var canvas = GameObject.Find(canvasname);
            if (canvas == null)
            {
                //LoggerInstance.Warning("CanvasUp not found");
                return;
            }

            var inGameUI = canvas.transform.Find(target);
            if (inGameUI == null)
            {
                return;
            }

            int childCount = inGameUI.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = inGameUI.GetChild(i);
                child.gameObject.SetActive(false);
            }//AIMah

        }

        private void HideTargetObject(string canvasname,string target)
        {

            var canvas = GameObject.Find(canvasname);
            if (canvas == null)
            {
                //LoggerInstance.Warning($"{canvasname} not found");
                return;
            }

            var inGameUI = canvas.transform.Find(target);
            if (inGameUI == null)
            {
                return;
            }

                inGameUI.gameObject.SetActive(false);


        }

        public void CheckForGoalType1()
        {
            if (goalType != 1)
            {
                return;
            }

            if (checkedArray[9] && checkedArray[18] && checkedArray[27] && checkedArray[36] && checkedArray[45] && checkedArray[54])
            {
                session.Socket.SendPacket(new StatusUpdatePacket() { Status = ArchipelagoClientState.ClientGoal });
            }
            return;
        }
        public void Connect(string server, string user, string pass, ArchipelagoSession session)
        {
            LoginResult result;

            try
            {
                // handle TryConnectAndLogin attempt here and save the returned object to `result`
                result = session.TryConnectAndLogin(server, user, ItemsHandlingFlags.AllItems);
            }
            catch (System.Exception e)
            {
                result = new LoginFailure(e.GetBaseException().Message);
                
            }

            if (!result.Successful)
            {
                LoginFailure failure = (LoginFailure)result;
                string errorMessage = $"Failed to Connect to {server} as {user}:";
                foreach (string error in failure.Errors)
                {
                    errorMessage += $"\n    {error}";
                }
                foreach (ConnectionRefusedError error in failure.ErrorCodes)
                {
                    errorMessage += $"\n    {error}";
                }
                MelonLogger.Msg(errorMessage);
                
                return; // Did not connect, show the user the contents of `errorMessage`
            }


            //Dictionary<string, object> slotData = new Dictionary<string, object>();

            // Successfully connected, `ArchipelagoSession` (assume statically defined as `session` from now on) can now be
            // used to interact with the server and the returned `LoginSuccessful` contains some useful information about the
            // initial connection (e.g. a copy of the slot data as `loginSuccess.SlotData`)
            MelonLogger.Msg("Successfully connected to archipelago");
            var loginSuccess = (LoginSuccessful)result;

            //ringLinkMode = session.DataStorage.GetSlotData();
            
            var slotData = session.DataStorage.GetSlotData();

            //slotData.TryGetValue("RingLink",out object value);//use this instead when you can tolerate json objects

            foreach (KeyValuePair<string, object> arg in slotData)
            { if (arg.Key == "RingLink" && arg.Value.ToString() != "0") 
                {
                    session.ConnectionInfo.UpdateConnectionOptions(new[] { "RingLink" });
                    ringLinkMode = 1;//add more later
                }

                if (arg.Key == "CompletionType" && arg.Value.ToString() != "0")
                {
                    goalType = 1;//add more later
                }



            }
            session.Socket.PacketReceived += OnPacketReceived;



            ILocationCheckHelper locationHelper = session.Locations;

            var checkedLocations = locationHelper.AllLocationsChecked;

            var count = checkedLocations.Count;
            for (int i = 0; i < count; i++)
            {
                MelonLogger.Msg(checkedLocations[i].ToString());
                checkedArray[checkedLocations[i]] = true;


            }




        }
    }

    public class APData
    {
        public string serverAddress { get; set; }
        public int serverPort { get; set; }
        public string slotName { get; set; }
    }


}
