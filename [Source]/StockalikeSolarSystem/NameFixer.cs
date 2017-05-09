using System.Collections.Generic;
using System.Collections;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using Random = System.Random;

namespace SASSPlugin
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    class KerbalRenamer : MonoBehaviour
    {
        static Random rnd = new Random();

        void Awake()
        {
            DontDestroyOnLoad(this);
            GameEvents.onKerbalAdded.Add(new EventData<ProtoCrewMember>.OnEvent(OnKerbalAdded));
        }

        void OnKerbalAdded(ProtoCrewMember kerbal)
        {
            ConfigNode Names = GameDatabase.Instance.GetConfigNodes("StockalikeSolarSystemNames").FirstOrDefault();
            if (Names == null || !Names.HasNode("FULL") || !Names.HasNode("FIRST") || !Names.GetNode("FIRST").HasValues(new[] { "coolM", "coolF" }) || !Names.HasNode("LAST") || !Names.GetNode("LAST").HasValues(new[] { "cool", "boring" })) return;
            ConfigNode specialNames = Names.GetNode("FULL");
            string[] coolM = Names.GetNode("FIRST").GetValues("coolM");
            string[] coolF = Names.GetNode("FIRST").GetValues("coolF");
            string[] lastCool = Names.GetNode("LAST").GetValues("cool");
            string[] lastBoring = Names.GetNode("LAST").GetValues("boring");



            if (specialNames.HasValue(kerbal.name.Replace(' ', '_')))
                kerbal.ChangeName(specialNames.GetValue(kerbal.name.Replace(' ', '_')));
            else
            {
                bool coolFirst = false;
                bool coolLast = false;


                // Cool First
                if (Pick(100) < 20)
                {
                    coolFirst = true;
                    if (kerbal.gender == ProtoCrewMember.Gender.Male)
                        kerbal.ChangeName(Pick(coolM) + " Kerman");
                    else
                        kerbal.ChangeName(Pick(coolF) + " Kerman");
                }

                // Cool Last
                if (Pick(100) < 20)
                    coolLast = true;

                if (coolLast)
                    kerbal.ChangeName(kerbal.name.Replace("Kerman", Pick(lastCool)));
                else
                    kerbal.ChangeName(kerbal.name.Replace("Kerman", Pick(lastBoring)));

                // Veteran
                if (coolFirst && coolLast)
                    kerbal.veteran = true;
                else
                    kerbal.veteran = false;
            }
        }

        string Pick(string[] list)
        {
            int n = rnd.Next(list.Length);
            return list[n];
        }
        int Pick(int max)
        {
            return rnd.Next(max);
        }
    }
}
