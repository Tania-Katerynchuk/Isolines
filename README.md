# Isolines

### What is it?
A project written in C# .NET Windows Forms that defines and builds an isoline map for a random set of 30 points with values that changes linearly with distance using the Delaunay triangulation method.

### To-do:
- remove the update of points when expanding the window;
- set a timer for drawing the graphic or create cancel token to prevent event spam and overload (1-2 seconds).

### Practical use:
Isolines are lines drawn on a map connecting data points of the same value. They are commonly used by geographers. Contour lines, for example, show relief and connect points on the map that have the same height. Equally, isobars show bands of high and low pressure and connect points that have the same atmospheric pressure.
