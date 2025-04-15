using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CastlePrototype.Battle.Visuals
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI valueLabel;
        public void SetProgress(float progress01) => image.fillAmount = progress01;

        public void SetValue(int value) => valueLabel.text = value.ToString();
    }
}