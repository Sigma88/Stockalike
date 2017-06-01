using UnityEngine;
using System;


namespace SASSPlugin
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class ColorFixer : MonoBehaviour
    {
        void Start()
        {
            // RECOLOR SATURN RINGS

            // Load OPM's texture 'Sarnus_ring'
            Texture2D saturn = PNGtools.Load("GameData/OPM/KopernicusConfigs/OuterPlanets/RingTextures/Sarnus_ring.png");

            // Skip if the texture does not exist
            if (saturn != null)
            {
                Color[] colors = saturn.GetPixels(0, 0, saturn.width, 1);

                // If the texture is vertical, switch to horizontal
                if (saturn.height > saturn.width)
                {
                    colors = saturn.GetPixels(0, 0, 1, saturn.height);
                    saturn = new Texture2D(saturn.height, 1);
                }

                // Fix the colors
                for (int x = 0; x < colors.Length; x++)
                {
                    colors[x] =
                    new Color
                    (
                        colors[x].r * 0.895f,
                        colors[x].g * 1.010f,
                        colors[x].b * 1.095f,
                        colors[x].a * 0.5f
                    );
                }

                // Set the colors, apply and rename
                saturn.SetPixels(0, 0, colors.Length, 1, colors);
                saturn.Apply();
                saturn.name = "SaturnRingRecolor";
            }


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

            // Load Revolting Jool Texture
            Texture2D neptune = PNGtools.Load("GameData/GregroxNeptune/Neptune_Colorbig.dds");

            // If the texture does not exist generate one
            if (neptune == null)
            {
                neptune = new Texture2D(2, 2);
                for (int i = 0; i < 4; i++)
                {
                    neptune.SetPixel(i % 2, i / 2, new Color(0.300f, 0.500f, 1.000f, 1));
                }
            }
            else
            {
                // Fix the color
                Color[] colors = neptune.GetPixels();

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

                    // Set the colors
                    neptune.SetPixels(colors);
                }
            }

            // Skip if the texture still does not exist
            if (neptune != null)
            {
                // Apply and Rename
                neptune.Apply();
                neptune.name = "NeptuneRecolor";
            }
        }
    }
}
