using System.Collections.Generic;
using UnityEngine.UIElements;

namespace TowerDefensePrototype.Scripts.Data.Definitions.Editor
{
    public abstract class CustomElement
    {
        public VisualElement VisualElement { get; private set;}

        private List<CustomElement> Children { get; } = new();

        public CustomElement Create(VisualElement visualElement = null)
        {
            VisualElement = visualElement ?? CreateVisualElement();
            OnCreate();
            return this;
        }
        public void Save()
        {
            OnSave();
            Children.ForEach(x=>x.Save());
        }

        public void AddChild(CustomElement customElement)
        {
            Children.Add(customElement);
            VisualElement.Add(customElement.VisualElement);
        }

        protected abstract VisualElement CreateVisualElement();
        protected abstract void OnCreate();
        protected abstract void OnSave();
    }
}