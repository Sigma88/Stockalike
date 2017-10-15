using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


namespace SASSPlugin
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    internal class TextureReplacementsLoader : MonoBehaviour
    {
        void Start()
        {
            TextureReplacements.replacements = Directory.GetFiles(TextureReplacements.path);
        }
    }

    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    internal class TextureReplacements : MonoBehaviour
    {
        internal static string[] replacements;
        internal static string path = "GameData/StockalikeSolarSystem/Textures/Replacements/";

        void Start()
        {
            if (AssemblyLoader.loadedAssemblies.FirstOrDefault(a => a.name == "GalacticNeighborhood") != null)
            {
                DestroyImmediate(this);
                return;
            }
            Replace();
        }

        void Update()
        {
            if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
                Replace();
        }

        void Replace()
        {
            var materials = Resources.FindObjectsOfTypeAll<Material>().Where(m => replacements.Contains(path + m?.mainTexture?.name + ".png")).ToArray();

            for (int i = 0; i < materials?.Length; i++)
            {
                Material material = materials[i];
                Texture oldTex = material?.mainTexture;

                if (oldTex?.name != null)
                {
                    Texture newTex = Resources.FindObjectsOfTypeAll<Texture>().FirstOrDefault(t => t.name == path.Remove(0, 9) + oldTex.name);
                    if (newTex != null)
                    {
                        newTex.anisoLevel = oldTex.anisoLevel;
                        newTex.wrapMode = oldTex.wrapMode;

                        material.mainTexture = newTex;
                    }
                }
            }

            var rawImages = Resources.FindObjectsOfTypeAll<RawImage>().Where(r => replacements.Contains(path + r?.texture?.name + ".png")).ToArray();

            for (int i = 0; i < rawImages?.Length; i++)
            {
                RawImage rawImage = rawImages[i];
                Texture oldTex = rawImage?.texture;

                if (oldTex?.name != null)
                {
                    Texture newTex = Resources.FindObjectsOfTypeAll<Texture>().FirstOrDefault(t => t.name == path.Remove(0, 9) + oldTex.name);
                    if (newTex != null)
                    {
                        newTex.anisoLevel = oldTex.anisoLevel;
                        newTex.wrapMode = oldTex.wrapMode;
                        rawImage.texture = newTex;
                    }
                }
            }
        }
    }
}
