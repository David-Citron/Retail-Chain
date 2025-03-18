using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements.Experimental;

public class Customer : MonoBehaviour
{
    const int MAX_STEPS = 3;

    [SerializeField] private ItemType desiredItem = ItemType.None;
    [SerializeField] private NavMeshAgent agent = null;

    private CustomerPoint reservedPoint = null;
    private Bill bill;

    private int stepsCount = 0;
    private ActionTimer timer = null;

    [SerializeField] private bool wantsToLeave = false;
    [SerializeField] private bool wantsToPay = false;
    [SerializeField] private bool isWalking = false;

    private CustomerBubble bubble = null;

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
        bubble = GetComponent<CustomerBubble>();
        bubble.HideBubble();
        if (bubble == null)
            Debug.LogError("Bubble not found!");
        stepsCount = 0;
        bill.itemType = ItemType.None;
        bill.price = 0;
        GenerateOffer();
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
        if (bill.itemType == desiredItem)
        {
            CustomerPoint point = CustomerManager.instance.FindAvailableQueuePoint(this);
            if (point == null)
            {
                Debug.LogError("No available point found!!!");
                return;
            }
            wantsToPay = true;
            ReservePoint(point);
            GoToPoint();
            return;
        }
        if (stepsCount >= MAX_STEPS)
        {
            Leave();
            ShopRating.instance.DecreaseRating(0.1f); //Decrease rating because customer did not find their items.
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
        {
            bubble.HideBubble();
            ReservePoint(foundPoint);
        }
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
            CashRegister.instance.UpdateHints();
            StartCoroutine(LookAtTarget());
            yield break;
        }
        if (wantsToLeave)
        {
            Optional<CustomerManager>.Of(CustomerManager.instance).IfPresent(manager => manager.CustomerLeaving(this));
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
        ItemData itemData = ItemManager.GetItemData(desiredItem);
        int maxPrice = PriceSystem.CalculateMaxPrice(itemData.sellPrice);
        int itemPrice = PriceSystem.GetPrice(itemData.itemType);
        if (itemPrice > maxPrice)
        {
            ShopRating.instance.DecreaseRating(0.05f); //Decrease rating because the prices are too high.
            bubble.SetBubbleData(BubbleState.TooExpensive, "$" + maxPrice);
            new ActionTimer(() =>
            {
                bubble.HideBubble();
            }, 1).Run();
            return;
        }

        if (CustomerManager.instance.FindAvailableQueuePoint(this) == null) return;
        
        if (!CustomerManager.instance.RemoveItemFromSlot(reservedPoint))
            return;
        bill.itemType = item.itemType;
        bill.price = itemPrice;
        bubble.HideBubble();
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
        if (PlayerManager.instance == null) return;
        PlayerManager.instance.GetLocalGamePlayer().IfPresent(player => player.bankAccount.AddBalance(bill.price));
        Leave();
        CustomerManager.instance.UpdateQueue();

        bool cheaperThanRecommended = bill.price <= PriceSystem.CalculateRecommendedPrice(ItemManager.GetItemData(bill.itemType).sellPrice);

        float rating = cheaperThanRecommended ? 0.15f : -0.1f;//If the price is cheaper than recommended increase rating.
        if(stepsCount >= 2) rating -= stepsCount * .05f; //If the steps count is >= 2 than decrease rating

        ShopRating.instance.IncreaseRating(rating);

        if (AudioManager.instance != null)
            AudioManager.instance.Play(1);
    }

    private void OnDestroy()
    {
        if (timer != null)
        {
            timer.Stop();
            timer = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Doors")) return;
        DoorsManager.instance.JoinRange();
        ActionTimer timer = new ActionTimer(() =>
        {
            bubble.SetBubbleData(BubbleState.DesiredItem, null);
        }, 1.3f).Run();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Doors")) return;
        DoorsManager.instance.LeaveRange();
    }

    public bool GetWantsToPay() => wantsToPay;
    public bool GetIsWalking() => isWalking;
    public ItemType GetDesiredItem() => desiredItem;
    public CustomerPoint GetReservation() => reservedPoint;
}

public struct Bill
{
    public ItemType itemType;
    public int price;
}