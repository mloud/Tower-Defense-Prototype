
using UnityEngine;
using UnityEngine.UIElements;

namespace TowerDefensePrototype.Scripts.Data.Definitions.Editor
{
    public static class VisualElementFactory
    {
        public static VisualElement CreatePanel(bool horizontal)
        {
            var visualElement = new VisualElement
            {
                style =
                {
                    flexDirection = horizontal ? FlexDirection.Row : FlexDirection.Column,
                    alignSelf = Align.FlexStart, // Prevents stretching in parent container
                    flexShrink = 1,
                    flexGrow = 0,
                    flexBasis = StyleKeyword.Auto,
                    minWidth = 0
                }
            };

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

        public static ScrollView CreateScrollView(bool horizontal, float? width = null)
        {
            var scrollView = new ScrollView(horizontal? ScrollViewMode.Horizontal: ScrollViewMode.Vertical)
            {
                style =
                {
                    flexGrow = 1,
                    borderTopWidth = 1,
                    borderBottomWidth = 1,
                    marginTop = 6,
                    marginBottom = 6,
                    flexBasis = StyleKeyword.Auto
                } 
            };
            if (width.HasValue)
            {
                scrollView.style.width = width.Value;
                scrollView.style.flexGrow = 0; // Don't stretch if width is fixed
            }
            return scrollView;
        }

        public static Label CreateLabel(string text, int fontSize = 14, Color? color = null, FontStyle fontStyle = FontStyle.Normal)
        {
            var label = new Label(text)
            {
                style =
                {
                    unityFontStyleAndWeight = fontStyle,
                    fontSize = fontSize,
                    color = color ?? Color.white,
                    unityTextAlign = TextAnchor.MiddleLeft,
                    flexGrow = 1,
                    flexShrink = 1,
                    flexBasis = StyleKeyword.Auto,
                }
            };

            return label;
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