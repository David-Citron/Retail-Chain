using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonTextColorChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
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
        buttonText.color = highlightedColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (pressed) return;
        buttonText.color = normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (pressed) return;
        pressed = true;
        buttonText.color = highlightedColor;
        PlayerManager.instance.StartCoroutine(UpdateButton());
    }

    private IEnumerator UpdateButton()
    {
        yield return new WaitForSecondsRealtime(.5f);
        buttonText.color = normalColor;
        pressed = false;
    }
}