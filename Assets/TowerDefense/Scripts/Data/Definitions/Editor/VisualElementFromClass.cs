using System;
using System.Linq;
using System.Reflection;
using UnityEngine.UIElements;

namespace TowerDefense.Scripts.Data.Definitions.Editor
{
    public static class VisualElementFromClass
    {
        public static Action FillVisualElement(object instance, VisualElement visualElement)
        {
            var type = instance.GetType();

            var allowedTypes = new[]
            {
                typeof(int),
                typeof(string),
                typeof(float),
                typeof(double),
                typeof(bool),
            };

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(f => allowedTypes.Contains(f.FieldType) || f.FieldType.IsEnum);

            Action saveAction = null;
            foreach (var field in fields)
            {
                
                if (field.FieldType == typeof(int))
                {
                    var f = new IntegerField(field.Name)
                    {
                        value = (int)field.GetValue(instance)
                    };
                    visualElement.Add(f);
                    saveAction += ()=>field.SetValue(instance, f.value);
                }
                else if (field.FieldType == typeof(string))
                {
                    var f = new TextField(field.Name)
                    {
                        value = (string)field.GetValue(instance)
                    };
                    visualElement.Add(f);
                    saveAction += ()=>field.SetValue(instance, f.value);
                }
                else if (field.FieldType == typeof(float))
                {
                    var f = new FloatField(field.Name)
                    {
                        value = (float)field.GetValue(instance)
                    };
                    visualElement.Add(f);
                    saveAction += ()=>field.SetValue(instance, f.value);
                }
                else if (field.FieldType == typeof(double))
                {
                    var f = new DoubleField(field.Name)
                    {
                        value = (double)field.GetValue(instance)
                    };
                    visualElement.Add(f);
                    saveAction += ()=>field.SetValue(instance, f.value);
                }
                else if (field.FieldType == typeof(bool))
                {
                    var f = new Toggle(field.Name)
                    {
                        value = (bool)field.GetValue(instance)
                    };
                    visualElement.Add(f);
                    saveAction += ()=>field.SetValue(instance, f.value);
                }
                else if (field.FieldType.IsEnum)
                {
                    var f = new EnumField(field.Name, (Enum)field.GetValue(instance));
                    visualElement.Add(f);
                    saveAction += ()=>field.SetValue(instance, f.value);
                }
                Console.WriteLine($"{field.Name} : {field.FieldType}");
            }
            return saveAction;
        }
    }
}