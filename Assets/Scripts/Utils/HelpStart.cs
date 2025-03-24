using TMPro;
using UnityEngine;

public class HelpStart : MonoBehaviour
{

    [SerializeField] private GameObject help;
    [SerializeField] private TMP_Text helpText;

    void Start()
    {
        helpText.text = "PRESS " + Hint.GetHintButton(ActionType.Help) + " FOR HELP";
        new ActionTimer(() => help.SetActive(false), 5).Run();
    }


    void Update() {}
}
