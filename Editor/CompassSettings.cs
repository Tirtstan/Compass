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
            slotNames[slot] = slotName?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Index of the named slot, or -1 when no slot has that name.
        /// </summary>
        public int FindSlot(string slotName)
        {
            EnsureSlotCount();
            string trimmed = slotName?.Trim();
            if (string.IsNullOrEmpty(trimmed))
                return -1;

            for (int i = 0; i < SlotCount; i++)
            {
                if (string.Equals(slotNames[i], trimmed, StringComparison.OrdinalIgnoreCase))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Index of the lowest unnamed slot, or -1 when all slots are named.
        /// </summary>
        public int FindFreeSlot()
        {
            EnsureSlotCount();
            for (int i = 0; i < SlotCount; i++)
            {
                if (string.IsNullOrEmpty(slotNames[i]))
                    return i;
            }

            return -1;
        }

        public int NamedSlotCount
        {
            get
            {
                EnsureSlotCount();
                int count = 0;
                foreach (string slotName in slotNames)
                {
                    if (!string.IsNullOrEmpty(slotName))
                        count++;
                }

                return count;
            }
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
