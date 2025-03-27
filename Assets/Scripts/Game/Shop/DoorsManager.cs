using UnityEngine;

public class DoorsManager : MonoBehaviour
{
    const string ANIMATOR_BOOL_NAME = "open";

    public static DoorsManager instance;

    private byte inRange = 0;
    [SerializeField] private Animator door1Animator;
    [SerializeField] private Animator door2Animator;

    public bool state { get; private set; } = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        state = false;
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
        if (inRange > 1) return;
        if ((state && inRange > 0) || (!state && inRange == 0)) return;
        state = !state;
        door1Animator.SetBool(ANIMATOR_BOOL_NAME, inRange > 0);
        door2Animator.SetBool(ANIMATOR_BOOL_NAME, inRange > 0);
        if(AudioManager.instance != null) AudioManager.instance.Play(4);
    }
}
