using UnityEngine;

public class Utilities
{
    /// <summary>
    /// Définit un nouvel objet parent sans modifier l'apparence dans la scène.
    /// </summary>
    /// <param name="child">L'objet à reparenté.</param>
    /// <param name="newParent">Le nouvel objet parent.</param>
    public static void SetParent(Transform child, Transform newParent)
    {
        var worldPosition = child.position;
        var worldRotation = child.rotation;
        var worldScale = child.lossyScale;
        
        child.SetParent(newParent);
        
        child.position = worldPosition;
        child.rotation = worldRotation;
        
        var parentScale = newParent != null ? newParent.lossyScale : Vector3.one;
        child.localScale = new Vector3(
            worldScale.x / parentScale.x,
            worldScale.y / parentScale.y,
            worldScale.z / parentScale.z
        );
    }
}
