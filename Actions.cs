using System;
using System.Linq;
using System.Security.Cryptography;
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
    public class Actions
    {
        public static float m_StartCameraFOV;
        public static Vector2 m_StartPitchLimit;
        public static Vector2 m_StartYawLimit;
        public static Vector3 m_StartPlayerPosition;
        public static Quaternion m_BoardRotation;
		public static float m_StartAngleX;
		public static float m_StartAngleY;
		public static int m_StartBoardLayer;
		public static float offSetRow;
		public static float offSetCol;

		public static IEnumerator shakeMat(CardGame cardGame)
		{
			for (int cardToShake = 0; cardToShake < 52; cardToShake++)
			{
				Vector3 amount = new Vector3(0.0f, 0.0f, UnityEngine.Random.Range(-1.2f, 1.2f));
				Vector3 matAmount = new Vector3(0.0f, 0.0f, UnityEngine.Random.Range(-1f, 1f));
				Vector3 amount2 = new Vector3(UnityEngine.Random.Range(-0.002f, 0.002f), UnityEngine.Random.Range(-0.002f, 0.002f), 0.0f);

				iTween.ShakeRotation(cardGame.gameObject, matAmount, 0.5f);
				iTween.RotateAdd(cardGame.playingCards[cardToShake].cardObject, amount, 0.2f);
				iTween.MoveBy(cardGame.playingCards[cardToShake].cardObject, amount2, 0.2f);
			}

			yield return null;
		}

		public static IEnumerator explode(CardGame cardGame)
		{
			if(cardGame)
			{
				for (int cardToExplode = 0; cardToExplode < 52; cardToExplode++)
				{
					cardGame.playingCards[cardToExplode].cardObject.AddComponent<Rigidbody>();
					cardGame.gameObject.layer = 17;
					cardGame.playingCards[cardToExplode].cardObject.layer = 17;
					cardGame.playingCards[cardToExplode].cardObject.GetComponent<BoxCollider>().size = new Vector3(cardGame.playingCards[cardToExplode].GetComponent<BoxCollider>().size.x * 1.2f, cardGame.playingCards[cardToExplode].gameObject.GetComponent<BoxCollider>().size.y * 1.2f, cardGame.playingCards[cardToExplode].gameObject.GetComponent<BoxCollider>().size.z * 2f);
					cardGame.playingCards[cardToExplode].cardObject.GetComponent<BoxCollider>().isTrigger = false;
					cardGame.playingCards[cardToExplode].cardObject.GetComponent<Rigidbody>().useGravity = true;
					cardGame.playingCards[cardToExplode].cardObject.GetComponent<Rigidbody>().mass = 0.002f;
					cardGame.playingCards[cardToExplode].cardObject.GetComponent<Rigidbody>().drag = 6f;
					//cardGame.playingCards[cardToExplode].cardObject.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
					cardGame.playingCards[cardToExplode].cardObject.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(UnityEngine.Random.Range(-0.03f, 0.03f), UnityEngine.Random.Range(-0.01f, 0.09f), UnityEngine.Random.Range(0.00f, 0.03f)), ForceMode.Impulse);
					cardGame.playingCards[cardToExplode].cardObject.GetComponent<Rigidbody>().AddTorque(new Vector3(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, 5f)), ForceMode.Impulse);
					cardGame.playingCards[cardToExplode].cardObject.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
				}
			}	

			yield return null;
		}

		public static bool EnterGameView(CardGame cardGame)
        {
            MelonLogger.Msg("Starting card game ...");
            InputManager.ShowCursor(true);
			CameraFade.StartAlphaFade(Color.black, true, 1, 0f, null);
			m_StartCameraFOV = GameManager.GetMainCamera().fieldOfView;
            m_StartPitchLimit = GameManager.GetVpFPSCamera().RotationPitchLimit;
            m_StartYawLimit = GameManager.GetVpFPSCamera().RotationYawLimit;
            m_StartPlayerPosition = GameManager.GetVpFPSPlayer().transform.position;

			m_StartAngleX = GameManager.GetVpFPSPlayer().transform.rotation.eulerAngles.x;
			m_StartAngleY = GameManager.GetVpFPSPlayer().transform.rotation.eulerAngles.y;
			m_StartBoardLayer = cardGame.gameObject.layer;
            cardGame.gameObject.layer = 14; // 5

			GameManager.GetPlayerManagerComponent().SetControlMode(PlayerControlMode.InSnowShelter);
            GameManager.GetPlayerManagerComponent().TeleportPlayer(cardGame.dummyCamera.transform.position - GameManager.GetVpFPSCamera().PositionOffset, cardGame.dummyCamera.transform.rotation);

			GameManager.GetVpFPSCamera().transform.position = cardGame.dummyCamera.transform.position;
            GameManager.GetVpFPSCamera().transform.localPosition = GameManager.GetVpFPSCamera().PositionOffset;
            GameManager.GetVpFPSCamera().SetAngle(cardGame.dummyCamera.transform.rotation.eulerAngles.y, cardGame.dummyCamera.transform.rotation.eulerAngles.x);            
            GameManager.GetVpFPSCamera().SetPitchLimit(new Vector2(-90,90));
            GameManager.GetVpFPSCamera().SetYawLimit(cardGame.dummyCamera.transform.rotation, new Vector2(-179, 179));
            GameManager.GetVpFPSCamera().LockRotationLimit();

			// Activate controlChips / buttons
			cardGame.cardControls[0].controlObject.SetActive(true);

            Color stdTransparentColor = new Color(1, 1, 1, 0.1f);

			for (int x = 0; x < 13; x++)
			{
				if (x != 1) // Position 1 = drawstack / Got no renderer
				{
					cardGame.cardPlaceholder[x].placeholderRenderer.material.shader = Shader.Find("Transparent/Diffuse");
					cardGame.cardPlaceholder[x].placeholderRenderer.material.SetColor("_Color", stdTransparentColor);
				}
			}

			Vars.isPlaying = true;
            return false;
        }

		public static bool ExitGameView(CardGame cardGame)
        {
            MelonLogger.Msg("Leaving card game ...");
			CameraFade.StartAlphaFade(Color.black, true, 1, 0f, null);
            cardGame.gameObject.layer = 17;
			InputManager.ShowCursor(false);
			GameManager.GetVpFPSCamera().m_PanViewCamera.m_IsDetachedFromPlayer = false;
            GameManager.GetPlayerManagerComponent().SetControlMode(PlayerControlMode.Normal);
            GameManager.GetVpFPSCamera().UnlockRotationLimit();

			GameManager.GetVpFPSCamera().RotationPitchLimit = m_StartPitchLimit;
            GameManager.GetVpFPSCamera().RotationYawLimit = m_StartYawLimit;
            GameManager.GetVpFPSPlayer().transform.position = m_StartPlayerPosition;
            GameManager.GetVpFPSCamera().transform.localPosition = GameManager.GetVpFPSCamera().PositionOffset;
			GameManager.GetVpFPSCamera().SetAngle(m_StartAngleY, m_StartAngleX);

			GameManager.GetVpFPSCamera().UpdateCameraRotation();
            GameManager.GetPlayerManagerComponent().StickPlayerToGround();


			// Activate controlChips / buttons
			cardGame.cardControls[0].controlObject.SetActive(false);
			Color stdTransparentColor = new Color(1, 1, 1, 0.0f);
            for (int x = 0; x < 13; x++)
            {				
				if (x != 1) // Position 1 = drawstack / Got no renderer
				{
					cardGame.cardPlaceholder[x].placeholderRenderer.material.shader = Shader.Find("Transparent/Diffuse");
					cardGame.cardPlaceholder[x].placeholderRenderer.material.SetColor("_Color", stdTransparentColor);
				}
            }
			Vars.isPlaying = false;
			return true;
        }


		public static IEnumerator pickUpCard(PlayingCard playingCard, float speed)
		{
			Vars.cardInHand = playingCard;			
            playingCard.cardObject.transform.parent = GameManager.m_vpFPSCamera.transform;

			Vector3 endPos = new Vector3(0.15f, -0.05f, 0.3f);

			System.Action<ITween<Vector3>> updatePickPos = (t) =>
			{
                playingCard.cardObject.transform.localPosition = t.CurrentValue;

				// Rotate towards camera
				//cardObject.gameObject.transform.rotation = Quaternion.LookRotation(Vector3.Cross(upAxis, Vector3.Cross(upAxis, Camera.main.transform.forward)), upAxis);
			};

			System.Action<ITween<Vector3>> pickCompleted = (t) =>
			{
                playingCard.cardObject.transform.localPosition = endPos;
			};

			Vector3 currentPos = playingCard.cardObject.transform.localPosition;
			Vector3 startPos = playingCard.cardObject.transform.localPosition;

			Quaternion startRotation = playingCard.cardObject.transform.localRotation;
			Quaternion endRotation = Quaternion.Euler(8f, 210f, 2f);
			

			//Vector3 rotation = new Vector3(-90, 0f, 0.0f);

			System.Action<ITween<Quaternion>> updatePickRot = (t) =>
			{
                playingCard.cardObject.transform.localRotation = t.CurrentValue;
			};

			System.Action<ITween<Quaternion>> pickRotCompleted = (t) =>
			{
                //MelonLogger.Msg("´Rotation completed");
                playingCard.cardObject.transform.localRotation = endRotation;
			};

            playingCard.cardObject.Tween(playingCard.cardName + "pickup", currentPos, endPos, speed, TweenScaleFunctions.SineEaseIn, updatePickPos, pickCompleted)
			.ContinueWith(new QuaternionTween().Setup(startRotation, endRotation, speed, TweenScaleFunctions.SineEaseIn, updatePickRot, pickRotCompleted));
			
			yield return new WaitForSeconds(0f);
		}

		public static IEnumerator pickUpGroup(CardGame cardGame, PlayingCard playingCard, float speed)
		{
			Vars.cardInHand = playingCard;
			Vars.cardListInHand = new PlayingCard[52];
			Vars.cardListCounter = 0;

			playingCard.cardObject.transform.parent = GameManager.m_vpFPSCamera.transform;
			
			int groupCounter = playingCard.cardRowPosition;
			

			while(cardGame.cardPositions[playingCard.cardColPosition, groupCounter] !=99)
            {				
				cardGame.playingCards[cardGame.cardPositions[playingCard.cardColPosition, groupCounter]].cardObject.transform.parent = Vars.cardInHand.cardTransform;

				Vars.cardListInHand[Vars.cardListCounter] = cardGame.playingCards[cardGame.cardPositions[playingCard.cardColPosition, groupCounter]];
				

				Vars.cardListCounter++;
				groupCounter++;
			}

			Vector3 endPos = new Vector3(0.15f, -0.05f, 0.3f);

			System.Action<ITween<Vector3>> updateGroupPos = (t) =>
			{
				playingCard.cardObject.transform.localPosition = t.CurrentValue;
			};

			System.Action<ITween<Vector3>> pickGroupCompleted = (t) =>
			{
				playingCard.cardObject.transform.localPosition = endPos;
			};

			Vector3 currentPos = playingCard.cardObject.transform.localPosition;
			Vector3 startPos = playingCard.cardObject.transform.localPosition;

			Quaternion startRotation = playingCard.cardObject.transform.localRotation;
			Quaternion endRotation = Quaternion.Euler(8f, 210f, 2f);


			//Vector3 rotation = new Vector3(-90, 0f, 0.0f);

			System.Action<ITween<Quaternion>> updateGroupRot = (t) =>
			{
				playingCard.cardObject.transform.localRotation = t.CurrentValue;
			};

			System.Action<ITween<Quaternion>> groupRotCompleted = (t) =>
			{
				//MelonLogger.Msg("´Rotation completed");
				playingCard.cardObject.transform.localRotation = endRotation;
			};

			playingCard.cardObject.Tween("cardgroup", currentPos, endPos, speed, TweenScaleFunctions.SineEaseIn, updateGroupPos, pickGroupCompleted)
			.ContinueWith(new QuaternionTween().Setup(startRotation, endRotation, speed, TweenScaleFunctions.SineEaseIn, updateGroupRot, groupRotCompleted));

			yield return new WaitForSeconds(0f);
		}


		public static IEnumerator drawCardFromStack(CardGame cardGame, float waitTime, float speed, bool reverse)
        {
            yield return new WaitForSeconds(waitTime);

			int targetCol = 1;
			int originCol = 0;
			bool cardIsUncovered = true;
			float endAngle = 270f;

			if (reverse)
			{
				endAngle = 90f;
				targetCol = 0;
				originCol = 1;
				cardIsUncovered = false;
			}

            PlayingCard playingCard = GameLogic.getCardOnColTop(cardGame, originCol);
                        
			GameLogic.sendCardToColTop(cardGame, playingCard, targetCol);

			Vector3 restPosition = playingCard.cardObject.transform.localPosition;
			Vector3 hoverPosition = new Vector3(playingCard.cardObject.transform.localPosition.x, playingCard.cardObject.transform.localPosition.y + 0.12f, playingCard.cardObject.transform.localPosition.z);

			float startAngle = playingCard.cardObject.gameObject.transform.localRotation.eulerAngles.x;
			
			//Vector3 rotation = new Vector3(-90, 0f, 0.0f);

			System.Action<ITween<Vector3>> updateDrawPos = (t) =>
			{
                playingCard.cardObject.gameObject.transform.localPosition = t.CurrentValue;
			};

			System.Action<ITween<float>> updateDrawRot = (t) =>
			{
                playingCard.cardObject.gameObject.transform.localRotation = Quaternion.Euler(t.CurrentValue, 90, 90);
			};

			System.Action<ITween<float>> drawPosCompleted = (t) =>
			{
                //MelonLogger.Msg("´Move completed");
                playingCard.cardObject.gameObject.transform.localRotation = Quaternion.Euler(endAngle, 90, 90);
                
                playingCard.cardIsUncovered = cardIsUncovered;

                // Move down
                CardMove.updateAllPhysicalPosition(cardGame, 0f, 0.5f);
			};

			System.Action<ITween<Quaternion>> drawRotCompleted = (t) =>
			{
				//MelonLogger.Msg("´Rotation completed");
			};

            playingCard.cardObject.Tween(playingCard.name+"draw", restPosition, hoverPosition, speed, TweenScaleFunctions.CubicEaseInOut, updateDrawPos)
				.ContinueWith(new FloatTween().Setup(startAngle, endAngle, speed, TweenScaleFunctions.CubicEaseInOut, updateDrawRot, drawPosCompleted));
				
		
			yield return new WaitForSeconds(0f);
		}

		public static void reStack(CardGame cardGame, float waitTime, float speed)
		{			
			int cardCounter = 51;

			for(cardCounter=51; cardCounter > -1; cardCounter--)
			{	
				if(cardGame.cardPositions[1, cardCounter] != 99)
				{
					MelonCoroutines.Start(Actions.drawCardFromStack(Vars.currentGame, 0.0f, 0.2f, true));
				}
			}		
		}
	}
}
