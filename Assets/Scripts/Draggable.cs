using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    CanvasGroup canvasGroup;
    Transform parentToReturnTo;
    Vector3 mousePos = Vector3.zero;

    private void Awake()
    {
        canvasGroup = this.GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentToReturnTo = transform.parent;
        transform.SetParent(GameManager.Instance.Canvas.transform);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        mousePos = Camera.main.ScreenToWorldPoint(eventData.position);
        mousePos.z = 0;
        transform.position = mousePos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("end");
        transform.SetParent(parentToReturnTo);
        canvasGroup.blocksRaycasts = true;

        /*List<GameObject> hoveredList = eventData.hovered;
        foreach (var GO in hoveredList)
        {
            Debug.Log("Hovering over: " + GO.name);
        }*/

    }

}
