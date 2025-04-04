using UnityEngine;
using UnityEngine.UI;

namespace CastlePrototype.Battle.Visuals
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private Image image;
        
        public void SetProgress(float progress01)
        {
            image.fillAmount = progress01;
        }
    }
}