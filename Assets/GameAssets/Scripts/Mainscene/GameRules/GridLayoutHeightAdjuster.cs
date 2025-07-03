using UnityEngine;
using UnityEngine.UI;
public class GridLayoutHeightAdjuster : MonoBehaviour
{
    public GridLayoutGroup Target;
    void Update()
    {
        //Debug.Log(Target.preferredHeight);
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x , Target.preferredHeight);
    }
}
