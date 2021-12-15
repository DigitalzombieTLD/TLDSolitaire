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

	[HarmonyPatch(typeof(PlayerManager), "StartPlaceMesh", new Type[] { typeof(GameObject), typeof(float), typeof(PlaceMeshFlags) })]
	public class DisableCardGameObjectsForMoving
	{
		public static void Prefix(ref PlayerManager __instance, ref GameObject objectToPlace)
		{
			if (objectToPlace.name.Contains("GEAR_CardGame"))
			{
				for (int x = 0; x < objectToPlace.transform.childCount; x++)
				{
					if (!objectToPlace.transform.GetChild(x).gameObject.name.Contains("Playmat"))
					{
						objectToPlace.transform.GetChild(x).gameObject.SetActive(false);
					}					
				}
			}
		}
	}

	[HarmonyPatch(typeof(PlayerManager), "RestoreOriginalTint")]
	public class EnableCardGameObjectsForMoving
	{
		public static void Prefix(ref PlayerManager __instance)
		{
			if (__instance.m_ObjectToPlaceGearItem && __instance.m_ObjectToPlaceGearItem.name.Contains("GEAR_CardGame"))
			{
				for (int x = 0; x < __instance.m_ObjectToPlaceGearItem.transform.childCount; x++)
				{
					if (!__instance.m_ObjectToPlaceGearItem.transform.GetChild(x).gameObject.name.Contains("Playmat")&&!__instance.m_ObjectToPlaceGearItem.transform.GetChild(x).gameObject.name.Contains("PROPSPlayingCards_Box")&&!__instance.m_ObjectToPlaceGearItem.transform.GetChild(x).gameObject.name.Contains("control"))
					{
						__instance.m_ObjectToPlaceGearItem.transform.GetChild(x).gameObject.SetActive(true);
					}
				}
			}
		}
	}
}