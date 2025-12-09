using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
public class ButtonClickOnDown : MonoBehaviour
{
    GraphicRaycaster raycaster;
    private PointerEventData pointerData;
    float cooldowntimestamp;

    void Start()
    {
        raycaster = GetComponent<GraphicRaycaster>();
        raycaster.enabled = false;
        pointerData = new PointerEventData(EventSystem.current);
    }

    void Update()
    {
       
        pointerData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        if (results.Count > 0)
        {
            GameObject hitObj = results[0].gameObject;
            if (hitObj.GetComponentInParent<ScrollRect>())
            {
                raycaster.enabled = true;
            }
            else
            {

                raycaster.enabled = false;

            }
        }

        if (Input.GetMouseButtonDown(0) && !raycaster.enabled) 
        {
            if (cooldowntimestamp > Time.time)
                return;
            cooldowntimestamp = Time.time + 0.1f;
            Click();
        }
    }
    void Click()
    {
        pointerData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        if (results.Count > 0)
        {
            GameObject hitObj = results[0].gameObject;

            Button btn = hitObj.GetComponentInParent<Button>();
            if (btn&&btn.interactable)
            {
                btn.onClick.Invoke();
                EventSystem.current.SetSelectedGameObject(null);
            }
            Toggle _toggle = hitObj.GetComponentInParent<Toggle>();
            if (_toggle&&_toggle.interactable)
            {
                _toggle.isOn = !_toggle.isOn;
                EventSystem.current.SetSelectedGameObject(null);
            }

        }
    }
    
}
