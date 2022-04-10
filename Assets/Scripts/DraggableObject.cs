using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DraggableObject : MonoBehaviour
{
    public event Action OnEndDrag;

    private Camera cam;

	void Awake()
    {
        cam = Camera.main;
    }

	void OnMouseOver()
    {
        if (Input.GetMouseButton(0))
        {
            var mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            this.transform.position = new Vector3(mousePos.x - .5f, mousePos.y - .5f, 0);
        }
    }

    void OnMouseUp()
    {
        OnEndDrag?.Invoke();
    }
}
