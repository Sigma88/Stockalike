using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Kopernicus;
using Random = System.Random;


namespace SASSPlugin
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class FixLoadingScreen : MonoBehaviour
    {
        string path = "GameData/StockalikeSolarSystem/Textures/LoadingScreen/";
        List<string> missing = new List<string>();

        public static NewLoadingScreen newLoadingScreen { get; set; }
        public static bool useSASSLoadingScreen = true;
        public static bool keepStockLogo = false;
        public static bool keepGreenKerbals = false;
        public static bool recolorKerbals = false; // true by default, false if keepGreenKerbals == true
        public static int LogoChance = 1;
        public static int KerbalsChance = 4;

        bool skip = false;
        bool skip2 = false;
        bool export = false;

        void Awake()
        {
            useSASSLoadingScreen = GetSetting("useSASSLoadingScreen", true);
            if (useSASSLoadingScreen)
            {
                keepStockLogo = GetSetting("keepStockLogo", false);
                keepGreenKerbals = GetSetting("keepGreenKerbals", false);
                if (!keepGreenKerbals)
                    recolorKerbals = GetSetting("recolorKerbals", true);

                newLoadingScreen = new SASSLoadingScreen();
            }

            DontDestroyOnLoad(this);
        }

        bool GetSetting(string name, bool Default)
        {
            bool output = Default;

            foreach (ConfigNode Settings in GameDatabase.Instance.GetConfigNodes("SASSLoadingScreen"))
            {
                if (Settings.HasValue(name))
                {
                    NumericParser<bool> userSetting = new NumericParser<bool>();
                    userSetting.SetFromString(Settings.GetValue(name));
                    if (userSetting.value == Default)
                    {
                        return Default;
                    }
                    else
                    {
                        output = !Default;
                    }
                }
            }

            return output;
        }

        void Update()
        {
            if (!skip && LoadingScreen.Instance != null)
            {
                newLoadingScreen.UpdateScreens(LoadingScreen.Instance.Screens[1]);
                skip = true;
            }

            if (skip && recolorKerbals)
            {
                UpdateSASS();
            }

            if (HighLogic.LoadedScene == GameScenes.MAINMENU)
            {
                DestroyObject(this);
            }
        }

        void UpdateSASS()
        {
            CheckFiles(SASSLoadingScreen.names);

            if (!skip2 && missing.Count > 0)
            {
                Texture2D check = Utility.CreateReadable(Resources.FindObjectsOfTypeAll<Texture2D>().FirstOrDefault(t => missing.Contains(t.name)));
                if (check != null)
                {
                    skip2 = check.GetPixel(check.width / 2, check.height / 2).a > 0;
                    export = skip2;
                }
            }

            if (export)
            {
                foreach (string name in missing)
                {
                    if (name == "WernherVonKerman")
                        Recolor(name, 808, 396, 0.4f);
                    else if (name == "KerbalRecruit")
                        Recolor(name, 934, 346, 0.4f);
                    else if (name == "kerbalspaceodyssey-v2")
                        Recolor(name, 382, 458, 0.5f);
                    else if (name == "KerbalGroundCrew")
                        Recolor(name, 486, 321, 0.455f);
                    else if (name == "KerbalMechanic")
                        Recolor(name, 554, 238, 0.4f);
                    else if (name == "GeneKerman")
                        Recolor(name, 746, 347, 0.5f);
                    else if (name == "BobKerman")
                        Recolor(name, 1062, 524, 0.5f);
                    else if (name == "BillKerman")
                        Recolor(name, 809, 460, 0.4f);
                    else if (name == "JebediahKerman")
                        Recolor(name, 963, 530, 0.5f);
                    else if (name == "bumpfist")
                        Recolor(name, 434, 394, 0.331f);
                    else if (name == "loudandclear")
                        Recolor(name, 841, 576, 0.331f);
                    else if (name == "landing")
                        Recolor(name, 477, 348, 0.331f);
                    else if (name == "apollo11")
                        PNGtools.Export(new Texture2D(1, 1), KSPUtil.ApplicationRootPath + path + "Recolored", name);
                }

                export = false;
            }
        }

        void CheckFiles(List<string> names)
        {
            foreach (string name in names.ToArray())
            {
                if (!File.Exists(KSPUtil.ApplicationRootPath + path + "Recolored/" + name + ".png"))
                {
                    names.Remove(name);
                    missing.Add(name);
                }
            }
        }

        void Recolor(string name, int dx, int dy, float hue = 0)
        {
            Texture2D mask = PNGtools.Load(KSPUtil.ApplicationRootPath + path + "Fixes/" + name + ".png");
            Texture2D tex = Utility.CreateReadable(Resources.FindObjectsOfTypeAll<Texture2D>().FirstOrDefault(t => t.name == name));
            if (mask == null || tex == null) return;


            for (int x = 0; (x < mask.width) && (x + dx < tex.width); x++)
            {
                for (int y = 0; (y < mask.height) && (y + dy < tex.height); y++)
                {
                    Color maskColor = mask.GetPixel(x, y);
                    if (maskColor.maxColorComponent == 0) continue;
                    Color recolored = PNGtools.Lerp(tex.GetPixel(x + dx, y + dy), hue);
                    tex.SetPixel(x + dx, y + dy, recolored);
                }
            }

            PNGtools.Export(tex, KSPUtil.ApplicationRootPath + path + "Recolored/", name);
        }
    }

    public interface NewLoadingScreen
    {
        void UpdateScreens(LoadingScreen.LoadingScreenState screen);
    }

    public class SASSLoadingScreen : NewLoadingScreen
    {
        public static List<string> names = new List<string>(new[] { "WernherVonKerman", "KerbalRecruit", "kerbalspaceodyssey-v2", "KerbalGroundCrew", "KerbalMechanic", "GeneKerman", "BobKerman", "BillKerman", "JebediahKerman", "bumpfist", "loudandclear", "landing", "apollo11" });
        string path = "GameData/StockalikeSolarSystem/Textures/LoadingScreen/";
        static Texture2D StockLogo = null;

        Random rnd = new Random();

        public virtual void UpdateScreens(LoadingScreen.LoadingScreenState screen)
        {
            StockLogo = (Texture2D)screen.screens.FirstOrDefault(t => t.name == "mainMenuBg");
            List<string> newTips = SASSTips();
            List<Texture2D> newScreens = SASSScreens();

            if (newTips.Count > 0)
                screen.tips = newTips.ToArray();

            if (newScreens.Count > 0)
                screen.screens = newScreens.ToArray();
        }

        public List<string> SASSTips()
        {
            List<string> newTips = File.ReadAllLines(KSPUtil.ApplicationRootPath + "GameData/StockalikeSolarSystem/Configs/Extras/LoadingTips.txt").Where(s => !string.IsNullOrEmpty(s)).ToList();
            if (newTips != null && newTips.Count > 0)
            {
                return newTips;
            }

            return new List<string>();
        }

        public List<Texture2D> SASSScreens()
        {
            List<Texture2D> list = new List<Texture2D>();

            bool addKerbals = (FixLoadingScreen.recolorKerbals || FixLoadingScreen.keepGreenKerbals) && (rnd.Next(FixLoadingScreen.LogoChance + FixLoadingScreen.KerbalsChance) < FixLoadingScreen.KerbalsChance);

            if (addKerbals)
            {
                if (FixLoadingScreen.keepGreenKerbals)
                    return new List<Texture2D>();

                foreach (string name in names)
                {
                    Texture2D custom = PNGtools.Load(path + "Recolored/" + name + ".png");

                    if (custom != null && name == "apollo11")
                        custom = Resources.FindObjectsOfTypeAll<Texture2D>().FirstOrDefault(t => t.name == name);

                    if (custom != null)
                        list.Add(custom);
                }
            }

            if (list.Count == 0)
                AddLogo(list);

            if (list.Count > 0)
                return list;

            return new List<Texture2D>();
        }

        void AddLogo(List<Texture2D> list)
        {
            List<string> planets = new[] { "Sun", "Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune" }.ToList();
            if (FixLoadingScreen.keepStockLogo)
                planets.Add("StockLogo");

            Texture2D tex = PNGtools.Load(KSPUtil.ApplicationRootPath + path + "Planets/" + planets[rnd.Next(planets.Count)] + ".png");
            Color[] logo = PNGtools.Load(KSPUtil.ApplicationRootPath + path + "Fixes/SASSlogo.png", false).GetPixels();

            if (tex == null || logo.Length == 0)
            {
                if (StockLogo != null)
                    list.Add(StockLogo);
                return;
            }

            int x = -1;
            int y = 0;
            for (int p = 0; p < logo.Length; p++)
            {
                x++;
                if (x == tex.width)
                {
                    x = 0;
                    y++;
                }
                if (y == tex.height) break;
                Color color = new Color(logo[p].r, logo[p].g, logo[p].b, 1);
                tex.SetPixel(x, y, Color.Lerp(tex.GetPixel(x, y), color, logo[p].a));
            }

            tex.Apply();
            list.Add(tex);
        }
    }

    public static class PNGtools
    {
        public static Texture2D Load(string filePath, bool nullable = true)
        {
            Texture2D tex = null;
            byte[] fileData;

            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2);
                tex.LoadImage(fileData);
            }

            if (tex == null && !nullable)
                return new Texture2D(0, 0);

            return tex;
        }

        public static Color Lerp(Color color, float hue)
        {
            float min = Math.Min(Math.Min(color.r, color.g), color.b);
            color =
            new Color
            (
                color.maxColorComponent,
                (color.maxColorComponent - min) * hue + min,
                min
            );
            return color;
        }

        public static void Export(Texture2D texture, string folder, string name)
        {
            if (texture == null || string.IsNullOrEmpty(folder) || string.IsNullOrEmpty(name)) return;

            if (folder.EndsWith("/"))
                folder = folder.Substring(0, folder.Length - 1);

            byte[] png = texture.EncodeToPNG();

            Directory.CreateDirectory(folder);
            File.WriteAllBytes(folder + "/TEMP.png", png);
            File.Move(folder + "/TEMP.png", folder + "/" + name + ".png");
        }
    }
}