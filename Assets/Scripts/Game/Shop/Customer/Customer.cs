using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour
{
    [SerializeField] private ItemType desiredItem = ItemType.None;
    [SerializeField] private NavMeshAgent agent = null;
    private Transform targetTransform;
    private List<Transform> pathPoints = new List<Transform>();
    private int currentPathIndex = 0;

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
        GenerateOffer();
        GeneratePath();
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
            if (data.sellable) validItemTypes.Add(data.itemType);
        });
        int generatedIndex = Random.Range(0, validItemTypes.Count);
        desiredItem = validItemTypes[generatedIndex];
    }

    private void GeneratePath()
    {
        currentPathIndex = 0;
        CustomerManager.instance.displayTables.ForEach(table =>
        {
            pathPoints.Add(table.transform);
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        float time = Random.Range(3f, 6f);
        if (other.gameObject == targetTransform.gameObject)
        {
            new ActionTimer(() => {
                NextMove();
            }, time);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        targetTransform = newTarget;
        agent.SetDestination(targetTransform.position);
    }

    private void NextMove()
    {
        currentPathIndex++;
        if (currentPathIndex < pathPoints.Count)
            SetTarget(pathPoints[currentPathIndex]);
        else
            Leave();
    }

    private void Leave()
    {
        Debug.LogWarning("Customer wants to leave");
    }
}
