using UnityEngine;
using UnityEngine.EventSystems;

public class UIDragGroup : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private bool enableReposition = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        LoadUIPosition();
    }



    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!enableReposition)
            return;

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!enableReposition)
            return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!enableReposition)
            return;

        SaveUIPosition(rectTransform.anchoredPosition);

        canvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// Hook this up to a button that will control whether or not we're in the mode to reposition UI elements.
    /// </summary>
    public void OnToggleEnableReposition() {
        enableReposition = !enableReposition;
    }





    private void SetUIPosition(Vector2 pos)
    {
        rectTransform.anchoredPosition = pos;
    }

    string GetUniqueKey()
    {
        Transform currentTransform = transform;
        string uniqueKey = "";

        while (currentTransform != null)
        {
            uniqueKey = currentTransform.name + "." + uniqueKey;
            currentTransform = currentTransform.parent;
        }

        return uniqueKey.TrimEnd('.') + "_UIPosition";
    }

    private void SaveUIPosition(Vector2 pos)
    {
        string key = GetUniqueKey();
        PlayerPrefs.SetString(key, $"{pos.x},{pos.y}");
        PlayerPrefs.Save();
    }

    private void LoadUIPosition()
    {
        string key = GetUniqueKey();
        if (PlayerPrefs.HasKey(key))
        {
            string[] savedPos = PlayerPrefs.GetString(key).Split(',');
            if (savedPos.Length == 2)
            {
                float x = float.Parse(savedPos[0]);
                float y = float.Parse(savedPos[1]);
                rectTransform.anchoredPosition = new Vector2(x, y);
            }
        }
    }

}

