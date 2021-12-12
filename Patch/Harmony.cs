using System;
using MelonLoader;
using Harmony;
using UnityEngine;
using System.Reflection;
using System.Xml.XPath;
using System.Globalization;
using UnhollowerRuntimeLib;
using ModComponent.Utils;

namespace CardGame
{   
    [HarmonyPatch(typeof(PlayerManager), "InteractiveObjectsProcessInteraction")]
    public class ExecuteInteractActionCardGame
    {
        public static bool Prefix(ref PlayerManager __instance)
        {           
            if (__instance.m_InteractiveObjectUnderCrosshair!=null &&__instance.m_InteractiveObjectUnderCrosshair.name.Contains("GEAR_CardGame"))
            {             
                if (!Vars.isPlaying)
                {                                  
                    Vars.currentGameObject = __instance.m_InteractiveObjectUnderCrosshair;

					Vars.currentGame = InitBoard.Init(Vars.currentGameObject);
					Actions.EnterGameView(Vars.currentGame);
					
					return false;
                }
                else
                {
                    return false;
                }                              
            }
			
            // if object is not the cardgame             
            return true;        
        }
    }

}