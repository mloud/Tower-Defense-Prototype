using OneDay.Core.Modules.Ui.Components;
using TMPro;
using UnityEngine;

namespace TowerDefense.Ui.Components
{
    public class ResourceWidget : MonoBehaviour
    {
        [SerializeField] private CImage image;
        [SerializeField] private TextMeshProUGUI valueLabel;

        public void Set(string imageKey, int value)
        {
            valueLabel.text = value.ToString();
            image.SetImage(imageKey);
        }
        public void Set(int value)
        {
            valueLabel.text = value.ToString();
        }
    }
}