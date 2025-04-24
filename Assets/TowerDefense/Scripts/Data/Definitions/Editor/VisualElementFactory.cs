
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

            visualElement.style.alignSelf = Align.FlexStart;    // Prevents stretching in parent container
            visualElement.style.flexShrink = 1;
            visualElement.style.flexGrow = 0;
            visualElement.style.flexBasis = StyleKeyword.Auto;
            visualElement.style.minWidth = 0;
            
            
            // visualElement.style.alignItems = Align.FlexStart;
           // visualElement.style.justifyContent = Justify.FlexStart;
           // visualElement.style.width = maxWidth;     // absolute width
           // visualElement.style.maxWidth = maxWidth;      // maximum width (still responsive)
           // visualElement.style.minWidth = maxWidth;
            
      
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
                    flexBasis = StyleKeyword.Auto,
                    
                }
            };
            return scrollView;
        }

        public static TextField CreateTextField(string title, string value)
        {
            var textField = new TextField(title)
            {
                style =
                {
                    flexGrow = 1,      // Allow it to grow and take up space
                    flexShrink = 1,    // Shrink it when space is limited
                    flexBasis = StyleKeyword.Auto,  // Size based on content
                },
                value = value
            };
            return textField;
        }
        
        public static IntegerField CreateIntegerField(string title, int value)
        {
            var field = new IntegerField(title)
            {
                style =
                {
                    flexGrow = 1,      // Allow it to grow and take up space
                    flexShrink = 1,    // Shrink it when space is limited
                    flexBasis = StyleKeyword.Auto,  // Size based on content
                    minWidth = 20
                },
                value = value
            };
            return field;
        }
        
        public static FloatField CreateFloatField(string title, float value)
        {
            var field = new FloatField(title)
            {
                style =
                {
                    flexGrow = 1,      // Allow it to grow and take up space
                    flexShrink = 1,    // Shrink it when space is limited
                    flexBasis = StyleKeyword.Auto,  // Size based on content
                    minWidth = 20
                },
                value = value
            };
            return field;
        }
    }
}