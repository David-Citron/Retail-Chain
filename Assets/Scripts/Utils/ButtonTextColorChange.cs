using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonTextColorChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler
{
    [SerializeField]
    private TMP_Text buttonText;

    private Color normalColor = Color.white;
    private Color highlightedColor;

    public bool pressed = false;

    void Start()
    {
        Color gammaColor = new Color(49f / 255f, 54f / 255f, 56f / 255f, 1f);
        highlightedColor = gammaColor.linear;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (pressed) return;
        ChangeColor(highlightedColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (pressed) return;
        ChangeColor(normalColor);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (pressed)
        {
            Debug.LogError("Returned.");
            return;
        }
        pressed = true;
        ChangeColor(highlightedColor);
        PlayerManager.instance.StartCoroutine(UpdateButton());
    }

    private void ChangeColor(Color color)
    {
        if (buttonText == null) return;
        buttonText.color = color;
    }

    private IEnumerator UpdateButton()
    {
        yield return new WaitForSecondsRealtime(.5f);
        ChangeColor(normalColor);
        pressed = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ChangeColor(normalColor);
    }
}