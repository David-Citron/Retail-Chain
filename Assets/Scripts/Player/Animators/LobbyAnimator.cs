using System.Collections;
using UnityEngine;

public class LobbyAnimator : MonoBehaviour
{

    [SerializeField] private Animator animator;
    [SerializeField] private Animator readyButtonAnimator;

    void Start()
    {
        StartCoroutine(CallAnimation());
    }

    private IEnumerator CallAnimation()
    {
        yield return new WaitForSeconds(Random.Range(10, 30));
        animator.SetTrigger("lobby_emote_wave");
        StartCoroutine(CallAnimation());
    }

    public void playReadyAnimation()
    {
        readyButtonAnimator.SetTrigger("ready");
    }
}