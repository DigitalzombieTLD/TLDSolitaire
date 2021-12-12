using System;
using MelonLoader;
using Harmony;
using UnityEngine;
using System.Reflection;
using System.Xml.XPath;
using System.Globalization;
using UnhollowerRuntimeLib;
using ModComponent.Mapper;
using DigitalRuby.Tween;

namespace CardGame
{
    public class CardGameMain : MelonMod
    {
		public static AssetBundle cardAudioBundle;
        public override void OnApplicationStart()
        {
            ClassInjector.RegisterTypeInIl2Cpp<CardGame>();
			ClassInjector.RegisterTypeInIl2Cpp<CardControl>();
			ClassInjector.RegisterTypeInIl2Cpp<CardPlaceholder>();
			ClassInjector.RegisterTypeInIl2Cpp<PlayingCard>();
			ClassInjector.RegisterTypeInIl2Cpp<TweenFactory>();

			cardAudioBundle = AssetBundle.LoadFromFile("Mods\\cardaudio.unity3d");	
			UnityEngine.Object.DontDestroyOnLoad(cardAudioBundle);
		}

		public override void OnUpdate()
		{           
            Input.Update();
		}

		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
			TweenFactory.SceneManagerSceneLoaded();

			string[] clipNamesInBundle = cardAudioBundle.AllAssetNames();
			string tmpClipName;
			AudioMain.cardClips.Clear();

			foreach (string singleClip in clipNamesInBundle)
			{
				tmpClipName = Tools.clipStringCutter(singleClip);
				AudioMain.cardClips.Add(tmpClipName, cardAudioBundle.LoadAsset<AudioClip>(singleClip));	
			}
		}
	}
}
