using System;
using System.Collections;

using MelonLoader;
using Harmony;
using UnityEngine;
using System.Reflection;
using System.Xml.XPath;
using System.Globalization;
using UnhollowerRuntimeLib;


namespace CardGame
{
	public class Input
	{
		public static RaycastHit hit;
		public static int layerMask = 0;
		public static string hitObjectName;
		public static GameObject hitObject;
		public static PlayingCard hitCard;
		public static CardPlaceholder hitPlaceholder;

		public static void Update()
		{
			if (Vars.isPlaying)
			{
				layerMask |= 1 << 5;
				layerMask |= 1 << 17; // gear layer
				layerMask |= 1 << 14;
				

				///////////////////////////////////
				// Continous raycast while playing
				///////////////////////////////////
				if (Physics.Raycast(GameManager.GetMainCamera().transform.position, GameManager.GetMainCamera().transform.TransformDirection(Vector3.forward), out hit, 2f, layerMask))
				{
					hitObject = hit.collider.gameObject;
					hitObjectName = hit.collider.gameObject.name;							
					
					if (InputManager.GetMouseButtonDown(InputManager.m_CurrentContext, 0)) // Left mouse button            
					{
						if (hitObjectName == "Playmat")
						{
							MelonCoroutines.Start(Actions.shakeMat(Vars.currentGame));
							AudioMain.playClip("cardshove4");
						}

						if (!Vars.cardInHand)
						{
							if (hitObjectName == "controlNewGame")
							{
								MelonLogger.Msg("Starting new game ... ");
								GameLogic.initStack(Vars.currentGame);
								CardMove.updateAllPhysicalPosition(Vars.currentGame, 0, 0.7f);
								GameLogic.dealFromStack(Vars.currentGame);
								CardMove.updateAllPhysicalPosition(Vars.currentGame, 1f, 0.7f);
								MelonCoroutines.Start(CardMove.turnAllCards(Vars.currentGame, 2.5f, 0.5f));
								AudioMain.playClip("cardfanmp3");

								Vars.currentGame.stackInitialized = true;
							}
							else if (hitObjectName == "controlLeaveGame")
							{
								AudioMain.playClip("chipshandle1");
								Actions.ExitGameView(Vars.currentGame);
							}
							else if (hitObjectName == "controlStowCards")
							{
								AudioMain.playClip("cardopenpackage2");
								Actions.ExitGameView(Vars.currentGame);
								
								GameManager.GetPlayerManagerComponent().ProcessPickupItemInteraction(Vars.currentGameObject.GetComponent<GearItem>(), false, false);
							}
							else if (hitObjectName == "INVpositionStack")
							{
								if(Vars.currentGame.cardPositions[0,0]==99)
								{
									Actions.reStack(Vars.currentGame, 0f, 0.2f);
								}
							}
							else if (Vars.currentGame.stackInitialized)
							{
								if (hitCard = hitObject.GetComponent<PlayingCard>())
								{
									if (hitCard.cardColPosition == 0 && hitCard.cardIsOnTop)
									{
										MelonCoroutines.Start(Actions.drawCardFromStack(Vars.currentGame, 0f, 0.5f, false));
									}
									else if (hitCard.cardColPosition != 0 && hitCard.cardIsOnTop)
									{
										MelonCoroutines.Start(Actions.pickUpCard(hitCard, 0.5f));
									}
									else if (hitCard.cardColPosition > 5 && hitCard.cardIsUncovered) // pickup card group
									{
										MelonCoroutines.Start(Actions.pickUpGroup(Vars.currentGame, hitCard, 0.5f));
									}
								}
							}
						}
						else // card in Hand
						{
							if (hitCard = hitObject.GetComponent<PlayingCard>())
							{
								if (hitCard.cardColPosition > 5 && hitCard.cardIsOnTop) // check for normal cols
								{
									if (GameLogic.canLayOnNormalCol(Vars.cardInHand, hitCard))
									{									
										if (Vars.cardListCounter!=0)
										{								
											GameLogic.sendStackToColTop(Vars.currentGame, hitCard.cardColPosition);
										}
                                        else
                                        {
											GameLogic.sendCardToColTop(Vars.currentGame, Vars.cardInHand, hitCard.cardColPosition);
										}
										
										CardMove.updateAllPhysicalPosition(Vars.currentGame, 0f, 0.5f);
										MelonCoroutines.Start(CardMove.turnAllCards(Vars.currentGame, 0.8f, 0.5f));
										Vars.cardInHand = null;
									}
								}
								else if (hitCard.cardColPosition > 1 && hitCard.cardColPosition < 6 && hitCard.cardIsOnTop) // check for ace col
								{
									if (GameLogic.canLayOnAceCol(Vars.cardInHand, hitCard))
									{
										GameLogic.sendCardToColTop(Vars.currentGame, Vars.cardInHand, hitCard.cardColPosition);
										CardMove.updateAllPhysicalPosition(Vars.currentGame, 0f, 0.5f);
										MelonCoroutines.Start(CardMove.turnAllCards(Vars.currentGame, 0.8f, 0.5f));
										Vars.cardInHand = null;
									}
								}
							}
							else if (hitPlaceholder = hitObject.GetComponent<CardPlaceholder>())
							{
								if (hitPlaceholder.placeholderID > 5 && Vars.cardInHand.cardNumValue == 13 && Vars.currentGame.cardPositions[hitPlaceholder.placeholderID, 0] == 99) // if standard col & king in hand & first row on col empty
								{
									GameLogic.sendCardToColTop(Vars.currentGame, Vars.cardInHand, hitPlaceholder.placeholderID);
									CardMove.updateAllPhysicalPosition(Vars.currentGame, 0f, 0.5f);
									MelonCoroutines.Start(CardMove.turnAllCards(Vars.currentGame, 0.8f, 0.5f));
									Vars.cardInHand = null;
								}
								else if (hitPlaceholder.placeholderID > 1 && hitPlaceholder.placeholderID < 6 && Vars.cardInHand.cardNumValue == 14 && Vars.currentGame.cardPositions[hitPlaceholder.placeholderID, 0] == 99) // if ace col & ace in hand & first row on col empty
								{
									GameLogic.sendCardToColTop(Vars.currentGame, Vars.cardInHand, hitPlaceholder.placeholderID);
									CardMove.updateAllPhysicalPosition(Vars.currentGame, 0f, 0.5f);
									MelonCoroutines.Start(CardMove.turnAllCards(Vars.currentGame, 0.8f, 0.5f));
									Vars.cardInHand = null;
								}
							}
						}
					}
				}


				if (InputManager.GetMouseButtonDown(InputManager.m_CurrentContext, 1)) // Right mouse button
				{
					if (Vars.cardInHand)
					{
						if (Vars.cardListCounter != 0) // if multiple cards in hand
						{
							for (int x = 0; x < Vars.cardListCounter; x++)
							{
								Vars.cardListInHand[x].cardObject.transform.parent = Vars.currentGameObject.transform;
								Vars.cardListInHand[x] = null;
							}
						}

						CardMove.updateAllPhysicalPosition(Vars.currentGame, 0f, 0.6f);
						Vars.cardListCounter = 0;
						Vars.cardInHand = null;
					}
					else
					{
						
					}
				}

				if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.Keypad0))
				{
					MelonLogger.Msg("*BOOM*");
					MelonCoroutines.Start(Actions.explode(Vars.currentGame));
				}

				if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.Keypad1))
				{
					MelonLogger.Msg("*RESTACK*");
					
					//MelonCoroutines.Start(Actions.drawCardFromStack(Vars.currentGame, 0.8f, 1f, true));
					Actions.reStack(Vars.currentGame, 0f, 0.2f);
				}				
			}
		}
	}
}