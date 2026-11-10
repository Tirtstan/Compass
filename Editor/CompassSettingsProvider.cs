using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Compass.Editor
{
    /// <summary>
    /// Project Settings page for naming navigation mask slots.
    /// </summary>
    public static class CompassSettingsProvider
    {
        private const string NewSlotPlaceholder = "New Slot";

        [SettingsProvider]
        public static SettingsProvider Create()
        {
            return new SettingsProvider(CompassSettings.SettingsPath, SettingsScope.Project)
            {
                label = "Compass",
                keywords = new[] { "compass", "navigation", "mask", "selection", "slot" },
                activateHandler = (_, root) => new SlotListView(root),
            };
        }

        private sealed class SlotListView
        {
            private readonly VisualElement slotList;
            private readonly TextField newSlotField;
            private readonly Button addButton;
            private readonly Label countLabel;

            private static CompassSettings Settings => CompassSettings.instance;

            public SlotListView(VisualElement root)
            {
                root.style.paddingLeft = 10f;
                root.style.paddingRight = 10f;
                root.style.paddingTop = 1f;

                root.Add(
                    new Label("Compass")
                    {
                        style =
                        {
                            fontSize = 19f,
                            unityFontStyleAndWeight = FontStyle.Bold,
                            marginBottom = 10f,
                        },
                    }
                );

                root.Add(
                    new HelpBox(
                        "Name the slots that Selection Navigator masks use. Two navigators can select each other "
                            + "only when their masks share a slot. Slot 0 is the default for new navigators.\n"
                            + "Removing a slot hides it from the mask dropdown. Navigators that already use it keep the bit.",
                        HelpBoxMessageType.Info
                    )
                );

                slotList = new VisualElement { style = { marginTop = 8f } };
                root.Add(slotList);

                var addRow = new VisualElement { style = { flexDirection = FlexDirection.Row, marginTop = 6f } };
                newSlotField = new TextField { value = NewSlotPlaceholder, style = { flexGrow = 1f } };
                newSlotField.RegisterValueChangedCallback(_ => RefreshAddButton());
                newSlotField.RegisterCallback<KeyDownEvent>(evt =>
                {
                    if (evt.keyCode is KeyCode.Return or KeyCode.KeypadEnter)
                        AddSlot();
                });
                addRow.Add(newSlotField);

                addButton = new Button(AddSlot) { text = "Add", style = { width = 64f } };
                addRow.Add(addButton);
                root.Add(addRow);

                countLabel = new Label
                {
                    style =
                    {
                        marginTop = 2f,
                        alignSelf = Align.FlexEnd,
                        fontSize = 10f,
                        color = new Color(0.6f, 0.6f, 0.6f, 1f),
                    },
                };
                root.Add(countLabel);

                Rebuild();

                Undo.undoRedoPerformed += OnUndoRedo;
                root.RegisterCallback<DetachFromPanelEvent>(_ => Undo.undoRedoPerformed -= OnUndoRedo);
            }

            private void Rebuild()
            {
                slotList.Clear();
                for (int i = 0; i < CompassSettings.SlotCount; i++)
                {
                    string slotName = Settings.GetSlotName(i);
                    if (i == 0 || !string.IsNullOrEmpty(slotName))
                        slotList.Add(CreateSlotRow(i, slotName));
                }

                countLabel.text = $"{Settings.NamedSlotCount} / {CompassSettings.SlotCount} slots";
                RefreshAddButton();
            }

            private VisualElement CreateSlotRow(int slot, string slotName)
            {
                bool isDefault = slot == 0;

                var row = new VisualElement
                {
                    style =
                    {
                        flexDirection = FlexDirection.Row,
                        alignItems = Align.Center,
                        marginBottom = 2f,
                    },
                };

                row.Add(CreateBitBadge(slot, isDefault));

                var nameField = new TextField
                {
                    value = slotName,
                    isDelayed = true,
                    style = { flexGrow = 1f },
                };
                nameField.RegisterValueChangedCallback(evt => RenameSlot(slot, evt.newValue));
                row.Add(nameField);

                var removeButton = new Button(() => RemoveSlot(slot))
                {
                    text = "−",
                    tooltip = isDefault
                        ? "Slot 0 is the default for new navigators and can't be removed."
                        : "Remove this slot from the mask dropdown.",
                    style = { width = 22f },
                };
                removeButton.SetEnabled(!isDefault);
                row.Add(removeButton);

                return row;
            }

            private static Label CreateBitBadge(int slot, bool isDefault)
            {
                return new Label(slot.ToString())
                {
                    tooltip = $"Mask bit {slot}",
                    style =
                    {
                        width = 26f,
                        unityTextAlign = TextAnchor.MiddleCenter,
                        marginRight = 4f,
                        paddingTop = 1f,
                        paddingBottom = 1f,
                        borderTopLeftRadius = 3f,
                        borderTopRightRadius = 3f,
                        borderBottomLeftRadius = 3f,
                        borderBottomRightRadius = 3f,
                        backgroundColor = isDefault
                            ? new Color(0.22f, 0.42f, 0.28f, 1f)
                            : new Color(0.25f, 0.25f, 0.25f, 1f),
                        color = new Color(0.85f, 0.85f, 0.85f, 1f),
                        fontSize = 10f,
                    },
                };
            }

            private void RefreshAddButton()
            {
                bool full = Settings.FindFreeSlot() < 0;
                addButton.SetEnabled(!full && IsAvailableName(newSlotField.value, -1));
                addButton.tooltip = full ? "All 32 slots are named." : string.Empty;
            }

            private void AddSlot()
            {
                string slotName = newSlotField.value;
                int slot = Settings.FindFreeSlot();
                if (slot < 0 || !IsAvailableName(slotName, -1))
                {
                    LogInvalidName(slotName);
                    return;
                }

                Undo.RecordObject(Settings, "Add Compass Slot");
                Settings.SetSlotName(slot, slotName);
                newSlotField.SetValueWithoutNotify(NewSlotPlaceholder);
                SaveAndRebuild();
            }

            private void RenameSlot(int slot, string slotName)
            {
                if (slotName.Trim() == Settings.GetSlotName(slot))
                    return;

                if (!IsAvailableName(slotName, slot))
                {
                    LogInvalidName(slotName);
                    Rebuild();
                    return;
                }

                Undo.RecordObject(Settings, "Rename Compass Slot");
                Settings.SetSlotName(slot, slotName);
                SaveAndRebuild();
            }

            private void RemoveSlot(int slot)
            {
                Undo.RecordObject(Settings, "Remove Compass Slot");
                Settings.SetSlotName(slot, string.Empty);
                SaveAndRebuild();
            }

            private static bool IsAvailableName(string slotName, int ownSlot)
            {
                if (string.IsNullOrWhiteSpace(slotName))
                    return false;

                int existing = Settings.FindSlot(slotName);
                return existing < 0 || existing == ownSlot;
            }

            private static void LogInvalidName(string slotName)
            {
                Debug.LogWarning(
                    $"[Compass] '{slotName}' isn't a valid slot name. Names must be non-empty and unique."
                );
            }

            private void OnUndoRedo()
            {
                Settings.SaveSettings();
                Rebuild();
            }

            private void SaveAndRebuild()
            {
                Settings.SaveSettings();
                Rebuild();
            }
        }
    }
}
