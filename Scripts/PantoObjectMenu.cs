using UnityEditor;
using UnityEngine;
public class PantoObjectMenu : MonoBehaviour
{
    // Add a menu item named "Do Something" to MyMenu in the menu bar.
    [MenuItem("DualPanto/Do Something")]
    static void DoSomething()
    {
        Debug.Log("Doing Something...");
    }

    // Add a menu item to create custom GameObjects.
    // Priority 1 ensures it is grouped with the other menu items of the same kind
    // and propagated to the hierarchy dropdown and hierarchy context menus.
    [MenuItem("GameObject/Panto/Player", false, 10)]
    static void CreatePantoPlayer(MenuCommand menuCommand)
    {
        // Create a custom game object
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.AddComponent<Player>();
        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }

    [MenuItem("GameObject/Panto/Wall", false, 9)]
    static void CreatePantoWall(MenuCommand menuCommand)
    {
        // Create a custom game object
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.AddComponent<PantoBoxCollider>();
        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(wall, menuCommand.context as GameObject);
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(wall, "Create " + wall.name);
        Selection.activeObject = wall;
    }
}
