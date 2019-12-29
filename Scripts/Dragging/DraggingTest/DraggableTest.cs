using UnityEngine;
using System.Collections;

public class DraggableTest : MonoBehaviour 
{
    public bool UsePointerDisplacement = true;
    private bool dragging = false;
    private Vector3 pointerDisplacement = Vector3.zero;
    private float zDisplacement;
    void OnMouseDown()
    {
        dragging = true;
        zDisplacement = -Camera.main.transform.position.z + transform.position.z;
        if (UsePointerDisplacement)
            pointerDisplacement = -transform.position + MouseInWorldCoords();
        else
            pointerDisplacement = Vector3.zero;
    }
    void Update ()
    {
        if (dragging)
        { 
            Vector3 mousePos = MouseInWorldCoords();
            Debug.Log(mousePos);
            transform.position = new Vector3(mousePos.x - pointerDisplacement.x, mousePos.y - pointerDisplacement.y, transform.position.z);   
        }
    }

    void OnMouseUp()
    {
        if (dragging)
        {
            dragging = false;
        }
    }    
    private Vector3 MouseInWorldCoords()
    {
        var screenMousePos = Input.mousePosition;
        Debug.Log(screenMousePos);
        screenMousePos.z = zDisplacement;
        return Camera.main.ScreenToWorldPoint(screenMousePos);
    }

}