using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using static HistoryScrollButton;

public class GamePlayHistory : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private ScrollRect scrollRect;

    [SerializeField] private float stepDistance = 50f; // Width of one item in pixels
    [SerializeField] private float scrollDuration = 0.2f;
    private RectTransform content;
    private float contentWidth;
    private float viewportWidth;
    Tween myTween;
    public Transform leftBtn;
    public Transform rightBtn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (scrollRect == null)
        {
            Debug.LogError("ScrollRect is not assigned on ScrollStepButton.");
            enabled = false;
            return;
        }

        content = scrollRect.content;
        contentWidth = content.rect.width;
        viewportWidth = scrollRect.viewport.rect.width;
    }

    private void ScrollOnce (ScrollDirection direction)
    {
        float currentNormalized = scrollRect.horizontalNormalizedPosition;

        float scrollableWidth = content.rect.width - scrollRect.viewport.rect.width;
        if (scrollableWidth <= 0f) return;

        float stepNormalized = stepDistance / scrollableWidth;
        if (direction == ScrollDirection.Left) stepNormalized *= -1f;

        float targetNormalized = Mathf.Clamp01(currentNormalized + stepNormalized);

        // Optional sound
        CommandCenter.Instance.soundManager_.PlaySound("BetButton");

        // Tween
        DOTween.Kill(myTween);
        myTween = DOTween.To(
            () => scrollRect.horizontalNormalizedPosition ,
            value => scrollRect.horizontalNormalizedPosition = value ,
            targetNormalized ,
            scrollDuration
        ).SetEase(Ease.OutQuad);
        Transform btn = null;
        if (direction == ScrollDirection.Right)
        {
            btn = rightBtn;
        }
        else if(direction == ScrollDirection.Left)
        {
            btn = leftBtn;
        }
        // Optional punch animation
        btn.transform.DOPunchScale(new Vector3(0.1f , 0.1f , 0.1f) , 0.25f , 0 , 1);
    }

    public void scrollleft ()
    {
        Debug.Log("scroll left!");
        ScrollOnce(ScrollDirection.Left);
    }

    public void scrollright ()
    {
        Debug.Log("scroll right!");
        ScrollOnce(ScrollDirection.Right);
    }
}
