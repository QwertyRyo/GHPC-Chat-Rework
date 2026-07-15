using MelonLoader;
using HarmonyLib;
using GHPC;
using GHPC.AI.Radios;
using GHPC.Effects.Voices;
using GHPC.Effects.Voices.VoiceRoutines;
using GHPC.Crew;
using GHPC.Player;
using GHPC.UI.Hud;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GHPC.Effects;
using GHPC.AI.Interfaces;


[assembly:MelonInfo(typeof(ChatReworkMod.ChatReworkModClass), "Chat Rework",
                    "1.0.0", "Qwertyryo")]
[assembly:MelonGame("Radian Simulations LLC", "GHPC")]

namespace ChatReworkMod {
  public static class Prefs {
    public static MelonPreferences_Category Category;
    // Distance (meters) within which AI crew can identify the tank type (e.g. "T-64").
    // Beyond this range only a generic short designation is reported (e.g. "TK", "PC").
    public static MelonPreferences_Entry<float> MidRange;
    // Distance (meters) within which AI crew can identify the specific variant (e.g. "T-64A 1983").
    // Between close_range and mid_range only the broad type is reported (e.g. "T-64").
    public static MelonPreferences_Entry<float> CloseRange;

    public static void Register() {
      Category = MelonPreferences.CreateCategory("ChatRework");
      MidRange   = Category.CreateEntry("mid_range",   1200f);
      CloseRange = Category.CreateEntry("close_range",  600f);
      MidRange.Comment   = "Distance in meters within which AI crew identifies the tank type (e.g. 'T-64'). Beyond this only a generic designation is reported.";
      CloseRange.Comment = "Distance in meters within which AI crew identifies the specific variant (e.g. 'T-64A 1983'). Between close_range and mid_range only the broad type is reported.";
    }
  }

  public class ChatReworkModClass : MelonMod {
    public override void OnInitializeMelon() {
      MelonLogger.Msg("Chat Rework initializing...");
      Prefs.Register();
      HarmonyInstance.PatchAll();
      var patched = HarmonyInstance.GetPatchedMethods();
      foreach (var m in patched)
        MelonLogger.Msg("[Patch] " + m.DeclaringType?.Name + "." + m.Name);
      MelonLogger.Msg("Chat Rework initialized.");
    }
  }

  public static class AllegianceHelper {
    public static bool IsFriendly(Faction unitAllegiance) {
      var player = PlayerInput.Instance?.CurrentPlayerUnit;
      return player != null && player.Allegiance == unitAllegiance;
    }
  }



  [HarmonyPatch(typeof(CommsTextMessageProcessor), "DisplaySpottingMessage")]
  public static class DisplaySpottingMessagePatch {
    internal static bool SkipNamePatch = false;
    private static readonly Dictionary<string, float> _recentSpots = new Dictionary<string, float>();
    private const float SpotWindow = 1.0f;
    public static int rough_range(float dist)
    {
      float t = Mathf.Clamp01((dist - 400f) / (2000f - 400f));
      float maxVariance = dist * 0.25f * t;
      float reported = dist + UnityEngine.Random.Range(-maxVariance, maxVariance);
      return (int)(Mathf.Round(reported / 100f) * 100f);
    }

