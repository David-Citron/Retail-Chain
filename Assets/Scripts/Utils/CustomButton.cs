using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(Shadow))]
public class CustomButton : MonoBehaviour,
        IPointerDownHandler, IPointerUpHandler,
        IPointerEnterHandler, IPointerExitHandler,
        ISelectHandler
{

    private static Dictionary<ButtonColor, ColorGroup> colorGroups = new Dictionary<ButtonColor, ColorGroup>();

    public ButtonColor buttonColor;

    private Button button;
    private Image image;
    private TMP_Text buttonText;
    private Shadow shadow;

    private bool disabled;

    private bool isPointerInside { get; set; }
    private bool isPointerDown { get; set; }

    void Awake()
    {
        if (colorGroups.Count != 0) return;
        colorGroups.Add(ButtonColor.Gray, new ColorGroup(new Color32(49, 54, 56, 255), new Color32(37, 37, 37, 255)));
        colorGroups.Add(ButtonColor.Pink, new ColorGroup(new Color32(236, 100, 97, 255), new Color32(147, 66, 66, 255)));
    }

    void Start()
    {
        button = GetComponent<Button>();
        buttonText = transform.GetComponentInChildren<TMP_Text>();
        shadow = GetComponent<Shadow>();
        image = GetComponent<Image>();

        button.interactable = true;
        button.transition = Selectable.Transition.SpriteSwap;
        SpriteState spriteState = button.spriteState;
        spriteState.highlightedSprite = image.sprite;
        button.spriteState = spriteState;

        shadow.effectDistance = new Vector2(10, -10);
        shadow.effectColor = GetColor(ButtonColorType.Shadow);

        ChangeTextColor(ButtonColorType.Normal);
    }

    private void OnDisable()
    {
        ChangeTextColor(ButtonColorType.Normal);
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
                if (disabled) return;
                ChangeTextColor(ButtonColorType.Shadow);
                break;
            case SelectionState.Normal:
                ChangeTextColor(ButtonColorType.Normal);
                break;
        }
    }

    /// <summary>
    /// Changes text & shadow colors
    /// </summary>
    /// <param name="color">New color</param>
    private void ChangeTextColor(ButtonColorType colorType)
    {
        if (buttonText != null) buttonText.color = colorType == ButtonColorType.Shadow ? GetColor(ButtonColorType.Normal) : Color.white;
        if(image != null) image.color = colorType == ButtonColorType.Shadow ? Color.white : GetColor(ButtonColorType.Normal);
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
        ChangeTextColor(ButtonColorType.Normal);
    }

    private Color32 GetColor(ButtonColorType colorType)
    {
        var colorGroup = colorGroups[buttonColor];
        if (colorGroup == null) return Color.white;
        switch (colorType)
        {
            case ButtonColorType.Normal: return colorGroup.color;
            case ButtonColorType.Shadow: return colorGroup.shadowColor;
        }
        return Color.white;
    }

    public void ChangeColorGroup(ButtonColor newColor)
    {
        if(buttonColor == newColor) return;
        buttonColor = newColor;

        ChangeTextColor(ButtonColorType.Normal);
        if(shadow != null) shadow.effectColor = GetColor(ButtonColorType.Shadow);
    }

    public void ToggleDisabled(bool value) => disabled = value;
}

public enum ButtonColor
{
    Gray,
    Pink
}

public enum ButtonColorType
{
    Normal,
    Shadow
}

public class ColorGroup
{
    public Color32 color;
    public Color32 shadowColor;

    public ColorGroup(Color32 color, Color32 shadowColor)
    {
        this.color = color;
        this.shadowColor = shadowColor;
    }
}