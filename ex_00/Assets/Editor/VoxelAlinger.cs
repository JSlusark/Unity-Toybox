using UnityEngine;
using UnityEditor;
using System.Linq;

public class VoxelAligner : EditorWindow
{
    [MenuItem("Tools/Voxel Layout/Line Up Horizontally (X Axis)")]
    public static void LayoutHorizontalX() => LineUpVoxels(Vector3.right);

    [MenuItem("Tools/Voxel Layout/Line Up Horizontally (Z Axis)")]
    public static void LayoutHorizontalZ() => LineUpVoxels(Vector3.back);

    [MenuItem("Tools/Voxel Layout/Line Up Vertically (Y Axis)")]
    public static void LayoutVertical() => LineUpVoxels(Vector3.up);

    private static void LineUpVoxels(Vector3 direction)
    {
        if (Selection.gameObjects.Length == 0) return;

        // 1. Sort the selected objects so they keep their rough relative order
        // (You can sort by current position or by hierarchy name)
        var sortedObjects = Selection.gameObjects
            .OrderBy(obj => Vector3.Dot(obj.transform.position, direction))
            .ToList();

        // 2. Use the first object's position as the starting point
        Vector3 currentPosition = sortedObjects[0].transform.position;

        // Record for Undo history in Unity
        Undo.RecordObjects(Selection.transforms, "Pack Voxels");

        for (int i = 0; i < sortedObjects.Count; i++)
        {
            GameObject obj = sortedObjects[i];
            
            // 3. Move the object to the next calculated position
            obj.transform.position = currentPosition;

            // 4. Calculate the size of this specific voxel using its Renderer bounds
            float voxelSize = 1.0f; // Default fallback
            if (obj.TryGetComponent<Renderer>(out var renderer))
            {
                // Get the size matching our target direction
                voxelSize = Vector3.Scale(renderer.bounds.size, direction).magnitude;
            }

            // 5. Advance the placement coordinate by exactly one voxel width for the next item
            currentPosition += direction * voxelSize;
        }

        Debug.Log($"Aligned {sortedObjects.Count} voxels side-by-side.");
    }
}
