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
    class FixLoadingScreen : MonoBehaviour
    {
        List<LoadingScreen.LoadingScreenState> LoadingScreens = new List<LoadingScreen.LoadingScreenState>();

        string path = "GameData/StockalikeSolarSystem/Textures/LoadingScreen/";
        List<string> names = new List<string>(new[] { "WernherVonKerman", "KerbalRecruit", "kerbalspaceodyssey-v2", "KerbalGroundCrew", "KerbalMechanic", "GeneKerman", "BobKerman", "BillKerman", "JebediahKerman" });
        List<string> missing = new List<string>();

        Random rnd = new Random();
        bool skip = false;
        bool skip2 = false;
        bool export = false;


        void Awake()
        {
            DontDestroyOnLoad(this);
        }

        void Update()
        {
            if (!skip && LoadingScreen.Instance != null)
            {
                CheckFiles(names);
                LoadingScreens = LoadingScreen.Instance.Screens;

                foreach (var screen in LoadingScreens)
                {
                    string[] newTips = File.ReadAllLines(KSPUtil.ApplicationRootPath + "GameData/StockalikeSolarSystem/Configs/Extras/LoadingTips.txt").ToList().Where(s => !string.IsNullOrEmpty(s)).ToArray();
                    if (screen.tips.Length > 1 && newTips != null && newTips.Length > 0)
                        screen.tips = newTips;
                    screen.screens = FixScreens(screen.screens);
                }

                skip = true;
            }

            if (skip && !skip2 && missing.Count > 0)
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
                }

                export = false;
            }

            if (HighLogic.LoadedScene == GameScenes.MAINMENU)
            {
                DestroyObject(this);
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

        Texture2D[] FixScreens(Texture2D[] screens)
        {
            List<Texture2D> list = new List<Texture2D>();

            foreach (Texture2D texture in screens)
            {
                if (texture.name == "mainMenuBg")
                {
                    AddLogo(list);
                }
                else
                {
                    Texture2D custom = LoadPNG(path + "Recolored/" + texture.name + ".png");
                    if (custom != null)
                        list.Add(custom);
                }
            }

            if (list.Count > 0)
                screens = list.ToArray();

            return screens;
        }

        void AddLogo(List<Texture2D> list)
        {
            string[] planets = new[] { "Sun", "Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune" };

            Texture2D tex = LoadPNG(KSPUtil.ApplicationRootPath + path + "Planets/" + planets[rnd.Next(planets.Length)] + ".png");
            Color[] logo = LoadPNG(KSPUtil.ApplicationRootPath + path + "Fixes/SASSlogo.png").GetPixels();

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

        void Recolor(string name, int dx, int dy, float hue = 0)
        {
            Texture2D mask = LoadPNG(KSPUtil.ApplicationRootPath + path + "Fixes/" + name + ".png");
            Texture2D tex = Utility.CreateReadable(Resources.FindObjectsOfTypeAll<Texture2D>().FirstOrDefault(t => t.name == name));
            if (mask == null || tex == null) return;


            for (int x = 0; (x < mask.width) && (x + dx < tex.width); x++)
            {
                for (int y = 0; (y < mask.height) && (y + dy < tex.height); y++)
                {
                    Color maskColor = mask.GetPixel(x, y);
                    if (maskColor.maxColorComponent == 0) continue;
                    Color recolored = Lerp(tex.GetPixel(x + dx, y + dy), hue);
                    tex.SetPixel(x + dx, y + dy, recolored);
                }
            }

            byte[] png = tex.EncodeToPNG();

            Directory.CreateDirectory(KSPUtil.ApplicationRootPath + path + "Recolored");
            File.WriteAllBytes(KSPUtil.ApplicationRootPath + path + "Recolored/TEMP.png", png);
            File.Move(KSPUtil.ApplicationRootPath + path + "Recolored/TEMP.png", KSPUtil.ApplicationRootPath + path + "Recolored/" + name + ".png");
        }

        Color Lerp(Color color, float hue)
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

        Texture2D LoadPNG(string filePath)
        {
            Texture2D tex = null;
            byte[] fileData;

            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2);
                tex.LoadImage(fileData);
            }
            return tex;
        }
    }
}
