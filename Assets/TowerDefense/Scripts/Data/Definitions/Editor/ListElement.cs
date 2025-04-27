using System;
using System.Collections.Generic;
using TowerDefensePrototype.Scripts.Data.Definitions.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TowerDefense.Scripts.Data.Definitions.Editor
{
    public class ListElement<T> : CustomElement where T: new()
    {
        private List<T> List { get; }
        private string Title { get; }
        private string ItemText { get; }

        private Action SaveAction;
        
        public ListElement(List<T> listData, string title, string itemText)
        {
            List = listData;
            Title = title;
            ItemText = itemText;
        }
        
        protected override VisualElement CreateVisualElement()
        {
            var visualElement = new VisualElement();
            return visualElement;
        }

        protected override void OnCreate()
        {
            VisualElement.Clear();
            SaveAction = null;
            
            var addButton = new Button(() =>
                {
                    OnSave();
                    List.Add(new T());
                    OnCreate();
                })
                { text = "+" };
            VisualElement.Add(addButton);

            if (!string.IsNullOrEmpty(Title))
            {
                VisualElement.Add(new Label(Title));
                VisualElement.Add(VisualElementFactory.CreateSeparator());
            }
            
            for (var index = 0; index < List.Count; index++)
            {
                var item = List[index];
                var itemVisualElement = new VisualElement();
                VisualElement.Add(VisualElementFactory.CreateSeparator(1));
                
                VisualElement.Add(VisualElementFactory.CreateLabel(ItemText +" " + index, 14, Color.yellow, FontStyle.Bold));
                SaveAction += VisualElementFromClass.FillVisualElement(item, itemVisualElement);
                VisualElement.Add(itemVisualElement);

                int i = index;
                // add delete button
                var deleteButton = new Button(() =>
                    {
                        List.RemoveAt(i);
                        OnCreate();
                    })
                    { text = "x" };
                deleteButton.style.width = 30;
                VisualElement.Add(deleteButton);
            }
        }

        protected override void OnSave()
        {
            SaveAction?.Invoke();
        }
    }
}