using TMPro;
using UnityEngine.UI;

namespace CastlePrototype.Ui
{
    public static class Extensions
    {
        public static void SetText(this Button button, string text)
        {
            button.GetComponentInChildren<TMP_Text>().text = text;
        }
    }
}