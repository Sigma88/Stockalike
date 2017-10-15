using UnityEngine;


namespace SASSPlugin
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    class ConfigNodeRemover : MonoBehaviour
    {
        void Start()
        {
            var nodes = GameDatabase.Instance.GetConfigs("StockalikeSolarSystem");

            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].url != "StockalikeSolarSystem/Settings/StockalikeSolarSystem")
                    nodes[i].parent.configs.Remove(nodes[i]);
            }
        }
    }
}
