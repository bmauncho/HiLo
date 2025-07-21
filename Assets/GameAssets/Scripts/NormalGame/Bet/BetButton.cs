using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;

public class BetButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private BetType betType;
    public float holdThreshold = 0.25f;
    public UnityEvent onClick;
    public UnityEvent onHold;

    private bool isPointerDown = false;
    private bool hasStartedHold = false;
    private bool interactable = true;
    private float pointerDownTime;
    private float nextHoldTime;

    public GameObject Mask;

    void Update ()
    {
        if (CommandCenter.Instance == null) return;

        switch (betType)
        {
            case BetType.Increase:
                interactable = !CommandCenter.Instance.betManager_.IsHighestBetAmount();
                if (Mask != null)
                {
                    Mask.SetActive(!interactable);
                }
                break;
            case BetType.Decrease:
                interactable = !CommandCenter.Instance.betManager_.IsLowestBetAmount();
                if (Mask != null)
                {
                    Mask.SetActive(!interactable);
                }
                break;
        }

        if (!interactable)
        {
            isPointerDown = false; // stop any ongoing hold
            transform.localScale = Vector3.one;
            return;
        }


        if (isPointerDown)
        {
            float elapsed = Time.unscaledTime - pointerDownTime;

            if (!hasStartedHold)
            {
                if (elapsed >= holdThreshold)
                {
                    hasStartedHold = true;
                    nextHoldTime = Time.unscaledTime + holdThreshold;
                    onHold?.Invoke();
                    transform.DOScale(0.8f , 0.25f);
                }
            }
            else if (Time.unscaledTime >= nextHoldTime)
            {
                onHold?.Invoke();
                nextHoldTime = Time.unscaledTime + holdThreshold;
            }
        }
    }

    public void OnPointerDown ( PointerEventData eventData )
    {
        if (!interactable) return;

        isPointerDown = true;
        hasStartedHold = false;
        pointerDownTime = Time.unscaledTime;
    }

    public void OnPointerUp ( PointerEventData eventData )
    {
        if (!interactable)
        {
            interactable = true;
            return;
        }

        if (!hasStartedHold)
        {
            transform.DOPunchScale(new Vector3(-0.2f , -0.2f , -0.2f) , 0.25f , 0 , 1);
            onClick?.Invoke();
        }
        else
        {
            transform.DOScale(1f , 0.25f);
        }

        isPointerDown = false;
    }

    public void OnPointerExit ( PointerEventData eventData )
    {
        if (!interactable)
        {
            interactable = true;
            return;
        }

        isPointerDown = false;
        transform.DOScale(1f , 0.25f); // Reset scale just in case
    }

    public void SetInteractable ( bool value )
    {
        interactable = value;
    }

    public bool IsInteractable ()
    {
        return interactable;
    }
}
