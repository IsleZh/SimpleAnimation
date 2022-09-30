using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Isle.AnimationMachine.TransitionCondition))]
public class ConditionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        using (new EditorGUI.PropertyScope(position, label, property))
        {
            //设置属性名宽度
            //EditorGUIUtility.labelWidth = 20;
            //position.height = EditorGUIUtility.singleLineHeight;
            var nameProperty = property.FindPropertyRelative("parameter");
            var typeProperty = property.FindPropertyRelative("parameterType");
            var nameRect = new Rect(position)
            {
                x = position.x,
                y = position.y, // + (40 - EditorGUIUtility.singleLineHeight) / 2,
                width = 50 + nameProperty.stringValue.Length * 10,
                //100
                height = position.height, //EditorGUIUtility.singleLineHeight
            };
            var typeRect = new Rect(nameRect)
            {
                x = nameRect.x + nameRect.width + 5,
                width = 50,
                height = position.height, //EditorGUIUtility.singleLineHeight
            };
            var valueRect = new Rect(typeRect)
            {
                x = typeRect.x + typeRect.width + 5,
                width = 50,
                height = position.height, //EditorGUIUtility.singleLineHeight
            };


            nameProperty.stringValue = EditorGUI.TextField(nameRect, nameProperty.stringValue);
            EditorGUI.PropertyField(typeRect, typeProperty, GUIContent.none);

            //EditorGUI.LabelField(typeRect, typeProperty.enumDisplayNames[typeProperty.enumValueIndex]);
            if (typeProperty.enumValueIndex == 0)
            {
                var floatProperty = property.FindPropertyRelative("FloatValue");
                floatProperty.floatValue = EditorGUI.FloatField(valueRect, floatProperty.floatValue);
            }else if (typeProperty.enumValueIndex == 1)
            {
                var intProperty = property.FindPropertyRelative("IntValue");
                intProperty.intValue = EditorGUI.IntField(valueRect, intProperty.intValue);
            }
            else if (typeProperty.enumValueIndex == 2)
            {
                var boolProperty = property.FindPropertyRelative("BoolValue");
                boolProperty.boolValue = EditorGUI.Toggle(valueRect, boolProperty.boolValue);
            }
            else{}
        }
    }
}