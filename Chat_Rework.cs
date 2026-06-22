using MelonLoader;
using HarmonyLib;
using GHPC;
using GHPC.AI.Radios;
using GHPC.Player;
using GHPC.UI.Hud;
using UnityEngine;

[assembly:MelonInfo(typeof(ChatReworkMod.ChatReworkModClass), "Chat Rework",
                    "1.0.0", "Qwertyryo")]
[assembly:MelonGame("Radian Simulations LLC", "GHPC")]

namespace ChatReworkMod {
  public class ChatReworkModClass : MelonMod {
    public override void OnInitializeMelon() {
      HarmonyInstance.PatchAll();
    }
  }

    [HarmonyPatch(typeof(CommsTextMessageProcessor), "MakeEnemyUnitName")]
    public static class MakeEnemyUnitNamePatch
    {
        [HarmonyPrefix]
        static bool Prefix(CommsTextMessageProcessor __instance, Unit unit, ref string __result)
        {
            string text = unit.UniqueName;
           
            __result = string.Concat(new string[]
            {
                "<color=#",
                ColorUtility.ToHtmlStringRGBA(__instance._enemyColor),
                ">",
                text,
                "</color>"
            });
            return false;
        }
    }

    [HarmonyPatch(typeof(CommsTextMessageProcessor), "DisplaySpottingMessage")]
    public static class DisplaySpottingMessagePatch
    {
        [HarmonyPrefix]
        static bool Prefix(CommsTextMessageProcessor __instance, SpottingData spotter, SpottingData spotted)
        {
            if (spotter.Unit == PlayerInput.Instance.CurrentPlayerUnit)
                return false;
            Vector3 position = spotter.Unit.transform.position;
            Vector3 vector = spotted.Unit.transform.position - position;
            vector.y = 0f;
            vector.Normalize();
            float num = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
            CommsTextMessageProcessor.CardinalDirections cardinalDirections = __instance.AngleToCardinal(num);
            string text = __instance.MakeMessagePrefix(spotter.Unit);
            string text2 = __instance.MakeEnemyUnitName(spotted.Unit);
            string text3 = text + "Spotted " + text2 + ", " + cardinalDirections.ToString();
            __instance._textMessageQueue.AppendMessage(text3, 0f);
            return false;
        }
    }
}
