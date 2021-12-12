using System;
using MelonLoader;
using Harmony;
using UnityEngine;
using System.Reflection;
using System.Xml.XPath;
using System.Globalization;
using UnhollowerRuntimeLib;
using System.Collections.Generic;

namespace CardGame
{
	public class Vars
	{
        public static GameObject currentGameObject;
		public static CardGame currentGame;
		public static PlayingCard cardInHand;
		public static bool isPlaying = false;
		public static PlayingCard[] cardListInHand;
		public static int cardListCounter = 0;
	}
}