







using Archipelago;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Packets;
using HarmonyLib;
using Il2CppSystem.Runtime.Remoting.Messaging;
using Il2CppTMPro;
using MelonLoader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

namespace PVZFusionArchipelago
{
    public class Class1 : MelonMod
    {
        public const int maxItems = 97; // holy shit good programming practice?
        public static ArchipelagoSession session;
        public static int currentPlayerSlot;
        public static bool[] unlockedArray = new bool[maxItems];
        const string page1 = "InGameUI(Clone)/Bottom/SeedLibrary/Grid/Pages/Page1/";
        const string page2 = "InGameUI(Clone)/Bottom/SeedLibrary/Grid/ColorfulCards/Page1/";
  
        const string advanturePage1 = "ChallengeMenu(Clone)/Levels/PageAdvantureLevel/Pages/Page1/";
        const string advanturePage2 = "ChallengeMenu(Clone)/Levels/PageAdvantureLevel/Pages/Page2/";
        const string advanturePage3 = "ChallengeMenu(Clone)/Levels/PageAdvantureLevel/Pages/Page3/";
        const string advanturePage4 = "ChallengeMenu(Clone)/Levels/PageNewAdvantureLevel/Pages/Page1/";
        private GameObject inputUI;

        public override void OnInitializeMelon()
        {
            for (int i = 0; i < maxItems; i++)
            {
                unlockedArray[i] = false;
            }
            string modDir = this.MelonAssembly.Location; // Full path to DLL
            string modFolder = Path.GetDirectoryName(modDir);
            string jsonPath = Path.Combine(modFolder, "config.json");

            if (File.Exists(jsonPath))
            {
                string jsonContent = File.ReadAllText(jsonPath);
                // Deserialize if needed
                MyData data = JsonConvert.DeserializeObject<MyData>(jsonContent);
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
            }
            if (unlockedArray[91] == false)
            {
                HideTargetObject("Canvas", advanturePage1 + "Lv10");
                HideTargetObject("Canvas", advanturePage1 + "Lv11");
                HideTargetObject("Canvas", advanturePage1 + "Lv12");
                HideTargetObject("Canvas", advanturePage1 + "Lv13");
                HideTargetObject("Canvas", advanturePage1 + "Lv14");
                HideTargetObject("Canvas", advanturePage1 + "Lv15");
                HideTargetObject("Canvas", advanturePage1 + "Lv16");
                HideTargetObject("Canvas", advanturePage1 + "Lv17");
                HideTargetObject("Canvas", advanturePage1 + "Lv18");
            }

            if (unlockedArray[92] == false)
            {
                HideTargetObject("Canvas", advanturePage2 + "Lv19");
                HideTargetObject("Canvas", advanturePage2 + "Lv20");
                HideTargetObject("Canvas", advanturePage2 + "Lv21");
                HideTargetObject("Canvas", advanturePage2 + "Lv22");
                HideTargetObject("Canvas", advanturePage2 + "Lv23");
                HideTargetObject("Canvas", advanturePage2 + "Lv24");
                HideTargetObject("Canvas", advanturePage2 + "Lv25");
                HideTargetObject("Canvas", advanturePage2 + "Lv26");
                HideTargetObject("Canvas", advanturePage2 + "Lv27");
            }
            if (unlockedArray[93] == false)
            {
                HideTargetObject("Canvas", advanturePage2 + "Lv28");
                HideTargetObject("Canvas", advanturePage2 + "Lv29");
                HideTargetObject("Canvas", advanturePage2 + "Lv30");
                HideTargetObject("Canvas", advanturePage2 + "Lv31");
                HideTargetObject("Canvas", advanturePage2 + "Lv32");
                HideTargetObject("Canvas", advanturePage2 + "Lv33");
                HideTargetObject("Canvas", advanturePage2 + "Lv34");
                HideTargetObject("Canvas", advanturePage2 + "Lv35");
                HideTargetObject("Canvas", advanturePage2 + "Lv36");
            }
            if (unlockedArray[94] == false)
            {
                HideTargetObject("Canvas", advanturePage3 + "Lv37");
                HideTargetObject("Canvas", advanturePage3 + "Lv38");
                HideTargetObject("Canvas", advanturePage3 + "Lv39");
                HideTargetObject("Canvas", advanturePage3 + "Lv40");
                HideTargetObject("Canvas", advanturePage3 + "Lv41");
                HideTargetObject("Canvas", advanturePage3 + "Lv42");
                HideTargetObject("Canvas", advanturePage3 + "Lv43");
                HideTargetObject("Canvas", advanturePage3 + "Lv44");
                HideTargetObject("Canvas", advanturePage3 + "Lv45");
                HideTargetObject("Canvas","ChallengeMenu(Clone)/Levels/PageMiniGames/Pages/Page2/Lv77");
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
            }

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
        }
        }

