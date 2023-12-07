using UnityEngine;

public class OnPipeInitialPlacement : MonoBehaviour
{

    public GameObject greenSpherePrefab; // Assign your green sphere prefab in the Unity Editor
                                         // Call this function to add a green sphere as a child to the last child of the given game object
    public GameObject parent;
    public void AddGreenSphereToLastChild()
    {
        Debug.Log(parent);
        if (parent.transform.childCount > 0)
        {
            // Get the last child transform
            Transform lastChild = parent.transform.GetChild(parent.transform.childCount - 2);
            Debug.Log(parent.transform.childCount -2);
            Debug.Log(lastChild);

            // Instantiate the green sphere prefab as a child of the last child
            GameObject greenSphere = Instantiate(greenSpherePrefab, lastChild);

            // Optionally, you can set the position and rotation of the green sphere
            greenSphere.transform.localPosition = Vector3.zero;
            greenSphere.transform.localRotation = Quaternion.identity;
            
        }
        else
        {
            Debug.Log("No children found.");
        }

    }
}
