
using Archipelago;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Packets;
using Harmony;
using HarmonyLib;
using Il2Cpp;
using Il2CppAutoChess;
using Il2CppGameLevel.RogueShooting;
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
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows;
using static Il2CppGameLevel.EventNodes.EventNodeInputHandler;
using static Il2CppSystem.Globalization.CultureInfo;
using static UnityEngine.GraphicsBuffer;
using System.Diagnostics;

namespace PVZFusionArchipelago
{
    public class Class1 : MelonMod
    {
        public const int maxItems = 350; // holy shit good programming practice?\
        public const int maxLocations = 2300;
        public int seedSlots = 0;
        public int trophyCount = 0;
        public int trophiesForGoal = 0;
        public static int extraStartSun = 0;
        public static ArchipelagoSession session;
        public static int currentPlayerSlot;
        public static bool[] unlockedArray = new bool[maxItems];
        public static bool[] checkedArray = new bool[maxLocations];
        public static bool negativeSun = false;

        const string page1 = "InGameUI(Clone)/Bottom/SeedLibrary/Grid/CardPagesContainer/NormalCards/";
        const string page2 = "InGameUI(Clone)/Bottom/SeedLibrary/Grid/CardPagesContainer/ColorCards/";
  
        const string advanturePage1 = "ChallengeMenu(Clone)/Levels/PageAdvantureLevel/Pages/Page1/";
        const string advanturePage2 = "ChallengeMenu(Clone)/Levels/PageAdvantureLevel/Pages/Page2/";
        const string advanturePage3 = "ChallengeMenu(Clone)/Levels/PageAdvantureLevel/Pages/Page3/";
        const string advanturePage4 = "ChallengeMenu(Clone)/Levels/PageNewAdvantureLevel/Pages/Page1/";
        const string minigamePage1 = "ChallengeMenu(Clone)/Levels/PageMiniGames/Pages/Page1/";
        const string minigamePage2 = "ChallengeMenu(Clone)/Levels/PageMiniGames/Pages/Page2/";
        const string minigamePage3  = "ChallengeMenu(Clone)/Levels/PageMiniGames/Pages/Page3/";
        const string minigamePage4 = "ChallengeMenu(Clone)/Levels/PageMiniGames/Pages/Page4/";

        const string survivalPage1 = "SurvivalMenu(Clone)/Level/Page1/";
        const string survivalPage2 = "SurvivalMenu(Clone)/Level/Page2/";


        const string odysseyadvanturePage1 = "Canvas/TravelAdvantureMenu(Clone)/WeakPage/SamplePage(Clone)/";


        const string odysseyminigamePage1 = "Canvas/TravelGameMenu(Clone)/Levels/PageNormalTravel/Page1";
        const string odysseyminigamePage2 = "Canvas/TravelGameMenu(Clone)/Levels/PageNormalTravel/Page2";
        const string odysseyminigamePage3 = "Canvas/TravelGameMenu(Clone)/Levels/PageNormalTravel/Page3";

        const string showcasePage = "ExploreMenu(Clone)/Level/Line1/";
        const string challengePage = "ChallengeMenu(Clone)/Levels/PageUnlockChallenge/Page1/";
        const string vasePage = "ChallengeMenu(Clone)/Levels/PageScaryPot/Pages/Page1/";
        const string slotPath = "InGameUI(Clone)/SeedBank/SeedGroup/";
        public int ringLinkMode = 0;
        public int deathLinkMode = 0;
        public static bool resetCD = false;
        public static float cdResetTime = 0.0F;
        public bool adventureExtraEnabled = false;
        public static int fusionsanity = 0;
        public static int prevSun;
        public static bool sunExists = false;
        public static int goalType = 0;
        public static int numOdysseyLevels = 0;
        public static bool lostThisFrame = false;
        public static GameObject boardGl;
        public static GameObject canvasGl;
        public static GameObject canvasupGl;
        public static GameObject GameAPPGl;
        public static Transform levelUIGl;

        public PlayerHelper playerInfo;
        public AssetBundle bundle;
        public GameObject sunflowerPrefabObject;
        public GameObject droppedCardPrefabObject;
        public GameObject gardenPlantPrefabObject;
        public GameObject diamondPrefabObject;
        public GameObject sunPrefabObject;
        public GameObject starMeteorPrefabObject;
        public GameObject fogPrefabObject;
        public GameObject loseMenuObject;
        public GameObject zombieStarObject;
        public GameObject normalZombieObject;
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





            sunflowerPrefabObject = Resources.LoadAll<GameObject>("").First(obj => obj.name == "SunflowerPrefab");
            droppedCardPrefabObject = Resources.LoadAll<GameObject>("").First(obj => obj.name == "DroppedCard");
            gardenPlantPrefabObject = Resources.LoadAll<GameObject>("").First(obj => obj.name == "SproutPotPrize");
            diamondPrefabObject = Resources.LoadAll<GameObject>("").First(obj => obj.name == "Diamond");
            sunPrefabObject = Resources.LoadAll<GameObject>("").First(obj => obj.name == "Sun");
            starMeteorPrefabObject = Resources.LoadAll<GameObject>("").First(obj => obj.name == "BigSuperStar");
            fogPrefabObject = Resources.LoadAll<GameObject>("").First(obj => obj.name == "Fog");
            loseMenuObject = Resources.LoadAll<GameObject>("").First(obj => obj.name == "LoseMenu");
            zombieStarObject = Resources.LoadAll<GameObject>("").First(obj => obj.name == "BigStar");
            normalZombieObject = Resources.LoadAll<GameObject>("").First(obj => obj.name == "NormalZombie");




            //foreach (GameObject obj in Resources.LoadAll<GameObject>(""))
            //{
            //    MelonLogger.Msg(obj.name);
            //    //if (obj.name == "SunflowerPrefab")
            //    // { sunflowerPrefabObject = obj; }
            //}


            //DroppedCard
            //    sunPrefab = Resources.Load<GameObject>("plants/doublepea");

            //if (sunPrefab == null)
            //{ MelonLogger.Msg("Failed to load prefab"); }


            //instantiate object whenever





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
            CheckForItemsStartup();
        }



