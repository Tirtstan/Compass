using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Compass.Editor
{
    /// <summary>
    /// Draws a <see cref="NavigationMask"/> as a mask field listing the named slots from <see cref="CompassSettings"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(NavigationMask))]
    public class NavigationMaskDrawer : PropertyDrawer
    {
        private const float ButtonWidth = 22f;

        private static readonly List<string> names = new();
        private static readonly List<int> slots = new();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);

            SerializedProperty valueProp = property.FindPropertyRelative("value");
            if (valueProp == null)
            {
                EditorGUI.LabelField(position, label.text, "Invalid NavigationMask structure.");
                EditorGUI.EndProperty();
                return;
            }

            Rect fieldRect =
                new(position.x, position.y, position.width - ButtonWidth - 2f, position.height);
            Rect buttonRect =
                new(position.xMax - ButtonWidth, position.y, ButtonWidth, position.height);

            CollectNamedSlots();

            int rawMask = valueProp.intValue;
            int displayedMask = 0;
            for (int i = 0; i < slots.Count; i++)
            {
                if ((rawMask & (1 << slots[i])) != 0)
                    displayedMask |= 1 << i;
            }

            EditorGUI.showMixedValue = valueProp.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            int newDisplayed = EditorGUI.MaskField(
                fieldRect,
                label,
                displayedMask,
                names.ToArray()
            );
            if (EditorGUI.EndChangeCheck())
            {
                // Bits of unnamed slots are preserved; only listed slots are edited.
                int newMask = rawMask;
                for (int i = 0; i < slots.Count; i++)
                {
                    int bit = 1 << slots[i];
                    if ((newDisplayed & (1 << i)) != 0)
                        newMask |= bit;
                    else
                        newMask &= ~bit;
                }

                valueProp.intValue = newMask;
            }
            EditorGUI.showMixedValue = false;

            if (
                GUI.Button(
                    buttonRect,
                    new GUIContent(
                        EditorGUIUtility.IconContent("Settings").image,
                        "Open Compass settings"
                    ),
                    EditorStyles.iconButton
                )
            )
            {
                SettingsService.OpenProjectSettings(CompassSettings.SettingsPath);
            }

            EditorGUI.EndProperty();
        }

        private static void CollectNamedSlots()
        {
            names.Clear();
            slots.Clear();

            CompassSettings settings = CompassSettings.instance;
            for (int i = 0; i < CompassSettings.SlotCount; i++)
            {
                string slotName = settings.GetSlotName(i);
                if (string.IsNullOrEmpty(slotName))
                    continue;

                names.Add(slotName);
                slots.Add(i);
            }
        }
    }
}
