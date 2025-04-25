using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIRenderTextureForwarder : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler, IDragHandler
{
    public RawImage rawImage;
    public Canvas targetCanvas;
    public GraphicRaycaster targetRaycaster;

    private RectTransform rawImageRect;
    private RectTransform canvasRect;
    private Camera uiCamera;

    void Awake()
    {
        rawImageRect = rawImage.rectTransform;
        canvasRect = targetCanvas.GetComponent<RectTransform>();
        uiCamera = targetCanvas.worldCamera;
    }

    public void OnPointerClick(PointerEventData eventData) => ForwardEvent(eventData, ExecuteEvents.pointerClickHandler);
    public void OnPointerDown(PointerEventData eventData) => ForwardEvent(eventData, ExecuteEvents.pointerDownHandler);
    public void OnPointerUp(PointerEventData eventData) => ForwardEvent(eventData, ExecuteEvents.pointerUpHandler);
    public void OnPointerEnter(PointerEventData eventData) => ForwardEvent(eventData, ExecuteEvents.pointerEnterHandler);
    public void OnPointerExit(PointerEventData eventData) => ForwardEvent(eventData, ExecuteEvents.pointerExitHandler);
    public void OnPointerMove(PointerEventData eventData) => ForwardEvent(eventData, ExecuteEvents.pointerMoveHandler);
    public void OnDrag(PointerEventData eventData) => ForwardEvent(eventData, ExecuteEvents.dragHandler);

    void ForwardEvent<T>(PointerEventData sourceEventData, ExecuteEvents.EventFunction<T> handler) where T : IEventSystemHandler
    {
        // Convert screen point to RawImage UV coordinates
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rawImageRect,
            sourceEventData.position,
            sourceEventData.pressEventCamera,
            out Vector2 localPoint);

        // Convert to normalized UV coordinates (0 to 1)
        Vector2 uv = new Vector2(
            (localPoint.x + rawImageRect.rect.width * 0.5f) / rawImageRect.rect.width,
            (localPoint.y + rawImageRect.rect.height * 0.5f) / rawImageRect.rect.height);

        // Flip Y if needed (depends on your texture setup)
        // uv.y = 1f - uv.y;

        // Map UV to target canvas space
        Vector2 canvasPos = new Vector2(
            uv.x * canvasRect.rect.width - canvasRect.rect.width * 0.5f,
            uv.y * canvasRect.rect.height - canvasRect.rect.height * 0.5f);

        // Convert canvas local position to world position
        Vector3 worldPos = canvasRect.TransformPoint(canvasPos);

        // Convert world position to screen position using the canvas' camera
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, worldPos);

        // Create new PointerEventData with corrected position
        PointerEventData newEventData = new PointerEventData(EventSystem.current)
        {
            position = screenPos,
            button = sourceEventData.button,
            pointerId = sourceEventData.pointerId,
            pointerCurrentRaycast = sourceEventData.pointerCurrentRaycast,
            pointerEnter = sourceEventData.pointerEnter,
            pointerPress = sourceEventData.pointerPress,
            pointerDrag = sourceEventData.pointerDrag,
            pressPosition = sourceEventData.pressPosition,
            clickTime = sourceEventData.clickTime,
            clickCount = sourceEventData.clickCount,
            scrollDelta = sourceEventData.scrollDelta,
            useDragThreshold = sourceEventData.useDragThreshold,
            dragging = sourceEventData.dragging,
            eligibleForClick = sourceEventData.eligibleForClick
        };

        // Raycast against target canvas
        List<RaycastResult> results = new List<RaycastResult>();
        targetRaycaster.Raycast(newEventData, results);

        // Execute events on hit objects
        foreach (var result in results)
        {
            ExecuteEvents.Execute(result.gameObject, newEventData, handler);
            
            // For move/enter/exit events, we want to handle only the topmost element
            if (IsMoveOrEnterExitEvent(handler))
            {
                break;
            }
        }
    }

    private bool IsMoveOrEnterExitEvent<T>(ExecuteEvents.EventFunction<T> handler) where T : IEventSystemHandler
    {
        return handler.Equals(ExecuteEvents.pointerMoveHandler) || 
               handler.Equals(ExecuteEvents.pointerEnterHandler) || 
               handler.Equals(ExecuteEvents.pointerExitHandler);
    }
}