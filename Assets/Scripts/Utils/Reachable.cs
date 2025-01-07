using UnityEngine;

public abstract class Reachable : MonoBehaviour
{

    protected bool isReachable = false;

    void Start() { }
    void Update() { }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        SetReachable(true);
        Debug.LogWarning(this.gameObject.name);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        SetReachable(false);
    }

    protected void SetReachable(bool reachable)
    {
        isReachable = reachable;
        OnReachableChange();
    }

    protected abstract void OnReachableChange();
}
