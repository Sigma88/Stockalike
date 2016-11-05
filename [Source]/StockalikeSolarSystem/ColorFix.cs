using UnityEngine;
using Kopernicus;
using Kopernicus.OnDemand;
using System;


namespace SASSPlugin
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class LateFixes : MonoBehaviour
    {
        void Start()
        {
            foreach (CelestialBody b in FlightGlobals.Bodies)
            {
                Debug.Log("SigmaLog: body " + b.name);
                if (b.name == "TEST")
                {
                    Debug.Log("SigmaLog: 1");
                    Renderer renderer = b.scaledBody.GetComponent<Renderer>();
                    Debug.Log("SigmaLog: renderer = " + renderer);
                    Material material = renderer.material;
                    Debug.Log("SigmaLog: material = " + material);
                    Texture2D MainTex = Utility.CreateReadable(material.GetTexture("_MainTex") as Texture2D);
                    Debug.Log("SigmaLog: MainTex = " + MainTex);

                    for (int x = 0; x < MainTex.width; x++)
                    {
                        for (int y = 0; y < MainTex.height; y++)
                        {
                            Color color = MainTex.GetPixel(x, y);

                            color = new Color(color.r * color.r, (color.r * color.r * 3 / 4) + (color.g * color.g / 4), color.b * color.b, color.a);
                            MainTex.SetPixel(x, y, color);
                        }
                    }

                    Debug.Log("SigmaLog: 2");
                    MainTex.Apply();
                    Debug.Log("SigmaLog: 3");
                    b.scaledBody.GetComponent<Renderer>().material.SetTexture("_MainTex", MainTex);
                    Debug.Log("SigmaLog: 4");
                    if (OnDemandStorage.useOnDemand)
                    {
                        ScaledSpaceDemand demand = b.scaledBody.GetComponent<ScaledSpaceDemand>();
                        demand.texture = MainTex.name;
                    }
                }
            }
        }
    }
}
