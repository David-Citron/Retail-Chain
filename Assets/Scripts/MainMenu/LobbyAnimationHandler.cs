using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyAnimationHandler : MonoBehaviour
{

    [SerializeField] private List<Animator> animators;

    void Start()
    {
        StartCoroutine(PlayEffects());
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private IEnumerator PlayEffects()
    {
        yield return new WaitForSecondsRealtime(Random.Range(10, 20));
        animators.ForEach(animator => animator.SetTrigger("effect"));
        StartCoroutine(PlayEffects());
    }
}
