using UnityEngine;
using System;
using System.Reflection;
using System.Linq;
using System.IO;
using System.Collections.Generic;


namespace SASSPlugin
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class ScienceFix : MonoBehaviour
    {
        void Awake()
        {
            foreach (ConfigNode config in GameDatabase.Instance.GetConfigNodes("EXPERIMENT_DEFINITION"))
            {
                if (!config.HasNode("RESULTS") || !config.HasNode("STOCK_RESULTS") || !config.GetNode("STOCK_RESULTS").HasNode("RESULTS")) continue;
                ConfigNode results = config.GetNode("RESULTS");
                ConfigNode stockResults = config.GetNode("STOCK_RESULTS").GetNode("RESULTS");
                string[] stockPlanets = new[] { "Sun", "Moho", "Eve", "Gilly", "Kerbin", "Mun", "Minmus", "Duna", "Ike", "Dres", "Jool", "Laythe", "Vall", "Tylo", "Bop", "Pol", "Eeloo" };

                foreach (string planet in stockPlanets)
                {
                    results.RemoveValuesStartWith(planet);
                }
                results.AddData(stockResults);
            }
        }
    }
}
