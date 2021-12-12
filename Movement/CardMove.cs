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
using DigitalRuby.Tween;

namespace CardGame
{
	public class CardMove
	{
		public static void updateAllPhysicalPosition(CardGame cardGame, float preWaitTime, float speed)
		{
			float offsetHeight;
			float offsetHeightMod = 0.0015f;
			float offsetRow;
			float offsetRowMod = 0.023f;
			Vector3 targetPosition;

			if (Vars.cardInHand)
			{
				Vars.cardInHand.cardObject.transform.parent = cardGame.gameObject.transform;
			}

			for (int targetPlaceID = 0; targetPlaceID < 13; targetPlaceID++)
			{
				for (int order = 0; order < 52; order++)
				{
					int card = cardGame.cardPositions[targetPlaceID, order];
					if (card != 99)
					{
						// Determine height offset
						offsetHeight = (order + 1) * offsetHeightMod;

						// Determine row offset
						// Stack / Drawn / Ace 0 / Ace 1 / Ace 2 / Ace 3
						if (targetPlaceID < 6)
						{
							offsetRow = 0f;
						}
						else
						{
							offsetRow = order * offsetRowMod;
						}
						UnityEngine.Object.Destroy(cardGame.playingCards[card].gameObject.GetComponent<Rigidbody>());


						targetPosition = new Vector3(cardGame.cardPlaceholder[targetPlaceID].placeholderTransform.localPosition.x, cardGame.cardPlaceholder[targetPlaceID].placeholderTransform.localPosition.y + offsetHeight, cardGame.cardPlaceholder[targetPlaceID].placeholderTransform.localPosition.z - offsetRow);
						MelonCoroutines.Start(CardMove.MoveTo(cardGame.playingCards[card], targetPosition, speed, preWaitTime));
						MelonCoroutines.Start(CardMove.UpdateRotation(cardGame.playingCards[card], speed, preWaitTime));
					}
				}
			}
			//TweenFactory.OwnUpdate();
		}

		public static IEnumerator MoveTo(PlayingCard playingCard, Vector3 targetPosition, float speed, float waitTime)
		{
			yield return new WaitForSeconds(waitTime);
			//AudioMain.playClip("cardslide7");
			System.Action<ITween<Vector3>> updateMovePos = (t) =>
			{
				playingCard.cardTransform.localPosition = t.CurrentValue;
			};

			System.Action<ITween<Vector3>> moveCompleted = (t) =>
			{
				playingCard.cardTransform.localPosition = targetPosition;
			};

			Vector3 currentPos = playingCard.cardTransform.localPosition;
			Vector3 startPos = playingCard.cardTransform.localPosition;
			Vector3 endPos = targetPosition;

			// completion defaults to null if not passed in
			//cardObject.Tween("MoveCircle", currentPos, startPos, 3f, TweenScaleFunctions.Linear, updateCirclePos)
			//.ContinueWith(new Vector3Tween().Setup(startPos, midPos, 1.75f, TweenScaleFunctions.Linear, updateCirclePos))
			//.ContinueWith(new Vector3Tween().Setup(startPos, endPos, 3f, TweenScaleFunctions.CubicEaseOut, updateCirclePos, circleMoveCompleted));
			playingCard.cardObject.Tween(playingCard.cardName + "moveto", currentPos, endPos, speed, TweenScaleFunctions.SineEaseInOut, updateMovePos, moveCompleted);

			yield return new WaitForSeconds(speed);
		}

		public static IEnumerator UpdateRotation(PlayingCard playingCard, float speed, float waitTime)
		{
			Quaternion currentRotation = playingCard.cardTransform.localRotation; //= cardObject.gameObject.transform.localRotation.eulerAngles.x;
			Quaternion targetRotation = playingCard.cardTransform.localRotation;
			//Vector3 rotation = new Vector3(-90, 0f, 0.0f);

			if (playingCard.cardIsUncovered)
			{
				targetRotation = Quaternion.Euler(270, 90, 90);
			}
			else
			{
				targetRotation = Quaternion.Euler(90, 90, 90);
			}

			System.Action<ITween<Quaternion>> updateRot = (t) =>
			{
				playingCard.cardTransform.localRotation = t.CurrentValue;
			};

			System.Action<ITween<Quaternion>> rotCompleted = (t) =>
			{
				playingCard.cardTransform.localRotation = targetRotation;
			};

			playingCard.cardObject.Tween(playingCard.cardName + "updaterotation", currentRotation, targetRotation, speed, TweenScaleFunctions.CubicEaseInOut, updateRot, rotCompleted);

			yield return new WaitForSeconds(speed * 2);
		}

		public static IEnumerator turnSingleCard(PlayingCard cardObject, float waitTime, float speed)
		{
			yield return new WaitForSeconds(waitTime);
			float endAngle = 270f;
			AudioMain.playClip("cardplace2");
			if (cardObject.cardIsUncovered)
			{
				endAngle = 90f;
				cardObject.cardIsUncovered = false;
			}
			else
			{
				endAngle = 270f;
				cardObject.cardIsUncovered = true;
			}
				
			Vector3 restPosition = cardObject.cardTransform.localPosition;
			Vector3 hoverPosition = new Vector3(cardObject.cardTransform.localPosition.x, cardObject.cardTransform.localPosition.y + 0.12f, cardObject.cardTransform.localPosition.z);

			float startAngle = cardObject.cardTransform.localRotation.eulerAngles.x;
			
			//Vector3 rotation = new Vector3(-90, 0f, 0.0f);


			System.Action<ITween<Vector3>> updateTurnPos = (t) =>
			{
				cardObject.gameObject.transform.localPosition = t.CurrentValue;				
			};

			System.Action<ITween<float>> updateTurnRot = (t) =>
			{
				cardObject.gameObject.transform.localRotation = Quaternion.Euler(t.CurrentValue, 90, 90);
			};

			System.Action<ITween<Vector3>> turnPosCompleted = (t) =>
			{
				cardObject.gameObject.transform.localPosition = restPosition;
			};

			System.Action<ITween<float>> turnRotCompleted = (t) =>
			{
				cardObject.gameObject.transform.localRotation = Quaternion.Euler(endAngle, 90, 90);
			};

			cardObject.cardObject.Tween(cardObject.cardName + "turnsingle", restPosition, hoverPosition, speed, TweenScaleFunctions.CubicEaseInOut, updateTurnPos)
				.ContinueWith(new FloatTween().Setup(startAngle, endAngle, speed, TweenScaleFunctions.CubicEaseInOut, updateTurnRot))
				.ContinueWith(new Vector3Tween().Setup(hoverPosition, restPosition, speed, TweenScaleFunctions.CubicEaseInOut, updateTurnPos, turnPosCompleted));
		}

		public static IEnumerator turnAllCards(CardGame cardGame, float waitTime, float speed)
		{
			yield return new WaitForSeconds(waitTime);
			MelonLogger.Msg("Uncovering cards ... ");

			for (int cardCounter = 0; cardCounter < 52; cardCounter++)
			{
				if (!cardGame.playingCards[cardCounter].cardIsUncovered && cardGame.playingCards[cardCounter].cardIsOnTop && cardGame.playingCards[cardCounter].cardColPosition != 0)
				{
					MelonCoroutines.Start(CardMove.turnSingleCard(cardGame.playingCards[cardCounter], 0, speed));
				}		
			}
		}		
	}
}