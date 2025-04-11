using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CastlePrototype.Ui.Panels
{
    public abstract class MainTabButton : MonoBehaviour
    {
        public Action<MainTabButton> OnClick;
        public bool Selected { get; private set; }
        [SerializeField] private float scaleFactor;
        [SerializeField] private Image icon; 
        [SerializeField] private Button button;


        private void Awake()
        {
            button.onClick.AddListener(()=>OnClick(this));
        }

        public void SetSelected(bool isSelected, bool callSelectAction)
        {
            if (Selected == isSelected)
                return;
            
            SetVisuallySelected(isSelected);
            
            Selected = isSelected;
            if (isSelected && callSelectAction)
            {
                OnSelectAction();
            }
        }

        public void SetVisuallySelected(bool selected)
        {
            icon.transform.DOScale(selected 
                ? new Vector3(scaleFactor, scaleFactor, scaleFactor) 
                : Vector3.one, 0.2f);
        }
        protected abstract void OnSelectAction();
    }
}