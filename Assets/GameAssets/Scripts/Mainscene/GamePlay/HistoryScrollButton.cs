using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HistoryScrollButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public enum ScrollDirection { Left, Right }

    [Header("Hold Settings")]
    [SerializeField] private float holdThreshold = 0.25f;

    [Header("Events")]
    public UnityEvent onClick;
    public UnityEvent onHold;

    private bool isPointerDown = false;
    private bool hasStartedHold = false;
    private float pointerDownTime;
    private float nextHoldTime;

    void Update ()
    {
        if (!isPointerDown) return;

        float elapsed = Time.unscaledTime - pointerDownTime;

        if (!hasStartedHold && elapsed >= holdThreshold)
        {
            hasStartedHold = true;
            nextHoldTime = Time.unscaledTime + holdThreshold;
            onHold?.Invoke();
        }
        else if (hasStartedHold && Time.unscaledTime >= nextHoldTime)
        {
            onHold?.Invoke();
            nextHoldTime = Time.unscaledTime + holdThreshold;
        }
    }

    public void OnPointerDown ( PointerEventData eventData )
    {
        isPointerDown = true;
        hasStartedHold = false;
        pointerDownTime = Time.unscaledTime;
    }

    public void OnPointerUp ( PointerEventData eventData )
    {
        if (!hasStartedHold)
        {
            onClick?.Invoke();
        }

        isPointerDown = false;
    }

    public void OnPointerExit ( PointerEventData eventData )
    {
        isPointerDown = false;
    }
}
