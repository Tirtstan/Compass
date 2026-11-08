using System;
using UnityEditor;
using UnityEngine;

namespace Compass.Editor
{
    /// <summary>
    /// Editor-only names for the 32 <see cref="NavigationMask"/> slots. Stored in ProjectSettings/CompassSettings.asset.
    /// </summary>
    [FilePath("ProjectSettings/CompassSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public sealed class CompassSettings : ScriptableSingleton<CompassSettings>
    {
        public const int SlotCount = 32;
        public const string DefaultSlotName = "Default";
        public const string SettingsPath = "Project/Compass";

        [SerializeField]
        private string[] slotNames = CreateDefaultNames();

        /// <summary>
        /// Name of a slot, or an empty string when the slot is unused.
        /// </summary>
        public string GetSlotName(int slot)
        {
            EnsureSlotCount();
            return slotNames[slot];
        }

        public void SetSlotName(int slot, string slotName)
        {
            EnsureSlotCount();
            slotNames[slot] = slotName.Trim();
        }

        public void SaveSettings() => Save(true);

        private void EnsureSlotCount()
        {
            if (slotNames != null && slotNames.Length == SlotCount)
                return;

            string[] resized = new string[SlotCount];
            if (slotNames != null)
                Array.Copy(slotNames, resized, Math.Min(slotNames.Length, SlotCount));

            for (int i = 0; i < SlotCount; i++)
                resized[i] ??= string.Empty;

            slotNames = resized;
        }

        private static string[] CreateDefaultNames()
        {
            string[] names = new string[SlotCount];
            Array.Fill(names, string.Empty);
            names[0] = DefaultSlotName;
            return names;
        }
    }
}
