using System;
using MelonLoader;
using Harmony;
using UnityEngine;
using System.Reflection;
using System.Xml.XPath;
using System.Globalization;
using UnhollowerRuntimeLib;
using System.Collections.Generic;
using ModComponent.AssetLoader;
using System.Collections;

namespace CardGame
{
	public static class AudioMain
	{
		public static AudioListener playerListener;
		public static AudioSource playerSource;
		public static AudioSource cardSource;
		public static Dictionary<string, AudioClip> cardClips = new Dictionary<string, AudioClip>();

		public static void init()
		{
			Camera mainCam = GameManager.GetMainCamera();

			if (mainCam != null)
			{
				playerListener = mainCam.GetComponent<AudioListener>();
				playerSource = mainCam.GetComponent<AudioSource>();
				
				if (playerListener == null)
				{
					playerListener = mainCam.gameObject.AddComponent<AudioListener>();
					AudioListener.volume = Settings.options.cardGameVolume;
				}

				if (playerSource == null)
				{
					playerSource = mainCam.gameObject.AddComponent<AudioSource>();
				}
			}
		}		

		public static void playClip(string clipName)
		{		
			MelonCoroutines.Start(Play(clipName));
		}

		public static IEnumerator Play(string clipName)
		{
			//AudioClip tempClip = CardGameMain.cardAudioBundle.LoadAsset<AudioClip>("cardfan1");			
			AudioClip tempClip = cardClips[clipName];
			cardSource.volume = 1f;
			cardSource.PlayOneShot(tempClip, 1.0f);
			yield return null;
		}
	}
}