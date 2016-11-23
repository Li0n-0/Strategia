using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using KSP;
using KSP.UI.Screens;
using TMPro;

namespace Strategia
{
    /// <summary>
    /// Special MonoBehaviour to fix admin building UI.
    /// </summary>
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class AdminResizer : MonoBehaviour
    {
        public static AdminResizer Instance;
        public int ticks = 0;
        public void Awake()
        {
            Instance = this;
        }

        public void Update()
        {
            // Wait for the strategy system to get loaded
            if (KSP.UI.Screens.Administration.Instance == null)
            {
                ticks = 0;
                return;
            }

            if (ticks++ == 0)
            {
                // Resize the root element that handles the width
                Transform aspectFitter = KSP.UI.Screens.Administration.Instance.transform.FindDeepChild("bg and aspectFitter");
                if (aspectFitter != null)
                {
                    RectTransform rect = aspectFitter.GetComponent<RectTransform>();
					rect.sizeDelta = new Vector2(Math.Min(GetUserWidth(), Screen.width), rect.sizeDelta.y);
                }

                // Clean up the strategy max text
                Transform stratCountTransform = KSP.UI.Screens.Administration.Instance.transform.FindDeepChild("ActiveStratCount");
                TextMeshProUGUI stratCountText = stratCountTransform.GetComponent<TextMeshProUGUI>();
                int limit = Administration.Instance.MaxActiveStrategies - 1;
                if (!stratCountText.text.Contains("Max: " + limit))
                {
                    stratCountText.text = "Active Strategies: " + Administration.Instance.ActiveStrategyCount + " [Max: " + limit + "]";
                }
            }
        }

		int GetUserWidth ()
		{
			// Return the width define by user from AdminWidth.cfg
			string settingPath = KSPUtil.ApplicationRootPath + "GameData/Strategia/AdminWidth.cfg";
			ConfigNode userWidth = ConfigNode.Load (settingPath);
			if (userWidth != null)
			{
				if (userWidth.HasValue("userWidth"))
				{
					return int.Parse (userWidth.GetValue ("userWidth"));
				}
			}

			// If AdminWidth.cfg doesn't exist
			userWidth = new ConfigNode ();
			userWidth.AddValue("userWidth", 1424, "For tri-monitor user, change the administration building screen width, scrollbar if lower than 1424");
			userWidth.Save (settingPath);
			return 1424;
		}
    }

    public static class TransformExtns
    {
        public static Transform FindDeepChild(this Transform parent, string name)
        {
            var result = parent.Find(name);
            if (result != null)
                return result;
            foreach (Transform child in parent)
            {
                result = child.FindDeepChild(name);
                if (result != null)
                    return result;
            }
            return null;
        }

        public static void Dump(this GameObject go, string indent = "")
        {
            foreach (Component c in go.GetComponents<Component>())
            {
                Debug.Log(indent + c);
                if (c is KerbalInstructor)
                {
                    return;
                }
            }

            foreach (Transform c in go.transform)
            {
                c.gameObject.Dump(indent + "    ");
            }
        }
    }
}
