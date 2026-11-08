# Compass

Compass is spatial selection navigation for uGUI in Unity 6. Add the Selection Navigator component to your buttons, and a d-pad, stick or arrow key press selects the nearest selectable in that direction. You don't wire explicit navigation links, so moving or resizing elements doesn't break navigation. Masks stop separate menus from selecting each other, and any direction can name a preferred target to override the spatial search.

[Installation](#installation) | [Quick start](#quick-start) | [How a move picks its target](#how-a-move-picks-its-target) | [Scripting](#scripting)

## Features

- Spatial search picks the closest valid target inside a search cone that starts at the selected element's facing edge. It reads positions at the moment of the move, so animated and runtime-built layouts work.
- Each navigator handles the four cardinal directions by default. You can enable diagonals per navigator and tune how far a stick must lean before a move counts as diagonal.
- Navigation masks have 32 named slots. Two navigators can reach each other only when their masks share a slot, so a popup and the menu behind it never select each other.
- Each direction has an ordered list of preferred targets. Compass tries them before the spatial search and selects the first valid one.
- Wrapping jumps to the opposite end of a row or column when the search finds nothing. It only wraps along the list's long axis, so grids never wrap.
- Priority decides between candidates at the same distance.
- Each direction can override the default search angle. An angle of 0 turns off spatial search for that direction.
- A focused `TMP_InputField` or `InputField` keeps its arrow keys while the player types.
- When you select a navigator, the Scene view draws its links. Preferred links are solid, spatial links are dotted, and wrap links have a "(wrap)" label. It also draws search cones and marks invalid links. This works in prefab mode too.

## Installation

Compass needs Unity 6000.3 or later and Unity UI (uGUI) 2.0, which includes TextMeshPro.

### Unity Package Manager (Git URL)

1. Open **Window > Package Manager**.
2. Click **+**, then **Add package from git URL...**.
3. Paste this URL and click **Add**:

```console
https://github.com/Tirtstan/Compass.git
```

This URL tracks the `main` branch, which holds the latest release. To install a specific version, add its tag:

```console
https://github.com/Tirtstan/Compass.git#v1.0.0
```

### manifest.json

Add this to `Packages/manifest.json`:

```json
{
    "dependencies": {
        "com.tirt.compass": "https://github.com/Tirtstan/Compass.git"
    }
}
```

## Quick start

1. Check that your scene has an EventSystem with an input module, such as the Input System UI Input Module or the Standalone Input Module.
2. Add **Compass > Selection Navigator** to every `Selectable` (Button, Toggle, Slider and so on) that should take part in navigation.
3. Enter Play Mode and move with a d-pad, stick or arrow keys.

Compass sets the Navigation mode of each `Selectable` to None, so Unity's built-in navigation doesn't run as well. If something sets it back, Compass sets it to None again and logs one warning.

### Separate menus with masks

1. Open **Edit > Project Settings > Compass** and name the slots you need, for example `Main Menu` and `Popup`. Slot 0 is `Default`. The mask dropdown hides slots with empty names.
2. On each navigator, choose its slots in the Navigation Mask dropdown.

Slot names exist only in the editor. Each navigator stores its mask as a plain bit mask, so renaming a slot doesn't change which navigators can select each other. Compass saves the names to `ProjectSettings/CompassSettings.asset`. Commit that file so your team sees the same names.

## How a move picks its target

When the selected navigator receives a move, it takes these steps:

1. It turns the move vector into a direction. If diagonals are allowed and the smaller axis is at least Diagonal Threshold times the larger axis, the move is diagonal. Otherwise Compass uses the larger axis.
2. It tries that direction's preferred targets in order and selects the first valid one.
3. It runs the spatial search. Every active navigator whose centre lies inside the direction's search angle is a candidate, and the closest one wins. Compass measures the angle and distance from the facing edge of the selected element.
4. If wrapping is on for that direction, it selects the element at the far end of the selected element's sibling list.

A target is valid when its GameObject is active, its `Selectable` is enabled and interactable, and its mask shares a slot with the source's mask.

### Inspector fields

| Field                | What it does                                                                                |
| -------------------- | ------------------------------------------------------------------------------------------- |
| Navigation Mask      | The slots this navigator belongs to.                                                        |
| Allowed Directions   | The directions this navigator handles: Cardinal, Diagonal, All, None or any combination.    |
| Wrap Directions      | The directions that wrap to the other end of the sibling list when the search finds nothing. |
| Priority             | The higher value wins when two candidates are the same distance away.                      |
| Default Search Angle | The widest angle from the direction, 0 to 180 degrees, for spatial candidates. Default 60. |
| Diagonal Threshold   | The smallest ratio of minor to major axis that counts as diagonal. Default 0.65.          |
| Directions           | For each direction, the preferred targets and an optional search angle override.          |

## Scripting

Every Inspector field has a property or method in the `Compass` namespace.

```csharp
using Compass;
using UnityEngine;

public class PopupNavigation : MonoBehaviour
{
    [SerializeField]
    private SelectionNavigator confirmButton;

    [SerializeField]
    private SelectionNavigator cancelButton;

    private void Awake()
    {
        // Pressing down from Confirm always selects Cancel.
        confirmButton.ReplacePreferredTargets(SelectionNavigationDirection.Down, new[] { cancelButton });

        // Search a narrow 20 degree cone to the right.
        confirmButton.SetSearchAngle(SelectionNavigationDirection.Right, 20f);

        // Allow diagonal moves and wrap horizontally.
        confirmButton.AllowedDirections = SelectionNavigationDirection.All;
        confirmButton.WrapDirections = SelectionNavigationDirection.Left | SelectionNavigationDirection.Right;
    }
}
```

| Member                                                   | What it does                                                                  |
| -------------------------------------------------------- | ----------------------------------------------------------------------------- |
| `NavigationMask`, `AllowedDirections`, `WrapDirections`, `Priority`, `DefaultSearchAngle`, `DiagonalThreshold` | Get or set the matching Inspector field. |
| `FindTarget(direction)`                                  | Returns the navigator that a move in `direction` would select, or `null`.     |
| `TryResolveDirection(moveVector, out direction)`         | Turns a raw move vector into a direction using this navigator's settings.     |
| `IsValidTarget(target)`                                  | Returns whether this navigator can select `target` right now.                |
| `ReplacePreferredTargets(direction, targets)`            | Replaces the preferred target list for one direction.                         |
| `GetSearchAngle`, `SetSearchAngle`, `ClearSearchAngle`   | Read, override or reset the search angle for one direction.                   |
| `FindAutomaticTarget`, `FindWrapTarget`                  | Run only the spatial search or only the wrap step. Both accept your own candidate list. |
| `SelectionNavigator.ActiveNavigators`                    | Lists every enabled navigator.                                                |

`NavigationMask` converts to and from `int` and supports the `&`, `|` and `~` operators. `NavigationMask.Everything` and `NavigationMask.Nothing` are the full and empty masks.

## License

Compass uses the [MIT license](LICENSE.md). Copyright 2026 Tristan.
