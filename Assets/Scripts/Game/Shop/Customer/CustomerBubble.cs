using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomerBubble : MonoBehaviour
{
    [SerializeField] private GameObject bubble;
    [SerializeField] private List<GameObject> bubbleTemplates;

    bool itemShown = false;

    // Start is called before the first frame update
    void Start()
    {
        itemShown = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HideBubble()
    {
        bubble.SetActive(false);
    }

    public void ShowBubble()
    {
        bubble.SetActive(true);
    }

    public void SetBubbleData(BubbleState state, string text)
    {
        if (itemShown && state == BubbleState.DesiredItem) return;
        ShowBubble();
        for (int i = 0; i < bubbleTemplates.Count; i++) bubbleTemplates[i].SetActive((((int)state) - 1) == i);
        if (state == BubbleState.DesiredItem)
        {
            itemShown = true;
            ItemType type = GetComponent<Customer>().GetDesiredItem();
            bubbleTemplates[((int)state) - 1].GetComponentInChildren<RawImage>().texture = ItemManager.GetIcon(type);
            return;
        }
        if (text == null) return;
        TMP_Text bubbleText = bubbleTemplates[((int)state) - 1].GetComponentInChildren<TMP_Text>();
        bubbleText.text = text;
    }
}

public enum BubbleState
{
    None,
    DesiredItem,
    TooExpensive,
}
