using System.Reflection;
using DoTweenMovement.Scripts;
using UnityEditor;
using UnityEngine;

namespace DoTweenMovement.Editor
{
    [CustomPropertyDrawer(typeof(PathPoint))]
    public class PathPointDrawer : PropertyDrawer
    {
        private DoTweenMove _dt;
        private PathPoint myScript;
        public override void OnGUI(Rect rect, SerializedProperty property,
            GUIContent label)
        {
            label = GUIContent.none;

            SerializedProperty position = property.FindPropertyRelative("position");
            //SerializedProperty doTweenMove = property.FindPropertyRelative("doTweenMove");
            SerializedProperty index = property.FindPropertyRelative("index");
            //var dt = doTweenMove.serializedObject.targetObject as DoTweenMove;
            //sp.vector3Value = EditorGUI.Vector3Field(new Rect(position.x, position.y, position.width, 16), "Position", sp.vector3Value, options, values);
            //positionProperty.vector3Value = EditorGUI.Vector3Field(position, "Position", positionProperty.vector3Value);
            
            rect = new Rect(rect.x, rect.y + 18, rect.width, GetPropertyHeight(position, label));
            var rect2 = new Rect(rect.x, rect.y - 27, rect.width, rect.height);
            EditorGUI.LabelField(rect2, $"Position {index.intValue}");
            position.vector3Value = EditorGUI.Vector3Field(rect, "", position.vector3Value);

            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            
           

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            
            
            
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) * 2;
        }
        
        
        // public object GetParent(SerializedProperty prop)
        // {
        //     var path = prop.propertyPath.Replace(".Array.data[", "[");
        //     object obj = prop.serializedObject.targetObject;
        //     var element = path;
        //     obj = GetValue(obj, element);
        //     return obj;
        // }
        // public object GetValue(object source, string name)
        // {
        //     if(source == null)
        //         return null;
        //     var type = source.GetType();
        //     var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        //     if(f == null)
        //     {
        //         var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        //         if(p == null)
        //             return null;
        //         return p.GetValue(source, null);
        //     }
        //     return f.GetValue(source);
        // }
    }
}