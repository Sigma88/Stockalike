using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KSP.Localization;
using Kopernicus.Configuration;


namespace SASSPlugin
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    internal class SigmaBinary : MonoBehaviour
    {
        /// <summary> List of all objects of type 'Body'. </summary>
        internal static List<CelestialBody> ListOfBodies = new List<CelestialBody>();

        void Start()
        {
            Kopernicus.Events.OnBodyPostApply.Add(AddBodyToList);
            Kopernicus.Events.OnLoaderLoadedAllBodies.Add(DescriptionFixer);
        }

        void AddBodyToList(Body body, ConfigNode node)
        {
            ListOfBodies.Add(body.generatedBody.celestialBody);
        }

        void DescriptionFixer(Loader ldr, ConfigNode cfgn)
        {
            for (int i = 0; i < ListOfBodies?.Count; i++)
            {
                CelestialBody body = ListOfBodies[i];
                string description = body?.bodyDescription;
                while(description?.Contains("<<") == true && description?.Contains(">>") == true)
                {
                    int start = description.IndexOf("<<");
                    int end = description.IndexOf(">>");

                    if (end < start)
                    {
                        description = description.Substring(0, end) + description.Substring(end + 2);
                    }
                    else
                    {
                        string name = "";

                        try
                        {
                            name = description.Substring(start + 2, end - start - 2);
                            if (name == "Earth") name = "Kerbin";
                            name = ListOfBodies.FirstOrDefault(b => b.name == name).bodyDisplayName;
                        }
                        catch
                        {
                            Debug.Log("[SigmaLog SASS] Localization: could not localize following string...");
                            Debug.Log("[SigmaLog SASS] String: " + description);
                        }

                        description = description.Replace(description.Substring(start, end - start + 2), name);
                    }

                    if (description.Contains("#SASS-Iron-"))
                        description = description.Replace("#SASS-Iron-RealSize", Localizer.Format("#SASS-Iron-RealSize")).Replace("#SASS-Iron-StockSize", Localizer.Format("#SASS-Iron-StockSize"));

                    body.bodyDescription = description;
                }
            }
        }
    }
}
