using TMPro;
using UnityEngine;

public class RemainingContractSummary : MonoBehaviour
{
    [SerializeField] private TMP_Text leftText;
    [SerializeField] private TMP_Text rightText;

    void Start() { }
    void Update() { }

    public void LoadData(string left, string right)
    {
        leftText.text = left;
        rightText.text = right;
    }
}
