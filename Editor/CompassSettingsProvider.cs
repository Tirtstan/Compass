using UnityEditor;
using UnityEngine;

namespace Compass.Editor
{
    /// <summary>
    /// Project Settings page for naming navigation mask slots.
    /// </summary>
    public static class CompassSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider Create()
        {
            return new SettingsProvider(CompassSettings.SettingsPath, SettingsScope.Project)
            {
                label = "Compass",
                keywords = new[] { "compass", "navigation", "mask", "selection" },
                guiHandler = _ => DrawGUI(),
            };
        }

        private static void DrawGUI()
        {
            CompassSettings settings = CompassSettings.instance;

            EditorGUILayout.HelpBox(
                "Name the slots used by Selection Navigator masks. Leave a name empty to hide that slot from the mask dropdown. Slot 0 is the default slot for new navigators.",
                MessageType.Info
            );

            bool changed = false;
            for (int i = 0; i < CompassSettings.SlotCount; i++)
            {
                string current = settings.GetSlotName(i);
                string edited = EditorGUILayout.DelayedTextField($"Slot {i}", current);
                if (edited != current)
                {
                    settings.SetSlotName(i, edited);
                    changed = true;
                }
            }

            if (changed)
                settings.SaveSettings();
        }
    }
}
