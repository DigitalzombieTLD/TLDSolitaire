using System;
using MelonLoader;
using Harmony;
using UnityEngine;
using System.Reflection;
using System.Xml.XPath;
using System.Globalization;
using UnhollowerRuntimeLib;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace CardGame
{
	public class GameLogic
	{
		public static void initStack(CardGame cardGame)
		{
			MelonLogger.Msg("Shuffling cards ... ");
			int[] cardOrderOnStack = new int[52];			

			// Fill position array with blanks (int 99)
			for (int x = 0; x < 13; x++)
			{
				for (int y = 0; y < 52; y++)
				{
					cardGame.cardPositions[x, y] = 99;
				}
			}

			// Randomize stack order
			for (int i = 0; i < 52; i++)
			{
				cardOrderOnStack[i] = i;
			}
			System.Random random = new System.Random();
			cardOrderOnStack = cardOrderOnStack.OrderBy(x => random.Next()).ToArray();

			Quaternion defaultRotation = new Quaternion(0, 0, 0, 0);
			Quaternion localRotation = Quaternion.Euler(90, 90, 90);

			// Move cards in random order from position array to card stack array
			for (int cardCounter = 0; cardCounter < 52; cardCounter++)
			{
				cardGame.cardPositions[0, cardCounter] = cardOrderOnStack[cardCounter];
				cardGame.playingCards[cardOrderOnStack[cardCounter]].cardIsUncovered = false;
				cardGame.playingCards[cardOrderOnStack[cardCounter]].cardColPosition = 0;
				cardGame.playingCards[cardOrderOnStack[cardCounter]].cardRowPosition = cardCounter;
				cardGame.playingCards[cardOrderOnStack[cardCounter]].cardIsOnTop = false;
			}			
		}

		public static bool canLayOnNormalCol(PlayingCard cardInHand, PlayingCard targetCard)
		{
			if (cardInHand.cardNumValue == targetCard.cardNumValue - 1)
			{
				if(cardInHand.cardSymbol == "Club") // Black
				{
					if(targetCard.cardSymbol == "Spades" || targetCard.cardSymbol == "Club")
					{
						return false;
					}
					return true;
				}
				else if (cardInHand.cardSymbol == "Diamond") // Red
				{
					if (targetCard.cardSymbol == "Heart" || targetCard.cardSymbol == "Diamond")
					{
						return false;
					}
					return true;
				}
				else if (cardInHand.cardSymbol == "Spades") // Black
				{
					if (targetCard.cardSymbol == "Club" || targetCard.cardSymbol == "Spades")
					{
						return false;
					}
					return true;
				}
				else if (cardInHand.cardSymbol == "Heart") // Red
				{
					if (targetCard.cardSymbol == "Diamond" || targetCard.cardSymbol == "Heart")
					{
						return false;
					}
					return true;
				}
			}

			return false;
		}

		public static bool canLayOnAceCol(PlayingCard cardInHand, PlayingCard targetCard)
		{			
			if (cardInHand.cardNumValue == targetCard.cardNumValue + 1 || cardInHand.cardNumValue == targetCard.cardNumValue -12)
			{
				if (cardInHand.cardSymbol == targetCard.cardSymbol)
				{					
					return true;
				}
			}			
			return false;
		}

		public static void dealFromStack(CardGame cardGame)
		{
			MelonLogger.Msg("Dealing cards ... ");

			int cardCounter = 51;

			// Single card on col 1
			cardGame.cardPositions[1, 0] = cardGame.cardPositions[0, cardCounter];
			cardGame.cardPositions[0, cardCounter] = 99;
			cardGame.playingCards[cardGame.cardPositions[1, 0]].cardIsOnTop = true;
			cardGame.playingCards[cardGame.cardPositions[1, 0]].cardColPosition = 1;
			cardGame.playingCards[cardGame.cardPositions[1, 0]].cardRowPosition = 0;
			cardCounter--;

			// Cols 1 - 8
			for (int placeID = 6; placeID < 13; placeID++)
			{
				for (int row = 0; row < placeID - 5; row++)
				{
					cardGame.cardPositions[placeID, row] = cardGame.cardPositions[0, cardCounter];
					cardGame.cardPositions[0, cardCounter] = 99;
					cardGame.playingCards[cardGame.cardPositions[placeID, row]].cardColPosition = placeID;
					cardGame.playingCards[cardGame.cardPositions[placeID, row]].cardRowPosition = row;

					if(row==placeID-6)
					{
						cardGame.playingCards[cardGame.cardPositions[placeID, row]].cardIsOnTop = true;						
					}

					cardCounter--;
				}
			}
			// Magic number 22
			cardGame.playingCards[cardGame.cardPositions[0, 22]].cardIsOnTop = true;
		}

		public static PlayingCard getCardOnColTop(CardGame cardGame, int placeID)
		{
			int cardCounter = 51;

			while(cardGame.cardPositions[placeID, cardCounter] == 99)
			{
				cardCounter--;				
			}

			if(cardGame.playingCards[cardGame.cardPositions[placeID, cardCounter]].cardIsOnTop)
			{
				return cardGame.playingCards[cardGame.cardPositions[placeID, cardCounter]];
			}		

			return null;
		}

		public static void sendCardToColTop(CardGame cardGame, PlayingCard playingCard, int newPlaceID)
		{
            // Remove from original position
            int oldRow = playingCard.cardRowPosition;
            int oldPlace = playingCard.cardColPosition;    

			int rowCounter = 0;

			// Set next card on old location to onTop
			if(oldRow!=0)
			{
				cardGame.playingCards[cardGame.cardPositions[oldPlace, oldRow-1]].cardIsOnTop = true;
			}

			while (cardGame.cardPositions[newPlaceID, rowCounter] != 99)
			{
				cardGame.playingCards[cardGame.cardPositions[newPlaceID, rowCounter]].cardIsOnTop = false; // No card is on top anymore
				rowCounter++;
			}

			// Finish it up
			cardGame.cardPositions[newPlaceID, rowCounter] = playingCard.cardID;
			cardGame.cardPositions[oldPlace, oldRow] = 99;
			playingCard.cardRowPosition = rowCounter;
			playingCard.cardColPosition = newPlaceID;
			playingCard.cardIsOnTop = true;
		}

		public static void sendStackToColTop(CardGame cardGame, int newPlaceID)
		{
			int oldRow;
			int oldPlace;
			PlayingCard oldNeighbour;

			int rowCounter = 0;

			// No card is on top anymore (destination)
			while (cardGame.cardPositions[newPlaceID, rowCounter] != 99)
			{
				cardGame.playingCards[cardGame.cardPositions[newPlaceID, rowCounter]].cardIsOnTop = false; 
				rowCounter++;
			}

			// Move cards from hand to location
			for (int x = 0; x < Vars.cardListCounter; x++)
			{
				Vars.cardListInHand[x].cardObject.transform.parent = Vars.currentGameObject.transform;

				oldRow = Vars.cardListInHand[x].cardRowPosition;
				oldPlace = Vars.cardListInHand[x].cardColPosition;

				cardGame.cardPositions[newPlaceID, rowCounter] = Vars.cardListInHand[x].cardID;
				cardGame.cardPositions[oldPlace, oldRow] = 99;

				Vars.cardListInHand[x].cardObject.transform.parent = Vars.currentGameObject.transform;
				Vars.cardListInHand[x].cardRowPosition = rowCounter;
				Vars.cardListInHand[x].cardColPosition = newPlaceID;	
				Vars.cardListInHand[x] = null;
				rowCounter++;

				// old stack got a new king baby
				if(x == 0 && oldRow != 0)
                {
					oldNeighbour = cardGame.playingCards[cardGame.cardPositions[oldPlace, oldRow - 1]];
					oldNeighbour.cardIsOnTop = true;
				}
			}

			// new stack got a new king ... baby
			cardGame.playingCards[cardGame.cardPositions[newPlaceID, rowCounter-1]].cardIsOnTop = true;
			
			Vars.cardListCounter = 0;
		}
	}
}