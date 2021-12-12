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
	public class InitBoard
	{
		public static CardGame Init(GameObject cardGameObject)
		{
			CardGame cardGame = ComponentUtils.GetOrCreateComponent<CardGame>(cardGameObject);

            if(!cardGame.boardInitialized)
            {
                cardGame.cardPositions = new int[13, 52];
                cardGame.playingCards = new PlayingCard[52];
                cardGame.cardControls = new CardControl[4];
                cardGame.cardPlaceholder = new CardPlaceholder[13];

                InitObjects.Init(cardGame);
                cardGame.boardInitialized = true;
            }
			
			return cardGame;
		}
	}
}