using UnityEngine;
using Kopernicus;
using Kopernicus.Components;
using Kopernicus.Configuration;
using Kopernicus.OnDemand;
using System.Text;
using System;
using System.Linq;
using KSP.UI.Screens;
using System.Collections.Generic;
using System.IO;
using Random = System.Random;


namespace SASSPlugin
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    class LoadingTips : MonoBehaviour
    {
        List<LoadingScreen.LoadingScreenState> LoadingScreens = new List<LoadingScreen.LoadingScreenState>();
        List<Texture2D> RecolorMasks = new List<Texture2D>();
        Random rnd = new Random();
        bool skip = false;


        void Awake()
        {
            string[] masks = new[] { "WernherVonKerman", "KerbalRecruit", "kerbalspaceodyssey-v2", "KerbalGroundCrew", "KerbalMechanic", "GeneKerman", "BobKerman", "BillKerman", "JebediahKerman" };
            foreach (string mask in masks)
            {
                RecolorMasks.Add(LoadPNG(KSPUtil.ApplicationRootPath + "GameData/StockalikeSolarSystem/Textures/LoadingScreen/Fixes/" + mask + ".png"));
                RecolorMasks.Last().name = mask + "_RecolorMask";
            }
            DontDestroyOnLoad(this);
        }

        void Update()
        {
            if (!skip && LoadingScreen.Instance != null)
            {
                LoadingScreens = LoadingScreen.Instance.Screens;

                foreach (var screen in LoadingScreens)
                {
                    FixScreens(screen.screens);
                }

                skip = true;
            }
            if (HighLogic.LoadedScene == GameScenes.MAINMENU)
            {
                DestroyObject(this);
            }
        }

        void FixScreens(Texture2D[] tex)
        {
            for (int i = 0; i < tex.Length; i++)
            {
                if (tex[i].name == "mainMenuBg")
                    tex[i] = AddLogo();
                else if (tex[i].name == "WernherVonKerman")
                    tex[i] = Recolor(tex[i], 808, 98, 0.4f); // OK
                else if (tex[i].name == "KerbalRecruit")
                    tex[i] = Recolor(tex[i], 934, 239, 0.4f); // OK
                else if (tex[i].name == "kerbalspaceodyssey-v2")
                    tex[i] = Recolor(tex[i], 382, 395, 0.5f); // OK
                else if (tex[i].name == "KerbalGroundCrew")
                    tex[i] = Recolor(tex[i], 486, 373, 0.455f); // OK
                else if (tex[i].name == "KerbalMechanic")
                    tex[i] = Recolor(tex[i], 554, 414, 0.4f); // OK
                else if (tex[i].name == "GeneKerman")
                    tex[i] = Recolor(tex[i], 746, 148, 0.5f); // OK
                else if (tex[i].name == "BobKerman")
                    tex[i] = Recolor(tex[i], 1062, 296, 0.5f); // OK
                else if (tex[i].name == "BillKerman")
                    tex[i] = Recolor(tex[i], 809, 242, 0.4f); // OK
                else if (tex[i].name == "JebediahKerman")
                    tex[i] = Recolor(tex[i], 963, 294, 0.5f); // OK
            }
        }

        Texture2D AddLogo()
        {
            string[] planets = new[] { "Sun", "Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune" };
            Texture2D tex = LoadPNG(KSPUtil.ApplicationRootPath + "GameData/StockalikeSolarSystem/Textures/LoadingScreen/Planets/" + planets[rnd.Next(planets.Length)] + ".png");

            Color[] logo = LoadPNG(KSPUtil.ApplicationRootPath + "GameData/StockalikeSolarSystem/Textures/LoadingScreen/Fixes/SASSlogo.png").GetPixels();

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
            return tex;
        }

        Texture2D Recolor(Texture2D texture, int dx, int dy, float hue = 0)
        {
            Texture2D mask = RecolorMasks.FirstOrDefault(t => t.name == (texture.name + "_RecolorMask"));
            Texture2D tex = Utility.CreateReadable(texture);

            for (int x = 0; (x < mask.width) && (x + dx < tex.width); x++)
            {
                for (int y = 0; (y < mask.height) && (y + dy < tex.height); y++)
                {
                    Color maskColor = mask.GetPixel(x, y);
                    if (maskColor == Color.black) continue;
                    Color recolored = Lerp(tex.GetPixel(x + dx, y + dy), hue);
                    tex.SetPixel(x + dx, y + dy, recolored);
                }
            }

            tex.Apply();

            return tex;
        }

        Color Lerp(Color color, float hue)
        {
            Debug.Log("Sigmalog: " + color + ", " + hue);
            float min = Math.Min(Math.Min(color.r, color.g), color.b);
            color =
            new Color
            (
                color.maxColorComponent,
                (color.maxColorComponent - min) * hue + min,
                min
            );
            Debug.Log("Sigmalog: " + color);
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
