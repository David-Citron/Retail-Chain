using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ButtonTextColorChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Button button;
    public TMP_Text buttonText;

    public Color normalColor = Color.white;
    public Color highlightedColor;

    public double elapsedTime => throw new System.NotImplementedException();

    void Start() {
        Color gammaColor = new Color(49f / 255f, 54f / 255f, 56f / 255f, 1f);
        highlightedColor = gammaColor.linear;
    }

    void Update() { }

    public void OnPointerClick(PointerEventData eventData)
    {
        buttonText.color = highlightedColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.color = highlightedColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.color = normalColor;
    }
}