    public static int accurate_range(float dist)
    {
      float t = Mathf.Clamp01((dist - 600f) / (2400f - 600f));
      float maxVariance = dist * 0.15f * t;
      float reported = dist + UnityEngine.Random.Range(-maxVariance, maxVariance);
      return (int)(Mathf.Round(reported / 100f) * 100f);
    }
    public static int exact_range(float dist)
    {
      return (int)(Mathf.Round(dist/100f) * 100f);
    }
    private static string returnRange(float dist, string unit)
    {

      switch (unit)
      {
        case "M1IP":
        case "M1":
        case "M60A3":
        case "M60A3TTS":
        case "T72M":
        case "T72M1":
        case "T64A84":
        case "T64B":
        case "T64B81":
        case "T64B1":
        case "T64B181":
        case "T80B":
          return exact_range(dist).ToString();
        case "M60A1":
        case "M60A1AOS":
        case "M60A1RISEP":
        case "M60A1RISEP77":
         case "LEO1A1":
          case "LEO1A1A2":
          case "LEO1A1A3":
          case "LEO1A1A4":
          case "LEO1A3":
          case "LEO1A3A1":
          case "LEO1A3A2":
          case "LEO1A3A3":
          case "LEO1A4":
          case "T64R":
          case "T64A74":
          case "T64A79":
          case "T64A81":
          case "T64A":
          case "T72":
          case "T72UV1":
          case "T72UV2":
          case "T72ULEM":
          case "T72GILLS":
          return accurate_range(dist).ToString();
          default:
          return rough_range(dist).ToString();
      }

    }
    internal static string FormatName(string name) {
      switch (name) {
        case "Infantry":          return "Infantry";

        case "STATIC_TOW":
        case "STATIC_TOW_TRENCH":         return "TOW";

        case "STATIC_SPG9_TRENCH":
        case "STATIC_SPG9":               return "SPG-9";

        case "STATIC_9K111":
        case "STATIC_9K111_TRENCH":
        case "STATIC_9K111_SA_TRENCH":
        case "STATIC_9K111_SA":           return "9K111";

        case "BRDM2_SA":
        case "BRDM2":                     return "BRDM-2";

        case "M2BRADLEY(ALT)":
        case "M2BRADLEY":                 return "M2 Bradley";

        case "BMP2_SA":
        case "BMP2":                      return "BMP-2";

        case "Mi24":
        case "Mi24_rockets":
        case "Mi24V_NVA":
        case "Mi24V_SA_rockets":
        case "Mi24V_NVA_rockets":
        case "Mi24V_SA":                  return "Mi-24";

        case "OH58A":                     return "OH-58A";
        case "Mi8T":                      return "Mi-8T";
        case "MI2":                       return "Mi-2";

        case "AH1":
        case "AH1_rockets":               return "AH-1";

        case "URAL375D_SA":
        case "URAL375D":                  return "URAL 375D";

        case "M923":                      return "M923";
        case "T80B":                      return "T-80B";
        case "T62":                       return "T-62";
        case "PT76B":                     return "PT-76B";
        case "UAZ469":                    return "UAZ-469";
        case "T3485":                     return "T-34-85M";

        case "BTR60PB_SA":
        case "BTR60PB":                   return "BTR-60";
        case "BTR70":                     return "BTR-70";

        case "BMP1_SA":
        case "BMP1":                      return "BMP-1";
        case "BMP1P":
        case "BMP1P_SA":                  return "BMP-1P";

        case "LEO1A1":                    return "Leopard 1A1";
        case "LEO1A1A2":                  return "Leopard 1A1A2";
        case "LEO1A1A3":                  return "Leopard 1A1A3";
        case "LEO1A1A4":                  return "Leopard 1A1A4";
        case "LEO1A3":                    return "Leopard 1A3";
        case "LEO1A3A1":                  return "Leopard 1A3A1";
        case "LEO1A3A2":                  return "Leopard 1A3A2";
        case "LEO1A3A3":                  return "Leopard 1A3A3";
        case "LEO1A4":                    return "Leopard 1A4";

        case "T64R":                      return "T-64R";
        case "T64A74":                    return "T-64A 1974";
        case "T64A79":                    return "T-64A 1979";
        case "T64A81":                    return "T-64A 1981";
        case "T64A":                      return "T-64A 1983";
        case "T64A84":                    return "T-64A 1984";
        case "T64B":                      return "T-64B 1984";
        case "T64B81":                    return "T-64B 1981";
        case "T64B1":                     return "T-64B1 1984";
        case "T64B181":                   return "T-64B1 1981";

        case "T72M":                      return "T-72M";
        case "T72":                       return "T-72";
        case "T72UV1":                    return "T-72 ÜV1";
        case "T72UV2":                    return "T-72 ÜV2";
        case "T72ULEM":                   return "T-72 LEM mod";
        case "T72M1":                     return "T-72M1";
        case "T72GILLS":                  return "T-72 LEM";

        case "T54A":                      return "T-54A";
        case "T55A":                      return "T-55A";

        case "M60A1":                     return "M60A1";
        case "M60A1AOS":                  return "M60A1AOS";
        case "M60A3":                     return "M60A3";
        case "M60A3TTS":                  return "M60A3 TTS";
        case "M60A1RISEP77":              return "M60A1 Rise Passive '77";
        case "M60A1RISEP":                return "M60A1 Rise Passive";

        case "M1IP Abrams":                      return "M1IP";
        case "M1":                        return "M1 Abrams";

        case "MARDER1A2":                 return "Marder 1A2";
        case "MARDERA1":
        case "MARDERA1_NO_ATGM":          return "Marder A1-";
        case "MARDERA1PLUS":              return "Marder A1+";

        case "M113G":
        case "M113":                      return "M113";

        default:                          return name;
      }
    }


