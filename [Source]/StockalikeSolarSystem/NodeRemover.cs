using UnityEngine;
using System.Linq;


namespace SASSPlugin
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    class ConfigNodeRemover : MonoBehaviour
    {
        void Start()
        {
            foreach (UrlDir.UrlConfig node in GameDatabase.Instance.GetConfigs("StockalikeSolarSystem").Where(c => c.url != "StockalikeSolarSystem/Settings/StockalikeSolarSystem"))
            {
                node.parent.configs.Remove(node);
            }
        }
    }
}
