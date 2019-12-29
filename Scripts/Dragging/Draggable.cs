using UnityEngine;
using System.Collections;
using DG.Tweening;
public class Draggable : MonoBehaviour {
    private bool dragging = false;

    private Vector3 pointerDisplacement;

    private float zDisplacement;

    private DraggingActions da;

    private static Draggable _draggingThis;
    public static Draggable DraggingThis
    {
        get{ return _draggingThis;}
    }

    void Awake()
    {
        da = GetComponent<DraggingActions>();
    }

    void OnMouseDown()
    {
        if (da!=null && da.CanDrag)
        {
            dragging = true;
            HoverPreview.PreviewsAllowed = false;
            _draggingThis = this;
            da.OnStartDrag();
            zDisplacement = -Camera.main.transform.position.z + transform.position.z;
            pointerDisplacement = -transform.position + MouseInWorldCoords();
        }
    }

    void Update ()
    {
        if (dragging)
        { 
            Vector3 mousePos = MouseInWorldCoords();
            Debug.Log(mousePos);
            transform.position = new Vector3(mousePos.x - pointerDisplacement.x, mousePos.y - pointerDisplacement.y, transform.position.z);   
            da.OnDraggingInUpdate();
        }
    }
	
    void OnMouseUp()
    {
        if (dragging)
        {
            dragging = false;
            HoverPreview.PreviewsAllowed = true;
            _draggingThis = null;
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