        public override void OnUpdate()
        {

            if (GameAPPGl == null)
            {
                GameAPPGl = GameObject.Find("GameAPP");
                if (GameAPPGl != null)
                {
                    var component = GameAPPGl.GetComponent(Il2CppType.Of<GameAPP>());

                    var type = component.GetIl2CppType();
                    var field = type.GetField("advantureLevel");
                    field.SetValue(component, 46);


                }
            }


        
        AttachClickHandler("TrophyPrefab");
            if (session == null)
            {
                MelonLogger.Msg("didnt find archipelago session");
                return;
            }
            CheckForNewItems();


            if (deathLinkMode != 0)
            {

                if (canvasupGl == null)
                { canvasupGl = GameObject.Find("CanvasUp"); }
                if (canvasGl != null)
                {
                    var loseMenu = canvasupGl.transform.Find("LoseMenu(Clone)");

                    if (loseMenu != null)//im too lazy to use the deathlink helper
                    {
                        if (lostThisFrame == false)
                        {
                            lostThisFrame = true;
                            MelonLogger.Msg("fuck");
                            string deathMessage = "PVZFusion had their brains eaten";
                            //send deathlink packet
                            var dataDict = new Dictionary<string, JToken>
                        {
                            { "time", JToken.FromObject(System.DateTimeOffset.UtcNow.ToUnixTimeSeconds()) },
                            { "source", JToken.FromObject(session.Players.ActivePlayer.Slot) },
                            { "cause", JToken.FromObject(deathMessage) }
                        };

                                session.Socket.SendPacket(new BouncePacket()
                                {
                                    Tags = new List<string> { "DeathLink" },
                                    Slots = new List<int> { session.Players.ActivePlayer.Slot },
                                    Data = dataDict
                                });
                            
                        }


                    } else
                    {
                        lostThisFrame = false;
                    }



                }

            }


            try
            {
                if (ringLinkMode != 0)
                {
                    if (canvasGl == null)
                    { canvasGl = GameObject.Find("Canvas"); }

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
                                MelonLogger.Msg($"{(intValue) - (prevSun)}");
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
                if (unlockedArray[1] == false) //peashooter
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 0); }
                if (unlockedArray[2] == false) //sunflower
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 1); }
                if (unlockedArray[3] == false) // cherry bomb
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 2); }
                if (unlockedArray[4] == false) //wallnut
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 3); }
                if (unlockedArray[5] == false) //potato mine
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 4); }
                if (unlockedArray[6] == false) //chomper
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 5); }
                if (unlockedArray[7] == false) //giftbox
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 6); }
                if (unlockedArray[8] == false) //tallnut
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 7); }
                if (unlockedArray[9] == false) // endoflame
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 8); }
                if (unlockedArray[10] == false) //puff shroom
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 9); }
                if (unlockedArray[11] == false) //fume-shroom
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 10); }
                if (unlockedArray[12] == false) // hypnoshroom
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 11); }
                if (unlockedArray[13] == false) //scaredy
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 12); }
                if (unlockedArray[14] == false) //ice shroom
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 13); }
                if (unlockedArray[15] == false) //doom shroom
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 14); }
                if (unlockedArray[16] == false)// zombie present
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 15); }
                if (unlockedArray[17] == false)//gloomshroom
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 16); }
                if (unlockedArray[18] == false)//grave buster
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 17); }
                if (unlockedArray[19] == false) //lily pad
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 18); }
                if (unlockedArray[20] == false)//squash
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 19); }
                if (unlockedArray[21] == false)//threepeater
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 20); }
                if (unlockedArray[22] == false)//tangle kelp
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 21); }
                if (unlockedArray[23] == false)// jalapeno
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 22); }
                if (unlockedArray[24] == false) //spikeweed
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 23); }
                if (unlockedArray[25] == false)//torchwood
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 24); }
                if (unlockedArray[26] == false) //spikerock
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 25); }
                if (unlockedArray[27] == false) //barley
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 26); }
                if (unlockedArray[28] == false) //seashroom
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 27); }
                if (unlockedArray[29] == false)//plantern
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 28); }
                if (unlockedArray[30] == false)//cactus
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 29); }
                if (unlockedArray[31] == false)//blover
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 30); }
                if (unlockedArray[32] == false)//starfruit
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 31); }
                if (unlockedArray[33] == false)//pumpkin
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 32); }
                if (unlockedArray[34] == false)//magnet shroom
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 33); }
                if (unlockedArray[35] == false)//cattail
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 34); }
                if (unlockedArray[36] == false)//imitater
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 35); }
                if (unlockedArray[37] == false)//cabbagepult
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 36); }
                if (unlockedArray[38] == false)//flower pot
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 37); }
                if (unlockedArray[39] == false)//kernel pult
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 38); }
                if (unlockedArray[40] == false)//garlic
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 39); }
                if (unlockedArray[41] == false)//Umbrellaleaf
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 40); }
                if (unlockedArray[42] == false)//marigold
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 41); }
                if (unlockedArray[43] == false)//melonpult
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 42); }
                if (unlockedArray[44] == false)//cobcannon
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 43); }
                if (unlockedArray[45] == false)//jicamagic
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 44); }
                if (unlockedArray[46] == false)//firnace
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 45); }
                if (unlockedArray[47] == false)//spruce sharpshooter
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 46); }
                if (unlockedArray[48] == false)//sawmenot
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 47); }
                if (unlockedArray[49] == false)//icelotus
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 48); }
                if (unlockedArray[50] == false)//aloe aqua
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 49); }
                if (unlockedArray[51] == false)//bamblock
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 50); }
                if (unlockedArray[52] == false)//hoarfrost lichen
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 51); }
                if (unlockedArray[53] == false)//spruce ballista
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 52); }
                if (unlockedArray[54] == false)//frozen giftbox
                { HideSpecifiedNewSeedSlot(canvasupGl, page1, 53); }

                if (unlockedArray[61] == false)//ohmion
                { HideSpecifiedNewSeedSlot(canvasupGl, page2, 0); }
                if (unlockedArray[55] == false)//cattail girl
                { HideSpecifiedNewSeedSlot(canvasupGl, page2, 1); }
                if (unlockedArray[56] == false)//swordmaster starfruit
                { HideSpecifiedNewSeedSlot(canvasupGl, page2, 2); }
                if (unlockedArray[57] == false)//nyan squash
                { HideSpecifiedNewSeedSlot(canvasupGl, page2, 3); }
                if (unlockedArray[58] == false)//burger blaster
                { HideSpecifiedNewSeedSlot(canvasupGl, page2, 4); }
                if (unlockedArray[60] == false)//ice bean
                { HideSpecifiedNewSeedSlot(canvasupGl, page2, 5); }
                if (unlockedArray[63] == false)//chrysanctum
                { HideSpecifiedNewSeedSlot(canvasupGl, page2, 6); }
                if (unlockedArray[64] == false)//icetip lilly
                { HideSpecifiedNewSeedSlot(canvasupGl, page2, 7); }
                if (unlockedArray[65] == false)//frozen pear
                { HideSpecifiedNewSeedSlot(canvasupGl, page2, 8); }
                if (unlockedArray[66] == false)//doubleblast passionfruit
                { HideSpecifiedNewSeedSlot(canvasupGl, page2, 9); }
                if (unlockedArray[67] == false)//luckyblover
                { HideSpecifiedNewSeedSlot(canvasupGl, page2, 10); }
                //if (unlockedArray[69] == false)//bucket
                //{ HideSpecifiedNewSeedSlot(canvasupGl, page2, 11); }
                //if (unlockedArray[84] == false)//football helmet
                //{ HideSpecifiedNewSeedSlot(canvasupGl, page2, 12); }
                if (unlockedArray[85] == false)//hearty apple
                { HideSpecifiedNewSeedSlot(canvasupGl, page2, 11); }
                if (unlockedArray[86] == false)//bamboom
                { HideSpecifiedNewSeedSlot(canvasupGl, page2, 12); }
                if (unlockedArray[87] == false)//gold recycling vase
                { HideSpecifiedNewSeedSlot(canvasupGl, page2, 13); }
                if (unlockedArray[59] == false)//queen endoflame
                { HideSpecifiedNewSeedSlot(canvasupGl, page2, 14); }
                if (unlockedArray[62] == false)//sniper pea
                { HideSpecifiedNewSeedSlot(canvasupGl, page2, 15); }
                if (unlockedArray[68] == false)//diamond imitater
                { HideSpecifiedNewSeedSlot(canvasupGl, page2, 16); }




            } 


            if (unlockedArray[70] == false)//shovel
            { HideTargetObject("Canvas", "InGameUI(Clone)/Tools/ShovelBank");
                HideTargetObject("CanvasUp", "InGameUI(Clone)/Tools/ShovelBank");
            }



                if (boardGl == null)
                { boardGl = GameObject.Find("Board"); }
            if (boardGl != null)
            {
                if (resetCD)
                {


                    cdResetTime -= Time.deltaTime;

                    if (cdResetTime < 0) { 


                    Board component = boardGl.GetComponent<Board>();
                    var type = component.GetIl2CppType();
                    var field = type.GetField("freeCD");
                    field.SetValue(component, false);

                    resetCD = false;
                }
                }



                if (unlockedArray[71] == false)//fertilizer
                {
                    var fertilizerTransform = boardGl.transform.Find("Ferilize(Clone)");

                    if (fertilizerTransform != null)
                    { GameObject.Destroy(fertilizerTransform.gameObject); }
                }


                if (unlockedArray[82] == false)//mowers
                {
                    var lawnmower = boardGl.transform.Find("LawnMowerPrefab_0");
                    if (lawnmower != null)
                    { GameObject.Destroy(lawnmower.gameObject); }
                    lawnmower = boardGl.transform.Find("LawnMowerPrefab_1");
                    if (lawnmower != null)
                    { GameObject.Destroy(lawnmower.gameObject); }
                    lawnmower = boardGl.transform.Find("LawnMowerPrefab_2");
                    if (lawnmower != null)
                    { GameObject.Destroy(lawnmower.gameObject); }
                    lawnmower = boardGl.transform.Find("LawnMowerPrefab_3");
                    if (lawnmower != null)
                    { GameObject.Destroy(lawnmower.gameObject); }
                    lawnmower = boardGl.transform.Find("LawnMowerPrefab_4");
                    if (lawnmower != null)
                    { GameObject.Destroy(lawnmower.gameObject); }
                    lawnmower = boardGl.transform.Find("LawnMowerPrefab_5");
                    if (lawnmower != null)
                    { GameObject.Destroy(lawnmower.gameObject); }
                    lawnmower = boardGl.transform.Find("LawnMowerPrefab_6");
                    if (lawnmower != null)
                    { GameObject.Destroy(lawnmower.gameObject); }
                    lawnmower = boardGl.transform.Find("LawnMowerPrefab_7");
                    if (lawnmower != null)
                    { GameObject.Destroy(lawnmower.gameObject); }
                    lawnmower = boardGl.transform.Find("LawnMowerPrefab_8");
                    if (lawnmower != null)
                    { GameObject.Destroy(lawnmower.gameObject); }
                    lawnmower = boardGl.transform.Find("LawnMowerPrefab_9");
                    if (lawnmower != null)
                    { GameObject.Destroy(lawnmower.gameObject); }
                    lawnmower = boardGl.transform.Find("LawnMowerPrefab_10");
                    if (lawnmower != null)
                    { GameObject.Destroy(lawnmower.gameObject); }
                    lawnmower = boardGl.transform.Find("LawnMowerPrefab_11");
                    if (lawnmower != null)
                    { GameObject.Destroy(lawnmower.gameObject); }
                }
                if (unlockedArray[83] == false)//pool cleaners
                {
                    var poolCleaner = boardGl.transform.Find("PoolCleanerPrefab_0");
                    if (poolCleaner != null)
                    { GameObject.Destroy(poolCleaner.gameObject); }
                    poolCleaner = boardGl.transform.Find("PoolCleanerPrefab_1");
                    if (poolCleaner != null)
                    { GameObject.Destroy(poolCleaner.gameObject); }
                    poolCleaner = boardGl.transform.Find("PoolCleanerPrefab_2");
                    if (poolCleaner != null)
                    { GameObject.Destroy(poolCleaner.gameObject); }
                    poolCleaner = boardGl.transform.Find("PoolCleanerPrefab_3");
                    if (poolCleaner != null)
                    { GameObject.Destroy(poolCleaner.gameObject); }
                    poolCleaner = boardGl.transform.Find("PoolCleanerPrefab_4");
                    if (poolCleaner != null)
                    { GameObject.Destroy(poolCleaner.gameObject); }
                    poolCleaner = boardGl.transform.Find("PoolCleanerPrefab_5");
                    if (poolCleaner != null)
                    { GameObject.Destroy(poolCleaner.gameObject); }
                    poolCleaner = boardGl.transform.Find("PoolCleanerPrefab_6");
                    if (poolCleaner != null)
                    { GameObject.Destroy(poolCleaner.gameObject); }
                    poolCleaner = boardGl.transform.Find("PoolCleanerPrefab_7");
                    if (poolCleaner != null)
                    { GameObject.Destroy(poolCleaner.gameObject); }
                }
            }
            

            if (canvasGl == null)
            { canvasGl = GameObject.Find("Canvas"); }
            if (canvasGl != null)
            {


                if (unlockedArray[71] == false)//fertilizer2
                { HideTargetObjectChildren(canvasGl, "GardenUI(Clone)/Tools/FertilizeBank"); }


                if (unlockedArray[72] == false)//gloves
                {
                    HideTargetObject("Canvas", "InGameUI(Clone)/Tools/GloveBank");
                    HideTargetObject("CanvasUp", "InGameUI(Clone)/Tools/GloveBank");
                    HideTargetObjectChildren(canvasGl, "GardenUI(Clone)/Tools/GloveBank");
                }
                if (unlockedArray[74] == false)//mallet
                { HideTargetObject("Canvas", "InGameUI(Clone)/Tools/HammerBank");
                    HideTargetObject("CanvasUp", "InGameUI(Clone)/HammerBank");
                }


                if (unlockedArray[76] == false)//watering can
                { HideTargetObjectChildren(canvasGl, "GardenUI(Clone)/Tools/WaterBank"); }

                if (unlockedArray[77] == false)//gramophone
                { HideTargetObjectChildren(canvasGl, "GardenUI(Clone)/Tools/PhonographBank"); }

                if (unlockedArray[78] == false)//bug spray
                { HideTargetObjectChildren(canvasGl, "GardenUI(Clone)/Tools/BugSprayBank"); }

                if (unlockedArray[79] == false)//wheelbarrow
                { HideTargetObjectChildren(canvasGl, "GardenUI(Clone)/Tools/WheelBarrowBank");
                    HideTargetObject("Canvas", "InGameUI(Clone)/Tools/WheelBank");
                }


                
                if (unlockedArray[88] == false)//coffee bean
                { HideTargetObject("CanvasUp", "CanvasUp/MoneyBank/BeanBank"); }


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
                if (goalType == 0 && !unlockedArray[131]) { HideTargetObject("Canvas", "ChallengeMenu(Clone)/Levels/PageMiniGames/Pages/Page2/Lv77"); }
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
                HideTargetObject("Canvas", advanturePage4 + "Lv10");
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
                if (!checkedArray[78]) { HideTargetObject("Canvas", advanturePage4 + "Lv10/Window/Trophy"); }
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
                if (!checkedArray[66]) { HideTargetObject("Canvas", challengePage + "Lv162/Window/Trophy"); }
            }
            if (unlockedArray[249] == false)
            {
                HideTargetObject("Canvas", "ChallengeMenu(Clone)/Levels/FirstBtns/ScaryPot");
            }
            else
            {
                if (!checkedArray[200]) { HideTargetObject("Canvas", vasePage + "Lv102/Window/Trophy"); }
                if (!checkedArray[201]) { HideTargetObject("Canvas", vasePage + "Lv103/Window/Trophy"); }
                if (!checkedArray[202]) { HideTargetObject("Canvas", vasePage + "Lv104/Window/Trophy"); }
                if (!checkedArray[203]) { HideTargetObject("Canvas", vasePage + "Lv175/Window/Trophy"); }
                if (!checkedArray[204]) { HideTargetObject("Canvas", vasePage + "Lv176/Window/Trophy"); }
                if (!checkedArray[205]) { HideTargetObject("Canvas", vasePage + "Lv177/Window/Trophy"); }

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
            if (unlockedArray[98] == false)
            {
                HideTargetObject("Canvas", "MainMenu(Clone)/Grave/LowerButtons/Travel");
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


            if (unlockedArray[168] == false)
            { HideTargetObject("Canvas", minigamePage1 + "Lv186"); }
            else { if (!checkedArray[135]) { HideTargetObject("Canvas", minigamePage1 + "Lv186/Window/Trophy"); } }
            if (unlockedArray[169] == false)
            { HideTargetObject("Canvas", minigamePage1 + "Lv187"); }
            else { if (!checkedArray[136]) { HideTargetObject("Canvas", minigamePage1 + "Lv187/Window/Trophy"); } }


            if (unlockedArray[128] == false)
            { HideTargetObject("Canvas", minigamePage2 + "Lv63"); }
            else { if (!checkedArray[96]) { HideTargetObject("Canvas", minigamePage2 + "Lv63/Window/Trophy"); } }
            if (unlockedArray[129] == false)
            { HideTargetObject("Canvas", minigamePage2 + "Lv65"); }
            else { if (!checkedArray[96]) { HideTargetObject("Canvas", minigamePage2 + "Lv65/Window/Trophy"); } }
            if (unlockedArray[130] == false)
            { HideTargetObject("Canvas", minigamePage2 + "Lv67"); }
            else { if (!checkedArray[98]) { HideTargetObject("Canvas", minigamePage2 + "Lv67/Window/Trophy"); } }


            if (!(unlockedArray[131] == true || (goalType == 0 && unlockedArray[94])))
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
           

            if (unlockedArray[160] == false)
            { HideTargetObject("Canvas", minigamePage3 + "Lv160"); }
            else { if (!checkedArray[127]) { HideTargetObject("Canvas", minigamePage3 + "Lv160/Window/Trophy"); } }
            if (unlockedArray[161] == false)
            { HideTargetObject("Canvas", minigamePage3 + "Lv161"); }
            else { if (!checkedArray[128]) { HideTargetObject("Canvas", minigamePage3 + "Lv161/Window/Trophy"); } }
            if (unlockedArray[162] == false)
            { HideTargetObject("Canvas", minigamePage3 + "Lv163"); }
            else { if (!checkedArray[129]) { HideTargetObject("Canvas", minigamePage3 + "Lv163/Window/Trophy"); } }
            if (unlockedArray[163] == false)
            { HideTargetObject("Canvas", minigamePage3 + "Lv164"); }
            else { if (!checkedArray[130]) { HideTargetObject("Canvas", minigamePage3 + "Lv164/Window/Trophy"); } }
            if (unlockedArray[164] == false)
            { HideTargetObject("Canvas", minigamePage4 + "Lv133"); }
            else { if (!checkedArray[131]) { HideTargetObject("Canvas", minigamePage4 + "Lv133/Window/Trophy"); } }
            if (unlockedArray[165] == false)
            { HideTargetObject("Canvas", minigamePage4 + "Lv166"); }
            else { if (!checkedArray[132]) { HideTargetObject("Canvas", minigamePage4 + "Lv166/Window/Trophy"); } }
            if (unlockedArray[166] == false)
            { HideTargetObject("Canvas", minigamePage4 + "Lv171"); }
            else { if (!checkedArray[133]) { HideTargetObject("Canvas", minigamePage4 + "Lv171/Window/Trophy"); } }
            if (unlockedArray[167] == false)
            { HideTargetObject("Canvas", minigamePage4 + "Lv178"); }
            else { if (!checkedArray[134 ]) { HideTargetObject("Canvas", minigamePage4 + "Lv178/Window/Trophy"); } }

            if (unlockedArray[170] == false)
            { HideTargetObject("Canvas", minigamePage4 + "Lv185"); }
            else { if (!checkedArray[137]) { HideTargetObject("Canvas", minigamePage4 + "Lv185/Window/Trophy"); } }
            if (unlockedArray[171] == false)
            { HideTargetObject("Canvas", minigamePage4 + "Lv193"); }
            else { if (!checkedArray[138]) { HideTargetObject("Canvas", minigamePage4 + "Lv193/Window/Trophy"); } }
            if (unlockedArray[172] == false)
            { HideTargetObject("Canvas", minigamePage4 + "Lv190"); }
            else { if (!checkedArray[139]) { HideTargetObject("Canvas", minigamePage4 + "Lv190/Window/Trophy"); } }


            if (canvasGl != null)
            {

                if (unlockedArray[250] == false)
                {
                    HideTargetChildByID(canvasGl, survivalPage1, 0);
                    HideTargetChildByID(canvasGl, survivalPage1, 3);
                    HideTargetChildByID(canvasGl, survivalPage1, 9);
                }
                else
                {
                    if (!checkedArray[280]) { HideTrophyIconById(canvasGl, survivalPage1, 0); }
                    if (!checkedArray[281]) { HideTrophyIconById(canvasGl, survivalPage1, 3); }
                }
                if (unlockedArray[251] == false)
                {
                    HideTargetChildByID(canvasGl, survivalPage1, 1);
                    HideTargetChildByID(canvasGl, survivalPage1, 4);
                    HideTargetChildByID(canvasGl, survivalPage1, 8);
                }
                else
                {
                    if (!checkedArray[282]) { HideTrophyIconById(canvasGl, survivalPage1, 1); }
                    if (!checkedArray[283]) { HideTrophyIconById(canvasGl, survivalPage1, 4); }
                }
                if (unlockedArray[252] == false)
                {
                    HideTargetChildByID(canvasGl, survivalPage1, 2);
                    HideTargetChildByID(canvasGl, survivalPage1, 5);
                    HideTargetChildByID(canvasGl, survivalPage1, 6);
                }
                else
                {
                    if (!checkedArray[284]) { HideTrophyIconById(canvasGl, survivalPage1, 2); }
                    if (!checkedArray[285]) { HideTrophyIconById(canvasGl, survivalPage1, 5); }
                }
                if (unlockedArray[253] == false)
                {
                    HideTargetChildByID(canvasGl, survivalPage1, 14);
                    HideTargetChildByID(canvasGl, survivalPage1, 15);
                    HideTargetChildByID(canvasGl, survivalPage1, 10);
                }
                else
                {
                    if (!checkedArray[286]) { HideTrophyIconById(canvasGl, survivalPage1, 14); }
                    if (!checkedArray[287]) { HideTrophyIconById(canvasGl, survivalPage1, 15); }
                }
                if (unlockedArray[254] == false)
                {
                    HideTargetChildByID(canvasGl, survivalPage1, 12);
                    HideTargetChildByID(canvasGl, survivalPage1, 13);
                    HideTargetChildByID(canvasGl, survivalPage1, 11);
                }
                else
                {
                    if (!checkedArray[288]) { HideTrophyIconById(canvasGl, survivalPage1, 12); }
                    if (!checkedArray[289]) { HideTrophyIconById(canvasGl, survivalPage1, 13); }
                }

            }

            if (unlockedArray[255] == false)
            {
                HideTargetChildByID(canvasGl, survivalPage1, 16);
            }
            if (unlockedArray[256] == false)
            {
                HideTargetChildByID(canvasGl, survivalPage1, 7);
            }
            if (unlockedArray[257] == false)
            {
                HideTargetChildByID(canvasGl, survivalPage1, 17);
            }
            if (unlockedArray[258] == false)
            {
                HideTargetChildByID(canvasGl, survivalPage2, 0);
            }
            if (unlockedArray[259] == false)
            {
                HideTargetChildByID(canvasGl, survivalPage2, 1);
            }
            GameObject odadvpage1 = GameObject.Find(odysseyadvanturePage1);
            if (odadvpage1 != null)
            {
                if (numOdysseyLevels < 1)
                { HideSpecifiedTargetChild(odadvpage1, 0); }
                else { if (!checkedArray[601]) { HideSpecifiedNewTrophy(odadvpage1, 0); } }
                if (numOdysseyLevels < 2)
                { HideSpecifiedTargetChild(odadvpage1, 1); }
                else { if (!checkedArray[602]) { HideSpecifiedNewTrophy(odadvpage1, 1); } }
                if (numOdysseyLevels < 3)
                { HideSpecifiedTargetChild(odadvpage1, 2); }
                else { if (!checkedArray[603]) { HideSpecifiedNewTrophy(odadvpage1, 2); } }
                if (numOdysseyLevels < 4)
                { HideSpecifiedTargetChild(odadvpage1, 3); }
                else { if (!checkedArray[604]) { HideSpecifiedNewTrophy(odadvpage1, 3); } }
                if (numOdysseyLevels < 5)
                { HideSpecifiedTargetChild(odadvpage1, 4); }
                else { if (!checkedArray[605]) { HideSpecifiedNewTrophy(odadvpage1, 4); } }
                if (numOdysseyLevels < 6)
                { HideSpecifiedTargetChild(odadvpage1, 5); }
                else { if (!checkedArray[606]) { HideSpecifiedNewTrophy(odadvpage1, 5); } }
                if (numOdysseyLevels < 7)
                { HideSpecifiedTargetChild(odadvpage1, 6); }
                else { if (!checkedArray[607]) { HideSpecifiedNewTrophy(odadvpage1, 6); } }
                if (numOdysseyLevels < 8)
                { HideSpecifiedTargetChild(odadvpage1, 7); }
                else { if (!checkedArray[608]) { HideSpecifiedNewTrophy(odadvpage1, 7); } }
                if (numOdysseyLevels < 9)
                { HideSpecifiedTargetChild(odadvpage1, 8); }
                else { if (!checkedArray[609]) { HideSpecifiedNewTrophy(odadvpage1, 8); } }
                if (numOdysseyLevels < 10)
                { HideSpecifiedTargetChild(odadvpage1, 9); }
                else { if (!checkedArray[610]) { HideSpecifiedNewTrophy(odadvpage1, 9); } }
                if (numOdysseyLevels < 11)
                { HideSpecifiedTargetChild(odadvpage1, 10); }
                else { if (!checkedArray[611]) { HideSpecifiedNewTrophy(odadvpage1, 10); } }
                if (numOdysseyLevels < 12)
                { HideSpecifiedTargetChild(odadvpage1, 11); }
                else { if (!checkedArray[612]) { HideSpecifiedNewTrophy(odadvpage1, 11); } }
                if (numOdysseyLevels < 13)
                { HideSpecifiedTargetChild(odadvpage1, 12); }
                else { if (!checkedArray[613]) { HideSpecifiedNewTrophy(odadvpage1, 12); } }
                if (numOdysseyLevels < 14)
                { HideSpecifiedTargetChild(odadvpage1, 13); }
                else { if (!checkedArray[614]) { HideSpecifiedNewTrophy(odadvpage1, 13); } }
                if (numOdysseyLevels < 15)
                { HideSpecifiedTargetChild(odadvpage1, 14); }
                else { if (!checkedArray[615]) { HideSpecifiedNewTrophy(odadvpage1, 14); } }
                if (numOdysseyLevels < 16)
                { HideSpecifiedTargetChild(odadvpage1, 15); }
                else { if (!checkedArray[616]) { HideSpecifiedNewTrophy(odadvpage1, 15); } }
                if (numOdysseyLevels < 17)
                { HideSpecifiedTargetChild(odadvpage1, 16); }
                else { if (!checkedArray[617]) { HideSpecifiedNewTrophy(odadvpage1, 16); } }
                if (numOdysseyLevels < 18)
                { HideSpecifiedTargetChild(odadvpage1, 17); }
                else { if (!checkedArray[618]) { HideSpecifiedNewTrophy(odadvpage1, 17); } }
                if (numOdysseyLevels < 19)
                { HideSpecifiedTargetChild(odadvpage1, 18); }
                else { if (!checkedArray[619]) { HideSpecifiedNewTrophy(odadvpage1, 18); } }




                //if (numOdysseyLevels < 19)
                //{ HideSpecifiedNewSeedSlot(canvasGl, odysseyadvanturePage1, 18); }
                //else { if (!checkedArray[618]) { HideSpecifiedNewTrophy(canvasGl, odysseyadvanturePage1, 18); } }
            }

            

            GameObject odminipage1 = GameObject.Find(odysseyminigamePage1);
            if (odminipage1 != null) {
                if (unlockedArray[300] == false)
                { HideSpecifiedTargetChild(odminipage1, 0); }
                else { if (!checkedArray[650]) { HideSpecifiedTargetTrophy(odminipage1, 0); } }
                if (unlockedArray[301] == false)
                { HideSpecifiedTargetChild(odminipage1, 1); }
                else { if (!checkedArray[651]) { HideSpecifiedTargetTrophy(odminipage1, 1); } }
                if (unlockedArray[302] == false)
                { HideSpecifiedTargetChild(odminipage1, 4); }
                else { if (!checkedArray[652]) { HideSpecifiedTargetTrophy(odminipage1, 4); } }
                if (unlockedArray[303] == false)
                { HideSpecifiedTargetChild(odminipage1, 6); }
                else { if (!checkedArray[653]) { HideSpecifiedTargetTrophy(odminipage1, 6); } }

                if (unlockedArray[304] == false)
                { HideSpecifiedTargetChild(odminipage1, 11); }
                else { if (!checkedArray[654]) { HideSpecifiedTargetTrophy(odminipage1, 11); } }
                if (unlockedArray[305] == false)
                { HideSpecifiedTargetChild(odminipage1, 5); }
                else { if (!checkedArray[655]) { HideSpecifiedTargetTrophy(odminipage1, 5); } }
                if (unlockedArray[306] == false)
                { HideSpecifiedTargetChild(odminipage1, 10); }
                else { if (!checkedArray[656]) { HideSpecifiedTargetTrophy(odminipage1, 10); } }
                if (unlockedArray[307] == false)
                { HideSpecifiedTargetChild(odminipage1, 2); }
                else { if (!checkedArray[657]) { HideSpecifiedTargetTrophy(odminipage1, 2); } }
                if (unlockedArray[308] == false)
                { HideSpecifiedTargetChild(odminipage1, 3); }
                else { if (!checkedArray[658]) { HideSpecifiedTargetTrophy(odminipage1, 3); } }
                if (unlockedArray[309] == false)
                { HideSpecifiedTargetChild(odminipage1, 7); }
                else { if (!checkedArray[659]) { HideSpecifiedTargetTrophy(odminipage1, 7); } }
                if (unlockedArray[310] == false)
                { HideSpecifiedTargetChild(odminipage1, 9); }
                else { if (!checkedArray[660]) { HideSpecifiedTargetTrophy(odminipage1, 9); } }

                if (unlockedArray[311] == false)
                { HideSpecifiedTargetChild(odminipage1, 8); }

                if (unlockedArray[312] == false)
                { HideSpecifiedTargetChild(odminipage1, 12); }
                else { if (!checkedArray[661]) { HideSpecifiedTargetTrophy(odminipage1, 12); } }
                if (unlockedArray[313] == false)
                { HideSpecifiedTargetChild(odminipage1, 13); }
                else { if (!checkedArray[662]) { HideSpecifiedTargetTrophy(odminipage1, 13); } }
                if (unlockedArray[314] == false)
                { HideSpecifiedTargetChild(odminipage1, 14); }
                else { if (!checkedArray[663]) { HideSpecifiedTargetTrophy(odminipage1, 14); } }
                if (unlockedArray[315] == false)
                { HideSpecifiedTargetChild(odminipage1, 15); }
                else { if (!checkedArray[664]) { HideSpecifiedTargetTrophy(odminipage1, 15); } }
                if (unlockedArray[316] == false)
                { HideSpecifiedTargetChild(odminipage1, 16); }
                else { if (!checkedArray[665]) { HideSpecifiedTargetTrophy(odminipage1, 16); } }
                HideSpecifiedTargetChild(odminipage1, 17);


            }
            GameObject odminipage2 = GameObject.Find(odysseyminigamePage2);
            if (odminipage2 != null)
            {
                if (unlockedArray[318] == false)
                { HideSpecifiedTargetChild(odminipage2, 17); }
                else { if (!checkedArray[666]) { HideSpecifiedTargetTrophy(odminipage2, 17); } }
                if (unlockedArray[319] == false)
                { HideSpecifiedTargetChild(odminipage2, 0); }
                else { if (!checkedArray[667]) { HideSpecifiedTargetTrophy(odminipage2, 0); } }
                if (unlockedArray[320] == false)
                { HideSpecifiedTargetChild(odminipage2, 1); }
                else { if (!checkedArray[668]) { HideSpecifiedTargetTrophy(odminipage2, 1); } }
                if (unlockedArray[321] == false)
                { HideSpecifiedTargetChild(odminipage2, 2); }
                else { if (!checkedArray[669]) { HideSpecifiedTargetTrophy(odminipage2, 2); } }
                if (unlockedArray[322] == false)
                { HideSpecifiedTargetChild(odminipage2, 3); }
                else { if (!checkedArray[670]) { HideSpecifiedTargetTrophy(odminipage2, 3); } }
                if (unlockedArray[323] == false)
                { HideSpecifiedTargetChild(odminipage2, 4); }
                else { if (!checkedArray[671]) { HideSpecifiedTargetTrophy(odminipage2, 4); } }
                if (unlockedArray[324] == false)
                { HideSpecifiedTargetChild(odminipage2, 6); }
                if (unlockedArray[325] == false)
                { HideSpecifiedTargetChild(odminipage2, 7); }
                else { if (!checkedArray[672]) { HideSpecifiedTargetTrophy(odminipage2, 7); } }
                if (unlockedArray[326] == false)
                { HideSpecifiedTargetChild(odminipage2, 8); }
                else { if (!checkedArray[673]) { HideSpecifiedTargetTrophy(odminipage2, 8); } }
                if (unlockedArray[327] == false)
                { HideSpecifiedTargetChild(odminipage2, 5); }
                else { if (!checkedArray[674]) { HideSpecifiedTargetTrophy(odminipage2, 5); } }
                if (unlockedArray[328] == false)
                { HideSpecifiedTargetChild(odminipage2, 9); }
                else { if (!checkedArray[675]) { HideSpecifiedTargetTrophy(odminipage2, 9); } }
                if (unlockedArray[329] == false)
                { HideSpecifiedTargetChild(odminipage2, 10); }
                else { if (!checkedArray[676]) { HideSpecifiedTargetTrophy(odminipage2, 10); } }
                if (unlockedArray[330] == false)
                { HideSpecifiedTargetChild(odminipage2, 11); }
                else { if (!checkedArray[677]) { HideSpecifiedTargetTrophy(odminipage2, 11); } }
                if (unlockedArray[331] == false)
                { HideSpecifiedTargetChild(odminipage2, 12); }
                else { if (!checkedArray[678]) { HideSpecifiedTargetTrophy(odminipage2, 12); } }
                if (unlockedArray[332] == false)
                { HideSpecifiedTargetChild(odminipage2, 13); }
                else { if (!checkedArray[679]) { HideSpecifiedTargetTrophy(odminipage2, 13); } }
                if (unlockedArray[333] == false)
                { HideSpecifiedTargetChild(odminipage2, 14); }
                else { if (!checkedArray[680]) { HideSpecifiedTargetTrophy(odminipage2, 14); } }
                if (unlockedArray[334] == false)
                { HideSpecifiedTargetChild(odminipage2, 15); }
                else { if (!checkedArray[681]) { HideSpecifiedTargetTrophy(odminipage2, 15); } }
                if (unlockedArray[335] == false)
                { HideSpecifiedTargetChild(odminipage2, 16); }
                else { if (!checkedArray[682]) { HideSpecifiedTargetTrophy(odminipage2, 16); } }
            }
            GameObject odminipage3 = GameObject.Find(odysseyminigamePage3);
            if (odminipage3 != null)
            {
                if (unlockedArray[336] == false)
                { HideSpecifiedTargetChild(odminipage3, 0); }
                else { if (!checkedArray[683]) { HideSpecifiedTargetTrophy(odminipage3, 0); } }
                if (unlockedArray[337] == false)
                { HideSpecifiedTargetChild(odminipage3, 1); }
                else { if (!checkedArray[683]) { HideSpecifiedTargetTrophy(odminipage3, 1); } }
                if (unlockedArray[338] == false)
                { HideSpecifiedTargetChild(odminipage3, 2); }
                else { if (!checkedArray[684]) { HideSpecifiedTargetTrophy(odminipage3, 2); } }
                if (unlockedArray[339] == false)
                { HideSpecifiedTargetChild(odminipage3, 3); }
                else { if (!checkedArray[685]) { HideSpecifiedTargetTrophy(odminipage3, 3); } }
                if (unlockedArray[340] == false)
                { HideSpecifiedTargetChild(odminipage3, 4); }
                else { if (!checkedArray[685]) { HideSpecifiedTargetTrophy(odminipage3, 4); } }
                if (unlockedArray[341] == false)
                { HideSpecifiedTargetChild(odminipage3, 5); }
                else { if (!checkedArray[686]) { HideSpecifiedTargetTrophy(odminipage3, 5); } }
                if (unlockedArray[342] == false)
                { HideSpecifiedTargetChild(odminipage3, 6); }
                else { if (!checkedArray[687]) { HideSpecifiedTargetTrophy(odminipage3, 6); } }
                if (unlockedArray[343] == false)
                { HideSpecifiedTargetChild(odminipage3, 7); }
                else { if (!checkedArray[688]) { HideSpecifiedTargetTrophy(odminipage3, 7); } }
                if (unlockedArray[344] == false)
                { HideSpecifiedTargetChild(odminipage3, 8); }
                else { if (!checkedArray[689]) { HideSpecifiedTargetTrophy(odminipage3, 8); } }
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
                    case PlantType.SmallPuff:
                        if (!unlockedArray[10]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.FumeShroom:
                        if (!unlockedArray[11]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.HypnoShroom:
                        if (!unlockedArray[12]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.ScaredyShroom:
                        if (!unlockedArray[13]) { GameObject.Destroy(child.gameObject); }
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
                    case PlantType.Garlic:
                        if (!unlockedArray[40]) { GameObject.Destroy(child.gameObject); }
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

                    case PlantType.IronPuff:
                        if (!unlockedArray[10]) { GameObject.Destroy(child.gameObject); }
                        break;

                    case PlantType.FireSquash:
                        if (!unlockedArray[20]) { GameObject.Destroy(child.gameObject); }
                        break;


                    case PlantType.HypnoEmperor:
                        if (!unlockedArray[12]) { GameObject.Destroy(child.gameObject); }
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
                    case PlantType.IcePuff:
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
                    case PlantType.HypnoMagnet:
                        if (!unlockedArray[12] || !unlockedArray[34]) { GameObject.Destroy(child.gameObject); }
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
                    case PlantType.LanternPumpkin:
                        if (!unlockedArray[29] || !unlockedArray[33]) { GameObject.Destroy(child.gameObject); }
                        break;
                    case PlantType.LanternMagnet:
                        if (!unlockedArray[29] || !unlockedArray[34]) { GameObject.Destroy(child.gameObject); }
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


                    case PlantType.HypnoQueen:
                        if (!unlockedArray[12] || !unlockedArray[3]) { GameObject.Destroy(child.gameObject); }
                        break;


                    case PlantType.UltimateTorch:
                        if (!unlockedArray[20] || !unlockedArray[23] || !unlockedArray[25]) { GameObject.Destroy(child.gameObject); }
                        break;
                }



            }//AIMah


        }





        private void AttachClickHandler(string path)
        {
            //SaveCache();

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
                        return;
                    } }//this stupid ass zuma trophy
                //MelonLogger.Warning("Background not found");
                return;
            }

            var targetTransform = boardGl.transform.Find("TrophyPrefab");
            if (targetTransform == null)
            {
                targetTransform = boardGl.transform.Find("TrophyPrefab(Clone)");
                if (targetTransform == null)
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
                if (canvas == null)
                {
                    MelonLogger.Warning("Somethings gone horribly wrong ERR1");
                    return;
                }

                var leveltext = canvas.transform.Find("CanvasUp/LevelName2");
                if (leveltext == null)
                {
                    leveltext = canvas.transform.Find("CanvasUp/LevelName3");

                    if (leveltext == null)
                    {
                        MelonLogger.Warning("Somethings gone horribly wrong ERR2");
                        return; }
                }
                    //case "Zum-nut!":
                    //checkedArray[96] = true;
                    //session.Locations.CompleteLocationChecks(96);
                    //session.Locations.CompleteLocationChecks(396);
                    //break;
                    var textComponent = leveltext.GetComponent<TextMeshProUGUI>();
                if (textComponent == null)//dumb error checking thats probably good practice
                {
                    MelonLogger.Warning("Somethings gone horribly wrong ERR3");
                    return;
                }
                string levelName = textComponent.text;

                MelonLogger.Msg("Finding level with name: "+levelName);
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

                    case "Snowy Night: Memory":
                        checkedArray[78] = true;
                        session.Locations.CompleteLocationChecks(78);
                        session.Locations.CompleteLocationChecks(378);
                        break;



                    case "Fusion Challenge: Explode-o-shooter":
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
                    case "Fusion Challenge: Jicamagic":
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
                    case "Z-Day":
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
                    case "Wall-nut Billiards "://killing myself
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
                    case "Squash Showdown! 2":
                        checkedArray[113] = true;
                        session.Locations.CompleteLocationChecks(113);
                        session.Locations.CompleteLocationChecks(413);
                        break;
                    case "Zombies vs Zombies":
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
                    case "True Art is an  Explosion! 2":
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
                    case "打砖块2"://graveout 2 duplicate
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
                    case "I, Zombie (Mini-game Version)":
                        checkedArray[129] = true;
                        session.Locations.CompleteLocationChecks(129);
                        session.Locations.CompleteLocationChecks(429);
                        break;
                    case "Archduke's Revenge":
                        checkedArray[130] = true;
                        session.Locations.CompleteLocationChecks(130);
                        session.Locations.CompleteLocationChecks(430);
                        break;
                    case "Beghouled 2: Botany Crush":
                        checkedArray[131] = true;
                        session.Locations.CompleteLocationChecks(131);
                        session.Locations.CompleteLocationChecks(431);
                        break;
                    case "Nut-o-mite":
                        checkedArray[132] = true;
                        session.Locations.CompleteLocationChecks(132);
                        session.Locations.CompleteLocationChecks(432);
                        break;


                    case "Lava Land":
                        checkedArray[133] = true;
                        session.Locations.CompleteLocationChecks(133);
                        session.Locations.CompleteLocationChecks(433);
                        break;
                    //location 134 is checked for in default case
                    case "Zombie Sorter":
                        checkedArray[135] = true;
                        session.Locations.CompleteLocationChecks(135);
                        session.Locations.CompleteLocationChecks(435);
                        break;
                    case "Explode-O-su!":
                        checkedArray[136] = true;
                        session.Locations.CompleteLocationChecks(136);
                        session.Locations.CompleteLocationChecks(436);
                        break;
                    case "I, Zomboss?":
                        checkedArray[137] = true;
                        session.Locations.CompleteLocationChecks(137);
                        session.Locations.CompleteLocationChecks(437);
                        break;
                    
                    case "Fusion Challenge: Wheelbarrow":
                        checkedArray[139] = true;
                        session.Locations.CompleteLocationChecks(139);
                        session.Locations.CompleteLocationChecks(439);
                        break;

                        


                    case "Vasebreaker":
                        checkedArray[200] = true;
                        session.Locations.CompleteLocationChecks(200);
                        session.Locations.CompleteLocationChecks(500);
                        break;
                    case "Vasebreaker 2":
                        checkedArray[201] = true;
                        session.Locations.CompleteLocationChecks(201);
                        session.Locations.CompleteLocationChecks(501);
                        break;
                    case "Chain Reaction":
                        checkedArray[202] = true;
                        session.Locations.CompleteLocationChecks(202);
                        session.Locations.CompleteLocationChecks(502);
                        break;
                    case "The Vase Less Travelled by":
                        checkedArray[203] = true;
                        session.Locations.CompleteLocationChecks(203);
                        session.Locations.CompleteLocationChecks(503);
                        break;
                    case "De-vase-tating!":
                        checkedArray[204] = true;
                        session.Locations.CompleteLocationChecks(204);
                        session.Locations.CompleteLocationChecks(504);
                        break;
                    case "Potluck":
                        checkedArray[205] = true;
                        session.Locations.CompleteLocationChecks(205);
                        session.Locations.CompleteLocationChecks(505);
                        break;



                    case "Survival: Day | Round 5":
                        checkedArray[280] = true;
                        session.Locations.CompleteLocationChecks(280);
                        session.Locations.CompleteLocationChecks(580);
                        break;
                    case "Survival: Day(Hard) | Round 5":
                        checkedArray[281] = true;
                        session.Locations.CompleteLocationChecks(281);
                        session.Locations.CompleteLocationChecks(581);
                        break;
                    case "Survival: Night | Round 5":
                        checkedArray[282] = true;
                        session.Locations.CompleteLocationChecks(282);
                        session.Locations.CompleteLocationChecks(582);
                        break;
                    case "Survival: Night(Hard) | Round 5":
                        checkedArray[283] = true;
                        session.Locations.CompleteLocationChecks(283);
                        session.Locations.CompleteLocationChecks(583);
                        break;
                    case "Survival: Pool | Round 5":
                        checkedArray[284] = true;
                        session.Locations.CompleteLocationChecks(284);
                        session.Locations.CompleteLocationChecks(584);
                        break;
                    case "Survival: Pool(Hard) | Round 5":
                        checkedArray[285] = true;
                        session.Locations.CompleteLocationChecks(285);
                        session.Locations.CompleteLocationChecks(585);
                        break;
                    case "Survival: Fog | Round 5":
                        checkedArray[286] = true;
                        session.Locations.CompleteLocationChecks(286);
                        session.Locations.CompleteLocationChecks(586);
                        break;
                    case "Survival: Fog(Hard) | Round 5":
                        checkedArray[287] = true;
                        session.Locations.CompleteLocationChecks(287);
                        session.Locations.CompleteLocationChecks(587);
                        break;
                    case "Survival: Roof | Round 5":
                        checkedArray[288] = true;
                        session.Locations.CompleteLocationChecks(288);
                        session.Locations.CompleteLocationChecks(588);
                        break;
                    case "Survival: Roof(Hard) | Round 5":
                        checkedArray[289] = true;
                        session.Locations.CompleteLocationChecks(289);
                        session.Locations.CompleteLocationChecks(589);
                        break;



                    case "Odyssey Adventure: Gatling Doom":
                        checkedArray[601] = true;
                        session.Locations.CompleteLocationChecks(601);
                        session.Locations.CompleteLocationChecks(901);
                        break;
                    case "Odyssey Adventure: Gatling Icicle-shroom":
                        checkedArray[602] = true;
                        session.Locations.CompleteLocationChecks(602);
                        session.Locations.CompleteLocationChecks(902);
                        break;
                    case "Odyssey Adventure: Big Bang Cherry":
                        checkedArray[603] = true;
                        session.Locations.CompleteLocationChecks(603);
                        session.Locations.CompleteLocationChecks(903);
                        break;
                    case "Odyssey Adventure: Twin Solar-nut":
                        checkedArray[604] = true;
                        session.Locations.CompleteLocationChecks(604);
                        session.Locations.CompleteLocationChecks(904);
                        break;
                    case "Odyssey Adventure: Aristocattail":
                        checkedArray[605] = true;
                        session.Locations.CompleteLocationChecks(605);
                        session.Locations.CompleteLocationChecks(905);
                        break;
                    case "Odyssey Adventure: Empress-shroom":
                        checkedArray[606] = true;
                        session.Locations.CompleteLocationChecks(606);
                        session.Locations.CompleteLocationChecks(906);
                        break;
                    case "Odyssey Adventure: Cornveyor Weed":
                        checkedArray[607] = true;
                        session.Locations.CompleteLocationChecks(607);
                        session.Locations.CompleteLocationChecks(907);
                        break;
                    case "Odyssey Adventure: Phoenix Threepeater":
                        checkedArray[608] = true;
                        session.Locations.CompleteLocationChecks(608);
                        session.Locations.CompleteLocationChecks(908);
                        break;

                    case "Odyssey Adventure: Obsidian Spikerock":
                        checkedArray[609] = true;
                        session.Locations.CompleteLocationChecks(609);
                        session.Locations.CompleteLocationChecks(909);
                        break;
                    case "Odyssey Adventure: Omni Pumpkin":
                        checkedArray[610] = true;
                        session.Locations.CompleteLocationChecks(610);
                        session.Locations.CompleteLocationChecks(910);
                        break;
                    case "Odyssey Adventure: Pharos Umbrella":
                        checkedArray[611] = true;
                        session.Locations.CompleteLocationChecks(611);
                        session.Locations.CompleteLocationChecks(911);
                        break;
                    case "Odyssey Adventure: Buck-shroom Squad":
                        checkedArray[612] = true;
                        session.Locations.CompleteLocationChecks(612);
                        session.Locations.CompleteLocationChecks(912);
                        break;
                    case "Odyssey Adventure: Calamity-shroom":
                        checkedArray[613] = true;
                        session.Locations.CompleteLocationChecks(613);
                        session.Locations.CompleteLocationChecks(913);
                        break;
                    case "Odyssey Adventure: Tesla Magnet":
                        checkedArray[614] = true;
                        session.Locations.CompleteLocationChecks(614);
                        session.Locations.CompleteLocationChecks(914);
                        break;
                    case "Odyssey Adventure: Radiant Pot":
                        checkedArray[615] = true;
                        session.Locations.CompleteLocationChecks(615);
                        session.Locations.CompleteLocationChecks(915);
                        break;
                    case "Odyssey Adventure: Midas Umbrella":
                        checkedArray[616] = true;
                        session.Locations.CompleteLocationChecks(616);
                        session.Locations.CompleteLocationChecks(916);
                        break;
                    case "Odyssey Adventure: Tycoonwood":
                        checkedArray[617] = true;
                        session.Locations.CompleteLocationChecks(617);
                        session.Locations.CompleteLocationChecks(917);
                        break;
                    case "Odyssey Adventure: Grim Arbiter":
                        checkedArray[618] = true;
                        session.Locations.CompleteLocationChecks(618);
                        session.Locations.CompleteLocationChecks(918);
                        break;
                    case "Odyssey Adventure: Dragonbreath Torcher":
                        checkedArray[619] = true;
                        session.Locations.CompleteLocationChecks(619);
                        session.Locations.CompleteLocationChecks(919);
                        if (goalType == 2)
                        { session.Socket.SendPacket(new StatusUpdatePacket() { Status = ArchipelagoClientState.ClientGoal }); }
                        break;


                    case "Odyssey: Last Stand | Round 21":
                        checkedArray[650] = true;
                        session.Locations.CompleteLocationChecks(650);
                        session.Locations.CompleteLocationChecks(950);
                        break;
                    case "The Gods 1: Trial of Ascension":
                        checkedArray[651] = true;
                        session.Locations.CompleteLocationChecks(651);
                        session.Locations.CompleteLocationChecks(951);
                        break;
                    case "The Gods 2: Trial of Rebirth":
                        checkedArray[652] = true;
                        session.Locations.CompleteLocationChecks(652);
                        session.Locations.CompleteLocationChecks(952);
                        break;
                    case "The Gods 3: Trial of Endurance":
                        checkedArray[653] = true;
                        session.Locations.CompleteLocationChecks(653);
                        session.Locations.CompleteLocationChecks(953);
                        break;
                    case "The Gods 4: Trial of Valor":
                        checkedArray[654] = true;
                        session.Locations.CompleteLocationChecks(654);
                        session.Locations.CompleteLocationChecks(954);
                        break;
                    case "Solar-nut Bowling":
                        checkedArray[655] = true;
                        session.Locations.CompleteLocationChecks(655);
                        session.Locations.CompleteLocationChecks(955);
                        break;
                    case "The Battle Zombies":
                        checkedArray[656] = true;
                        session.Locations.CompleteLocationChecks(656);
                        session.Locations.CompleteLocationChecks(956);
                        break;
                    case "Flag: The Gods 1: Trial of Ascension":
                        checkedArray[657] = true;
                        session.Locations.CompleteLocationChecks(657);
                        session.Locations.CompleteLocationChecks(957);
                        break;
                    case "Ten-Flag: The Gods 2: Trial of Rebirth":
                        checkedArray[658] = true;
                        session.Locations.CompleteLocationChecks(658);
                        session.Locations.CompleteLocationChecks(958);
                        break;
                    case "Ten-Flag: The Gods 3: Trial of Endurance":
                        checkedArray[659] = true;
                        session.Locations.CompleteLocationChecks(659);
                        session.Locations.CompleteLocationChecks(959);
                        break;
                    case "Solitary Spear":
                        checkedArray[660] = true;
                        session.Locations.CompleteLocationChecks(660);
                        session.Locations.CompleteLocationChecks(960);
                        break;
                    case "The Gods 5: Trial of Radiance":
                        checkedArray[661] = true;
                        session.Locations.CompleteLocationChecks(661);
                        session.Locations.CompleteLocationChecks(961);
                        break;
                    case "Whack-a-Zombie 2":
                        checkedArray[662] = true;
                        session.Locations.CompleteLocationChecks(662);
                        session.Locations.CompleteLocationChecks(962);
                        break;
                    case "Odyssey: Rush Mode | Round 21":
                        checkedArray[663] = true;
                        session.Locations.CompleteLocationChecks(663);
                        session.Locations.CompleteLocationChecks(963);
                        break;
                    case "The Gods 7: Trial Resurgence":
                        checkedArray[664] = true;
                        session.Locations.CompleteLocationChecks(664);
                        session.Locations.CompleteLocationChecks(964);
                        break;
                    case "Gacha Battle":
                        checkedArray[665] = true;
                        session.Locations.CompleteLocationChecks(665);
                        session.Locations.CompleteLocationChecks(965);
                        break;
                    case "Odyssey: Randomized | Round 21":
                        checkedArray[666] = true;
                        session.Locations.CompleteLocationChecks(666);
                        session.Locations.CompleteLocationChecks(966);
                        break;
                    case "Super Odyssey Gacha":
                        checkedArray[667] = true;
                        session.Locations.CompleteLocationChecks(667);
                        session.Locations.CompleteLocationChecks(967);
                        break;
                    case "Ten-Flag: Odyssey Gacha - 12 Lane":
                        checkedArray[668] = true;
                        session.Locations.CompleteLocationChecks(668);
                        session.Locations.CompleteLocationChecks(968);
                        break;
                    case "Ten-Flag: Equivalent Exchange 2":
                        checkedArray[669] = true;
                        session.Locations.CompleteLocationChecks(669);
                        session.Locations.CompleteLocationChecks(969);
                        break;
                    case "Ten-Flag: Super Conveyor Belt":
                        checkedArray[670] = true;
                        session.Locations.CompleteLocationChecks(670);
                        session.Locations.CompleteLocationChecks(970);
                        break;
                    case "Barley Battle!":
                        checkedArray[671] = true;
                        session.Locations.CompleteLocationChecks(671);
                        session.Locations.CompleteLocationChecks(971);
                        break;
                    case "Odyssey Gacha: Imitater":
                        checkedArray[672] = true;
                        session.Locations.CompleteLocationChecks(672);
                        session.Locations.CompleteLocationChecks(972);
                        break;
                    case "Odyssey Gacha: Unleashed":
                        checkedArray[673] = true;
                        session.Locations.CompleteLocationChecks(673);
                        session.Locations.CompleteLocationChecks(973);
                        break;
                    case "Barley Battle 2! Fission":
                        checkedArray[674] = true;
                        session.Locations.CompleteLocationChecks(674);
                        session.Locations.CompleteLocationChecks(974);
                        break;
                    case "Odyssey Gacha: Fusion Mode":
                        checkedArray[675] = true;
                        session.Locations.CompleteLocationChecks(675);
                        session.Locations.CompleteLocationChecks(975);
                        break;
                    case "Odyssey Gacha: Fusion Mode 2":
                        checkedArray[676] = true;
                        session.Locations.CompleteLocationChecks(676);
                        session.Locations.CompleteLocationChecks(976);
                        break;
                    case "Odyssey Gacha: Upgrade":
                        checkedArray[677] = true;
                        session.Locations.CompleteLocationChecks(677);
                        session.Locations.CompleteLocationChecks(977);
                        break;
                    case "Odyssey Gacha: Blessings and Curses":
                        checkedArray[678] = true;
                        session.Locations.CompleteLocationChecks(678);
                        session.Locations.CompleteLocationChecks(978);
                        break;
                    case "The Gods 6: Trial of Precision":
                        checkedArray[679] = true;
                        session.Locations.CompleteLocationChecks(679);
                        session.Locations.CompleteLocationChecks(979);
                        break;
                    case "Zombies vs Zombies: Odyssey":
                        checkedArray[680] = true;
                        session.Locations.CompleteLocationChecks(680);
                        session.Locations.CompleteLocationChecks(980);
                        break;
                    case "Odyssey Gacha: Gashapon":
                        checkedArray[681] = true;
                        session.Locations.CompleteLocationChecks(681);
                        session.Locations.CompleteLocationChecks(981);
                        break;
                    case "Odyssey Gacha: Treasure Hunt":
                        checkedArray[682] = true;
                        session.Locations.CompleteLocationChecks(682);
                        session.Locations.CompleteLocationChecks(982);
                        break;
                    case "Odyssey Gacha: Diamond Imitater":
                        checkedArray[683] = true;
                        session.Locations.CompleteLocationChecks(683);
                        session.Locations.CompleteLocationChecks(983);
                        break;
                    case "Odyssey Gacha: Rerolled":
                        checkedArray[684] = true;
                        session.Locations.CompleteLocationChecks(684);
                        session.Locations.CompleteLocationChecks(984);
                        break;
                    case "Purgatory Gacha: Chibi vs. Goliath":
                        checkedArray[685] = true;
                        session.Locations.CompleteLocationChecks(685);
                        session.Locations.CompleteLocationChecks(985);
                        break;
                    case "The Gods 8: Trial of Acclimation":
                        checkedArray[686] = true;
                        session.Locations.CompleteLocationChecks(686);
                        session.Locations.CompleteLocationChecks(986);
                        break;
                    case "Fusion Gacha: Chaos":
                        checkedArray[687] = true;
                        session.Locations.CompleteLocationChecks(687);
                        session.Locations.CompleteLocationChecks(987);
                        break;
                    case "Advanced Gacha: Chaos":
                        checkedArray[688] = true;
                        session.Locations.CompleteLocationChecks(688);
                        session.Locations.CompleteLocationChecks(988);
                        break;
                    case "Purgatory Gacha: Chaos":
                        checkedArray[689] = true;
                        session.Locations.CompleteLocationChecks(689);
                        session.Locations.CompleteLocationChecks(989);
                        break;
                    case "Stats VS Mechanics 1":
                        checkedArray[690] = true;
                        session.Locations.CompleteLocationChecks(690);
                        session.Locations.CompleteLocationChecks(990);
                        break;
                    case "Odyssey Gacha: Gacha Box":
                        checkedArray[691] = true;
                        session.Locations.CompleteLocationChecks(691);
                        session.Locations.CompleteLocationChecks(991);
                        break;

                    case "Dr. Zomboss' Revenge":
                        checkedArray[99] = true;
                        session.Locations.CompleteLocationChecks(99);
                        session.Locations.CompleteLocationChecks(399);
                        if (goalType == 0)
                        { session.Socket.SendPacket(new StatusUpdatePacket() { Status = ArchipelagoClientState.ClientGoal }); }
                        break;





                    default:
                        if ( levelName.StartsWith("Total num. of Mines"))//nutsweeper
                        {
                            checkedArray[134] = true;
                            session.Locations.CompleteLocationChecks(134);
                            session.Locations.CompleteLocationChecks(434);
                            break;
                        }
                        if (levelName.StartsWith("时间陷阱："))//time attack
                        {
                            checkedArray[138] = true;
                            session.Locations.CompleteLocationChecks(138);
                            session.Locations.CompleteLocationChecks(438);
                            break;
                        }
                        

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





                        if ((string)tag == "DeathLink")
                        {
                            if (boardGl == null)
                            {
                                boardGl = GameObject.Find("Board");
                            }
                            if (boardGl == null)
                            {
                                continue;
                            }
                            MelonLogger.Msg("received deathlink");
                            GameObject newZombie = GameObject.Instantiate(normalZombieObject, new Vector3((float)-8.0, (float)0, 0), Quaternion.identity);
                            var zombComp = newZombie.GetComponent(Il2CppType.Of<BasicZombie>());

                            var type = zombComp.GetIl2CppType();
                            var field = type.GetField("board");


                            var boardComp = boardGl.GetComponent(Il2CppType.Of<Board>());
                            var typeB = boardComp.GetIl2CppType();
                            var fieldB = type.GetField("Board");

                            field.SetValue(zombComp, fieldB);

                            lostThisFrame = true;

                            newZombie.transform.SetParent(boardGl.transform);//todo error checking
                                    MelonLogger.Msg("got through deathlink code");

                            //BoxType Grass
                            //Column


                        }

                            if ((string)tag == "RingLink")

                        {

                            try
                            {
                                foreach (var slotNum in jObject["slots"])
                                { //
                                    if ((int)slotNum == session.Players.ActivePlayer.Slot)
                                    {
                                        return;
                                    }
                                }
                            }
                            catch (System.NullReferenceException)
                            { }//MelonLogger.Msg("Ringlink packet recieved from SA1 or SA2"); }
                            catch (System.Exception)
                            { }//MelonLogger.Msg("Ringlink packet recieved from SA1 or SA2 (1)"); }



                                var board = GameObject.Find("Board");
                                if (board == null)
                                {
                                    continue;
                                }
                                var boardComp = board.GetComponent(Il2CppType.Of<Board>());

                                var type = boardComp.GetIl2CppType();
                                var field = type.GetField("theSun");
                                Il2CppSystem.Object rawValue = field.GetValue(boardComp);
                                int intValue = Il2CppSystem.Convert.ToInt32(rawValue)/10;
                            int oldValue = Il2CppSystem.Convert.ToInt32(rawValue);


                                var data = jObject["data"];
                                int amount = data.Value<int>("amount");

                                prevSun = intValue + amount;
                                if (!negativeSun)//lazy ahh programming
                                {
                                    if (prevSun < 0)
                                    {
                                        prevSun = 0;
                                        field.SetValue(boardComp, 0);
                                    }
                                    else
                                    {
                                        field.SetValue(boardComp, oldValue + (amount * 10));
                                    }
                                }
                                else { field.SetValue(boardComp, oldValue + (amount * 10)); }





                            }

                    }
                    break;

                default:
                    break;
            }
        }
        private void CheckForItemsStartup()
        {
            while (session.Items.Any())
            {
                var networkItem = session.Items.DequeueItem();

                //if (networkItem.ItemId == null)
                //{ continue; }
                if (networkItem.ItemId < maxItems)
                { unlockedArray[networkItem.ItemId] = true; }
                if (networkItem.ItemId == 89)//progressive odyssey adventure
                { numOdysseyLevels++; } 
                if (networkItem.ItemId == 201)
                {
                    seedSlots += 1;
                }
                if (networkItem.ItemId == 206)
                {
                    trophyCount += 1;
                    if (trophyCount >= trophiesForGoal && goalType == 3)
                    {
                        session.Socket.SendPacket(new StatusUpdatePacket() { Status = ArchipelagoClientState.ClientGoal });
                    }
                }
                if (networkItem.ItemId == 208)//50 starting sun
                {
                    extraStartSun += 50;
                }

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

                if (networkItem.ItemId == 89)//progressive odyssey adventure
                { numOdysseyLevels++; }


                if (canvasupGl == null)
                { canvasupGl = GameObject.Find("CanvasUp"); }
                if (canvasGl != null)
                {


                    if (networkItem.ItemId < 75 | (networkItem.ItemId > 80 && networkItem.ItemId < 200) | networkItem.ItemId == 201 | networkItem.ItemId > 248)

                    {
                        var tutor = canvasupGl.transform.Find("Tutor(Clone)");

                        var textMan = tutor.GetComponent<Il2CppCore.InGameText>();

                        textMan.DebugWarning("Received " + ItemIdToString(networkItem.ItemId));

                    }
                    //networkItem.Player.Name


                }




                if (networkItem.ItemId == 208)//50 starting sun
                {
                    extraStartSun += 50;
                }


                    if (networkItem.ItemId == 200)//bonus sun
                {
                    if (boardGl == null)
                    {
                        boardGl = GameObject.Find("Board");
                    }
                    if (boardGl == null)
                    {
                        return;
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            GameObject newSun = GameObject.Instantiate(sunPrefabObject, new Vector3((float)i - 3, (float)j - 1, 0), Quaternion.identity);
                            newSun.transform.SetParent(boardGl.transform);
                            CoinSun component = newSun.AddComponent<CoinSun>();
                        }
                    }



                    continue;

                    GameObject newPlant = GameObject.Instantiate(droppedCardPrefabObject, new Vector3((float)0.5231, (float)-0.938, 0), Quaternion.identity);
                    newPlant.transform.SetParent(boardGl.transform);
                }

                if (networkItem.ItemId == 201)
                {
                    seedSlots += 1;
                }
                if (networkItem.ItemId == 202)//zen garden plant
                {
                    GameObject.Instantiate(gardenPlantPrefabObject, new Vector3((float)0.5231, (float)-0.838, 0), Quaternion.identity);
                }

                if (networkItem.ItemId == 203)//diamond
                {
                    if (boardGl == null)
                    {
                        boardGl = GameObject.Find("Board");
                    }
                    if (boardGl == null)
                    {
                        return;
                    }
                    GameObject newGem = GameObject.Instantiate(diamondPrefabObject, new Vector3((float)0.5231, (float)-0.838, 0), Quaternion.identity);
                    newGem.transform.SetParent(boardGl.transform);
                    CoinMoney component = newGem.AddComponent<CoinMoney>();
                    var type = component.GetIl2CppType();
                    var field = type.GetField("moneyPrice");
                    field.SetValue(component, 1000);
                }

                if (networkItem.ItemId == 204)//free sunflowers
                {
                    if (boardGl == null)
                    {
                        boardGl = GameObject.Find("Board");
                    }
                    if (boardGl == null)
                    {
                        return;
                    }
                    GameObject uiParent = GameObject.Find("CanvasUp/UIParent");

                    if (uiParent == null)
                    {
                        return;
                    }



                    var boardComp = boardGl.GetComponent<CreateItem>();


                    for (int i = 0; i < 3; i++)
                    {
                        GameObject newCard = boardComp.SetCoin(0, 0, 9, 0);
                        newCard.transform.SetParent(uiParent.transform);


                        CardUI component = newCard.GetComponent<DroppedCard>();
                        var type = component.GetIl2CppType();
                        var field = type.GetField("thePlantType");
                        field.SetValue(component, 1);


                        newCard.transform.position = new Vector3(-1 + i, 3, 100);
                        newCard.transform.localScale = new Vector3(1, 1, 1);
                    }


                }

                if (networkItem.ItemId == 209)//ice-shroom assist
                {
                    if (boardGl == null)
                    {
                        boardGl = GameObject.Find("Board");
                    }
                    if (boardGl == null)
                    {
                        return;
                    }


                    Board board = boardGl.GetComponent<Board>();

                    Il2CppSystem.Collections.Generic.List<Zombie> zlist = board.zombieArray;

                    foreach (Zombie zombie in zlist)
                    {

                        zombie.SetFreeze(4);


                    }



                }



                    if (networkItem.ItemId == 205)//star meteor
                {
                    GameObject.Instantiate(starMeteorPrefabObject, new Vector3((float)-8, (float)5, 0), Quaternion.identity);
                }
                if (networkItem.ItemId == 206)//trophy
                {
                    trophyCount += 1;
                    if (trophyCount >= trophiesForGoal && goalType == 3)
                    {
                        session.Socket.SendPacket(new StatusUpdatePacket() { Status = ArchipelagoClientState.ClientGoal });

                    }
                }
                if (networkItem.ItemId == 207)//refreshed cooldowns
                {
                    if (boardGl == null)
                    {
                        boardGl = GameObject.Find("Board");
                    }
                    if (boardGl == null)
                    {
                        return;
                    }
                    if (canvasupGl == null)
                    { canvasupGl = GameObject.Find("CanvasUp"); }
                    if (canvasGl != null)
                    {
                        var tutor = canvasupGl.transform.Find("Tutor(Clone)");
                        var textMan = tutor.GetComponent<Il2CppCore.InGameText>();
                        textMan.DebugWarning("No planting cooldown for 10 seconds");
                    }
                    Board component = boardGl.GetComponent<Board>();
                    var type = component.GetIl2CppType();
                    var field = type.GetField("freeCD");
                    field.SetValue(component, true);




                    cdResetTime = 10.0f;
                    resetCD = true;


                }




                if (networkItem.ItemId == 220)//10x speed trap
                {
                    Time.timeScale = 10.0f;
                }
                if (networkItem.ItemId == 221)//fuck you
                {

                    if (boardGl == null)
                    {
                        boardGl = GameObject.Find("Board");
                    }
                    if (boardGl == null)
                    {
                        return;
                    }
                    int childCount = boardGl.transform.childCount;
                    for (int i = 1; i < childCount; i++)
                    {

                        var child = boardGl.transform.GetChild(i);
                        GameObject.Destroy(child.gameObject);

                    }//AIMah
                }
                if (networkItem.ItemId == 222)//rough economy
                {
                    if (canvasGl == null)
                    { canvasGl = GameObject.Find("Canvas"); }
                    if (canvasGl == null)
                    { return; }

                    for (int i = 0; i < 14; i++)
                    {
                        var seedPacket = canvasGl.transform.Find(slotPath + "seed" + i.ToString() + "/Packet");
                        if (seedPacket == null)
                        {
                            seedPacket = canvasGl.transform.Find(slotPath + "seed" + i.ToString() + "/Packet ");//stupid bullshit

                            if (seedPacket == null)
                            {

                                seedPacket = canvasGl.transform.Find(slotPath + "seed" + i.ToString() + "/Packet (Clone)");//stupid bullshit

                                if (seedPacket == null)
                                {


                                    MelonLogger.Msg("couldnt find seed packet");
                                    MelonLogger.Msg("if theres another space in the name im going to kill myself");
                                    continue;
                                }
                            }
                        }
                        GameObject obj = seedPacket.gameObject;

                        var temp = obj.GetComponent(Il2CppType.Of<CardUI>());
                        if (temp != null)
                        {
                            var type = temp.GetIl2CppType();
                            var field = type.GetField("theSeedCost");
                            Il2CppSystem.Object rawValue = field.GetValue(temp);
                            int intValue = Il2CppSystem.Convert.ToInt32(rawValue);
                            field.SetValue(temp, intValue * 2);


                        }
                        else
                        { MelonLogger.Msg("couldnt find card component"); }

                    }
                }
                if (networkItem.ItemId == 223)//the fog is coming
                {
                    if (boardGl == null)
                    {
                        boardGl = GameObject.Find("Board");
                    }
                    if (boardGl == null)
                    {
                        return;
                    }

                    if (canvasupGl == null)
                    { canvasupGl = GameObject.Find("CanvasUp"); }
                    if (canvasGl != null)
                    {
                        var tutor = canvasupGl.transform.Find("Tutor(Clone)");
                        var textMan = tutor.GetComponent<Il2CppCore.InGameText>();
                        textMan.DebugWarning("The fog is coming...");
                    }

                    var fog = GameObject.Instantiate(fogPrefabObject, new Vector3(2.0f, 2.87f, 0), Quaternion.identity);
                    fog.transform.SetParent(boardGl.transform.GetChild(0));

                    fog.transform.position = new Vector3(13, (float)2.87, 100);
                    var fogmgr = fog.GetComponent<FogMgr>();




                    var boardComp = boardGl.GetComponent(Il2CppType.Of<Board>());

                    var type = boardComp.GetIl2CppType();
                    var field = type.GetField("fog");
                    field.SetValue(boardComp, fog);
                    fogmgr.FadeFog(new Vector3(2, (float)2.87, 0));
                }

                if (networkItem.ItemId == 224)//zombie meteor
                {
                    var evilStar = GameObject.Instantiate(zombieStarObject, new Vector3((float)18, (float)5, 0), Quaternion.identity);


                    var boardComp = evilStar.GetComponent(Il2CppType.Of<BigStar>());

                    var type = boardComp.GetIl2CppType();
                    var field = type.GetField("zombie");
                    field.SetValue(boardComp, true);

                }
                if (networkItem.ItemId == 226)//grave danger
                {
                    if (boardGl == null)
                    {
                        boardGl = GameObject.Find("Board");
                    }
                    if (boardGl == null)
                    {
                        return;
                    }

                    var boardComp = boardGl.GetComponent<Board>();
                    boardComp.SetGrave();

                }

                if (networkItem.ItemId == 227)//mystery box
                {
                    if (boardGl == null)
                    {
                        boardGl = GameObject.Find("Board");
                    }
                    if (boardGl == null)
                    {
                        return;
                    }

                    var boardComp = boardGl.GetComponent<CreateZombie>();


                    for (int i = 0; i < 5; i++)
                    {

                        if (UnityEngine.Random.Range(0, 5) == 0) { boardComp.SetZombie(i, ZombieType.RandomPlusZombie, 9.9f, false); }
                        else { boardComp.SetZombie(i, ZombieType.RandomZombie,9.9f,false); }


                    }


                }

                if (networkItem.ItemId == 228)//Extra flag
                {
                    if (boardGl == null)
                    {
                        boardGl = GameObject.Find("Board");
                    }
                    if (boardGl == null)
                    {
                        return;
                    }
                    if (canvasupGl == null)
                    { canvasupGl = GameObject.Find("CanvasUp"); }
                    if (canvasGl != null)
                    {
                        var tutor = canvasupGl.transform.Find("Tutor(Clone)");
                        var textMan = tutor.GetComponent<Il2CppCore.InGameText>();
                        textMan.DebugWarning("Extra Flag");
                    }
                    var boardComp = boardGl.GetComponent(Il2CppType.Of<Board>());

                    var type = boardComp.GetIl2CppType();
                    var field = type.GetField("theWave");
                    Il2CppSystem.Object rawValue = field.GetValue(boardComp);
                    int intValue = Il2CppSystem.Convert.ToInt32(rawValue);
                    field.SetValue(boardComp, intValue - 10);

                }

                if (networkItem.ItemId == 229)//literature trap
                {

                    if (canvasupGl == null)
                    { canvasupGl = GameObject.Find("CanvasUp"); }
                    if (canvasGl != null)
                    {
                        var tutor = canvasupGl.transform.Find("Tutor(Clone)");
                        var textMan = tutor.GetComponent<Il2CppCore.InGameText>();
                        var rand = UnityEngine.Random.Range(0, 10);
                        switch (rand)
                        {
                            case 0:
                                textMan.DebugWarning("Beware!   There could be\nan invis. wall   around here.\nTry to   go around it.");
                                break;
                            case 1 or 2 or 3://receive a random plant thats not in the game
                                var temp =  UnityEngine.Random.Range(0, 13);
                                string plantstring = "";
                                switch (temp)
                                {
                                    case 1: plantstring = "Pokra"; break;
                                    case 2: plantstring = "Beeshooter"; break;
                                    case 3: plantstring = "Mildew Flower"; break;
                                    case 4: plantstring = "Tricarrotops"; break;
                                    case 5: plantstring = "Garlic Angel"; break;
                                    case 6: plantstring = "Banana Launcher"; break;
                                    case 7: plantstring = "Wise Mystical Oak"; break;
                                    case 8: plantstring = "Ohm-ion"; break;
                                    case 9: plantstring = "Pomegrenade"; break;
                                    case 10: plantstring = "Brawlccoli"; break;
                                    case 11: plantstring = "Progressive Shop"; break;
                                    case 12: plantstring = "Amp-nion (but like the one from 3.7)"; break;




                                    default: plantstring = "Twin Sunflower with slightly larger petals"; break;
                                }
                                textMan.DebugWarning("Received "+plantstring);
                                break;

                            case 7:
                                textMan.DebugWarning("i love how you are still on about grass sanity\r\nits literally not that bad \r\nalso grass sanity * 20 = 12k\r\nthats less than powerwash max sanity");
                                break;
                            case 8:
                                textMan.DebugWarning("Do not post links to ROM files or other copyrighted content.\r\nThe @Moderator team has been alerted to this incident.\r\nThe potentially problematic file(s) in question were:\r\n- z.json");
                                break;
                            case 4:
                                textMan.DebugWarning("One of your nuts were busted!");
                                break;
                            case 5:
                                textMan.DebugWarning("You don't have a tall enough tree of wisdom for that");
                                break;
                            case 6:
                                textMan.DebugWarning("this guy fucking sucks at plants vs zombies fusion. in this video we can see a player so atrociously bad that he almost gets his vital sun production eaten multiple times a second.\ndespite the obviously zealous zombies he sucks so fucking bad at the game that he doesnt know youre supposed to protect your primary source of sun.\nit's only by sheer luck that he just barely avoids sending our chloraphilled friends 12 feet under.\nsubscribe if you think this total embarrassment of a gamer should never be allowed to touch a fangame again");
                                break;

                            default:
                                textMan.DebugWarning("No! Vanilla! Changes!!!");
                                break;

                        }
                    }

                }
            }
        }

        private void HideSpecifiedTargetChild(GameObject obj, int childnum)
        {
            if (obj == null)
            {
                return;
            }
            int childCount = obj.transform.childCount;

                var child = obj.transform.GetChild(childnum);
                child.gameObject.SetActive(false);

        }
        private void HideSpecifiedTargetTrophy(GameObject obj, int childnum)
        {
            if (obj == null)
            {
                return;
            }
            int childCount = obj.transform.childCount;

            var child = obj.transform.GetChild(childnum);
            child = child.transform.GetChild(1);
            child = child.transform.GetChild(1);

            child.gameObject.SetActive(false);

        }

        private void HideSpecifiedNewSeedSlot(GameObject obj, string target, int childnum)
        {


            if (obj == null)
            {
                //LoggerInstance.Warning("CanvasUp not found");
                return;
            }
            var inGameUI = obj.transform.Find(target);
            if (inGameUI == null)
            {
                return;
            }
            var child = inGameUI.transform.GetChild(0);
            child = child.transform.GetChild(childnum);

            int childCount = child.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child2 = child.GetChild(i);
                child2.gameObject.SetActive(false);
            }

        }
        private void HideSpecifiedNewTrophy(GameObject obj, int childnum)
        {
            if (obj == null)
            {
                return;
            }
            var child = obj.transform.GetChild(childnum);
                var child2 = child.GetChild(4);
                child2.gameObject.SetActive(false);


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

        private void HideTargetChildByID(GameObject canvas, string target,int id)
        {
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
            var child = inGameUI.GetChild(id);
            child.gameObject.SetActive(false);


        }

        private void HideTrophyIconById(GameObject canvas, string target, int id)
        {
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
            var child = inGameUI.GetChild(id);

            var transformChild = child.transform.Find("Window/Trophy");

            transformChild.gameObject.SetActive(false);


        }


        public void CheckForGoalType1()
        {
            if (goalType != 1)
            {
                return;
            }

            if (checkedArray[9] && checkedArray[18] && checkedArray[27] && checkedArray[36] && checkedArray[45])
            {
                if (adventureExtraEnabled)
                {
                     if (checkedArray[54])
                    {session.Socket.SendPacket(new StatusUpdatePacket() { Status = ArchipelagoClientState.ClientGoal }); }
                }
                else
                {
                    session.Socket.SendPacket(new StatusUpdatePacket() { Status = ArchipelagoClientState.ClientGoal });
                }

            }
            return;
        }

        public static string ItemIdToString(long id)
        {
            string str = string.Empty;
            switch (id)
            {

                case 1: return "Peashooter"; case 2: return "Sunflower";
                case 3: return "Cherry Bomb"; case 4: return "Wall-nut";
                case 5: return "Potato Mine"; case 6: return "Chomper";
                case 7: return "Plant Giftbox"; case 8: return "Tall-nut";
                case 9: return "Endoflame"; case 10: return "Puff-shroom";
                case 11: return "Fume-shroom"; case 12: return "Hypno-shroom";
                case 13: return "Scaredy-shroom"; case 14: return "Ice-shroom";
                case 15: return "Doom-shroom"; case 16: return "Zombie Giftbox";
                case 17: return "Gloom-shroom"; case 18: return "Grave Buster";
                case 19: return "Lily Pad"; case 20: return "Squash";
                case 21: return "Threepeater"; case 22: return "Tangle Kelp";
                case 23: return "Jalapeno"; case 24: return "Spikeweed";
                case 25: return "Torchwood"; case 26: return "Spikerock";
                case 27: return "Barley"; case 28: return "Sea-shroom";
                case 29: return "Plantern"; case 30: return "Cactus";
                case 31: return "Blover"; case 32: return "Starfruit";
                case 33: return "Pumpkin"; case 34: return "Magnet-shroom";
                case 35: return "Cattail"; case 36: return "Imitater";
                case 37: return "Cabbage-pult"; case 38: return "Flower Pot";
                case 39: return "Kernel-pult"; case 40: return "Garlic";
                case 41: return "Umbrella Leaf"; case 42: return "Marigold";
                case 43: return "Melon-pult"; case 44: return "Cob Cannon";
                case 45: return "Jicamagic"; case 46: return "Firnace";
                case 47: return "Spruce Sharpshooter"; case 48: return "Saw-me-not";
                case 49: return "Snow Lotus"; case 50: return "Aloe Aqua";
                case 51: return "Bamblock"; case 52: return "Hoarfrost Lichen";
                case 53: return "Spruce Ballista";
                case 54: return "Frozen Giftbox";

                case 55: return "Cattail Girl"; case 56: return "Swordmaster Starfruit";
                case 57: return "Nyan Squash"; case 58: return "Burger Blaster";
                case 59: return "Queen Endoflame"; case 60: return "Coldsnap Bean";
                case 61: return "Amp-nion"; case 62: return "Snipea";
                case 63: return "Chrysanctum"; case 64: return "Icetip Lily";
                case 65: return "Pearmafrost"; case 66: return "Doubleblast Passionfruit";
                case 67: return "Lucky Blover"; case 68: return "Diamond Imitater";
                case 69: return "Bucket"; case 84: return "Football Helmet";
                case 85: return "Hearty Apple"; case 86: return "Bamboom";
                case 87: return "Golden Recycling Vase";
                case 88: return "Coffee Bean";

                case 70: return "Shovel";
                case 71: return "Fertilizer";
                case 72: return "Plant Gloves";
                case 74: return "Mallet";
                



                case 82: return "Lawnmowers";
                case 83: return "Pool Cleaners";

                case 89: return "Progressive Odyssey Adventure";
                case 90: return "Day Access";
                case 91: return "Night Access";
                case 92: return "Pool Access";
                case 93: return "Fog Access";
                case 94: return "Roof Access";
                case 95: return "Snow Access";
                case 96: return "Fusion Challenge Access";
                case 98: return "Odyssey Key";
                case 249: return "Vasebreaker Access";
                case 250: return "Survival Day";
                case 251: return "Survival Night";
                case 252: return "Survival Pool";
                case 253: return "Survival Fog";
                case 254: return "Survival Roof";


                case 201: return "Seed Slot";

                case 206: return "Trophy";


                default:
                    if (id > 99 && id < 180) { return "A New Minigame"; }
                    if (id >= 300 && id < 350) { return "A New Odyssey Minigame"; }
                    if (id >= 255 && id < 265) { return "A Survival Endless"; }


                    return "ID NOT FOUND (REPORT THIS TO THE AP DEV)";
            }






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
            {

                if (arg.Key == "DeathLink" && arg.Value.ToString() != "0")
                {

                    deathLinkMode = 1;//add more later
                }

                if (arg.Key == "RingLink" && arg.Value.ToString() != "0") 
                {
                    
                    ringLinkMode = 1;//add more later
                }

                if (arg.Key == "CompletionType")
                {
                    if (arg.Value.ToString() == "1")
                    {
                        goalType = 1;//add more later
                    }
                    else if (arg.Value.ToString() == "2")
                    {
                        goalType = 2;
                    }
                    else if (arg.Value.ToString() == "3")
                    {
                        goalType = 3;
                    }
                }

                if (arg.Key == "Fusionsanity")
                {
                    MelonLogger.Msg(arg.Value);
                    if (arg.Value.ToString() == "False")
                    {
                        fusionsanity = 0;//add more later
                    }
                    else if (arg.Value.ToString() == "True")
                    {
                        fusionsanity = 1;
                    }

                }


                if (arg.Key == "AdventureExtra" && arg.Value.ToString() == "2")
                {
                    adventureExtraEnabled = true;
                }
                if (arg.Key == "NegativeSun" && arg.Value.ToString() != "0")
                {
                    negativeSun = true;
                }
                if (arg.Key == "GoalTrophies")
                {
                    trophiesForGoal = System.Convert.ToInt32(arg.Value.ToString());

                    
                }
            }
            if (ringLinkMode != 0)
            {
                if (deathLinkMode != 0) { session.ConnectionInfo.UpdateConnectionOptions(new[] { "DeathLink", "RingLink" }); }
                else { session.ConnectionInfo.UpdateConnectionOptions(new[] { "RingLink" }); }
                
            } else if (deathLinkMode != 0)
            {
                session.ConnectionInfo.UpdateConnectionOptions(new[] { "DeathLink" });
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


        [HarmonyLib.HarmonyPatch(typeof(CreatePlant), "CheckFlying")]
        public static class Patch64
        {
            private static void Prefix(int theBoxColumn, int theBoxRow)
            {
                MelonLogger.Msg($"checkflying 1:{theBoxColumn} 2:{theBoxRow}");


            }

            }

            [HarmonyLib.HarmonyPatch(typeof(InitBoard), "Awake")]
        public static class Patch65
        {

            private static void Postfix()
            {
                if (boardGl == null)
                {
                    boardGl = GameObject.Find("Board");
                }

                if (boardGl == null) { MelonLogger.Msg("failed to find board"); return; }
                var boardComp = boardGl.GetComponent(Il2CppType.Of<Board>());

                var type = boardComp.GetIl2CppType();
                var field = type.GetField("theSun");
                Il2CppSystem.Object rawValue = field.GetValue(boardComp);
                int intValue = Il2CppSystem.Convert.ToInt32(rawValue);
                field.SetValue(boardComp, intValue+extraStartSun);
            }
        }






        [HarmonyLib.HarmonyPatch(typeof(Money), "ReinforcePlant")]
        public static class Patch66
        {

            private static void Prefix(Plant plant)
            {
                

                int tempid = -1;
                switch (plant.thePlantType.ToString())
                {
                    case "SilverCabbage": tempid = 2200; break;
                    case "SilverPot": tempid = 2201; break;
                    case "SilverCorn": tempid = 2202; break;
                    case "SilverGarlic": tempid = 2203; break;
                    case "SilverUmbrella": tempid = 2204; break;
                    case "SilverMelon": tempid = 2205; break;
                    case "SilverSunflower": tempid = 2206; break;
                    case "SilverDoom": tempid = 2207; break;
                    case "SilverNut": tempid = 2208; break;
                    case "SilverIceShroom": tempid = 2209; break;
                    default: break;
                }

                if (tempid != -1)
                {
                    if (!checkedArray[tempid])
                    {

                        checkedArray[tempid] = true;
                        session.Locations.CompleteLocationChecks(tempid);

                    }
                }



            }
        }

            [HarmonyLib.HarmonyPatch(typeof(CreatePlant), "CheckMix")]
        public static class Patch67
        {
            private static bool Prefix(int theColumn, int theRow, PlantType theUsedType)
            {


                

                if (fusionsanity == 0) { return true; }
                Il2CppSystem.Collections.Generic.List<Plant> il2cpplist = Lawnf.Get1x1Plants(theColumn, theRow);


                if (il2cpplist != null)
                {
                    PlantType c = PlantType.Nothing;
                    PlantType highest = PlantType.Nothing;
                    foreach (Plant plant in il2cpplist) {

                        MixData.TryGetMix(plant.thePlantType, theUsedType, out c);

                        //if (c != PlantType.Nothing && c != PlantType.Peashooter)//peashooter seems to be used to denote there are no mixes available
                        //{
                        //
                        //    highest = c;
                        //
                        //}

                        highest = theUsedType;
                    

                    if (highest != PlantType.Nothing)
                    {
                        
                        int tempid = 0;
                        switch (c.ToString())
                        {

                            case "DoubleShooter": tempid = 1500; break;
                            case "PeaSunFlower": tempid = 1501; break;
                            case "Cherryshooter": tempid = 1502; break;
                            case "PeaNut": tempid = 1503; break;
                            case "PeaMine": tempid = 1504; break;
                            case "PeaChomper": tempid = 1505; break;
                            case "PeaPuff": tempid = 1506; break;
                            case "PeaFume": tempid = 1507; break;
                            case "HypnoPeashooter": tempid = 1508; break;
                            case "PeaScaredy": tempid = 1509; break;
                            case "SnowPeaShooter": tempid = 1510; break;
                            case "DoomPeashooter": tempid = 1511; break;
                            case "PeaSquash": tempid = 1512; break;
                            case "AllPeater": tempid = 1513; break;
                            case "JalaPeashooter": tempid = 1514; break;
                            case "LanternPea": tempid = 1515; break;
                            case "PeaBlover": tempid = 1516; break;
                            case "StarPea": tempid = 1517; break;
                            case "PeaPumpkin": tempid = 1518; break;
                            case "PeaPot": tempid = 1519; break;
                            case "GarlicPea": tempid = 1520; break;
                            case "TwinFlower": tempid = 1521; break;
                            case "SunBomb": tempid = 1522; break;
                            case "SunNut": tempid = 1523; break;
                            case "SunMine": tempid = 1524; break;
                            case "SunChomper": tempid = 1525; break;
                            case "SunShroom": tempid = 1526; break;
                            case "BigSunShroom": tempid = 1527; break;
                            case "SunHypno": tempid = 1528; break;
                            case "ScaredSun": tempid = 1529; break;
                            case "SunIceShroom": tempid = 1530; break;
                            case "DoomSunflower": tempid = 1531; break;
                            case "SunSquash": tempid = 1532; break;
                            case "SunJalapeno": tempid = 1533; break;
                            case "SunCaltrop": tempid = 1534; break;
                            case "TorchSunflower": tempid = 1535; break;
                            case "SeaSunShroom": tempid = 1536; break;
                            case "SunBlover": tempid = 1537; break;
                            case "SunStar": tempid = 1538; break;
                            case "SunPumpkin": tempid = 1539; break;
                            case "SunMagnet": tempid = 1540; break;
                            case "SunCabbage": tempid = 1541; break;
                            case "SunPot": tempid = 1542; break;
                            case "SilverSunflower": tempid = 1543; break;
                            case "CherryNut": tempid = 1544; break;
                            case "CherryMine": tempid = 1545; break;
                            case "CherryChomper": tempid = 1546; break;
                            case "CherryHypno": tempid = 1547; break;
                            case "CherryScaredy": tempid = 1548; break;
                            case "IceCherry": tempid = 1549; break;
                            case "DoomCherry": tempid = 1550; break;
                            case "CherrySquash": tempid = 1551; break;
                            case "CherryThreePeater": tempid = 1552; break;
                            case "CherryJalapeno": tempid = 1553; break;
                            case "CherryTorch": tempid = 1554; break;
                            case "CherryBlover": tempid = 1555; break;
                            case "CherryPumpkin": tempid = 1556; break;
                            case "CherryMagnet": tempid = 1557; break;
                            case "CherryPot": tempid = 1558; break;
                            case "CherryUmbrella": tempid = 1559; break;
                            case "PotatoNut": tempid = 1560; break;
                            case "NutChomper": tempid = 1561; break;
                            case "PuffNut": tempid = 1562; break;
                            case "NutFume": tempid = 1563; break;
                            case "HypnoNut": tempid = 1564; break;
                            case "ScaredyNut": tempid = 1565; break;
                            case "IceNut": tempid = 1566; break;
                            case "DoomNut": tempid = 1567; break;
                            case "SquashNut": tempid = 1568; break;
                            case "ThreeNut": tempid = 1569; break;
                            case "KelpNut": tempid = 1570; break;
                            case "FireNut": tempid = 1571; break;
                            case "CaltropNut": tempid = 1572; break;
                            case "NutTorch": tempid = 1573; break;
                            case "SeaNut": tempid = 1574; break;
                            case "LanternNut": tempid = 1575; break;
                            case "CactusNut": tempid = 1576; break;
                            case "NutBlover": tempid = 1577; break;
                            case "StarNut": tempid = 1578; break;
                            case "NutPumpkin": tempid = 1579; break;
                            case "MagnetNut": tempid = 1580; break;
                            case "CabbageNut": tempid = 1581; break;
                            case "NutPot": tempid = 1582; break;
                            case "CornNut": tempid = 1583; break;
                            case "GarlicNut": tempid = 1584; break;
                            case "NutUmbrella": tempid = 1585; break;
                            case "SilverNut": tempid = 1586; break;
                            case "MelonNut": tempid = 1587; break;
                            case "PotatoChomper": tempid = 1588; break;
                            case "PotatoPuff": tempid = 1589; break;
                            case "PotatoFume": tempid = 1590; break;
                            case "HypnoMine": tempid = 1591; break;
                            case "ScaredyPotato": tempid = 1592; break;
                            case "IceMine": tempid = 1593; break;
                            case "PotatoDoom": tempid = 1594; break;
                            case "PotatoSquashBody": tempid = 1595; break;
                            case "ThreeMine": tempid = 1596; break;
                            case "KelpMine_water": tempid = 1597; break;
                            case "KelpMine_land": tempid = 1597; break;
                            case "JalaMine": tempid = 1598; break;
                            case "TorchMine": tempid = 1599; break;
                            case "SeaMine_land": tempid = 1600; break;
                            case "SeaMine_water": tempid = 1600; break;
                            case "BloverMine": tempid = 1601; break;
                            case "PotatoPumpkin": tempid = 1602; break;
                            case "TreasureMine": tempid = 1603; break;
                            case "HypnoChomper": tempid = 1604; break;
                            case "DoomChomper": tempid = 1605; break;
                            case "ChomperSquash": tempid = 1606; break;
                            case "LanternChomper": tempid = 1607; break;
                            case "ChomperPumpkin": tempid = 1608; break;
                            case "HypnoPuff": tempid = 1609; break;
                            case "SmallIceShroom": tempid = 1610; break;
                            case "PuffDoom": tempid = 1611; break;
                            case "PuffSquash": tempid = 1612; break;
                            case "KelpPuff": tempid = 1613; break;
                            case "PuffJalapeno": tempid = 1614; break;
                            case "PuffSeaShroom": tempid = 1615; break;
                            case "StarPuff": tempid = 1616; break;
                            case "PuffPumpkin": tempid = 1617; break;
                            case "CabbagePuff": tempid = 1618; break;
                            case "CornPuff": tempid = 1619; break;
                            case "SmallUmbrella": tempid = 1620; break;
                            case "MelonPuff": tempid = 1621; break;
                            case "HypnoFume": tempid = 1622; break;
                            case "ScaredFume": tempid = 1623; break;
                            case "IceFumeShroom": tempid = 1624; break;
                            case "DoomFume": tempid = 1625; break;
                            case "KelpFume": tempid = 1626; break;
                            case "FireFume": tempid = 1627; break;
                            case "TorchFume": tempid = 1628; break;
                            case "SeaFume": tempid = 1629; break;
                            case "LanternFume": tempid = 1630; break;
                            case "CactusFume": tempid = 1631; break;
                            case "StarFume": tempid = 1632; break;
                            case "FumePumpkin": tempid = 1633; break;
                            case "MagnetFume": tempid = 1634; break;
                            case "CabbageFume": tempid = 1635; break;
                            case "CornFume": tempid = 1636; break;
                            case "GarlicFume": tempid = 1637; break;
                            case "MelonFume": tempid = 1638; break;
                            case "ScaredyHypno": tempid = 1639; break;
                            case "IceHypno": tempid = 1640; break;
                            case "HypnoDoom": tempid = 1641; break;
                            case "HypnoSquash": tempid = 1642; break;
                            case "HypnoJalapeno": tempid = 1643; break;
                            case "HypnoTorch": tempid = 1644; break;
                            case "SeaHypno": tempid = 1645; break;
                            case "HypnoBlover": tempid = 1646; break;
                            case "StarHypno": tempid = 1647; break;
                            case "HypnoPumpkin": tempid = 1648; break;
                            case "HypnoMagnet": tempid = 1649; break;
                            case "HypnoPot": tempid = 1650; break;
                            case "HypnoGarlic": tempid = 1651; break;
                            case "HypnoMelon": tempid = 1652; break;
                            case "IceScaredyShroom": tempid = 1653; break;
                            case "ScaredyDoom": tempid = 1654; break;
                            case "SeaScaredyshroom": tempid = 1655; break;
                            case "ScaredyBlover": tempid = 1656; break;
                            case "ScaredyStar": tempid = 1657; break;
                            case "ScaredyPumpkin": tempid = 1658; break;
                            case "ScaredyPot": tempid = 1659; break;
                            case "IceDoom": tempid = 1660; break;
                            case "IceSquash": tempid = 1661; break;
                            case "ObsidianJalapeno": tempid = 1662; break;
                            case "IceCaltrop": tempid = 1663; break;
                            case "IceTorch": tempid = 1664; break;
                            case "IceSeashroom": tempid = 1665; break;
                            case "IcePlantern": tempid = 1666; break;
                            case "IceBlover": tempid = 1667; break;
                            case "IceCabbage": tempid = 1668; break;
                            case "IcePot": tempid = 1669; break;
                            case "IceCorn": tempid = 1670; break;
                            case "SilverIceShroom": tempid = 1671; break;
                            case "WinterMelon": tempid = 1672; break;
                            case "DoomSquash": tempid = 1673; break;
                            case "DoomKelp": tempid = 1674; break;
                            case "DoomJalapeno": tempid = 1675; break;
                            case "DoomTorch": tempid = 1676; break;
                            case "DoomSeaShroom": tempid = 1677; break;
                            case "DoomPlantern": tempid = 1678; break;
                            case "DoomCactus": tempid = 1679; break;
                            case "DoomBlover": tempid = 1680; break;
                            case "DoomStar": tempid = 1681; break;
                            case "DoomPumpkin": tempid = 1682; break;
                            case "MagnetDoom": tempid = 1683; break;
                            case "DoomCabbage": tempid = 1684; break;
                            case "DoomCorn": tempid = 1685; break;
                            case "DoomGarlic": tempid = 1686; break;
                            case "SilverDoom": tempid = 1687; break;
                            case "DoomMelon": tempid = 1688; break;
                            case "ThreeSquash": tempid = 1689; break;
                            case "Squashkelp": tempid = 1690; break;
                            case "JalaSquash": tempid = 1691; break;
                            case "SquashSpike": tempid = 1692; break;
                            case "SquashTorch": tempid = 1693; break;
                            case "SeaSquash": tempid = 1694; break;
                            case "SquashBlover": tempid = 1695; break;
                            case "SquashPumpkin": tempid = 1696; break;
                            case "CabbageSquash": tempid = 1697; break;
                            case "SquashCorn": tempid = 1698; break;
                            case "SquashMelon": tempid = 1699; break;
                            case "Threekelp": tempid = 1700; break;
                            case "DarkThreePeater": tempid = 1701; break;
                            case "ThreeSpike": tempid = 1702; break;
                            case "ThreeTorch": tempid = 1703; break;
                            case "ThreePlantern": tempid = 1704; break;
                            case "GarlicThreePeater": tempid = 1705; break;
                            case "Jalakelp": tempid = 1706; break;
                            case "CaltropKelp_water": tempid = 1707; break;
                            case "CaltropKelp_land": tempid = 1707; break;
                            case "KelpTorch": tempid = 1708; break;
                            case "JalaCaltrop": tempid = 1709; break;
                            case "JalaTorch": tempid = 1710; break;
                            case "JalaStar": tempid = 1711; break;
                            case "JalaPumpkin": tempid = 1712; break;
                            case "FireCabbage": tempid = 1713; break;
                            case "JalaCorn": tempid = 1714; break;
                            case "FireMelon": tempid = 1715; break;
                            case "TorchSpike": tempid = 1716; break;
                            case "CactusCaltrop": tempid = 1717; break;
                            case "CabbageCaltrop": tempid = 1718; break;
                            case "CaltropPot": tempid = 1719; break;
                            case "CornCaltrop": tempid = 1720; break;
                            case "MelonCaltrop": tempid = 1721; break;
                            case "StarTorch": tempid = 1722; break;
                            case "TorchPumpkin": tempid = 1723; break;
                            case "GarlicTorch": tempid = 1724; break;
                            case "SeaLantern": tempid = 1725; break;
                            case "SeaCactus": tempid = 1726; break;
                            case "SeaBlover": tempid = 1727; break;
                            case "SeaStarfruit": tempid = 1728; break;
                            case "SeaPumpkin": tempid = 1729; break;
                            case "SeaMagnet": tempid = 1730; break;
                            case "LanternCactus": tempid = 1731; break;
                            case "LanternBlover": tempid = 1732; break;
                            case "LanternStar": tempid = 1733; break;
                            case "LanternPumpkin": tempid = 1734; break;
                            case "LanternMagnet": tempid = 1735; break;
                            case "LanternPot": tempid = 1736; break;
                            case "LanternUmbrella": tempid = 1737; break;
                            case "CactusBlover": tempid = 1738; break;
                            case "CacstusStar": tempid = 1739; break;
                            case "CactusPumpkin": tempid = 1740; break;
                            case "MagnetCactus": tempid = 1741; break;
                            case "CactusUmbrella": tempid = 1742; break;
                            case "HurricaneBlover": tempid = 1743; break;
                            case "StarBlover": tempid = 1744; break;
                            case "BlowerPumpkin": tempid = 1745; break;
                            case "MagnetBlover": tempid = 1746; break;
                            case "CabbageBlover": tempid = 1747; break;
                            case "CornBlover": tempid = 1748; break;
                            case "GarlicBlover": tempid = 1749; break;
                            case "BloverUmbrella": tempid = 1750; break;
                            case "MelonBlover": tempid = 1751; break;
                            case "MagnetCorn": tempid = 1752; break;
                            case "GoldMagnet": tempid = 1753; break;
                            case "MagnetMelon": tempid = 1754; break;
                            case "CabbagePot": tempid = 1755; break;
                            case "CornCabbage": tempid = 1756; break;
                            case "GarlicCabbage": tempid = 1757; break;
                            case "CabbageUmbrella": tempid = 1758; break;
                            case "SilverCabbage": tempid = 1759; break;
                            case "CabbageMelon": tempid = 1760; break;
                            case "CornPot": tempid = 1761; break;
                            case "GarlicPot": tempid = 1762; break;
                            case "UmbrellaPot": tempid = 1763; break;
                            case "SilverPot": tempid = 1764; break;
                            case "MelonPot": tempid = 1765; break;
                            case "GarlicCorn": tempid = 1766; break;
                            case "CornUmbrella": tempid = 1767; break;
                            case "SilverCorn": tempid = 1768; break;
                            case "CornMelon": tempid = 1769; break;
                            case "GarlicUmbrella": tempid = 1770; break;
                            case "SilverGarlic": tempid = 1771; break;
                            case "GarlicMelon": tempid = 1772; break;
                            case "SilverUmbrella": tempid = 1773; break;
                            case "MelonUmbrella": tempid = 1774; break;
                            case "SilverMelon": tempid = 1775; break;


                            case "StarPumpkin": tempid = 1792; break;
                            case "MagnetStar": tempid = 1793; break;
                            case "Pumpiner": tempid = 1794; break;
                            case "GarlicPumpkin": tempid = 1795; break;
                            case "MelonPumpkin": tempid = 1796; break;
                            case "TwinMarigold": tempid = 1797; break;
                            case "BigWallNut": tempid = 1811; break;

                            case "SpruceShulk": tempid = 1776; break;
                            case "LotusSpruce": tempid = 1777; break;
                            case "WaterSpruce": tempid = 1778; break;
                            case "BambooSpruce": tempid = 1779; break;
                            case "ThornsSpruce": tempid = 1780; break;
                            case "Twinshulk": tempid = 1781; break;
                            case "ShulkLotus": tempid = 1782; break;
                            case "WaterShulk": tempid = 1783; break;
                            case "ShulkBamboo": tempid = 1784; break;
                            case "ThornsShulk": tempid = 1785; break;
                            case "LotusAloes": tempid = 1786; break;
                            case "LotusBamboo": tempid = 1787; break;
                            case "ThornsLotus": tempid = 1788; break;
                            case "WaterBamboo": tempid = 1789; break;
                            case "ThronsAloes": tempid = 1790; break;
                            case "ThornsBamboo": tempid = 1791; break;


                            case "IceCactus": tempid = 1798; break;
                            case "CherryFume": tempid = 1799; break;
                            case "TorchSeaShroom": tempid = 1800; break;
                            case "ThreeMelon": tempid = 1801; break;
                            case "ThreeCorn": tempid = 1802; break;
                            case "ThreePumpkin": tempid = 1803; break;
                            case "IcePumpkin": tempid = 1804; break;
                            case "BloverPot": tempid = 1805; break;
                            case "CaltropFume": tempid = 1806; break;
                            case "ThreeCabbage": tempid = 1807; break;
                            case "DoomThreePeater": tempid = 1808; break;
                            case "StarSquash": tempid = 1809; break;
                            case "ThreePot": tempid = 1810; break;


                            case "DoubleCherry": tempid = 1900; break;
                            case "HypnoRepeater": tempid = 1901; break;
                            case "DoubleSnow": tempid = 1902; break;
                            case "JalaDoubleshooter": tempid = 1903; break;
                            case "LanternRepeater": tempid = 1904; break;
                            case "GarlicRepeater": tempid = 1905; break;
                            case "SplitPea": tempid = 1906; break;
                            case "CherrySplit": tempid = 1907; break;
                            case "HypnoSplit": tempid = 1908; break;
                            case "SnowSplit": tempid = 1909; break;
                            case "JalaSplit": tempid = 1910; break;
                            case "LanternSplit": tempid = 1911; break;
                            case "GarlicSplit": tempid = 1912; break;
                            case "GatlingPea": tempid = 1913; break;
                            case "CherryGatling": tempid = 1914; break;
                            case "HypnoGatling": tempid = 1915; break;
                            case "SnowGatling": tempid = 1916; break;
                            case "JalaGatling": tempid = 1917; break;
                            case "LanternGatling": tempid = 1918; break;
                            case "GarlicGatling": tempid = 1919; break;

                            case "IronPea": tempid = 1930; break;
                            case "IronNut": tempid = 1931; break;
                            case "TallNutFootball": tempid = 1932; break;
                            case "IronStar": tempid = 1935; break;
                            case "PickaxeStar": tempid = 1934; break;
                            case "JackboxStar": tempid = 1933; break;
                            case "IronPumpkin": tempid = 1936; break;
                            case "JackboxPumpkin": tempid = 1937; break;
                            case "PickaxePumpkin": tempid = 1938; break;
                            case "MachineNut": tempid = 1939; break;
                            case "IronPuff": tempid = 1940; break;
                            case "IronFume": tempid = 1941; break;
                            case "PortalPea": tempid = 1942; break;
                            case "PortalCorn": tempid = 1943; break;
                            case "IronCorn": tempid = 1944; break;
                            case "PortalDoom": tempid = 1945; break;
                            case "PortalNut": tempid = 1946; break;
                            case "BucketDoom": tempid = 1947; break;
                            case "JackboxDoom": tempid = 1948; break;
                            case "IronSquash": tempid = 1949; break;

                            case "IronMelon": tempid = 1950; break;
                            case "PortalMelon": tempid = 1951; break;


                            case "SuperCherryShooter": tempid = 2000; break;
                            case "SuperChomper": tempid = 2001; break;
                            case "SuperHypno": tempid = 2002; break;
                            case "SuperFume": tempid = 2003; break;
                            case "SuperTorch": tempid = 2004; break;
                            case "SuperKelp": tempid = 2005; break;
                            case "SuperStar": tempid = 2006; break;
                            case "SuperPumpkin": tempid = 2007; break;
                            case "SuperMelon": tempid = 2008; break;
                            case "MelonCannon": tempid = 2009; break;
                            case "SuperUmbrella": tempid = 2010; break;
                            case "HelmetFume": tempid = 2011; break;
                            case "BigGatling": tempid = 2012; break;
                            case "BigPumpkin": tempid = 2013; break;
                            case "BigChomper": tempid = 2014; break;
                            case "BigSeaShroom": tempid = 2015; break;
                            case "SuperSpruce": tempid = 2016; break;
                            case "SuperFurnace": tempid = 2017; break;
                            case "HugeWallNut": tempid = 2018; break;
                            case "BigGloom": tempid = 2019; break;
                            case "HelmetGatling": tempid = 2020; break;
                            case "CabbageCannon": tempid = 2021; break;



                            default: tempid = -1; break;







                        }
                        if (tempid != -1)
                        {
                            if (!checkedArray[tempid])
                            {

                                checkedArray[tempid] = true;
                                session.Locations.CompleteLocationChecks(tempid);

                            }
                        }


                        }
                    }

                }
                     
                return true;
                
            }
        }






        [HarmonyLib.HarmonyPatch(typeof(CreatePlant), "SetPlant")]
        public static class Patch69
        {
            private static bool Prefix(int newColumn, int newRow, Il2Cpp.PlantType theSeedType, Il2Cpp.Plant targetPlant, UnityEngine.Vector2 puffV, bool isFreeSet, bool withEffect, Il2Cpp.Plant hidplant)
            {


                if (boardGl == null) {
                    boardGl = GameObject.Find("Board");


                }

                if (boardGl == null) { MelonLogger.Msg("failed to find board"); return true; }

                InitBoard temp = boardGl.GetComponent<InitBoard>();


                if (!temp.ready)
                {
                    MelonLogger.Msg("not ready");
                    return true;
                }


                //dont do this if glove or wheelbarrow is in use, but do it if either has already been used this frame


                MelonLogger.Msg($"setplant 1:{theSeedType} (lazy)");
                switch (theSeedType.ToString()) {

                    case "Peashooter": if (!unlockedArray[1]) { return false; } break;
                    case "Sunflower": if (!unlockedArray[2]) { return false; } break;
                    case "CherryBomb": if (!unlockedArray[3]) { return false; } break;
                    case "WallNut": if (!unlockedArray[4]) { return false; } break;
                    case "PotatoMine": if (!unlockedArray[5]) { return false; } break;
                    case "Chomper": if (!unlockedArray[6]) { return false; } break;
                    case "SmallPuff": if (!unlockedArray[10]) { return false; } break;
                    case "FumeShroom": if (!unlockedArray[11]) { return false; }break;
                    case "HypnoShroom": if (!unlockedArray[12]) { return false; } break;
                    case "ScaredyShroom": if (!unlockedArray[13]) { return false; } break;
                    case "IceShroom": if (!unlockedArray[14]) { return false; } break;
                    case "DoomShroom": if (!unlockedArray[15]) { return false; } break;
                    case "LilyPad": if (!unlockedArray[19]) { return false; } break;
                    case "Squash": if (!unlockedArray[20]) { return false; } break;
                    case "ThreePeater": if (!unlockedArray[21]) { return false; } break;
                    case "Tanglekelp": if (!unlockedArray[22]) { return false; } break;
                    case "Jalapeno": if (!unlockedArray[23]) { return false; } break;
                    case "Caltrops": if (!unlockedArray[24]) { return false; } break;
                    case "TorchWood": if (!unlockedArray[25]) { return false; } break;
                    case "SeaShroom": if (!unlockedArray[28]) { return false; } break;
                    case "Plantern": if (!unlockedArray[29]) { return false; } break;
                    case "Cactus": if (!unlockedArray[30]) { return false; } break;
                    case "Blover": if (!unlockedArray[31]) { return false; } break;
                    case "StarFruit": if (!unlockedArray[32]) { return false; } break;
                    case "Pumpkin": if (!unlockedArray[33]) { return false; } break;
                    case "Magnetshroom": if (!unlockedArray[34]) { return false; } break;
                    case "Cabbagepult": if (!unlockedArray[37]) { return false; } break;
                    case "Pot": if (!unlockedArray[38]) { return false; } break;
                    case "Cornpult": if (!unlockedArray[39]) { return false; } break;
                    case "Garlic": if (!unlockedArray[40]) { return false; } break;
                    case "Umbrellaleaf": if (!unlockedArray[41]) { return false; } break;
                    case "Marigold": if (!unlockedArray[42]) { return false; } break;
                    case "Melonpult": if (!unlockedArray[43]) { return false; } break;

                    case "PineFurnace": if (!unlockedArray[46]) { return false; } break;
                    case "SpruceShooter": if (!unlockedArray[47]) { return false; } break;
                    case "Shulkflower": if (!unlockedArray[48]) { return false; } break;
                    case "IceLotus": if (!unlockedArray[49]) { return false; } break;
                    case "WaterAloes": if (!unlockedArray[50]) { return false; } break;
                    case "Bamboo": if (!unlockedArray[51]) { return false; } break;
                    case "Thorns": if (!unlockedArray[52]) { return false; } break;

                        

                    default: break;
                }

                int tempid = 0;
                switch (theSeedType.ToString())
                {
                    case "IronPea": tempid = 1930; break;
                    case "IronNut": tempid = 1931; break;
                    case "TallNutFootball": tempid = 1932; break;
                    case "IronStar": tempid = 1935; break;
                    case "PickaxeStar": tempid = 1934; break;
                    case "JackboxStar": tempid = 1933; break;
                    case "IronPumpkin": tempid = 1936; break;
                    case "JackboxPumpkin": tempid = 1937; break;
                    case "PickaxePumpkin": tempid = 1938; break;
                    case "MachineNut": tempid = 1939; break;
                    case "IronPuff": tempid = 1940; break;
                    case "IronFume": tempid = 1941; break;
                    case "PortalPea": tempid = 1942; break;
                    case "PortalCorn": tempid = 1943; break;
                    case "IronCorn": tempid = 1944; break;
                    case "PortalDoom": tempid = 1945; break;
                    case "PortalNut": tempid = 1946; break;
                    case "BucketDoom": tempid = 1947; break;
                    case "JackboxDoom": tempid = 1948; break;
                    case "IronSquash": tempid = 1949; break;

                    case "IronMelon": tempid = 1950; break;
                    case "PortalMelon": tempid = 1951; break;


                    case "MelonCannon": tempid = 2009; break;

                    case "HelmetFume": tempid = 2011; break;
                    case "BigGatling": tempid = 2012; break;
                    case "BigPumpkin": tempid = 2013; break;
                    case "BigChomper": tempid = 2014; break;
                    case "BigSeaShroom": tempid = 2015; break;

                    case "SuperFurnace": tempid = 2017; break;
                    case "HugeWallNut": tempid = 2018; break;
                    case "BigGloom": tempid = 2019; break;
                    case "HelmetGatling": tempid = 2020; break;
                    case "CabbageCannon": tempid = 2021; break;



                    default: tempid = -1; break;







                }
                if (tempid != -1)
                {
                    
                    if (!checkedArray[tempid])
                    {

                        checkedArray[tempid] = true;
                        session.Locations.CompleteLocationChecks(tempid);

                    }
                }

                //code here to prevent placing plants you dont have unlocked

                //MelonLogger.Msg($"1:{newColumn} 2:{newRow} 3:{theSeedType} 4:{targetPlant} 5:{puffV} 6:{isFreeSet} 7:{withEffect} 8:{hidplant}");


                return true;
            }
            
        }







    }


    //[HarmonyLib.HarmonyPatch(typeof(InitBoard), "InitMowers")]
    //public static class ImagineReadingMyCodeLMAO
    //{
    //    private static void Prefix()//too lazy to write a new function to do this again
    //    {
    //
    //
    //    }
    //
    // }

        public class APData
    {
        public string serverAddress { get; set; }
        public int serverPort { get; set; }
        public string slotName { get; set; }
        public string password { get; set; }

    }


}
