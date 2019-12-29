using UnityEngine;
using System.Collections;

public class DraggableTestWithActions : MonoBehaviour {

    public bool UsePointerDisplacement = true;
    private DraggingActionsTest da;
    private bool dragging = false;
    private Vector3 pointerDisplacement = Vector3.zero;
    private float zDisplacement;
    void Awake()
    {
        da = GetComponent<DraggingActionsTest>();
    }

    void OnMouseDown()
    {
        if (da.CanDrag)
        {
            dragging = true;
            da.OnStartDrag();
            zDisplacement = -Camera.main.transform.position.z + transform.position.z;
            if (UsePointerDisplacement)
                pointerDisplacement = -transform.position + MouseInWorldCoords();
            else
                pointerDisplacement = Vector3.zero;
        }
    }

    void Update ()
    {
        if (dragging)
        { 
            Vector3 mousePos = MouseInWorldCoords();
            da.OnDraggingInUpdate();
            Debug.Log(mousePos);
            transform.position = new Vector3(mousePos.x - pointerDisplacement.x, mousePos.y - pointerDisplacement.y, transform.position.z);   
        }
    }

    void OnMouseUp()
    {
        if (dragging)
        {
            dragging = false;
            da.OnEndDrag();
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
