







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
        public const int maxItems = 180; // holy shit good programming practice?\
        public const int maxLocations = 500;
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
        const string minigamePage1 = "ChallengeMenu(Clone)/Levels/PageMiniGames/Pages/Page1/";
        const string minigamePage2 = "ChallengeMenu(Clone)/Levels/PageMiniGames/Pages/Page2/";
        const string minigamePage3  = "ChallengeMenu(Clone)/Levels/PageMiniGames/Pages/Page3/";


        const string showcasePage = "ExploreMenu(Clone)/Level/Line1/";
        const string challengePage = "ChallengeMenu(Clone)/Levels/PageUnlockChallenge/Page1/";
        const string slotPath = "InGameUI(Clone)/SeedBank/SeedGroup/";
        public int ringLinkMode = 0;
        public bool adventureExtraEnabled = false;
        public static int prevSun;
        public static bool sunExists = false;
        public static int goalType = 0;
        public static GameObject boardGl;
        public static GameObject canvasGl;
        public static GameObject canvasupGl;
        public static Transform levelUIGl;

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
                Connect("Plants Vs Zombies Fusion", data.slotName, data.password, session);
            }
            else
            {
                MelonLogger.Warning("config.json not found, place it in the same folder as PVZFusionArchipelago.dll");
            }

        }



        public override void OnUpdate()
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
                    //if (boardGl == null)
                    //{ }


                    if (canvasGl == null)
                    { canvasGl = GameObject.Find("Canvas");}
                    
                    if (canvasGl != null)
                    {
                        if (levelUIGl == null)
                        { levelUIGl = canvasGl.transform.Find("InGameUI(Clone)"); }


                        if (boardGl == null) { 
                        boardGl = GameObject.Find("Board");
                        }



                        if ((boardGl != null) && (levelUIGl != null))
                        {

                            var boardComp = boardGl.GetComponent(Il2CppType.Of<Board>());
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

            if (canvasupGl == null)
            { canvasupGl = GameObject.Find("CanvasUp"); }



            if (canvasupGl != null)
            {
                //MelonLogger.Msg("Found CanvasUp");
                if (unlockedArray[1] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "PeaShooter"); }
                if (unlockedArray[2] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "SunFlower"); }
                if (unlockedArray[3] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "CherryBomb"); }
                if (unlockedArray[4] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "WallNut"); }
                if (unlockedArray[5] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "PotatoMine"); }
                if (unlockedArray[6] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "Chomper"); }
                if (unlockedArray[7] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "Present"); }
                if (unlockedArray[8] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "TallNut"); }
                if (unlockedArray[9] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "EndoFlame"); }
                if (unlockedArray[10] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "SmallPuff"); }
                if (unlockedArray[11] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "FumeShroom"); }
                if (unlockedArray[12] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "HypnoShroom"); }
                if (unlockedArray[13] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "ScaredyShroom"); }
                if (unlockedArray[14] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "IceShroom"); }
                if (unlockedArray[15] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "DoomShroom"); }
                if (unlockedArray[16] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "PresentZombie"); }
                if (unlockedArray[17] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "GloomShroom"); }
                if (unlockedArray[18] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "GraveBust"); }
                if (unlockedArray[19] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "LilyPad"); }
                if (unlockedArray[20] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "Squash"); }
                if (unlockedArray[21] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "ThreePeater"); }
                if (unlockedArray[22] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "Tanglekelp"); }
                if (unlockedArray[23] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "Jalapeno"); }
                if (unlockedArray[24] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "Caltrop"); }
                if (unlockedArray[25] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "TorchWood"); }
                if (unlockedArray[26] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "Spike"); }
                if (unlockedArray[27] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "Wheat"); }
                if (unlockedArray[28] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "SeaShroom"); }
                if (unlockedArray[29] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "Plantern"); }
                if (unlockedArray[30] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "Cactus"); }
                if (unlockedArray[31] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "Blover"); }
                if (unlockedArray[32] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "StarFruit"); }
                if (unlockedArray[33] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "Pumpkin"); }
                if (unlockedArray[34] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "Magnetshroom"); }
                if (unlockedArray[35] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "Cattail"); }
                if (unlockedArray[36] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "Imitater"); }
                if (unlockedArray[37] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "Cabbage"); }
                if (unlockedArray[38] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "Pot"); }
                if (unlockedArray[39] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "Cornpult"); }
                if (unlockedArray[40] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "Garlic"); }
                if (unlockedArray[41] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "Umbrellaleaf"); }
                if (unlockedArray[42] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "Marigold"); }
                if (unlockedArray[43] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "Melonpult"); }
                if (unlockedArray[44] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "CobCannon"); }
                if (unlockedArray[45] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "MixBomb"); }
                if (unlockedArray[46] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "PineFurnace"); }
                if (unlockedArray[47] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "SpruceShooter"); }
                if (unlockedArray[48] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "ShulkFlower"); }
                if (unlockedArray[49] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "IceLotus"); }
                if (unlockedArray[50] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "WaterAloes"); }
                if (unlockedArray[51] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "Bamboo"); }
                if (unlockedArray[52] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "SnowPresent"); }
                if (unlockedArray[53] == false)
                { HideTargetObjectChildren(canvasupGl, page1 + "SpruceBallista"); }

                if (unlockedArray[55] == false)
                { HideTargetObjectChildren(canvasupGl, page2 + "CattailGirl"); }
                if (unlockedArray[56] == false)
                { HideTargetObjectChildren(canvasupGl, page2 + "SwordStar"); }
                if (unlockedArray[57] == false)
                { HideTargetObjectChildren(canvasupGl, page2 + "Squalour"); }
                if (unlockedArray[58] == false)
                { HideTargetObjectChildren(canvasupGl, page2 + "Hamburger"); }
                if (unlockedArray[59] == false)
                { HideTargetObjectChildren(canvasupGl, page2 + "EndoFlameGirl"); }
                if (unlockedArray[60] == false)
                { HideTargetObjectChildren(canvasupGl, page2 + "IceBean"); }
                if (unlockedArray[61] == false)
                { HideTargetObjectChildren(canvasupGl, page2 + "Prismflower"); }
                if (unlockedArray[62] == false)
                { HideTargetObjectChildren(canvasupGl, page2 + "SniperPea"); }
                if (unlockedArray[63] == false)
                { HideTargetObjectChildren(canvasupGl, page2 + "Chrysantheautumn"); }
                if (unlockedArray[64] == false)
                { HideTargetObjectChildren(canvasupGl, page2 + "IcePeach"); }
                if (unlockedArray[65] == false)
                { HideTargetObjectChildren(canvasupGl, page2 + "FrozenPear"); }
                if (unlockedArray[66] == false)
                { HideTargetObjectChildren(canvasupGl, page2 + "PassionFruit"); }


            }


            if (unlockedArray[70] == false)//shovel
            { HideTargetObject("Canvas", "InGameUI(Clone)/ShovelBank");}

            if (unlockedArray[71] == false)//fertilizer
            {

                if (boardGl == null)
                { boardGl = GameObject.Find("Board"); }
                if (boardGl != null)
                {
                    var fertilizerTransform = boardGl.transform.Find("Ferilize(Clone)");

                    if (fertilizerTransform != null)
                    { GameObject.Destroy(fertilizerTransform.gameObject); }
                }
            }

            if (canvasGl == null)
            { canvasGl = GameObject.Find("Canvas"); }
            if (canvasGl != null)
            {
                if (unlockedArray[72] == false)//gloves
                {
                    HideTargetObject("Canvas", "InGameUI(Clone)/GloveBank");
                    HideTargetObjectChildren(canvasGl, "GardenUI(Clone)/Tools/GloveBank");
                }
                if (unlockedArray[74] == false)//mallet
                { HideTargetObject("Canvas", "InGameUI(Clone)/HammerBank"); }


                if (unlockedArray[76] == false)//watering can
                { HideTargetObjectChildren(canvasGl, "GardenUI(Clone)/Tools/WaterBank"); }

                if (unlockedArray[77] == false)//gramophone
                { HideTargetObjectChildren(canvasGl, "GardenUI(Clone)/Tools/PhonographBank"); }

                if (unlockedArray[78] == false)//bug spray
                { HideTargetObjectChildren(canvasGl, "GardenUI(Clone)/Tools/BugSprayBank"); }

                if (unlockedArray[79] == false)//wheelbarrow
                { HideTargetObjectChildren(canvasGl, "GardenUI(Clone)/Tools/WheelBarrowBank"); }


            }





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
            if (unlockedArray[110] == false)
            { HideTargetObject("Canvas", minigamePage1 + "Lv25"); }
            else { if (!checkedArray[80]) { HideTargetObject("Canvas", minigamePage1 + "Lv25/Window/Trophy"); } }
            if (unlockedArray[111] == false)
            { HideTargetObject("Canvas", minigamePage1 + "Lv28"); }
            else { if (!checkedArray[81]) { HideTargetObject("Canvas", minigamePage1 + "Lv28/Window/Trophy"); } }
            if (unlockedArray[112] == false)
            { HideTargetObject("Canvas", minigamePage1 + "Lv29"); }
            else { if (!checkedArray[82]) { HideTargetObject("Canvas", minigamePage1 + "Lv29/Window/Trophy"); } }
            if (unlockedArray[113] == false)
            { HideTargetObject("Canvas", minigamePage1 + "Lv30"); }
            else { if (!checkedArray[83]) { HideTargetObject("Canvas", minigamePage1 + "Lv30/Window/Trophy"); } }
            if (unlockedArray[114] == false)
            { HideTargetObject("Canvas", minigamePage1 + "Lv34"); }
            else { if (!checkedArray[84]) { HideTargetObject("Canvas", minigamePage1 + "Lv34/Window/Trophy"); } }
            if (unlockedArray[115] == false)
            { HideTargetObject("Canvas", minigamePage1 + "Lv37"); }
            else { if (!checkedArray[85]) { HideTargetObject("Canvas", minigamePage1 + "Lv37/Window/Trophy"); } }
            if (unlockedArray[116] == false)
            { HideTargetObject("Canvas", minigamePage1 + "Lv38"); }
            else { if (!checkedArray[86]) { HideTargetObject("Canvas", minigamePage1 + "Lv38/Window/Trophy"); } }
            if (unlockedArray[117] == false)
            { HideTargetObject("Canvas", minigamePage1 + "Lv66"); }
            else { if (!checkedArray[87]) { HideTargetObject("Canvas", minigamePage1 + "Lv66/Window/Trophy"); } }
            if (unlockedArray[118] == false)
            { HideTargetObject("Canvas", minigamePage1 + "Lv42"); }
            else { if (!checkedArray[88]) { HideTargetObject("Canvas", minigamePage1 + "Lv42/Window/Trophy"); } }
            if (unlockedArray[119] == false)
            { HideTargetObject("Canvas", minigamePage1 + "Lv43"); }
            else { if (!checkedArray[89]) { HideTargetObject("Canvas", minigamePage1 + "Lv43/Window/Trophy"); } }
            if (unlockedArray[120] == false)
            { HideTargetObject("Canvas", minigamePage1 + "Lv44"); }
            else { if (!checkedArray[90]) { HideTargetObject("Canvas", minigamePage1 + "Lv44/Window/Trophy"); } }
            if (unlockedArray[121] == false)
            { HideTargetObject("Canvas", minigamePage1 + "Lv47"); }
            else { if (!checkedArray[91]) { HideTargetObject("Canvas", minigamePage1 + "Lv47/Window/Trophy"); } }
            if (unlockedArray[122] == false)
            { HideTargetObject("Canvas", minigamePage1 + "Lv49"); }
            else { if (!checkedArray[92]) { HideTargetObject("Canvas", minigamePage1 + "Lv49/Window/Trophy"); } }
            if (unlockedArray[123] == false)
            { HideTargetObject("Canvas", minigamePage1 + "Lv52"); }
            if (unlockedArray[124] == false)
            { HideTargetObject("Canvas", minigamePage1 + "Lv53"); }
            if (unlockedArray[125] == false)
            { HideTargetObject("Canvas", minigamePage1 + "Lv58"); }
            else { if (!checkedArray[93]) { HideTargetObject("Canvas", minigamePage1 + "Lv58/Window/Trophy"); } }
            if (unlockedArray[126] == false)
            { HideTargetObject("Canvas", minigamePage1 + "Lv59"); }
            else { if (!checkedArray[94]) { HideTargetObject("Canvas", minigamePage1 + "Lv59/Window/Trophy"); } }
            if (unlockedArray[127] == false)
            { HideTargetObject("Canvas", minigamePage1 + "Lv62"); }
            else { if (!checkedArray[95]) { HideTargetObject("Canvas", minigamePage1 + "Lv62/Window/Trophy"); } }
            if (unlockedArray[128] == false)
            { HideTargetObject("Canvas", minigamePage2 + "Lv63"); }
            else { if (!checkedArray[96]) { HideTargetObject("Canvas", minigamePage2 + "Lv63/Window/Trophy"); } }
            if (unlockedArray[129] == false)
            { HideTargetObject("Canvas", minigamePage2 + "Lv65"); }
            else { if (!checkedArray[96]) { HideTargetObject("Canvas", minigamePage2 + "Lv65/Window/Trophy"); } }
            if (unlockedArray[130] == false)
            { HideTargetObject("Canvas", minigamePage2 + "Lv67"); }
            else { if (!checkedArray[98]) { HideTargetObject("Canvas", minigamePage2 + "Lv67/Window/Trophy"); } }
            if (unlockedArray[131] == false)
            { HideTargetObject("Canvas", minigamePage2 + "Lv77"); }
            else { if (!checkedArray[99]) { HideTargetObject("Canvas", minigamePage2 + "Lv77/Window/Trophy"); } }
            if (unlockedArray[132] == false)
            { HideTargetObject("Canvas", minigamePage2 + "Lv78"); }
            else { if (!checkedArray[100]) { HideTargetObject("Canvas", minigamePage2 + "Lv78/Window/Trophy"); } }
            if (unlockedArray[133] == false)
            { HideTargetObject("Canvas", minigamePage2 + "Lv82"); }
            else { if (!checkedArray[101]) { HideTargetObject("Canvas", minigamePage2 + "Lv82/Window/Trophy"); } }
            if (unlockedArray[134] == false)
            { HideTargetObject("Canvas", minigamePage2 + "Lv99"); }
            else { if (!checkedArray[102]) { HideTargetObject("Canvas", minigamePage2 + "Lv99/Window/Trophy"); } }
            if (unlockedArray[135] == false)
            { HideTargetObject("Canvas", minigamePage2 + "Lv100"); }
            else { if (!checkedArray[103]) { HideTargetObject("Canvas", minigamePage2 + "Lv100/Window/Trophy"); } }
            if (unlockedArray[136] == false)
            { HideTargetObject("Canvas", minigamePage2 + "Lv107"); }
            else { if (!checkedArray[104]) { HideTargetObject("Canvas", minigamePage2 + "Lv107/Window/Trophy"); } }
            if (unlockedArray[137] == false)
            { HideTargetObject("Canvas", minigamePage2 + "Lv108"); }
            else { if (!checkedArray[105]) { HideTargetObject("Canvas", minigamePage2 + "Lv108/Window/Trophy"); } }
            if (unlockedArray[138] == false)
            { HideTargetObject("Canvas", minigamePage2 + "Lv117"); }
            else { if (!checkedArray[106]) { HideTargetObject("Canvas", minigamePage2 + "Lv117/Window/Trophy"); } }
            if (unlockedArray[139] == false)
            { HideTargetObject("Canvas", minigamePage2 + "Lv118"); }
            else { if (!checkedArray[107]) { HideTargetObject("Canvas", minigamePage2 + "Lv118/Window/Trophy"); } }
            if (unlockedArray[140] == false)
            { HideTargetObject("Canvas", minigamePage2 + "Lv123"); }
            else { if (!checkedArray[108]) { HideTargetObject("Canvas", minigamePage2 + "Lv123/Window/Trophy"); } }
            if (unlockedArray[141] == false)
            { HideTargetObject("Canvas", minigamePage2 + "Lv125"); }
            else { if (!checkedArray[109]) { HideTargetObject("Canvas", minigamePage2 + "Lv125/Window/Trophy"); } }
            if (unlockedArray[142] == false)
            { HideTargetObject("Canvas", minigamePage2 + "Lv126"); }
            else { if (!checkedArray[110]) { HideTargetObject("Canvas", minigamePage2 + "Lv126/Window/Trophy"); } }
            if (unlockedArray[143] == false)
            { HideTargetObject("Canvas", minigamePage2 + "Lv127"); }
            else { if (!checkedArray[111]) { HideTargetObject("Canvas", minigamePage2 + "Lv127/Window/Trophy"); } }
            if (unlockedArray[144] == false)
            { HideTargetObject("Canvas", minigamePage2 + "Lv131"); }
            else { if (!checkedArray[112]) { HideTargetObject("Canvas", minigamePage2 + "Lv131/Window/Trophy"); } }
            if (unlockedArray[145] == false)
            { HideTargetObject("Canvas", minigamePage2 + "Lv132"); }
            else { if (!checkedArray[113]) { HideTargetObject("Canvas", minigamePage2 + "Lv132/Window/Trophy"); } }
            if (unlockedArray[146] == false)
            { HideTargetObject("Canvas", minigamePage3 + "Lv133"); }
            else { if (!checkedArray[114]) { HideTargetObject("Canvas", minigamePage3 + "Lv133/Window/Trophy"); } }
            if (unlockedArray[147] == false)
            { HideTargetObject("Canvas", minigamePage3 + "Lv135"); }
            else { if (!checkedArray[115]) { HideTargetObject("Canvas", minigamePage3 + "Lv135/Window/Trophy"); } }
            if (unlockedArray[148] == false)
            { HideTargetObject("Canvas", minigamePage3 + "Lv137"); }
            else { if (!checkedArray[116]) { HideTargetObject("Canvas", minigamePage3 + "Lv137/Window/Trophy"); } }
            if (unlockedArray[149] == false)
            { HideTargetObject("Canvas", minigamePage3 + "Lv139"); }
            else { if (!checkedArray[117]) { HideTargetObject("Canvas", minigamePage3 + "Lv139/Window/Trophy"); } }
            if (unlockedArray[150] == false)
            { HideTargetObject("Canvas", minigamePage3 + "Lv141"); }
            else { if (!checkedArray[118]) { HideTargetObject("Canvas", minigamePage3 + "Lv141/Window/Trophy"); } }
            if (unlockedArray[151] == false)
            { HideTargetObject("Canvas", minigamePage3 + "Lv142"); }
            else { if (!checkedArray[119]) { HideTargetObject("Canvas", minigamePage3 + "Lv142/Window/Trophy"); } }
            if (unlockedArray[152] == false)
            { HideTargetObject("Canvas", minigamePage3 + "Lv143"); }
            else { if (!checkedArray[120]) { HideTargetObject("Canvas", minigamePage3 + "Lv143/Window/Trophy"); } }
            if (unlockedArray[153] == false)
            { HideTargetObject("Canvas", minigamePage3 + "Lv146"); }
            else { if (!checkedArray[121]) { HideTargetObject("Canvas", minigamePage3 + "Lv146/Window/Trophy"); } }
            if (unlockedArray[154] == false)
            { HideTargetObject("Canvas", minigamePage3 + "Lv147"); }
            else { if (!checkedArray[122]) { HideTargetObject("Canvas", minigamePage3 + "Lv147/Window/Trophy"); } }
            if (unlockedArray[155] == false)
            { HideTargetObject("Canvas", minigamePage3 + "Lv148"); }
            else { if (!checkedArray[123]) { HideTargetObject("Canvas", minigamePage3 + "Lv143/Window/Trophy"); } }
            if (unlockedArray[156] == false)
            { HideTargetObject("Canvas", minigamePage3 + "Lv150"); }
            else { if (!checkedArray[124]) { HideTargetObject("Canvas", minigamePage3 + "Lv150/Window/Trophy"); } }
            if (unlockedArray[157] == false)
            { HideTargetObject("Canvas", minigamePage3 + "Lv156"); }
            else { if (!checkedArray[125]) { HideTargetObject("Canvas", minigamePage3 + "Lv156/Window/Trophy"); } }
            if (unlockedArray[158] == false)
            { HideTargetObject("Canvas", minigamePage3 + "Lv157"); }
            else { if (!checkedArray[126]) { HideTargetObject("Canvas", minigamePage3 + "Lv157/Window/Trophy"); } }
            if (unlockedArray[159] == false)
            { HideTargetObject("Canvas", minigamePage3 + "Lv158"); }
            else { if (!checkedArray[127]) { HideTargetObject("Canvas", minigamePage3 + "Lv158/Window/Trophy"); } }
            if (unlockedArray[160] == false)
            { HideTargetObject("Canvas", minigamePage3 + "Lv160"); }
            else { if (!checkedArray[128]) { HideTargetObject("Canvas", minigamePage3 + "Lv160/Window/Trophy"); } }
            if (unlockedArray[161] == false)
            { HideTargetObject("Canvas", minigamePage3 + "Lv161"); }
            else { if (!checkedArray[129]) { HideTargetObject("Canvas", minigamePage3 + "Lv161/Window/Trophy"); } }
            if (unlockedArray[162] == false)
            { HideTargetObject("Canvas", minigamePage3 + "Lv163"); }
            else { if (!checkedArray[130]) { HideTargetObject("Canvas", minigamePage3 + "Lv163/Window/Trophy"); } }

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



            if (canvasGl == null)
            { canvasGl = GameObject.Find("Canvas"); }
            if (canvasGl == null)
            {
                //MelonLogger.Warning("Background not found");
                return;
            }
            var targetTransform = canvasGl.transform.Find("InGameUI(Clone)/ConeryorBelt/Content");
            if (targetTransform == null)
            {
                return;
            }

            if (ringLinkMode!=0)
            {
                var board = GameObject.Find("Board");
                if (board != null)
                {
                    var conveyorTransform = canvasGl.transform.Find("InGameUI(Clone)/ConeryorBelt");


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


                switch (plantType)
                {
                    case PlantType.Peashooter:
                        if (!unlockedArray[1]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.CherryBomb:
                        if (!unlockedArray[3]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.WallNut:
                        if (!unlockedArray[4]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.PotatoMine:
                        if (!unlockedArray[5]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.Chomper:
                        if (!unlockedArray[6]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.HypnoShroom:
                        if (!unlockedArray[12]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.IceShroom:
                        if (!unlockedArray[14]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.DoomShroom:
                        if (!unlockedArray[15]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.LilyPad:
                        if (!unlockedArray[19]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.Squash:
                        if (!unlockedArray[20]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.ThreePeater:
                        if (!unlockedArray[21]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.Tanglekelp:
                        if (!unlockedArray[22]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.Jalapeno:
                        if (!unlockedArray[23]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.Caltrop:
                        if (!unlockedArray[24]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.TorchWood:
                        if (!unlockedArray[25]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.SeaShroom:
                        if (!unlockedArray[28]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.Plantern:
                        if (!unlockedArray[29]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.Cactus:
                        if (!unlockedArray[30]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.Blover:
                        if (!unlockedArray[31]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.StarFruit:
                        if (!unlockedArray[32]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.Pumpkin:
                        if (!unlockedArray[33]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.Magnetshroom:
                        if (!unlockedArray[34]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.Cabbagepult:
                        if (!unlockedArray[37]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.Pot:
                        if (!unlockedArray[38]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.Cornpult:
                        if (!unlockedArray[39]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.Umbrellaleaf:
                        if (!unlockedArray[41]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.Marigold:
                        if (!unlockedArray[42]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.Melonpult:
                        if (!unlockedArray[43]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.PineFurnace:
                        if (!unlockedArray[46]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.SpruceShooter:
                        if (!unlockedArray[47]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.Shulkflower:
                        if (!unlockedArray[48]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.IceLotus:
                        if (!unlockedArray[49]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.WaterAloes:
                        if (!unlockedArray[50]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.Bamboo:
                        if (!unlockedArray[51]) { GameObject.Destroy(child.gameObject); }
                        break;


                    case PlantType.PortalNut:
                        if (!unlockedArray[4]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.FireSquash:
                        if (!unlockedArray[20]) { GameObject.Destroy(child.gameObject); }
                        break;

                    case PlantType.PeaChomper:
                        if (!unlockedArray[1] || !unlockedArray[6]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.GatlingPuff:
                        if (!unlockedArray[1] || !unlockedArray[10]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.SnowGatling:
                        if (!unlockedArray[1] || !unlockedArray[14]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.SunChomper:
                        if (!unlockedArray[2] || !unlockedArray[6]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.SunMagnet:
                        if (!unlockedArray[2] || !unlockedArray[34]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.CherryNut:
                        if (!unlockedArray[3] || !unlockedArray[4]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.CherryChomper:
                        if (!unlockedArray[3] || !unlockedArray[6]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.CherryJalapeno:
                        if (!unlockedArray[3] || !unlockedArray[23]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.NutChomper:
                        if (!unlockedArray[4] || !unlockedArray[6]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.HypnoNut:
                        if (!unlockedArray[4] || !unlockedArray[12]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.IceNut:
                        if (!unlockedArray[4] || !unlockedArray[14]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.DoomNut:
                        if (!unlockedArray[4] || !unlockedArray[15]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.SquashNut:
                        if (!unlockedArray[4] || !unlockedArray[20]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.FireNut:
                        if (!unlockedArray[4] || !unlockedArray[23]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.CaltropNut:
                        if (!unlockedArray[4] || !unlockedArray[24]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.PotatoChomper:
                        if (!unlockedArray[5] || !unlockedArray[6]) { GameObject.Destroy(child.gameObject); }
                        break;


                    case PlantType.HypnoSquash:
                        if (!unlockedArray[12] || !unlockedArray[20]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.IceDoom:
                        if (!unlockedArray[14] || !unlockedArray[15]) { GameObject.Destroy(child.gameObject); }
                        break;

                    case PlantType.IceSquash:
                        if (!unlockedArray[14] || !unlockedArray[20]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.ObsidianJalapeno:
                        if (!unlockedArray[14] || !unlockedArray[23]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.IceBlover:
                        if (!unlockedArray[14] || !unlockedArray[31]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.DoomBlover:
                        if (!unlockedArray[15] || !unlockedArray[31]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.DoomStar:
                        if (!unlockedArray[15] || !unlockedArray[32]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.JackboxDoom:
                        if (!unlockedArray[15] || !unlockedArray[34]) { GameObject.Destroy(child.gameObject); }
                        break;

                    case PlantType.JalaSquash:
                        if (!unlockedArray[20] || !unlockedArray[23]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.SquashSpike:
                        if (!unlockedArray[20] || !unlockedArray[24]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.SquashTorch:
                        if (!unlockedArray[20] || !unlockedArray[25]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.DarkThreePeater:
                        if (!unlockedArray[21] || !unlockedArray[23]) { GameObject.Destroy(child.gameObject); }
                        break;

                    case PlantType.JalaStar:
                        if (!unlockedArray[23] || !unlockedArray[32]) { GameObject.Destroy(child.gameObject); }
                        break;

                    case PlantType.IronPumpkin:
                        if (!unlockedArray[33] || !unlockedArray[34]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.GoldMagnet:
                        if (!unlockedArray[34] || !unlockedArray[42]) { GameObject.Destroy(child.gameObject); }
                        break;

                    case PlantType.GoldCabbage:
                        if (!unlockedArray[37] || !unlockedArray[42]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.GoldCorn:
                        if (!unlockedArray[39] || !unlockedArray[42]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.GoldMelon:
                        if (!unlockedArray[42] || !unlockedArray[43]) { GameObject.Destroy(child.gameObject); }
                        break;



                    case PlantType.SuperChomper:
                        if (!unlockedArray[1] || !unlockedArray[4] || !unlockedArray[6]) { GameObject.Destroy(child.gameObject); }
                        break;
                }



            }//AIMah


        }





        private void AttachClickHandler(string path)
        {


            if (boardGl == null)
            { boardGl = GameObject.Find("Board"); }
            if (boardGl == null)
            {
                var zumaBG = GameObject.Find("Background(Clone)");
                if (zumaBG != null)
                {var  stupidZumaTrophy = zumaBG.transform.Find("TrophyZuma(Clone)");
                    if (stupidZumaTrophy != null)
                    {
                        checkedArray[96] = true;
                        session.Locations.CompleteLocationChecks(96);
                        session.Locations.CompleteLocationChecks(396);
                    } }//this stupid ass zuma trophy
                //MelonLogger.Warning("Background not found");
                return;
            }

            var targetTransform = boardGl.transform.Find("TrophyPrefab");
            if (targetTransform == null)
            {

                targetTransform = boardGl.transform.Find("TrophyZuma(Clone)");
                if (targetTransform == null)
                { return; }
                else
                {

                    return;
                }
                    
                }

                GameObject obj = targetTransform.gameObject;

            if (obj.GetComponent<OnClickHandler2D>() == null)
            { 
                obj.AddComponent<OnClickHandler2D>();
                MelonLogger.Msg($"Attached OnClickHandler2D to '{obj.name}'");
                var canvas = GameObject.Find("CanvasUp");
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
                    //case "Zum-nut!":
                    //checkedArray[96] = true;
                    //session.Locations.CompleteLocationChecks(96);
                    //session.Locations.CompleteLocationChecks(396);
                    //break;
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
                    case "Fusion Challenge: Chomper":
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
                    case "Fusion Challenge: Hydra Kelp":
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
                    case "Chomper Maw":
                        checkedArray[71] = true;
                        session.Locations.CompleteLocationChecks(71);
                        session.Locations.CompleteLocationChecks(371);
                        break;
                    case "Foul-shroom":
                        checkedArray[72] = true;
                        session.Locations.CompleteLocationChecks(72);
                        session.Locations.CompleteLocationChecks(372);
                        break;
                    case "Mind-blover":
                        checkedArray[73] = true;
                        session.Locations.CompleteLocationChecks(73);
                        session.Locations.CompleteLocationChecks(373);
                        break;
                    case "Boomwood":
                        checkedArray[74] = true;
                        session.Locations.CompleteLocationChecks(74);
                        session.Locations.CompleteLocationChecks(374);
                        break;
                    case "Bamboom":
                        checkedArray[75] = true;
                        session.Locations.CompleteLocationChecks(75);
                        session.Locations.CompleteLocationChecks(375);
                        break;
                    case "Spike-nut":
                        checkedArray[76] = true;
                        session.Locations.CompleteLocationChecks(76);
                        session.Locations.CompleteLocationChecks(376);
                        break;
                    case "Kraken-shroom":
                        checkedArray[77] = true;
                        session.Locations.CompleteLocationChecks(77);
                        session.Locations.CompleteLocationChecks(377);
                        break;
                    case "Scaredy's Dream":
                        checkedArray[80] = true;
                        session.Locations.CompleteLocationChecks(80);
                        session.Locations.CompleteLocationChecks(380);
                        break;
                    case "Pole Vaulting Disco":
                        checkedArray[81] = true;
                        session.Locations.CompleteLocationChecks(81);
                        session.Locations.CompleteLocationChecks(381);
                        break;
                    case "Compact Planting":
                        checkedArray[82] = true;
                        session.Locations.CompleteLocationChecks(82);
                        session.Locations.CompleteLocationChecks(382);
                        break;
                    case "Newspaper War":
                        checkedArray[83] = true;
                        session.Locations.CompleteLocationChecks(83);
                        session.Locations.CompleteLocationChecks(383);
                        break;
                    case "D-Day":
                        checkedArray[84] = true;
                        session.Locations.CompleteLocationChecks(84);
                        session.Locations.CompleteLocationChecks(384);
                        break;
                    case "Matryoshka":
                        checkedArray[85] = true;
                        session.Locations.CompleteLocationChecks(85);
                        session.Locations.CompleteLocationChecks(385);
                        break;
                    case "Columns Like You See 'Em":
                        checkedArray[86] = true;
                        session.Locations.CompleteLocationChecks(86);
                        session.Locations.CompleteLocationChecks(386);
                        break;
                    case "Mirrors Like You See 'Em":
                        checkedArray[87] = true;
                        session.Locations.CompleteLocationChecks(87);
                        session.Locations.CompleteLocationChecks(387);
                        break;
                    case "It's Raining Seeds":
                        checkedArray[88] = true;
                        session.Locations.CompleteLocationChecks(88);
                        session.Locations.CompleteLocationChecks(388);
                        break;
                    case "Last Stand":
                        checkedArray[89] = true;
                        session.Locations.CompleteLocationChecks(89);
                        session.Locations.CompleteLocationChecks(389);
                        break;
                    case "Air Raid":
                        checkedArray[90] = true;
                        session.Locations.CompleteLocationChecks(90);
                        session.Locations.CompleteLocationChecks(390);
                        break;
                    case "Advanced Challenge: 12-Lane Day":
                        checkedArray[91] = true;
                        session.Locations.CompleteLocationChecks(91);
                        session.Locations.CompleteLocationChecks(391);
                        break;
                    case "Advanced Challenge: 12-Lane Pool":
                        checkedArray[92] = true;
                        session.Locations.CompleteLocationChecks(92);
                        session.Locations.CompleteLocationChecks(392);
                        break;
                    case "True Art is  an Explosion!":
                        checkedArray[93] = true;
                        session.Locations.CompleteLocationChecks(93);
                        session.Locations.CompleteLocationChecks(393);
                        break;
                    case "Pogo Party!":
                        checkedArray[94] = true;
                        session.Locations.CompleteLocationChecks(94);
                        session.Locations.CompleteLocationChecks(394);
                        break;
                    case "Attack on Gargantuar!":
                        checkedArray[95] = true;
                        session.Locations.CompleteLocationChecks(95);
                        session.Locations.CompleteLocationChecks(395);
                        break;
                    case "Squash Showdown!":
                        checkedArray[97] = true;
                        session.Locations.CompleteLocationChecks(97);
                        session.Locations.CompleteLocationChecks(397);
                        break;
                    case "Hypno-tism!":
                        checkedArray[98] = true;
                        session.Locations.CompleteLocationChecks(98);
                        session.Locations.CompleteLocationChecks(398);
                        break;
                    case "Protect the Gold Magnet":
                        checkedArray[100] = true;
                        session.Locations.CompleteLocationChecks(100);
                        session.Locations.CompleteLocationChecks(400);
                        break;
                    case "Compact Planting 2":
                        checkedArray[101] = true;
                        session.Locations.CompleteLocationChecks(101);
                        session.Locations.CompleteLocationChecks(401);
                        break;
                    case "Bungee Blitz":
                        checkedArray[102] = true;
                        session.Locations.CompleteLocationChecks(102);
                        session.Locations.CompleteLocationChecks(402);
                        break;
                    case "Beghouled":
                        checkedArray[103] = true;
                        session.Locations.CompleteLocationChecks(103);
                        session.Locations.CompleteLocationChecks(403);
                        break;
                    case "Seeing Stars":
                        checkedArray[104] = true;
                        session.Locations.CompleteLocationChecks(104);
                        session.Locations.CompleteLocationChecks(404);
                        break;
                    case "Wall-nut Billiards":
                        checkedArray[105] = true;
                        session.Locations.CompleteLocationChecks(105);
                        session.Locations.CompleteLocationChecks(405);
                        break;
                    case "Wall-nut Billiards 2":
                        checkedArray[106] = true;
                        session.Locations.CompleteLocationChecks(106);
                        session.Locations.CompleteLocationChecks(406);
                        break;
                    case "Wall-nut Billiards 3":
                        checkedArray[107] = true;
                        session.Locations.CompleteLocationChecks(107);
                        session.Locations.CompleteLocationChecks(407);
                        break;
                    case "Whack a Zombie":
                        checkedArray[108] = true;
                        session.Locations.CompleteLocationChecks(108);
                        session.Locations.CompleteLocationChecks(408);
                        break;
                    case "Zombie Nimble Zombie Quick":
                        checkedArray[109] = true;
                        session.Locations.CompleteLocationChecks(109);
                        session.Locations.CompleteLocationChecks(409);
                        break;
                    case "High Gravity":
                        checkedArray[110] = true;
                        session.Locations.CompleteLocationChecks(110);
                        session.Locations.CompleteLocationChecks(410);
                        break;
                    case "Chomper Snake":
                        checkedArray[111] = true;
                        session.Locations.CompleteLocationChecks(111);
                        session.Locations.CompleteLocationChecks(411);
                        break;
                    case "Chinese Chezz":
                        checkedArray[112] = true;
                        session.Locations.CompleteLocationChecks(112);
                        session.Locations.CompleteLocationChecks(412);
                        break;
                    case "Squash Showdown 2":
                        checkedArray[113] = true;
                        session.Locations.CompleteLocationChecks(113);
                        session.Locations.CompleteLocationChecks(413);
                        break;
                    case "Zombies VS Zombies 2":
                        checkedArray[114] = true;
                        session.Locations.CompleteLocationChecks(114);
                        session.Locations.CompleteLocationChecks(414);
                        break;
                    case "2048: Pea-volution":
                        checkedArray[115] = true;
                        session.Locations.CompleteLocationChecks(115);
                        session.Locations.CompleteLocationChecks(415);
                        break;
                    case "Splash and Clash":
                        checkedArray[116] = true;
                        session.Locations.CompleteLocationChecks(116);
                        session.Locations.CompleteLocationChecks(416);
                        break;
                    case "Melon Ninja, Recommended Diff: 0-3":
                        checkedArray[117] = true;
                        session.Locations.CompleteLocationChecks(117);
                        session.Locations.CompleteLocationChecks(417);
                        break;
                    case "Eclipse":
                        checkedArray[118] = true;
                        session.Locations.CompleteLocationChecks(118);
                        session.Locations.CompleteLocationChecks(418);
                        break;
                    case "Wall-nut Bowling, Recommended Diff: 0-4":
                        checkedArray[119] = true;
                        session.Locations.CompleteLocationChecks(119);
                        session.Locations.CompleteLocationChecks(419);
                        break;
                    case "Iceborg Executrix's Revenge":
                        checkedArray[120] = true;
                        session.Locations.CompleteLocationChecks(120);
                        session.Locations.CompleteLocationChecks(420);
                        break;
                    case "Big Trouble Little Zombie":
                        checkedArray[121] = true;
                        session.Locations.CompleteLocationChecks(121);
                        session.Locations.CompleteLocationChecks(421);
                        break;
                    case "True Art is an Explosion 2":
                        checkedArray[122] = true;
                        session.Locations.CompleteLocationChecks(122);
                        session.Locations.CompleteLocationChecks(422);
                        break;
                    case "Capture the Flag":
                        checkedArray[123] = true;
                        session.Locations.CompleteLocationChecks(123);
                        session.Locations.CompleteLocationChecks(423);
                        break;
                    case "Attack on Gargantuar! 2":
                        checkedArray[124] = true;
                        session.Locations.CompleteLocationChecks(124);
                        session.Locations.CompleteLocationChecks(424);
                        break;
                    case "Graveout":
                        checkedArray[125] = true;
                        session.Locations.CompleteLocationChecks(125);
                        session.Locations.CompleteLocationChecks(425);
                        break;
                    case "Graveout 2":
                        checkedArray[126] = true;
                        session.Locations.CompleteLocationChecks(126);
                        session.Locations.CompleteLocationChecks(426);
                        break;
                    case "The Floor is Lava":
                        checkedArray[127] = true;
                        session.Locations.CompleteLocationChecks(127);
                        session.Locations.CompleteLocationChecks(427);
                        break;
                    case "Art Challenge: Wall-nut":
                        checkedArray[128] = true;
                        session.Locations.CompleteLocationChecks(128);
                        session.Locations.CompleteLocationChecks(428);
                        break;
                    case "Challenge Mode":
                        checkedArray[129] = true;
                        session.Locations.CompleteLocationChecks(129);
                        session.Locations.CompleteLocationChecks(429);
                        break;
                        
                    case "Dr. Zomboss' Revenge":
                        checkedArray[99] = true;
                        session.Locations.CompleteLocationChecks(99);
                        session.Locations.CompleteLocationChecks(399);
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


        private void HideTargetObjectChildren(GameObject canvas, string target)
        {

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
                if (adventureExtraEnabled && checkedArray[54])
                {
                    session.Socket.SendPacket(new StatusUpdatePacket() { Status = ArchipelagoClientState.ClientGoal });
                }
                else if (!adventureExtraEnabled)
                {
                    session.Socket.SendPacket(new StatusUpdatePacket() { Status = ArchipelagoClientState.ClientGoal });
                }

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
                if (arg.Key == "AdventureExtra" && arg.Value.ToString() == "2")
                {
                    adventureExtraEnabled = true;
                }
                

            }
            session.Socket.PacketReceived += OnPacketReceived;



            ILocationCheckHelper locationHelper = session.Locations;

            var checkedLocations = locationHelper.AllLocationsChecked;

            var count = checkedLocations.Count;
            for (int i = 0; i < count; i++)
            {
                //MelonLogger.Msg(checkedLocations[i].ToString());
                checkedArray[checkedLocations[i]] = true;


            }




        }
    }

    public class APData
    {
        public string serverAddress { get; set; }
        public int serverPort { get; set; }
        public string slotName { get; set; }
        public string password { get; set; }

    }


}