    internal static string IdentifyEnemy(string name, float dist) {
      // Units always reported at full specificity regardless of distance.
      switch (name) {
        case "Infantry":
        case "STATIC_TOW":
        case "STATIC_TOW_TRENCH":
        case "STATIC_SPG9_TRENCH":
        case "STATIC_SPG9":
        case "STATIC_9K111":
        case "STATIC_9K111_TRENCH":
        case "STATIC_9K111_SA_TRENCH":
        case "STATIC_9K111_SA":
        case "BRDM2_SA":
        case "BRDM2":
        case "M2BRADLEY(ALT)":
        case "M2BRADLEY":
        case "BMP2_SA":
        case "BMP2":
        case "Mi24":
        case "Mi24_rockets":
        case "Mi24V_NVA":
        case "Mi24V_SA_rockets":
        case "Mi24V_NVA_rockets":
        case "Mi24V_SA":
        case "OH58A":
        case "Mi8T":
        case "MI2":
        case "AH1":
        case "AH1_rockets":
        case "URAL375D_SA":
        case "URAL375D":
        case "M923":
        case "T80B":
        case "T62":
        case "PT76B":
        case "UAZ469":
        case "T3485":
          return FormatName(name);
      }

      if (dist <= Prefs.CloseRange.Value)
        return FormatName(name);

      // Mid-range: report at group level.
      switch (name) {
        case "BTR60PB_SA":
        case "BTR60PB":
        case "BTR70":                     return "BTR";

        case "BMP1_SA":
        case "BMP1":
        case "BMP1P":
        case "BMP1P_SA":                  return "BMP-1";

        case "LEO1A1":
        case "LEO1A1A2":
        case "LEO1A1A3":
        case "LEO1A1A4":
        case "LEO1A3":
        case "LEO1A3A1":
        case "LEO1A3A2":
        case "LEO1A3A3":
        case "LEO1A4":                    return "Leopard 1";

        case "T64R":
        case "T64A74":
        case "T64A79":
        case "T64A81":
        case "T64A":
        case "T64A84":
        case "T64B":
        case "T64B81":
        case "T64B1":
        case "T64B181":                   return "T-64";

        case "T72M":
        case "T72":
        case "T72UV1":
        case "T72UV2":
        case "T72ULEM":
        case "T72M1":
        case "T72GILLS":                  return "T-72";

        case "T54A":
        case "T55A":                      return "T-55";

        case "M60A1":
        case "M60A1AOS":
        case "M60A3":
        case "M60A3TTS":
        case "M60A1RISEP77":
        case "M60A1RISEP":                return "M60";

        case "M1IP":
        case "M1":                        return "M1 Abrams";

        case "MARDER1A2":
        case "MARDERA1":
        case "MARDERA1_NO_ATGM":
        case "MARDERA1PLUS":              return "Marder 1";

        case "M113G":
        case "M113":                      return "M113";

        default:                          return FormatName(name);
      }
    }

    [HarmonyPrefix]
    static bool Prefix(CommsTextMessageProcessor __instance,
                       SpottingData spotter, SpottingData spotted) {
      if (spotter.Unit == PlayerInput.Instance.CurrentPlayerUnit)
        return false;
      string spotKey = spotter.Unit.GetInstanceID() + "_" + spotted.Unit.GetInstanceID();
      float now = Time.time;
      if (_recentSpots.TryGetValue(spotKey, out float lastSpot) && now - lastSpot < SpotWindow)
        return false;
      _recentSpots[spotKey] = now;
      Vector3 position = spotter.Unit.transform.position;
      Vector3 vector = spotted.Unit.transform.position - position;
      vector.y = 0f;
      float dist = vector.magnitude;
      string text2 = "";
      //MelonLogger.Msg($"Spotting Dist {dist}");
      if (dist > Prefs.MidRange.Value) {
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
            text2 = IdentifyEnemy(spotted.Unit.UniqueName, dist);
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
      string reportedRange = returnRange(dist, spotter.Unit.UniqueName);
      string text3 =
          text + "Spotted " + text2 + ", " + cardinalDirections.ToString();
      text3 = text3 + " at "+reportedRange+"m!";
      __instance._textMessageQueue.AppendMessage(text3, 0f);
      return false;
    }
  }

  [HarmonyPatch(typeof(CrewManager), "DoEvacuationCoroutine")]
  public static class DoEvacuationCoroutinePatch {
    private static readonly Dictionary<CrewManager, float> _recent = new Dictionary<CrewManager, float>();
    private const float Window = 0.5f;

    [HarmonyPostfix]
    static void Postfix(CrewManager __instance) {
      if (!AllegianceHelper.IsFriendly(__instance.Unit.Allegiance)) return;
      float now = Time.time;
      if (_recent.TryGetValue(__instance, out float last) && now - last < Window) return;
      _recent[__instance] = now;
      CommsTextMessageProcessor i = CommsTextMessageProcessor.I;
      string text2 = "We're bailing out.";
      i.DisplayCustomMessage(DisplaySpottingMessagePatch.FormatName(__instance.Unit.UniqueName), text2);
    }
  }

  [HarmonyPatch(typeof(CrewManager), "KillAllRemainingCrew")]
  public static class KillAllRemainingCrewPatch {
    private static readonly Dictionary<CrewManager, float> _recent = new Dictionary<CrewManager, float>();
    private const float Window = 0.5f;

