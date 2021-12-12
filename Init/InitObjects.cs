using System;
using MelonLoader;
using Harmony;
using UnityEngine;
using System.Reflection;
using System.Xml.XPath;
using System.Globalization;
using UnhollowerRuntimeLib;


namespace CardGame
{
	public class InitObjects : MelonMod
	{
		public static bool Init(CardGame cardGame)
		{
			int cardCreateCounter = 0;
			int controlCreateCounter = 0;
			int placeholderCreateCounter = 0;
			bool createCard = false;
			string tmpString, tmpString2;
            
			if (!cardGame.boardInitialized)
			{
				Transform[] allCards = cardGame.gameObject.GetComponentsInChildren<Transform> (true);

				foreach (Transform singleCard in allCards)
				{
					if (singleCard.gameObject.name.Contains("Playmat"))
					{
                        cardGame.playMat = singleCard.gameObject;                        
                    }
					else if (singleCard.gameObject.name.Contains("control"))
					{
						if (singleCard.gameObject.name.Contains("controlChips"))
						{
							controlCreateCounter = 0;
							createCard = true;
						}
						else if (singleCard.gameObject.name.Contains("controlNewGame"))
						{
							controlCreateCounter = 1;
							createCard = true;
						}
						else if (singleCard.gameObject.name.Contains("controlLeaveGame"))
						{
							controlCreateCounter = 2;
							createCard = true;
						}
						else if (singleCard.gameObject.name.Contains("controlStowCards"))
						{
							controlCreateCounter = 3;
							createCard = true;
						}

						if (createCard == true)
						{
							cardGame.cardControls[controlCreateCounter] = singleCard.gameObject.AddComponent<CardControl>();
							cardGame.cardControls[controlCreateCounter].controlObject = singleCard.gameObject;
							cardGame.cardControls[controlCreateCounter].controlID = controlCreateCounter;
							cardGame.cardControls[controlCreateCounter].controlName = singleCard.gameObject.name;
							cardGame.cardControls[controlCreateCounter].controlTransform = singleCard.gameObject.transform;
							cardGame.cardControls[controlCreateCounter].controlRenderer = singleCard.gameObject.GetComponent<MeshRenderer>();
                     
                            createCard = false;
						}
					}
					else if (singleCard.gameObject.name.Contains("INV"))
					{
						if (singleCard.gameObject.name == "INVpositionStack")
						{
							placeholderCreateCounter = 0;
							createCard = true;
						}
						else if (singleCard.gameObject.name == "INVpositionDraw")
						{
							placeholderCreateCounter = 1;
							createCard = true;
						}						
						else if (singleCard.gameObject.name == "INVpositionAce0")
						{
							placeholderCreateCounter = 2;
							createCard = true;
						}
						else if (singleCard.gameObject.name == "INVpositionAce1")
						{
							placeholderCreateCounter = 3;
							createCard = true;
						}
						else if (singleCard.gameObject.name == "INVpositionAce2")
						{
							placeholderCreateCounter = 4;
							createCard = true;
						}
						else if (singleCard.gameObject.name == "INVpositionAce3")
						{
							placeholderCreateCounter = 5;
							createCard = true;
						}
						else if (singleCard.gameObject.name == "INVpositionCol0")
						{
							placeholderCreateCounter = 6;
							createCard = true;
						}
						else if (singleCard.gameObject.name == "INVpositionCol1")
						{
							placeholderCreateCounter = 7;
							createCard = true;
						}
						else if (singleCard.gameObject.name == "INVpositionCol2")
						{
							placeholderCreateCounter = 8;
							createCard = true;
						}
						else if (singleCard.gameObject.name == "INVpositionCol3")
						{
							placeholderCreateCounter = 9;
							createCard = true;
						}
						else if (singleCard.gameObject.name == "INVpositionCol4")
						{
							placeholderCreateCounter = 10;
							createCard = true;
						}
						else if (singleCard.gameObject.name == "INVpositionCol5")
						{
							placeholderCreateCounter = 11;
							createCard = true;
						}
						else if (singleCard.gameObject.name == "INVpositionCol6")
						{
							placeholderCreateCounter = 12;
							createCard = true;
						}
						else if (singleCard.gameObject.name == "INVCameraDummy")
						{
							cardGame.dummyCamera = singleCard.gameObject;
						}

						if (createCard == true)
						{
							cardGame.cardPlaceholder[placeholderCreateCounter] = singleCard.gameObject.AddComponent<CardPlaceholder>();
							cardGame.cardPlaceholder[placeholderCreateCounter].placeholderObject = singleCard.gameObject;
							cardGame.cardPlaceholder[placeholderCreateCounter].placeholderID = placeholderCreateCounter;
							cardGame.cardPlaceholder[placeholderCreateCounter].placeholderName = singleCard.gameObject.name;
							cardGame.cardPlaceholder[placeholderCreateCounter].placeholderTransform = singleCard.gameObject.transform;

							if(cardGame.cardPlaceholder[placeholderCreateCounter].placeholderName !="INVpositionDraw")
							{
								cardGame.cardPlaceholder[placeholderCreateCounter].placeholderRenderer = singleCard.gameObject.GetComponent<MeshRenderer>();
							}
							
							createCard = false;
						}
                    }
					else if (singleCard.gameObject.name.Contains("PROPS"))
					{
						singleCard.gameObject.SetActive(false);
					}
					else if (singleCard.gameObject.name.Contains("GEAR"))
					{

					}
					else
					{
						cardGame.playingCards[cardCreateCounter] = singleCard.gameObject.AddComponent<PlayingCard>();
						cardGame.playingCards[cardCreateCounter].cardObject = singleCard.gameObject;
						cardGame.playingCards[cardCreateCounter].cardID = cardCreateCounter;
						cardGame.playingCards[cardCreateCounter].cardTransform = singleCard.gameObject.transform;
						cardGame.playingCards[cardCreateCounter].cardName = singleCard.gameObject.name;
						cardGame.playingCards[cardCreateCounter].cardRenderer = singleCard.gameObject.GetComponent<MeshRenderer>();

						cardGame.playingCards[cardCreateCounter].cardIsUncovered = false;
						cardGame.playingCards[cardCreateCounter].cardIsOnTop = false;

						cardGame.playingCards[cardCreateCounter].cardColPosition = 99;
						cardGame.playingCards[cardCreateCounter].cardRowPosition = 99;

						if (cardGame.playingCards[cardCreateCounter].cardName.Contains("Club"))
						{
							cardGame.playingCards[cardCreateCounter].cardSymbol = "Club";
						}
						else if (cardGame.playingCards[cardCreateCounter].cardName.Contains("Heart"))
						{
							cardGame.playingCards[cardCreateCounter].cardSymbol = "Heart";
						}
						else if (cardGame.playingCards[cardCreateCounter].cardName.Contains("Spades"))
						{
							cardGame.playingCards[cardCreateCounter].cardSymbol = "Spades";
						}
						else if (cardGame.playingCards[cardCreateCounter].cardName.Contains("Diamond"))
						{
							cardGame.playingCards[cardCreateCounter].cardSymbol = "Diamond";
						}

							tmpString = cardGame.playingCards[cardCreateCounter].cardName.Substring(0, 1);

						if (tmpString == "A")
						{
							cardGame.playingCards[cardCreateCounter].cardNumValue = 14;
						}
						else if (tmpString == "K")
						{
							cardGame.playingCards[cardCreateCounter].cardNumValue = 13;
						}
						else if (tmpString == "Q")
						{
							cardGame.playingCards[cardCreateCounter].cardNumValue = 12;
						}
						else if (tmpString == "J")
						{
							cardGame.playingCards[cardCreateCounter].cardNumValue = 11;
						}
						else
						{
							tmpString2 = cardGame.playingCards[cardCreateCounter].cardName.Substring(0, 2);
							cardGame.playingCards[cardCreateCounter].cardNumValue = Convert.ToInt32(tmpString2);
						}
						cardCreateCounter++;
					}
				}				
			}			
			return true;
		}
	}
}