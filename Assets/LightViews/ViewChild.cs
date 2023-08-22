using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Assets { 

    [AttributeUsage (AttributeTargets.Field)]
    public class ViewChild : PropertyAttribute
    {
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(ViewChild))]
    public class ViewChildPropertyDrawer : PropertyDrawer
    {
        public SerializedProperty _property;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _property = property;

            var obj = property.serializedObject.FindProperty(property.propertyPath).objectReferenceValue;

            var variants = GetVariantsOf(property.serializedObject.targetObject);
            if (variants == null) return;

            var variantsNames = variants.Select(x => "Object: " + x.name).ToArray();
            var objVariant = variants.ToList().IndexOf(obj);

            EditorGUI.BeginChangeCheck();
            var selected = EditorGUI.Popup(position, label.text, objVariant, variantsNames);
            if(EditorGUI.EndChangeCheck())
            {
                property.objectReferenceValue = variants[selected];
            }
        }

        private UnityEngine.Object[] GetVariantsOf(UnityEngine.Object obj)
        {
            if (obj is not MonoBehaviour gameObject) return null;

            Type sOType = obj.GetType();
            FieldInfo[] infos = sOType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            FieldInfo info = infos.FirstOrDefault(x => x.Name.Equals(_property.name));

            if (info == null) return null;

            Type type = info.FieldType;
            var childs = gameObject
                .GetComponentsInChildren(type)
                .Where(x => !x.transform.Equals(gameObject.transform))
                .ToArray();

            return childs;
        }
    }
#endif

}