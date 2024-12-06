using System.Collections;
using UnityEngine;

public class PlayerLobbyEmoteHandler : MonoBehaviour
{

    [SerializeField] private Animator animator;

    void Start()
    {
        StartCoroutine(CallAnimation());
    }

    private IEnumerator CallAnimation()
    {
        yield return new WaitForSeconds(Random.Range(10, 20));
        animator.SetBool("lobby_emote_waving", true);
        StartCoroutine(StopAnimation());
    }

    private IEnumerator StopAnimation()
    {
        yield return new WaitForSeconds(4.5f);
        animator.SetBool("lobby_emote_waving", false);
        if (LayoutManager.Instance().GetValueOrDefault() != null) StartCoroutine(CallAnimation());
    }
}