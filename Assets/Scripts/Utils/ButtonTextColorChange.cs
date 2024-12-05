using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Shadow))]
public class ButtonTextColorChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, ISelectHandler
{
    private TMP_Text buttonText;
    private Button button;
    private Shadow shadow;

    private Color normalColor = Color.white;
    private Color32 highlightedColor = new Color32(49, 54, 56, 255);
    private Color32 shadowColor = new Color32(188, 188, 188, 255);

    void Start()
    {
        buttonText = transform.GetComponentInChildren<TMP_Text>();
        button = GetComponent<Button>();
        shadow = GetComponent<Shadow>();

        shadow.effectDistance = new Vector2(10, -10);
        shadow.effectColor = shadowColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ChangeColor(highlightedColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ChangeColor(normalColor);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ChangeColor(highlightedColor);
        PlayerManager.instance.StartCoroutine(UpdateButton());
    }

    private void ChangeColor(Color color)
    {
        if (buttonText != null) buttonText.color = color;
        if (shadow != null) shadow.effectColor = color == normalColor ? shadowColor : highlightedColor;
    }

    private IEnumerator UpdateButton()
    {
        yield return new WaitForSecondsRealtime(.2f);
        ChangeColor(normalColor);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ChangeColor(normalColor);
    }

    public void OnSelect(BaseEventData eventData)
    {
        PlayerManager.instance.StartCoroutine(DeselectAfterFrame());
    }


    private IEnumerator DeselectAfterFrame()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(null);
        ChangeColor(normalColor);
    }
}