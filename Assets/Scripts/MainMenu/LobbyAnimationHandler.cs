using System.Collections;
using UnityEngine;

public class LobbyAnimationHandler : MonoBehaviour
{

    [SerializeField] private Animator televisionAnimator;

    void Start()
    {
        StartCoroutine(PlayTelevisionEffects());
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private IEnumerator PlayTelevisionEffects()
    {
        yield return new WaitForSecondsRealtime(Random.Range(10, 20));
        televisionAnimator.SetTrigger("effect");
        StartCoroutine(PlayTelevisionEffects());
    }
}
