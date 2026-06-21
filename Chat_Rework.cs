using MelonLoader;
using HarmonyLib;
using GHPC;
using GHPC.AI.Radios;
using GHPC.Player;
using GHPC.UI.Hud;
using UnityEngine;
using System.Runtime.CompilerServices;

[assembly:MelonInfo(typeof(ChatReworkMod.ChatReworkModClass), "Chat Rework",
                    "1.0.0", "Qwertyryo")]
[assembly:MelonGame("Radian Simulations LLC", "GHPC")]

namespace ChatReworkMod {
  public class ChatReworkModClass : MelonMod {
    public override void OnInitializeMelon() {
      HarmonyInstance.PatchAll();
    }
  }



  [HarmonyPatch(typeof(CommsTextMessageProcessor), "DisplaySpottingMessage")]
  public static class DisplaySpottingMessagePatch {
    internal static bool SkipNamePatch = false;
    private static string FormatEnemyName(string name, float dist) {
      switch (name) {
          // units which don't need to be differentiated at this 600-1200m range

        case "Infantry":
          return "Infantry";

        case "STATIC_TOW":
        case "STATIC_TOW_TRENCH":
          return "TOW";

        case "STATIC_9K111":
        case "STATIC_9K111_TRENCH":
        case "STATIC_9K111_SA_TRENCH":
        case "STATIC_9K111_SA":
          return "9K111";

        case "BRDM2_SA":
        case "BRDM2":
          return "BRDM-2";



        case "M2BRADLEY(ALT)":
        case "M2BRADLEY":
          return "M2 Bradley";

        case "BMP2_SA":
        case "BMP2":
          return "BMP-2";

        case "BTR60PB_SA":
        case "BTR60PB":
          return "BTR-60";
        case "BTR70":
          return "BTR-70";
        case "Mi24":
        case "Mi24_rockets":
        case "Mi24V_NVA":
        case "Mi24V_SA_rockets":
        case "Mi24V_NVA_rockets":
        case "Mi24V_SA":
          return "Mi-24";

        case "OH58A":
        return "OH-58A";
        

        case "Mi8T":
          return "Mi-8T";

        case "MI2":
          return "Mi-2";

        case "AH1":
        case "AH1_rockets":
          return "AH-1";

        case "URAL375D_SA":
        case "URAL375D":
          return "URAL 375D";

        case "M923":
            return "M923";

        case "T80B":
          return "T-80B";

        case "T62":
          return "T-62";

      }

      if (dist > 600f) {
        switch (name) {

        case "BMP1_SA":
        case "BMP1":
        case "BMP1P":
        case "BMP1P_SA":
          return "BMP-1";

          case "LEO1A1":
          case "LEO1A1A2":
          case "LEO1A1A3":
          case "LEO1A1A4":
          case "LEO1A3":
          case "LEO1A3A1":
          case "LEO1A3A2":
          case "LEO1A3A3":
          case "LEO1A4":
            return "Leopard 1";

          case "T64R":
          case "T64A74":
          case "T64A79":
          case "T64A81":
          case "T64A":
          case "T64A84":
          case "T64B":
          case "T64B81":
          case "T64B1":
          case "T64B181":
            return "T-64";

          case "T72M":
          case "T72":
          case "T72UV1":
          case "T72UV2":
          case "T72ULEM":
          case "T72M1":
          case "T72GILLS":
            return "T-72";

          case "T54A":
          case "T55A":
            return "T-55";

          case "M60A1":
          case "M60A1AOS":
          case "M60A3":
          case "M60A3TTS":
          case "M60A1RISEP77":
          case "M60A1RISEP":
            return "M60";

          case "M1IP":
          case "M1":
            return "M1 Abrams";

          case "MARDER1A2":
          case "MARDERA1":
          case "MARDERA1_NO_ATGM":
          case "MARDERA1PLUS":
            return "Marder 1";

        case "M113G":
        case "M113":
          return "M113";


          default:
            return $"{name}";
        }
      } else {
        // sub 600m
        switch (name) {

        case "BMP1_SA":
        case "BMP1":
                  return "BMP-1";
        case "BMP1P":
        case "BMP1P_SA":
        return "BMP-1P";

          case "LEO1A1":
            return "Leopard 1A1";
          case "LEO1A1A2":
            return "Leopard 1A1A2";
          case "LEO1A1A3":
            return "Leopard 1A1A3";
          case "LEO1A1A4":
            return "Leopard 1A1A4";
          case "LEO1A3":
            return "Leopard 1A3";
          case "LEO1A3A1":
            return "Leopard 1A3A1";
          case "LEO1A3A2":
            return "Leopard 1A3A2";
          case "LEO1A3A3":
            return "Leopard 1A3A3";
          case "LEO1A4":
            return "Leopard 1A4";

          case "T64R":
            return "T-64R";
          case "T64A74":
            return "T-64A 1974";
          case "T64A79":
            return "T-64A 1979";
          case "T64A81":
            return "T-64A 1981";
          case "T64A":
            return "T-64A 1983";
          case "T64A84":
            return "T-64A 1984";
          case "T64B":
            return "T-64B 1984";
          case "T64B81":
            return "T-64B 1981";
          case "T64B1":
            return "T-64B1 1984";
          case "T64B181":
            return "T-64B1 1981";

          case "T72M":
            return "T-72M";
          case "T72":
            return "T-72";
          case "T72UV1":
            return "T-72 ÜV1";
          case "T72UV2":
            return "T-72 ÜV2";

          case "T72ULEM":
            return "T-72 LEM mod";
          case "T72M1":
            return "T-72M1";
          case "T72GILLS":
            return "T-72 LEM";

          case "T54A":
            return "T-54A";
          case "T55A":
            return "T-55A";

          case "M60A1":
            return "M60A1";
          case "M60A1AOS":
            return "M60A1AOS";
          case "M60A3":
            return "M60A3";
          case "M60A3TTS":
            return "M60A3 TTS";
          case "M60A1RISEP77":
            return "M60A1 Rise Passive '77";
          case "M60A1RISEP":
            return "M60A1 Rise Passive";

          case "M1IP":
            return "M1IP Abrams";
          case "M1":
            return "M1 Abrams";

          case "MARDER1A2":
            return "Marder 1A2";
          case "MARDERA1":
          case "MARDERA1_NO_ATGM":
            return "Marder A1-";
          case "MARDERA1PLUS":
            return "Marder A1+";

            
        case "M113G":
            return "M113";
        case "M113":
          return "M113";

          default:
            return $"DEBUG: (plz report) ${name}$";
        }
      }
    }

