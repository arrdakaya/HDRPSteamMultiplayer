using UnityEngine;

public class OverlapControl : MonoBehaviour
{
    public static OverlapControl Instance;
    public Material blueprintMaterial;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    
    private void OnTriggerStay(Collider other)
    {
        
        if (other.CompareTag("Trap"))
        {
            TrapPlacer.instance.isOverlapping = true;
            foreach (Renderer previewObjectColor in GetComponentsInChildren<Renderer>())
            {
                previewObjectColor.material.color = Color.red;
            }
        }
       
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Trap"))
        {
            TrapPlacer.instance.isOverlapping = false;
            foreach (Renderer previewObjectColor in GetComponentsInChildren<Renderer>())
            {
                previewObjectColor.material = blueprintMaterial;
            }
        }
    }
   

}
