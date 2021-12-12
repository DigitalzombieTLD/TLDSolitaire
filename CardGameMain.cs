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
        public override void OnApplicationStart()
        {
            MelonLogger.Msg("Solitaire Mod test!");
            ClassInjector.RegisterTypeInIl2Cpp<CardGame>();
			ClassInjector.RegisterTypeInIl2Cpp<CardControl>();
			ClassInjector.RegisterTypeInIl2Cpp<CardPlaceholder>();
			ClassInjector.RegisterTypeInIl2Cpp<PlayingCard>();
			ClassInjector.RegisterTypeInIl2Cpp<TweenFactory>();
		}

		public override void OnUpdate()
		{           
            Input.Update();
		}

		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{

			TweenFactory.SceneManagerSceneLoaded();
		}
	}
}