    [HarmonyPrefix]
    static bool Prefix(CommsTextMessageProcessor __instance,
                       SpottingData spotter, SpottingData spotted) {
      if (spotter.Unit == PlayerInput.Instance.CurrentPlayerUnit)
        return false;
      Vector3 position = spotter.Unit.transform.position;
      Vector3 vector = spotted.Unit.transform.position - position;
      vector.y = 0f;
      float dist = vector.magnitude;
      string text2 = "";
      //MelonLogger.Msg($"Spotting Dist {dist}");
      if (dist > 1200f) {
        string shortText = spotted.Unit.ShortNameUs.ToString();
        if (shortText == "Pc") shortText = "PC";
        text2 = string.Concat(new string[]
			{
				"<color=#",
				ColorUtility.ToHtmlStringRGBA(__instance._enemyColor),
				">",
				shortText,
				"</color>"
			});
      }
    else
            {
            text2 = FormatEnemyName(spotted.Unit.UniqueName, dist);
            text2 = string.Concat(new string[]
			{
				"<color=#",
				ColorUtility.ToHtmlStringRGBA(__instance._enemyColor),
				">",
				text2,
				"</color>"
			});
            }		
      float num = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
      CommsTextMessageProcessor.CardinalDirections cardinalDirections =
          __instance.AngleToCardinal(num);
      string text = __instance.MakeMessagePrefix(spotter.Unit);
      string text3 =
          text + "Spotted " + text2 + ", " + cardinalDirections.ToString();
      __instance._textMessageQueue.AppendMessage(text3, 0f);
      return false;
    }
  }
}
