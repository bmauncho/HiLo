using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;

public class BetButton : MonoBehaviour , IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{

    public float holdThreshold = 0.25f; // Time in seconds to qualify as hold

    public UnityEvent onClick;
    public UnityEvent onHold;

    private bool isPointerDown = false;
    private float pointerDownTime;
    private float nextHoldTime;
    private bool hasStartedHold = false;

    void Update ()
    {
        if (isPointerDown)
        {
            float elapsed = Time.unscaledTime - pointerDownTime;

            if (!hasStartedHold)
            {
                if (elapsed >= holdThreshold)
                {
                    hasStartedHold = true;
                    nextHoldTime = Time.unscaledTime + holdThreshold;
                    onHold?.Invoke(); // First hold call
                    transform.DOScale(.8f , .25f);
                }
            }
            else
            {
                if (Time.unscaledTime >= nextHoldTime)
                {
                    onHold?.Invoke();
                    nextHoldTime = Time.unscaledTime + holdThreshold;
                }
            }
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
            transform.DOPunchScale(
                new Vector3(-0.2f , -0.2f , -0.2f) ,.25f ,0 ,1);
            onClick?.Invoke(); // Only trigger click if it wasn't a hold
        }
        else
        {
            transform.DOScale(1f , .25f);
        }

         isPointerDown = false;
    }

    public void OnPointerExit ( PointerEventData eventData )
    {
        isPointerDown = false;
    }
}
