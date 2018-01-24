using System.Linq;
using UnityEngine;


namespace StockalikeSolarSystemVisuals
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    class StockalikeSolarSystemVisuals : MonoBehaviour
    {
        void Start()
        {
            if (AssemblyLoader.loadedAssemblies.FirstOrDefault(a => a.name == "scatterer") == null) return;

            CelestialBody jupiter = FlightGlobals.Bodies.FirstOrDefault(b => b.name == "Jupiter");
            CelestialBody saturn = FlightGlobals.Bodies.FirstOrDefault(b => b.name == "Saturn");
            CelestialBody uranus = FlightGlobals.Bodies.FirstOrDefault(b => b.name == "Uranus");
            CelestialBody neptune = FlightGlobals.Bodies.FirstOrDefault(b => b.name == "Neptune");
            jupiter.scaledBody.AddComponent<JupiterFixer>();
            saturn.scaledBody.AddComponent<SaturnFixer>();
            uranus.scaledBody.AddComponent<IceGiantFixer>();
            neptune.scaledBody.AddComponent<IceGiantFixer>();
        }
    }

    class JupiterFixer : MonoBehaviour
    {
        void LateUpdate()
        {
            GameObject sphere = gameObject.GetChild("Sphere");
            if (sphere?.transform?.localScale != null)
            {
                sphere.transform.localScale = new Vector3(sphere.transform.localScale.x, sphere.transform.localScale.x * 0.9f, sphere.transform.localScale.z);
            }
        }
    }

    class SaturnFixer : MonoBehaviour
    {
        void LateUpdate()
        {
            GameObject sphere = gameObject.GetChild("Sphere");
            if (sphere?.transform?.localScale != null)
            {
                sphere.transform.localScale = new Vector3(sphere.transform.localScale.x, sphere.transform.localScale.x * 0.87f, sphere.transform.localScale.z);
            }
        }
    }

    class IceGiantFixer : MonoBehaviour
    {
        void LateUpdate()
        {
            GameObject sphere = gameObject.GetChild("Sphere");
            if (sphere?.transform?.localScale != null)
            {
                sphere.transform.localScale = new Vector3(sphere.transform.localScale.x, sphere.transform.localScale.x * 0.95f, sphere.transform.localScale.z);
            }
        }
    }
}
