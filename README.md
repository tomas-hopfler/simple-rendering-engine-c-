# Simple Software Rendering Engine (C#)

This project is a simple software rendering engine implemented in C#. It provides basic 3D rendering capabilities without relying on hardware acceleration.

## Features

- Import, view, and modify .obj files
- Export modified .obj files
- User input handling (W, A, S, D keys and mouse)
- Two rendering modes:
  - WinAPI window
  - Direct console rendering

## Core Components

### Base Classes

- **Vertex**: Represents a point in 3D space
  - Methods: rotation, movement, scaling, basic operators, cross product, distance calculation
- **Edge**: Represents an edge (two vertices)
  - Used for rendering purposes
- **Face**: A list of vertices
  - Methods: center, normal, getEdges (for rendering)
- **Mesh**: Represents any 3D object
  - Contains lists of vertices and faces, position, rotation, and color
  - Methods: rotation, movement, scaling, simplifying, bounding box calculation
- **FrameBuffer**: Represents the frame as an integer array
  - Methods: setPixel, getPixel, drawLine, drawPolygon, fillPolygon

### Display Classes

- **ConsoleDisplay**: Double-buffered console display with shading and adjustable pixel size
- **WindowsDisplay**: WinAPI-based display (no shading)

### Object Classes

- Cube, Prism, Pyramid, Sphere

### Scene Management

- **Camera**: Handles 3D to 2D projection
- **Scene**: Container for all objects, implements render methods for each display type

### Utility Classes

- **Input**: Handles keyboard and mouse input (mouse is locked to the center of the display)
- **MeshLoader**: Imports and exports .obj files (exporting entire scenes)

## Usage

The `Program.cs` file provides an example of how to use the engine. Refer to this file for a quick start guide.