        [RegisterTypeInIl2Cpp]
        public class OnClickHandler2D : MonoBehaviour
        {
            public OnClickHandler2D(IntPtr ptr) : base(ptr) { }

            void OnMouseDown()
            {
                MelonLogger.Msg($"Clicked: {gameObject.name}");

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

                var textComponent = leveltext.GetComponent<TextMeshProUGUI>();
                //if (textComponent == null)//dumb error checking thats probably good practice
                //{
                //    return;
                //}
                string levelName = textComponent.text;


                switch (levelName)
                {
                    case "Adventure Mode: Classic | Level 1":
                        session.Locations.CompleteLocationChecks(1);//theres a way to do this in 1 cmd but i will actually die before i figure out what it is
                        session.Locations.CompleteLocationChecks(101);
                        break;
                    case "Adventure Mode: Classic | Level 2":
                        session.Locations.CompleteLocationChecks(2);
                        session.Locations.CompleteLocationChecks(102);
                        break;
                    case "Adventure Mode: Classic | Level 3":
                        session.Locations.CompleteLocationChecks(3);
                        session.Locations.CompleteLocationChecks(103);
                        break;
                    case "Adventure Mode: Classic | Level 4":
                        session.Locations.CompleteLocationChecks(4);
                        session.Locations.CompleteLocationChecks(104);
                        break;
                    case "Adventure Mode: Classic | Level 5":
                        session.Locations.CompleteLocationChecks(5);
                        session.Locations.CompleteLocationChecks(105);
                        break;
                    case "Adventure Mode: Classic | Level 6":
                        session.Locations.CompleteLocationChecks(6);
                        session.Locations.CompleteLocationChecks(106);
                        break;
                    case "Adventure Mode: Classic | Level 7":
                        session.Locations.CompleteLocationChecks(7);
                        session.Locations.CompleteLocationChecks(107);
                        break;
                    case "Adventure Mode: Classic | Level 8":
                        session.Locations.CompleteLocationChecks(8);
                        session.Locations.CompleteLocationChecks(108);
                        break;
                    case "Adventure Mode: Classic | Level 9":
                        session.Locations.CompleteLocationChecks(9);
                        session.Locations.CompleteLocationChecks(109);
                        break;
                    case "Adventure Mode: Classic | Level 10":
                        session.Locations.CompleteLocationChecks(10);
                        session.Locations.CompleteLocationChecks(110);
                        break;
                    case "Adventure Mode: Classic | Level 11":
                        session.Locations.CompleteLocationChecks(11);
                        session.Locations.CompleteLocationChecks(111);
                        break;
                    case "Adventure Mode: Classic | Level 12":
                        session.Locations.CompleteLocationChecks(12);
                        session.Locations.CompleteLocationChecks(112);
                        break;
                    case "Adventure Mode: Classic | Level 13":
                        session.Locations.CompleteLocationChecks(13);
                        session.Locations.CompleteLocationChecks(113);
                        break;
                    case "Adventure Mode: Classic | Level 14":
                        session.Locations.CompleteLocationChecks(14);
                        session.Locations.CompleteLocationChecks(113);
                        break;
                    case "Adventure Mode: Classic | Level 15":
                        session.Locations.CompleteLocationChecks(15);
                        session.Locations.CompleteLocationChecks(115);
                        break;
                    case "Adventure Mode: Classic | Level 16":
                        session.Locations.CompleteLocationChecks(16);
                        session.Locations.CompleteLocationChecks(116);
                        break;
                    case "Adventure Mode: Classic | Level 17":
                        session.Locations.CompleteLocationChecks(17);
                        session.Locations.CompleteLocationChecks(117);
                        break;
                    case "Adventure Mode: Classic | Level 18":
                        session.Locations.CompleteLocationChecks(18);
                        session.Locations.CompleteLocationChecks(118);
                        break;
                    case "Adventure Mode: Classic | Level 19":
                        session.Locations.CompleteLocationChecks(19);
                        session.Locations.CompleteLocationChecks(119);
                        break;
                    case "Adventure Mode: Classic | Level 20":
                        session.Locations.CompleteLocationChecks(20);
                        session.Locations.CompleteLocationChecks(120);
                        break;
                    case "Adventure Mode: Classic | Level 21":
                        session.Locations.CompleteLocationChecks(21);
                        session.Locations.CompleteLocationChecks(121);
                        break;
                    case "Adventure Mode: Classic | Level 22":
                        session.Locations.CompleteLocationChecks(22);
                        session.Locations.CompleteLocationChecks(122);
                        break;
                    case "Adventure Mode: Classic | Level 23":
                        session.Locations.CompleteLocationChecks(23);
                        session.Locations.CompleteLocationChecks(123);
                        break;
                    case "Adventure Mode: Classic | Level 24":
                        session.Locations.CompleteLocationChecks(24);
                        session.Locations.CompleteLocationChecks(124);
                        break;
                    case "Adventure Mode: Classic | Level 25":
                        session.Locations.CompleteLocationChecks(25);
                        session.Locations.CompleteLocationChecks(125);
                        break;
                    case "Adventure Mode: Classic | Level 26":
                        session.Locations.CompleteLocationChecks(26);
                        session.Locations.CompleteLocationChecks(126);
                        break;
                    case "Adventure Mode: Classic | Level 27":
                        session.Locations.CompleteLocationChecks(27);
                        session.Locations.CompleteLocationChecks(127);
                        break;
                    case "Adventure Mode: Classic | Level 28":
                        session.Locations.CompleteLocationChecks(28);
                        session.Locations.CompleteLocationChecks(128);
                        break;
                    case "Adventure Mode: Classic | Level 29":
                        session.Locations.CompleteLocationChecks(29);
                        session.Locations.CompleteLocationChecks(129);
                        break;
                    case "Adventure Mode: Classic | Level 30":
                        session.Locations.CompleteLocationChecks(30);
                        session.Locations.CompleteLocationChecks(130);
                        break;
                    case "Adventure Mode: Classic | Level 31":
                        session.Locations.CompleteLocationChecks(31);
                        session.Locations.CompleteLocationChecks(131);
                        break;
                    case "Adventure Mode: Classic | Level 32":
                        session.Locations.CompleteLocationChecks(32);
                        session.Locations.CompleteLocationChecks(132);
                        break;
                    case "Adventure Mode: Classic | Level 33":
                        session.Locations.CompleteLocationChecks(33);
                        session.Locations.CompleteLocationChecks(133);
                        break;
                    case "Adventure Mode: Classic | Level 34":
                        session.Locations.CompleteLocationChecks(34);
                        session.Locations.CompleteLocationChecks(134);
                        break;
                    case "Adventure Mode: Classic | Level 35":
                        session.Locations.CompleteLocationChecks(35);
                        session.Locations.CompleteLocationChecks(135);
                        break;
                    case "Adventure Mode: Classic | Level 36":
                        session.Locations.CompleteLocationChecks(36);
                        session.Locations.CompleteLocationChecks(136);
                        break;
                    case "Adventure Mode: Classic | Level 37":
                        session.Locations.CompleteLocationChecks(37);
                        session.Locations.CompleteLocationChecks(137);
                        break;
                    case "Adventure Mode: Classic | Level 38":
                        session.Locations.CompleteLocationChecks(38);
                        session.Locations.CompleteLocationChecks(138);
                        break;
                    case "Adventure Mode: Classic | Level 39":
                        session.Locations.CompleteLocationChecks(39);
                        session.Locations.CompleteLocationChecks(139);
                        break;
                    case "Adventure Mode: Classic | Level 40":
                        session.Locations.CompleteLocationChecks(40);
                        session.Locations.CompleteLocationChecks(140);
                        break;
                    case "Adventure Mode: Classic | Level 41":
                        session.Locations.CompleteLocationChecks(41);
                        session.Locations.CompleteLocationChecks(141);
                        break;
                    case "Adventure Mode: Classic | Level 42":
                        session.Locations.CompleteLocationChecks(42);
                        session.Locations.CompleteLocationChecks(142);
                        break;
                    case "Adventure Mode: Classic | Level 43":
                        session.Locations.CompleteLocationChecks(43);
                        session.Locations.CompleteLocationChecks(143);
                        break;
                    case "Adventure Mode: Classic | Level 44":
                        session.Locations.CompleteLocationChecks(44);
                        session.Locations.CompleteLocationChecks(144);
                        break;
                    case "Adventure Mode: Classic | Level 45":
                        session.Locations.CompleteLocationChecks(45);
                        session.Locations.CompleteLocationChecks(145);
                        break;
                    case "Adventure Mode: Snow | Level 1":
                        session.Locations.CompleteLocationChecks(46);
                        session.Locations.CompleteLocationChecks(146);
                        break;
                    case "Adventure Mode: Snow | Level 2":
                        session.Locations.CompleteLocationChecks(47);
                        session.Locations.CompleteLocationChecks(147);
                        break;
                    case "Adventure Mode: Snow | Level 3":
                        session.Locations.CompleteLocationChecks(48);
                        session.Locations.CompleteLocationChecks(148);
                        break;
                    case "Adventure Mode: Snow | Level 4":
                        session.Locations.CompleteLocationChecks(49);
                        session.Locations.CompleteLocationChecks(149);
                        break;
                    case "Adventure Mode: Snow | Level 5":
                        session.Locations.CompleteLocationChecks(50);
                        session.Locations.CompleteLocationChecks(150);
                        break;
                    case "Adventure Mode: Snow | Level 6":
                        session.Locations.CompleteLocationChecks(51);
                        session.Locations.CompleteLocationChecks(151);
                        break;
                    case "Adventure Mode: Snow | Level 7":
                        session.Locations.CompleteLocationChecks(52);
                        session.Locations.CompleteLocationChecks(152);
                        break;
                    case "Adventure Mode: Snow | Level 8":
                        session.Locations.CompleteLocationChecks(53);
                        session.Locations.CompleteLocationChecks(153);
                        break;
                    case "Adventure Mode: Snow | Level 9":
                        session.Locations.CompleteLocationChecks(54);
                        session.Locations.CompleteLocationChecks(154);
                        break;
                    case "Dr.Zomboss' Revenge":
                        session.Locations.CompleteLocationChecks(100);
                        session.Socket.SendPacket(new StatusUpdatePacket() { Status = ArchipelagoClientState.ClientGoal });
                        break;
                    default:
                        MelonLogger.Msg("Level not implemented");
                        break;
                }

                






            }
        }




        private void OnTextSubmitted(string text)
        {
            MelonLogger.Msg($"Text submitted: {text}");
            GameObject.Destroy(inputUI); // remove overlay after input
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



        public static void Connect(string server, string user, string pass, ArchipelagoSession session)
        {
            LoginResult result;

            try
            {
                // handle TryConnectAndLogin attempt here and save the returned object to `result`
                result = session.TryConnectAndLogin(server, user, ItemsHandlingFlags.AllItems);
            }
            catch (Exception e)
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

            // Successfully connected, `ArchipelagoSession` (assume statically defined as `session` from now on) can now be
            // used to interact with the server and the returned `LoginSuccessful` contains some useful information about the
            // initial connection (e.g. a copy of the slot data as `loginSuccess.SlotData`)
            MelonLogger.Msg("Successfully Connect to archipelago");
            var loginSuccess = (LoginSuccessful)result;
        }
    }

    public class MyData
    {
        public string serverAddress { get; set; }
        public int serverPort { get; set; }
        public string slotName { get; set; }
    }

}
