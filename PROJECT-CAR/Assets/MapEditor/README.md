# Road Map Creator
## Complete Documentation

![Road Map Creator Logo](Images/LogoPlaceholder.png)

**Version 1.0**  
**© 2023 Your Company Name**

---

## Table of Contents

1. [Introduction](#1-introduction)
2. [Installation](#2-installation)
3. [Quick Start Guide](#3-quick-start-guide)
4. [MapEditor Component](#4-mapeditor-component)
    1. [Properties](#41-properties)
    2. [Edit Modes](#42-edit-modes)
    3. [Working with Control Points](#43-working-with-control-points)
5. [RoadGenerator Component](#5-roadgenerator-component)
    1. [Road Settings](#51-road-settings)
    2. [Terrain Settings](#52-terrain-settings)
    3. [Texture Settings](#53-texture-settings)
6. [Step-by-Step Tutorials](#6-step-by-step-tutorials)
    1. [Creating a Basic Road](#61-creating-a-basic-road)
    2. [Customizing Road Appearance](#62-customizing-road-appearance)
    3. [Creating a Closed Loop Road](#63-creating-a-closed-loop-road)
    4. [Advanced Curve Editing](#64-advanced-curve-editing)
7. [Import/Export System](#7-importexport-system)
8. [API Reference](#8-api-reference)
    1. [MapEditor Class](#81-mapeditor-class)
    2. [RoadGenerator Class](#82-roadgenerator-class)
9. [Tips and Best Practices](#9-tips-and-best-practices)
10. [Troubleshooting](#10-troubleshooting)
11. [Support and Contact](#11-support-and-contact)

---

## 1. Introduction

Road Map Creator is a powerful yet easy-to-use tool for creating roads, paths, and tracks in your Unity projects. Whether you're developing a racing game, city simulator, or any project that requires custom path creation, this tool provides an intuitive editor workflow and high-quality mesh generation.

**Key Features:**
- Intuitive point-and-click interface for creating paths
- Bezier curve system for smooth roads with precise control
- Automatic mesh generation for both the road surface and adjacent terrain
- Customizable road width, materials, and texture tiling
- Support for multiple roads per editor
- Import/export functionality to save and load your road designs
- Support for both open paths and closed loops
- Complete editor integration with undo/redo support

---

## 2. Installation

1. Import the Road Map Creator package into your Unity project
2. Ensure you have a terrain or other collidable surface in your scene where you want to place the road
3. Import the MapEditor prefab into your scene
4. Ensure the MapEditor prefab is positioned at (0,0,0)

**Minimum Requirements:**
- Unity 2020.3 or higher
- Universal Render Pipeline (URP) or Built-In Render Pipeline

---

## 3. Quick Start Guide

Follow these steps to quickly create your first road:

1. Select the MapEditor prefab in your scene
2. In the Inspector, click "Create New Road" to add a road to your map
3. Change the Edit Mode to "AddPoints"
4. Click on your terrain in the Scene view to add points along your desired path
5. Add at least 3-4 points to see a nice curve
6. Switch to "EditPoints" mode to fine-tune your path
7. Select any point to adjust its position or control handles
8. Select the road in the hierarchy to customize its properties
9. Ensure the RoadGenerator component has appropriate materials assigned

You can create multiple roads using the "Create New Road" button in the MapEditor inspector.

Your first road is now complete! Continue reading for more detailed instructions on customization and advanced features.

---

## 4. MapEditor Component

The MapEditor component is the core of the road creation system. It manages the path points and provides the interface for editing.

### 4.1 Properties

| Property | Description |
|----------|-------------|
| Edit Mode | Controls the current editing behavior (Disabled, AddPoints, EditPoints) |
| Show Control Points | Toggles visibility of Bezier curve control handles |
| Curve Resolution | Controls the smoothness of the curve (higher values = smoother curves but more vertices) |
| Closed Path | When enabled, connects the last point to the first point to form a loop |
| Multiple Roads | The MapEditor can have multiple roads as children, each with its own settings |

### 4.2 Edit Modes

- **Disabled**: No editing is possible in the Scene view
- **AddPoints**: Click on surfaces to add new points to your path
- **EditPoints**: Select and modify existing points and their control handles

### 4.3 Working with Control Points

Each point on your path is a Bezier anchor point with two control handles:
- The **In Handle** controls how the curve approaches this point
- The **Out Handle** controls how the curve leaves this point

To edit control points:
1. Ensure "Show Control Points" is enabled
2. Switch to "EditPoints" mode
3. Select a point on your path
4. Blue spheres will appear representing the control handles
5. Click and drag these handles to adjust the curve shape
6. Control handles can also be edited numerically in the Inspector when a point is selected

---

## 5. RoadGenerator Component

The RoadGenerator takes a path defined by a MapEditor and generates a mesh representation of a road along that path.

### 5.1 Road Settings

| Setting | Description |
|---------|-------------|
| Map Editor | Reference to the MapEditor component defining the path |
| Road Width | Width of the road in world units |
| Road Material | Material to be applied to the road surface |
| Height Offset | Small Y-axis offset to prevent z-fighting with terrain |

### 5.2 Terrain Settings

| Setting | Description |
|---------|-------------|
| Terrain Material | Material to apply to side terrain areas |
| Terrain Size | Width of terrain on each side of the road |
| Height Offset | Vertical offset of terrain relative to road (negative values = below road) |

### 5.3 Texture Settings

| Setting | Description |
|---------|-------------|
| UV Tiling Density | Controls texture repetition along the road's length |
| UV Tiling Width | Controls texture tiling across the road's width |
| Flip Normals | Toggle if road material appears upside-down |

---

## 6. Step-by-Step Tutorials

### 6.1 Creating a Basic Road

1. Import the MapEditor prefab into your scene
2. Ensure the MapEditor prefab is positioned at (0,0,0)
3. Assign a material to the "Road Material" field in the RoadGenerator component
4. In the MapEditor, set Edit Mode to "AddPoints"
5. Click on your terrain to add points, creating a path
6. Add at least 3-4 points to form a nice curve
7. The road mesh will generate automatically along this path

### 6.2 Customizing Road Appearance

1. Select the MapEditor prefab
2. Find the RoadGenerator component
3. Adjust the "Road Width" to make the road wider or narrower
4. Modify "UV Tiling Density" to control texture repetition along the road:
   - Lower values create stretched textures
   - Higher values create more repetition
5. Adjust "UV Tiling Width" to control texture tiling across the width
6. Assign a different material to change the road's appearance
7. Click "Regenerate Mesh" to apply changes if needed

### 6.3 Creating a Closed Loop Road

1. Create a road using the steps from the basic road tutorial
2. Add enough points to form your desired loop shape
3. Position the last point near the first point
4. In the MapEditor Inspector, check the "Closed Path" option
5. The system will automatically connect the last point to the first
6. Adjust any control points as needed to ensure a smooth transition

### 6.4 Advanced Curve Editing

1. Create a road with at least 4-5 points
2. Switch the MapEditor to "EditPoints" mode
3. Select a point in the middle of your path
4. Notice the blue control handles extending from the point
5. Drag the handles to adjust the curve shape:
   - Pulling a handle further from its anchor point creates a sharper curve
   - Moving handles in different directions creates asymmetrical curves
6. Select different points and adjust their handles to perfect your path
7. For precise control, select a point and edit its handle positions numerically in the Inspector

---

## 7. Import/Export System

The Road Map Creator includes functionality to save and load your paths:

**To Export a Path:**
1. Select your MapEditor GameObject
2. Click the "Export Path" button in the Inspector
3. Choose a location to save the JSON file
4. Your path data is now saved

**To Import a Path:**
1. Select your MapEditor GameObject
2. Click the "Import Path" button in the Inspector
3. Select a previously exported JSON file
4. Your path will be loaded, replacing the current path

---

## 8. API Reference

### 8.1 MapEditor Class

**Public Properties:**
- `currentMode`: Controls the editing mode (Enum: EditMode.Disabled, AddPoints, EditPoints)
- `selectedPointIndex`: Index of the currently selected point
- `showControlPoints`: Toggles control points visibility
- `curveResolution`: Resolution of the Bezier curve (segments per curve)

**Public Methods:**
- `AddPoint(Vector3 point)`: Adds a new point to the path
- `UpdatePointPosition(int index, Vector3 newPosition)`: Updates the position of an existing point
- `UpdateControlPoint(int pointIndex, HandleType handleType, Vector3 newPosition)`: Updates a control point position
- `SelectPoint(int index)`: Selects a point by index
- `GetSavedPoints()`: Returns a list of all path points
- `GetBezierPoints()`: Returns a list of all Bezier points
- `GetPointCount()`: Returns the number of points in the path
- `GetPointAt(int index)`: Returns the position of a specific point
- `GetBezierPointAt(int index)`: Returns the Bezier point at the specified index
- `ClearPoints()`: Removes all points
- `GetBezierCurvePoints()`: Returns a list of points along the bezier curve

### 8.2 RoadGenerator Class

**Public Properties:**
- `mapEditor`: Reference to the MapEditor component
- `roadWidth`: Width of the road
- `roadMaterial`: Material for the road surface
- `terrainMaterial`: Material for the side terrain
- `terrainSize`: Width of terrain on each side
- `terrainHeightOffset`: Vertical offset for terrain
- `uvTilingDensity`: Texture tiling along road length
- `uvTilingWidth`: Texture tiling across road width
- `flipNormals`: Option to flip normals if needed
- `heightOffset`: Slight offset to prevent z-fighting

**Public Methods:**
- `SetClosedPath(bool isClosed)`: Sets whether the path is closed (loops)
- `RegenerateMesh()`: Forces regeneration of the road mesh

---

## 9. Tips and Best Practices

- **Multiple Roads**: Use multiple roads for different road types (highways, dirt paths, etc.) sharing the same path
- **Performance**: Keep curve resolution at a reasonable level (10-15) for good performance
- **Road Planning**: Sketch your road layout before placing points for better results
- **Point Spacing**: Place points closer together for tight curves, farther apart for straighter sections
- **Control Handles**: For sharp turns, keep control handles closer to their anchor points
- **Materials**: Use materials with tiling textures designed for roads for best visual results
- **Terrain Integration**: Adjust the terrain height offset to blend roads seamlessly with your landscape
- **Undo Support**: Remember that you can use Ctrl+Z (Cmd+Z on Mac) to undo changes
- **Selection**: Click away from points to deselect the current point
- **Prefab Position**: Ensure the MapEditor prefab remains at position (0,0,0) for best results

---

## 10. Troubleshooting

**Road mesh is not generating:**
- Ensure the MapEditor prefab is at position (0,0,0)
- Verify that you have at least 2 points in your path
- Check that materials are assigned to the RoadGenerator
- Click the "Regenerate Mesh" button to force an update
- Make sure the road's MapEditor reference is set correctly

**Road appears upside-down:**
- Try enabling the "Flip Normals" option in the RoadGenerator

**Z-fighting between road and terrain:**
- Increase the "Height Offset" value slightly

**Curves are too angular:**
- Increase the "Curve Resolution" value in the MapEditor
- Add more points to create smoother transitions

**Editor is unresponsive:**
- Ensure you're in the correct Edit Mode for your desired action
- Check the Console for any error messages
- Restart Unity if issues persist

---

## 11. Support and Contact

For additional support:
- Email: valvano.m@live.it
- Website: www.maurovalvano.it
- Documentation Updates: Check the Unity Asset Store page for the latest documentation

We welcome your feedback and feature suggestions for future updates!

---

*This document was last updated on 02/03/2025.*
