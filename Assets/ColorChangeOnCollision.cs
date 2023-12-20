using UnityEngine;

public class ColorChangeOnCollision : MonoBehaviour
{
    private Material originalMaterial;
    public Material collisionMaterial;

    private Renderer sphereRenderer;

    private void Start()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
                // Get the Renderer component of the sphere
        sphereRenderer = GetComponent<Renderer>();

        // Set the original material initially
        originalMaterial = sphereRenderer.material;
        // Check if the colliding object has a specific tag or other criteria if needed
        // For simplicity, we'll change the material on any collision
        sphereRenderer.material = collisionMaterial;
    }

    private void OnCollisionExit(Collision collision)
    {
        // Revert to the original material when the collision ends
        sphereRenderer.material = originalMaterial;

    }
}
