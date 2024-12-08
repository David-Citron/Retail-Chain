using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Shadow))]
public class CustomButton : Selectable
{
    private TMP_Text buttonText;
    private Button button;
    private Shadow shadow;

    private Color normalColor = Color.white;
    private Color32 highlightedColor = new Color32(49, 54, 56, 255);

    private Color32 shadowColor = new Color32(188, 188, 188, 255);
    private Color32 highlightedColorShadow = new Color32(33, 37, 39, 255);

    protected override void Start()
    {
        base.Start();
        buttonText = transform.GetComponentInChildren<TMP_Text>();
        button = GetComponent<Button>();
        shadow = GetComponent<Shadow>();

        shadow.effectDistance = new Vector2(10, -10);
        shadow.effectColor = shadowColor;

        transition = Transition.None;
    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);

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

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        PlayerManager.instance.StartCoroutine(DeselectAfterFrame());
    }

    private void ChangeColor(Color color)
    {
        if (buttonText != null) buttonText.color = color;
        if (shadow != null) shadow.effectColor = color == normalColor ? shadowColor : highlightedColorShadow;
    }
    private IEnumerator DeselectAfterFrame()
    {
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(null);
        ChangeColor(normalColor);
    }
}