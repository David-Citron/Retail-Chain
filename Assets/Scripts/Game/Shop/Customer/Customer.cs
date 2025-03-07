using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour
{
    const int MAX_STEPS = 3;

    [SerializeField] private ItemType desiredItem = ItemType.None;
    [SerializeField] private NavMeshAgent agent = null;

    private CustomerPoint reservedPoint = null;
    private ItemType inventory;

    private int stepsCount = 0;
    private ActionTimer timer = null;

    [SerializeField] private bool wantsToLeave = false;
    [SerializeField] private bool wantsToPay = false;
    [SerializeField] private bool isWalking = false;

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
        reservedPoint = null;
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
            CustomerPoint point = CustomerManager.instance.FindAvailableQueuePoint(this);
            if (point == null)
            {
                Debug.LogError("No available point found!!!");
                return;
            }
            wantsToPay = true;
            Debug.Log("About to reserve: " + point.point.position);
            ReservePoint(point);
            GoToPoint();
            return;
        }
        if (stepsCount >= MAX_STEPS)
        {
            Leave();
            return;
        }
        CustomerPoint previousPoint = null;
        if (reservedPoint != null)
        {
            previousPoint = reservedPoint;
            CancelReservation();
        }
        CustomerPoint foundPoint = CustomerManager.instance.TryFindDisplaySlot(desiredItem);
        if (foundPoint != null)
            ReservePoint(foundPoint);
        while (reservedPoint == null)
        {
            CustomerPoint point = CustomerManager.instance.TryFindRandomDisplaySlot();
            if (point == null)
            {
                Debug.Log("No random target found!");
                return;
            }
            if (previousPoint != null && previousPoint == point) continue;
            ReservePoint(point);
        }
        stepsCount++;
        GoToPoint();
    }

    private IEnumerator WaitForArrival()
    {
        yield return new WaitUntil(() =>
            !agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance &&
            agent.velocity.sqrMagnitude < 0.1f
        );
        isWalking = false;
        if (wantsToPay)
        {
            Debug.Log("Waiting in queue");
            StartCoroutine(LookAtTarget());
            yield break;
        }
        if (wantsToLeave)
        {
            Debug.Log("Leaving!");
            Optional<CustomerManager>.Of(CustomerManager.instance).IfPresent(manager => manager.CustomerLeaving());
            Destroy(gameObject);
            yield break;
        }
        StartCoroutine(LookAtTarget());
        timer = new ActionTimer(() => 
        {
            TryTakeItem();
            timer = new ActionTimer(() => FindTarget(), 1).Run();
        }, 3).Run();
    }

    private void TryTakeItem()
    {
        if (reservedPoint == null)
            return;
        Item item = CustomerManager.instance.GetItemFromSlot(reservedPoint);
        if (item == null) return;
        if (item.itemType != desiredItem) return;
        // TODO: add validation
        if (!CustomerManager.instance.RemoveItemFromSlot(reservedPoint))
            return;
        inventory = item.itemType;
        return;
    }

    private IEnumerator LookAtTarget()
    {
        if (reservedPoint == null)
            yield break;

        float duration = 0.5f;
        float elapsed = 0f;
        Quaternion startRotation = transform.rotation;

        Vector3 direction = reservedPoint.point.position - transform.position;
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

    public void Leave()
    {
        wantsToLeave = true;
        CancelReservation();
        agent.SetDestination(CustomerManager.instance.customerSpawnPoint.position);
        StartCoroutine(WaitForArrival());
    }

    private bool ReservePoint(CustomerPoint newPoint)
    {
        if (reservedPoint != null)
            CancelReservation();

        if (!CustomerManager.instance.MakeReservation(newPoint, this))
        {
            Debug.LogError("Couldn't reserve the point!");
            return false;
        }
        reservedPoint = newPoint;
        return true;
    }

    private bool CancelReservation()
    {
        if (!CustomerManager.instance.CancelReservation(reservedPoint))
        {
            Debug.LogError("Couldn't cancel the reservation!");
            return false;
        }
        reservedPoint = null;
        return true;
    }

    public void GoToPoint()
    {
        if (reservedPoint == null)
        {
            Debug.LogError("Can't go to unassigned point!");
            return;
        }
        isWalking = true;
        agent.SetDestination(reservedPoint.point.position);
        StartCoroutine(WaitForArrival());
    }

    public void RequestReservation(CustomerPoint point)
    {
        ReservePoint(point);
        GoToPoint();
    }

    public void Pay()
    {
        wantsToPay = false;
        Leave();
        CustomerManager.instance.UpdateQueue();
    }

    private void OnDestroy()
    {
        if (timer != null)
        {
            timer.Stop();
            timer = null;
        }
    }

    public bool GetWantsToPay() => wantsToPay;
    public bool GetIsWalking() => isWalking;
    public CustomerPoint GetReservation() => reservedPoint;
}
