using UnityEngine;
using UnityEngine.EventSystems;

public class RemoveComponentOnClick : MonoBehaviour, IPointerClickHandler
{
    // Specify the type of component to remove
    public string componentName;

    public void OnPointerClick(PointerEventData eventData)
    {
        // Check if the right mouse button was clicked
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("Right-click detected");

            // Remove the specified component
            RemoveComponent(componentName);
        }
    }

    void RemoveComponent(string componentName)
    {
        // Use reflection to get the component type
        System.Type componentType = System.Type.GetType(componentName);
        if (componentType != null)
        {
            Component component = GetComponent(componentType);
            if (component != null)
            {
                Destroy(component);
                Debug.Log(componentName + " has been removed from " + gameObject.name);
            }
            else
            {
                Debug.Log(gameObject.name + " does not have a " + componentName + " component to remove.");
            }
        }
        else
        {
            Debug.Log("Component type " + componentName + " not found.");
        }
    }
}
