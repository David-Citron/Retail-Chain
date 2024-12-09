using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Shadow))]
public class CustomButton : MonoBehaviour,
        IPointerDownHandler, IPointerUpHandler,
        IPointerEnterHandler, IPointerExitHandler,
        ISelectHandler
{
    private TMP_Text buttonText;
    private Button button;
    private Shadow shadow;

    private Color normalColor = Color.white;
    private Color32 highlightedColor = new Color32(49, 54, 56, 255);

    private Color32 shadowColor = new Color32(188, 188, 188, 255);
    private Color32 highlightedColorShadow = new Color32(33, 37, 39, 255);

    private bool isPointerInside { get; set; }
    private bool isPointerDown { get; set; }

    void Start()
    {
        buttonText = transform.GetComponentInChildren<TMP_Text>();
        button = GetComponent<Button>();
        shadow = GetComponent<Shadow>();

        shadow.effectDistance = new Vector2(10, -10);
        shadow.effectColor = shadowColor;
    }

    protected enum SelectionState
    {
        Normal, Highlighted
    }

    protected SelectionState currentSelectionState
    {
        get
        {
            if (isPointerDown) return SelectionState.Normal;
            if (isPointerInside) return SelectionState.Highlighted;
            return SelectionState.Normal;
        }
    }

    protected void DoStateTransition(SelectionState state)
    {
        switch (state)
        {
            case SelectionState.Highlighted:
                ChangeColor(highlightedColor);
                break;
            case SelectionState.Normal:
                ChangeColor(normalColor);
                break;
        }
    }

    private void ChangeColor(Color color)
    {
        if (buttonText != null) buttonText.color = color;
        if (shadow != null) shadow.effectColor = color == normalColor ? shadowColor : highlightedColorShadow;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        isPointerDown = true;
        DoStateTransition(currentSelectionState);
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        isPointerDown = false;
        DoStateTransition(currentSelectionState);
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        isPointerInside = true;
        DoStateTransition(currentSelectionState);
    }
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        isPointerInside = false;
        DoStateTransition(currentSelectionState);
    }

    public virtual void OnSelect(BaseEventData eventData)
    {
        PlayerManager.instance.StartCoroutine(DeselectAfterFrame());
    }

    private IEnumerator DeselectAfterFrame()
    {
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(null);
        ChangeColor(normalColor);
    }
}