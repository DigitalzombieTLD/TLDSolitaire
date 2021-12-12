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
    public class CardControl : MonoBehaviour
    {
        public CardControl(IntPtr intPtr) : base(intPtr)
        {
        }

		public int controlID;
		public string controlName;
		public GameObject controlObject;
		public Transform controlTransform;
		public MeshRenderer controlRenderer;				
	}
}