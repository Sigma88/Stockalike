using UnityEngine;
using Kopernicus;
using Kopernicus.Components;
using Kopernicus.Configuration;
using Kopernicus.OnDemand;
using System.Text;
using System;
using System.Reflection;
using System.Linq;
using KSP.UI.Screens;


namespace SASSPlugin
{
    [ExternalParserTarget("ScaledVersion")]
    public class SigmaLoader : ExternalParserTargetLoader, IParserEventSubscriber
    {
        void IParserEventSubscriber.Apply(ConfigNode node)
        {
        }

        void IParserEventSubscriber.PostApply(ConfigNode node)
        {
            if (Loader.currentBody.name == "Saturn")
            {
                Texture2D ringtext = Utility.CreateReadable(Loader.currentBody.rings.First().ring.texture);
                ringtext.name = "SaturnRingRecolor";
                ringtext.Apply();
                Loader.currentBody.rings.First().ring.texture = ringtext;
            }
        }
    }
    
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class ColorFixer : MonoBehaviour
    {
        void Start()
        {
            // Recolor Saturn Rings

            Texture2D RingText = Utility.CreateReadable(Resources.FindObjectsOfTypeAll<Texture2D>().First(t => t.name == "OPM/KopernicusConfigs/OuterPlanets/RingTextures/Sarnus_ring"));
            Texture2D MainText = Resources.FindObjectsOfTypeAll<Texture2D>().First(t => t.name == "SaturnRingRecolor");

            for (int x = 0; x < RingText.height; x++)
            {
                Color color = RingText.GetPixel(0, x);

                color =
                    new Color
                    (
                        color.r * 0.895f,
                        color.g * 1.010f,
                        color.b * 1.095f,
                        color.a * 0.5f
                    );

                MainText.SetPixel(0, x, color);
            }

            MainText.Apply();




            // Recolor Jool Texture

            CelestialBody jupiter = FlightGlobals.Bodies.First(b => b.transform.name == "Jupiter");
            if (jupiter != null)
            {
                Texture2D MainTex = Utility.CreateReadable(jupiter.scaledBody.GetComponent<Renderer>().material.GetTexture("_MainTex") as Texture2D);
                
                for (int x = 0; x < MainTex.width; x++)
                {
                    for (int y = 0; y < MainTex.height; y++)
                    {
                        Color color = MainTex.GetPixel(x, y);

                        color =
                            new Color
                            (
                                Math.Max(color.r, color.g),
                                Math.Min(color.r, color.g) * 0.6f + color.b * 0.4f,
                                color.b,
                                color.a
                            );

                        MainTex.SetPixel(x, y, color);
                    }
                }

                MainTex.Apply();
                MainTex.name = "RevoltingJoolRecolor";
                jupiter.scaledBody.GetComponent<Renderer>().material.SetTexture("_MainTex", MainTex);

                if (OnDemandStorage.useOnDemand)
                {
                    ScaledSpaceDemand demand = jupiter.scaledBody.GetComponent<ScaledSpaceDemand>();
                    demand.texture = MainTex.name;
                }
            }
        }
        void LateUpdate()
        {
            if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
            {
                foreach (RDArchivesController controller in Resources.FindObjectsOfTypeAll<RDArchivesController>())
                    controller.gameObject.AddOrGetComponent<RnDFixer>();
            }
        }
    }

    public class RnDFixer : MonoBehaviour
    {
        void Start()
        {
            Texture2D jupiter = Resources.FindObjectsOfTypeAll<Texture2D>().First(t => t.name == "RevoltingJoolRecolor");
            RDPlanetListItemContainer item = Resources.FindObjectsOfTypeAll<RDPlanetListItemContainer>().First(i => i.name == "Jupiter");

            if (jupiter != null && item != null)
                item.planet.GetComponent<Renderer>().material.mainTexture = jupiter;
        }
    }
}