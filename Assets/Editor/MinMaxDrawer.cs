// NOTE put in a Editor folder

using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MinMaxAttribute))]
public class MinMaxDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // cast the attribute to make life easier
        MinMaxAttribute minMax = attribute as MinMaxAttribute;

        // This only works on a vector2! ignore on any other property type (we should probably draw an error message instead!)
        if (property.propertyType == SerializedPropertyType.Vector2)
        {
            // if we are flagged to draw in a special mode, lets modify the drawing rectangle to draw only one line at a time
            if (minMax.ShowDebugValues || minMax.ShowEditRange)
            {
                position = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            }

            // pull out a bunch of helpful min/max values....
            float minValue = property.vector2Value.x; 
            float maxValue = property.vector2Value.y;
            float minLimit = minMax.MinLimit;
            float maxLimit = minMax.MaxLimit;

            EditorGUI.MinMaxSlider(position, label, ref minValue, ref maxValue, minLimit, maxLimit);

            var vec = Vector2.zero;
            vec.x = minValue;
            vec.y = maxValue;

            property.vector2Value = vec;

            if (minMax.ShowDebugValues || minMax.ShowEditRange) {
                bool isEditable = false;
                if (minMax.ShowEditRange) isEditable = true;

                if (!isEditable) GUI.enabled = false;

                position.y += EditorGUIUtility.singleLineHeight;

                GUI.enabled = false;
                Vector2 val = new Vector4(minValue, maxValue);
                val = EditorGUI.Vector2Field(position, "", val);
                position.y += EditorGUIUtility.singleLineHeight;

                position.y += EditorGUIUtility.singleLineHeight;
                GUI.enabled = true;

                if (isEditable) property.vector2Value = new Vector2(val.x, val.y);
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        MinMaxAttribute minMax = attribute as MinMaxAttribute;

        // by default just return the standard line height
        float size = EditorGUIUtility.singleLineHeight;
        
        // if we have a special mode, add two extra lines!
        if (minMax.ShowEditRange || minMax.ShowDebugValues)
        {
            size += EditorGUIUtility.singleLineHeight*2;
        }

        return size;
    }
}