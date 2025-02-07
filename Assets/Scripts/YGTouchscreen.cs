using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class YGTouchscreen : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
{
    [field: SerializeField] public bool Invert { get; private set; }
    public float sensitivityPanelRotate = 1;

    public bool pressed = false;
    public int fingerId;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            pressed = true;
            fingerId = eventData.pointerId;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;
    }
    
    public Vector2 GetTouchscreenInput()
    {
        float mouseX = 0;
        float mouseY = 0;
        
        if (pressed)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.fingerId == fingerId)
                {
                    if (touch.phase == TouchPhase.Moved)
                    {
                        mouseY = touch.deltaPosition.y * sensitivityPanelRotate;
                        mouseX = touch.deltaPosition.x * sensitivityPanelRotate;
                    }

                    if (touch.phase == TouchPhase.Stationary)
                    {
                        mouseY = 0;
                        mouseX = 0;
                    }
                }
            }
        }

        if (Invert)
        {
            return new Vector2(-mouseX, -mouseY);
        }
        return new Vector2(mouseX, mouseY);
    }
}