    [HarmonyPostfix]
    static void Postfix(CrewManager __instance) {
      if (!AllegianceHelper.IsFriendly(__instance.Unit.Allegiance)) return;
      float now = Time.time;
      if (_recent.TryGetValue(__instance, out float last) && now - last < Window) return;
      _recent[__instance] = now;
      CommsTextMessageProcessor i = CommsTextMessageProcessor.I;
      string text2 = "AAAAAHH-";
      i.DisplayCustomMessage(__instance.Unit.UniqueName, text2);
    }
  }


  [HarmonyPatch(typeof(TextMessageQueue), "Awake")]
  public static class TextMessageQueueAwakePatch {
    [HarmonyPostfix]
    static void Postfix(TextMessageQueue __instance) {
      if (__instance is SubtitleMessageQueue) return;
      __instance._maxMessagesVisible = 10;
      __instance._defaultMessageTimeToLive = 10f;

      RectTransform rt = __instance.GetComponent<RectTransform>();
      if (rt != null) {
        Vector2 size = rt.sizeDelta;
        size.y *= 1.3f;
        rt.sizeDelta = size;
      }
    }
  }

  [HarmonyPatch(typeof(VoiceRoutinesBase), "TargetStruck")]
  public static class VoiceRoutinesBaseTargetStruckPatch {
    [HarmonyPrefix]
    static void Prefix(VoiceRoutinesBase __instance, ITarget target) {
      string shooter = __instance._crewVoiceHandler?.UnitInfoBroker?.Unit?.UniqueName ?? "?";
      MelonLogger.Msg($"[TargetStruck] {shooter} hit {target?.Owner?.UniqueName ?? "null"}");
    }
  }

  [HarmonyPatch(typeof(DEVoiceRoutines), "TargetStruck")]
  public static class DEVoiceRoutinesTargetStruckPatch {
    [HarmonyPrefix]
    static void Prefix(DEVoiceRoutines __instance, ITarget target) {
      string shooter = __instance._crewVoiceHandler?.UnitInfoBroker?.Unit?.UniqueName ?? "?";
      MelonLogger.Msg($"[TargetStruck:DE] {shooter} hit {target?.Owner?.UniqueName ?? "null"}");
    }
  }

  [HarmonyPatch(typeof(USSRVoiceRoutines), "TargetStruck")]
  public static class USSRVoiceRoutinesTargetStruckPatch {
    [HarmonyPrefix]
    static void Prefix(USSRVoiceRoutines __instance, ITarget target) {
      string shooter = __instance._crewVoiceHandler?.UnitInfoBroker?.Unit?.UniqueName ?? "?";
      MelonLogger.Msg($"[TargetStruck:USSR] {shooter} hit {target?.Owner?.UniqueName ?? "null"}");
    }
  }

  [HarmonyPatch(typeof(Unit), "NotifyStruck")]
  public static class UnitNotifyStruckPatch {
    public static readonly Dictionary<IUnit, IUnit> LastShooter = new Dictionary<IUnit, IUnit>();

    [HarmonyPrefix]
    static void Prefix(Unit __instance, IUnit shooter) {
      if (shooter != null) {
        LastShooter[__instance] = shooter;
      }
    }
  }

  [HarmonyPatch(typeof(Unit), "onNeutralized")]
  public static class UnitNeutralizedPatch {
    [HarmonyPostfix]
    static void Postfix(Unit __instance) {
      if (!UnitNotifyStruckPatch.LastShooter.TryGetValue(__instance, out IUnit shooter)) return;
      UnitNotifyStruckPatch.LastShooter.Remove(__instance);

      IUnit playerUnit = PlayerInput.Instance?.CurrentPlayerUnit;
      if (playerUnit == null) return;
      if (shooter == playerUnit) return;

      Unit shooterUnit = shooter as Unit;
      if (shooterUnit == null) return;
      if (shooterUnit.Allegiance != playerUnit.Allegiance) return;

      CommsTextMessageProcessor cmp = CommsTextMessageProcessor.I;
      if (cmp == null) return;
      float dist = Vector3.Distance(shooterUnit.transform.position, __instance.transform.position);
      string enemyName;
      if (dist > Prefs.MidRange.Value) {
        string shortText = __instance.ShortNameUs.ToString();
        if (shortText == "Pc") shortText = "PC";
        enemyName = shortText;
      } else {
        enemyName = DisplaySpottingMessagePatch.IdentifyEnemy(__instance.UniqueName, dist);
      }
      string msg = cmp.MakeMessagePrefix(shooterUnit) + "Destroyed <color=#D9774A>" + enemyName + "</color>";
      cmp._textMessageQueue.AppendMessage(msg, 0f);
    }
  }

}
