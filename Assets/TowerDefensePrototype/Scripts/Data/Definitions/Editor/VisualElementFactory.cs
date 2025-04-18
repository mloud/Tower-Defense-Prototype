
using UnityEngine;
using UnityEngine.UIElements;

namespace TowerDefensePrototype.Scripts.Data.Definitions.Editor
{
    public static class VisualElementFactory
    {
        public static VisualElement CreatePanel(bool horizontal, float maxWidth)
        {
            var visualElement = new VisualElement();
            visualElement.style.flexDirection = horizontal ? FlexDirection.Row : FlexDirection.Column;
           // visualElement.style.alignItems = Align.FlexStart;
           // visualElement.style.justifyContent = Justify.FlexStart;
            visualElement.style.width = maxWidth;     // absolute width
            visualElement.style.maxWidth = maxWidth;      // maximum width (still responsive)
            visualElement.style.minWidth = maxWidth;
            
            visualElement.style.flexGrow = 1;
            return visualElement;
        }



        public static VisualElement CreateSeparator(float height = 1)
        {
            var separator = new VisualElement();
            separator.style.height = height;
            separator.style.marginTop = 8;
            separator.style.marginBottom = 8;
            separator.style.backgroundColor = new Color(0.4f, 0.4f, 0.4f, 1f); // gray line
            separator.style.flexGrow = 1;
            return separator;
        }

        public static ScrollView CreateScrollView(bool horizontal)
        {
            ScrollView scrollView = new ScrollView(horizontal? ScrollViewMode.Horizontal: ScrollViewMode.Vertical)
            {
                style =
                {
                    flexGrow = 1,
                    borderTopWidth = 1,
                    borderBottomWidth = 1,
                    marginTop = 6,
                    marginBottom = 6,
                }
            };
            return scrollView;
        }
        
    }
}