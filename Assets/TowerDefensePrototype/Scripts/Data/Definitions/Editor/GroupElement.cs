
using UnityEngine.UIElements;

namespace TowerDefensePrototype.Scripts.Data.Definitions.Editor
{
    public class GroupElement : CustomElement
    {
        protected override VisualElement CreateVisualElement()
        {
            return new VisualElement();
        }

        protected override void OnCreate() { }

        protected override void OnSave() { }
    }
}