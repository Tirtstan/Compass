# Changelog

This file lists the notable changes in each release. Versions follow [Semantic Versioning](https://semver.org/).

## [1.0.0] - 2026-11-08

### Added

- The `SelectionNavigator` component moves uGUI selection to the nearest target in up to 8 directions. Each direction has its own preferred targets and search angle, and priority settles ties.
- Wrapping selects the far end of a row or column along its long axis.
- `NavigationMask` has 32 slots. You name them in **Project Settings > Compass**.
- The Scene view draws each navigator's links, search cones and invalid targets, including in prefab mode.
- A focused `TMP_InputField` or `InputField` keeps its arrow keys while the player types.
