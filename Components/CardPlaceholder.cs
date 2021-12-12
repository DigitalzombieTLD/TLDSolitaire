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
    public class CardPlaceholder : MonoBehaviour
    {
        public CardPlaceholder(IntPtr intPtr) : base(intPtr)
        {
        }

		public int placeholderID;
		public string placeholderName;
		public GameObject placeholderObject;
		public Transform placeholderTransform;
		public MeshRenderer placeholderRenderer;
	}
}