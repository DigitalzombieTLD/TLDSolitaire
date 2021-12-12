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

namespace CardGame
{
	public class Tools
	{		
	public static string clipStringCutter(string stringToCut)
		{
			string[] clipNameSplit;
			string tmpClipName;

			clipNameSplit = stringToCut.Split('/');
			tmpClipName = clipNameSplit[clipNameSplit.Length - 1];

			clipNameSplit = tmpClipName.Split('.');
			tmpClipName = clipNameSplit[0];

			return (tmpClipName);
		}
	}
}