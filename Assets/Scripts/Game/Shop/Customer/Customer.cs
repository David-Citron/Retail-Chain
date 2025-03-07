using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour
{
    [SerializeField] private ItemType desiredItem = ItemType.None;
    [SerializeField] private NavMeshAgent agent = null;
    private Transform currentTarget;
    private DisplaySlot currentTargetSlot;
    private ItemType inventory;

    private int stepsCount = 0;
    private ActionTimer timer = null;

    [SerializeField] private bool wantsToLeave = false;
    [SerializeField] private bool wantToPay = false;

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
        inventory = ItemType.None;
        GenerateOffer();
        Debug.Log("New customer wants: " + desiredItem);
        FindTarget();
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
        List<ItemType> validItemTypes = new List<ItemType>();
        ItemManager.GetAllSellableItemData().ForEach(data => validItemTypes.Add(data.itemType));
        int generatedIndex = Random.Range(0, validItemTypes.Count);
        desiredItem = validItemTypes[generatedIndex];
    }

    private void FindTarget()
    {
        if (inventory == desiredItem)
        {
            Debug.LogWarning("Going to cash register!!!");
            CustomerPoint point = CustomerManager.instance.FindAvailableQueuePoint(this);
            if (point == null)
            {
                Debug.LogError("No available point found!!!");
                return;
            }
            currentTarget = point.point;
            currentTargetSlot = point.displayTable;
            wantToPay = true;
            agent.SetDestination(currentTarget.position);
            StartCoroutine(WaitForArrival());
            return;
        }
        if (stepsCount >= 3)
        {
            Leave();
            return;
        }
        Transform previousTarget = currentTarget;
        currentTarget = null;
        currentTargetSlot = null;
        Transform target = null;
        DisplaySlot slot = null;
        if (CustomerManager.instance.TryFindDisplaySlot(desiredItem, out slot, out target))
        {
            currentTarget = target;
            currentTargetSlot = slot;
        }
        while (target == null && currentTarget == null)
        {
            Debug.LogWarning("SEARCHING FOR RANDOM");
            if (!CustomerManager.instance.TryFindRandomDisplaySlot(out slot, out target))
            {
                Debug.Log("No random target found!");
                return;
            }
            if (previousTarget == null) break;
            if (previousTarget.position == target.position) target = null;
        }
        currentTarget = target;
        agent.SetDestination(target.position);
        stepsCount++;
        StartCoroutine(WaitForArrival());
    }

    private IEnumerator WaitForArrival()
    {
        yield return new WaitUntil(() =>
            !agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance &&
            agent.velocity.sqrMagnitude < 0.1f
        );
        if (wantToPay)
        {
            Debug.Log("Waiting in queue");
            yield break;
        }
        if (wantsToLeave)
        {
            Debug.Log("Leaving!");
            Optional<CustomerManager>.Of(CustomerManager.instance).IfPresent(manager => manager.CustomerLeaving());
            Destroy(gameObject);
            yield break;
        }
        ArrivedToDestination();
        timer = new ActionTimer(() => 
        {
            TryTakeItem();
            timer = new ActionTimer(() => FindTarget(), 2).Run();
        }, 2).Run();
    }

    private void TryTakeItem()
    {
        if (currentTargetSlot == null)
            return;
        GameObject slot = currentTargetSlot.FindClosestItemSlot(transform.position);
        if (slot != null && currentTargetSlot.GetItemFromSlot(slot).itemType == desiredItem)
        {
            currentTargetSlot.RemoveItemFromSlot(slot);
            inventory = desiredItem;
            return;
        }
        return;
    }

    private void ArrivedToDestination()
    {
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
        wantsToLeave = true;
        agent.SetDestination(CustomerManager.instance.customerSpawnPoint.position);
        StartCoroutine(WaitForArrival());
    }
}
