using UnityEngine;

public class DoorsManager : MonoBehaviour
{
    const string ANIMATOR_BOOL_NAME = "open";

    public static DoorsManager instance;

    private byte inRange = 0;
    [SerializeField] private Animator animator;

    private void Awake()
    {
        if (instance != null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        inRange = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void JoinRange()
    {
        inRange++;
        UpdateState();
    }

    public void LeaveRange()
    {
        inRange--;
        UpdateState();
    }

    private void UpdateState()
    {
        animator.SetBool(ANIMATOR_BOOL_NAME, inRange > 0);
    }
}
