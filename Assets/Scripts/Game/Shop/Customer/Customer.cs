using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour
{
    [SerializeField] private ItemType desiredItem = ItemType.None;
    [SerializeField] private NavMeshAgent agent = null;
    private Transform currentTarget;

    private int stepsCount = 0;
    private ActionTimer timer = null;

    void Awake()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                Debug.LogError("Agent not found");
                return;
            }
        }
        stepsCount = 0;
        GenerateOffer();
        //TestTarget();
        FindTarget(); //TODOODODODOODDOO
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void GenerateOffer()
    {
        List<ItemData> itemData = ItemManager.GetAllItemData();
        List<ItemType> validItemTypes = new List<ItemType>();
        itemData.ForEach(data => {
            if (data.IsSellable()) validItemTypes.Add(data.itemType);
        });
        int generatedIndex = Random.Range(0, validItemTypes.Count);
        desiredItem = validItemTypes[generatedIndex];
    }

    private void FindTarget()
    {
        if (stepsCount >= 3)
            Leave();
        Transform target = CustomerManager.instance.GetDisplayTableWithItem(desiredItem);
        while (target == null || (target != null && currentTarget.position == target.position))
        {
            target = CustomerManager.instance.GetRandomDisplayTable();
        }
        currentTarget = target;
        agent.SetDestination(target.position);
        stepsCount++;
        StartCoroutine(WaitForArrival());
    }

    private void TestTarget()
    {
        Transform target = CustomerManager.instance.displayTablePoints[0].transform;
        currentTarget = target;
        agent.SetDestination(target.position);
        StartCoroutine(WaitForArrival());
    }

    private IEnumerator WaitForArrival()
    {
        yield return new WaitUntil(() =>
            !agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance &&
            agent.velocity.sqrMagnitude < 0.1f
        );
        ArrivedToDestination();
        timer = new ActionTimer(() => FindTarget(), 5).Run();
    }

    private void ArrivedToDestination()
    {
        Debug.LogWarning("ARRIVED!!!!!!");
        if (currentTarget != null)
        {
            StartCoroutine(LookAtTarget(currentTarget));
        }
    }

    private IEnumerator LookAtTarget(Transform target)
    {
        float duration = 0.5f;
        float elapsed = 0f;
        Quaternion startRotation = transform.rotation;

        Vector3 direction = target.position - transform.position;
        direction.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
    }

    private void Leave()
    {
        Debug.LogWarning("Customer wants to leave");
    }
}
