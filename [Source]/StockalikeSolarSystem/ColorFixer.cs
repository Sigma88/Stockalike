using UnityEngine;
using System;
using Kopernicus;
using System.Linq;

namespace SASSPlugin
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class ColorFixer : MonoBehaviour
    {
        void Start()
        {
            // FIX URANUS RINGS

            // Load OPM's texture 'Urlum_ring'
            Texture2D uranus = PNGtools.Load("GameData/OPM/KopernicusConfigs/OuterPlanets/RingTextures/Urlum_ring.png");

            // If the texture does not exist generate one
            if (uranus == null)
            {
                uranus = new Texture2D(2, 2);

                for (int i = 0; i < 4; i++)
                {
                    uranus.SetPixel(i % 2, i / 2, new Color(1f, 1f, 1f, 0));
                }
            }
            // If the texture is vertical, switch to horizontal
            else if (uranus.height > uranus.width)
            {
                Color[] colors = uranus.GetPixels(0, 0, uranus.width, uranus.height);
                uranus = new Texture2D(uranus.height, uranus.width);

                for (int i = 0; i < uranus.height; i++)
                {
                    uranus.SetPixels(0, i, uranus.width, 1, colors.Skip(i * uranus.width).Take(uranus.width).ToArray());
                }
            }

            // Apply and Rename
            uranus.Apply();
            uranus.name = "UranusRingFix";


            // RECOLOR SATURN RINGS

            // Load OPM's texture 'Sarnus_ring'
            Texture2D saturn = PNGtools.Load("GameData/OPM/KopernicusConfigs/OuterPlanets/RingTextures/Sarnus_ring.png");

            // If the texture does not exist generate one
            if (saturn == null)
            {
                saturn = new Texture2D(2, 2);

                for (int i = 0; i < 4; i++)
                {
                    saturn.SetPixel(i % 2, i / 2, new Color(1f, 1f, 1f, 0));
                }
            }
            else
            {
                Color[] colors = saturn.GetPixels(0, 0, saturn.width, saturn.height);

                // Fix the colors
                for (int x = 0; x < colors.Length; x++)
                {
                    colors[x] =
                    new Color
                    (
                        colors[x].r * 0.895f,
                        colors[x].g * 1.010f,
                        colors[x].b * 1.095f,
                        colors[x].a
                    );
                }

                // If the texture is vertical, switch to horizontal
                if (saturn.height > saturn.width)
                {
                    saturn = new Texture2D(saturn.height, saturn.width);

                    for (int i = 0; i < saturn.height; i++)
                    {
                        saturn.SetPixels(0, i, saturn.width, 1, colors.Skip(i * saturn.width).Take(saturn.width).ToArray());
                    }
                }
                else
                {
                    saturn.SetPixels(0, 0, saturn.width, saturn.height, colors);
                }

                // Move F Ring
                Color[] RingF = saturn.GetPixels(saturn.width - saturn.width / 16, 0, saturn.width / 16, saturn.height).Reverse().ToArray();
                
                Color[] Rings = saturn.GetPixels(0, 0, saturn.width * 15 / 16, saturn.height);
                saturn.SetPixels(saturn.width / 16, 0, Rings.Length / saturn.height, saturn.height, Rings);
                saturn.SetPixels(0, 0, saturn.width / 16, saturn.height, RingF);
                PNGtools.Export(saturn, "GameData/StockalikeSolarSystem", "saturn_rings");
            }

            // Apply and Rename
            saturn.Apply();
            saturn.name = "SaturnRingRecolor";


            // RECOLOR JUPITER

            // Load Revolting Jool Texture
            Texture2D jupiter = PNGtools.Load("GameData/RevoltingJoolRecolor/Textures/Revolting_Jool_Color.png");

            // If the texture does not exist generate one
            if (jupiter == null)
            {
                jupiter = new Texture2D(2, 2);
                for (int i = 0; i < 4; i++)
                {
                    jupiter.SetPixel(i % 2, i / 2, new Color(0.463f, 0.259f, 0.173f, 1));
                }
            }
            else
            {
                // Fix the colors
                Color[] colors = jupiter.GetPixels();

                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i] =
                        new Color
                        (
                            Math.Max(colors[i].r, colors[i].g),
                            Math.Min(colors[i].r, colors[i].g) * 0.6f + colors[i].b * 0.4f,
                            colors[i].b,
                            colors[i].a
                        );
                }

                // Set the colors
                jupiter.SetPixels(colors);
            }

            // Skip if the texture still does not exist
            if (jupiter != null)
            {
                // Apply and Rename
                jupiter.Apply();
                jupiter.name = "JupiterRecolor";
            }


            // RECOLOR NEPTUNE

            // Generate a temporary texture
            Texture2D neptune = neptune = new Texture2D(2, 2);

            // Color it blue in case the real texture is not found later
            for (int i = 0; i < 4; i++)
            {
                neptune.SetPixel(i % 2, i / 2, new Color(0.300f, 0.500f, 1.000f, 1));
            }

            // Apply and Rename
            neptune.Apply();
            neptune.name = "NeptuneRecolor";
        }
    }

    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class NeptuneFixer : MonoBehaviour
    {
        void Start()
        {
            Texture2D texture = Utility.CreateReadable(Resources.FindObjectsOfTypeAll<Texture>().FirstOrDefault(t => t.name == "GregroxNeptune/Neptune_Colorbig") as Texture2D);
            Texture2D neptune = Resources.FindObjectsOfTypeAll<Texture2D>().FirstOrDefault(t => t.name == "NeptuneRecolor");

            if (texture != null && neptune != null)
            {
                // Fix the color
                Color[] colors = texture.GetPixels();

                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i] =
                        new Color
                        (
                            (float)(Math.Pow(colors[i].r, 3) + Math.Pow(colors[i].b, 3) - 1),
                            (float)(Math.Pow(colors[i].g / colors[i].b, 3) * 0.2f + (Math.Pow(colors[i].r, 3) + Math.Pow(colors[i].b, 3) - 1) * 0.8f),
                            1,
                            1
                        );
                }

                // Set the colors and apply
                neptune.Resize(texture.width, texture.height);
                neptune.SetPixels(colors);
                neptune.Apply();
            }
        }
    }
}
