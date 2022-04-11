using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DraggableObject : MonoBehaviour
{
    public event Action OnStartDrag;
    public event Action OnEndDrag;

    private bool isDragging;

    private Camera cam;

	void Awake()
    {
        cam = Camera.main;
    }

    void Update() {
        if (isDragging) {
            var mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            this.transform.position = new Vector3(mousePos.x - .5f, mousePos.y - .5f, 0);
        }
    }

    void OnMouseDown()
    {
        isDragging = true;
        OnStartDrag?.Invoke();
    }

    void OnMouseUp()
    {
        isDragging = false;
        OnEndDrag?.Invoke();
    }
}
