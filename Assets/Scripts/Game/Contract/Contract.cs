using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Contract : NetworkBehaviour
{
    public List<ContractItem> currentContractItems { get; private set; }
    [SerializeField] private PlayerRole ownerRole = PlayerRole.Unassigned;
    [SyncVar(hook = nameof(HookStatus))]
    public ContractStatus status = ContractStatus.Unknown;
    private ActionTimer timer = null;

    private void Start()
    {
        /*if ((isLocalPlayer && !isServer) || (!isLocalPlayer && isServer))*/ syncDirection = SyncDirection.ClientToServer; // !!! Maybe it can be set ClientToServer everytime
        currentContractItems = new List<ContractItem>();

        // Local player only:
        if (!isLocalPlayer) return;
        PlayerManager.instance.GetLocalGamePlayer().IfPresentOrElse(localPlayer =>
        {
            ownerRole = localPlayer.playerRole;
        }, () => Debug.LogError("Local player was not found"));
    }

    // Local player only
    public void StartNewContract(List<ContractItem> newContractItems, int newContractTime)
    {
        if (!isLocalPlayer)
        {
            Debug.LogError("Trying to start a new contract on non-local instance");
            return;
        }
        status = ContractStatus.Pending;
        currentContractItems = newContractItems;
        ReloadRemainingContractDataUI();
        ReloadTimeUI(newContractTime, 0);
        timer = new ActionTimer(actionTimer => 
        {
            ReloadTimeUI(newContractTime, (int)actionTimer.passedTime);
        }, () => 
        {
            CheckContractStatus();
        }, null, newContractTime, 1).Run();
    }

    public void HookStatus(ContractStatus oldValue, ContractStatus newValue)
    {
        if (newValue == ContractStatus.Failed || newValue == ContractStatus.Success) ContractManager.instance.CmdCheckContracts();
    }

    private void CheckContractStatus()
    {
        switch (ownerRole)
        {
            case PlayerRole.Factory:
                bool allItemsFulfilled = true;
                currentContractItems.ForEach(item => {
                    if (item.fulfilled) return;
                    allItemsFulfilled = false;
                });
                if (allItemsFulfilled) status = ContractStatus.Success;
                else status = ContractStatus.Failed;
                break;
            case PlayerRole.Shop:
                int sum = 0;
                currentContractItems.ForEach(item => sum += item.price);
                if (PlayerManager.instance == null)
                {
                    Debug.LogError("PlayerManager is null!");
                    status = ContractStatus.Failed;
                }
                PlayerManager.instance.GetLocalGamePlayer().IfPresentOrElse(localPlayer =>
                {
                    if (localPlayer.bankAccount.GetBalance() >= sum) status = ContractStatus.Success;
                    else status = ContractStatus.Failed;
                }, () =>
                {
                    Debug.LogError("Local game player was not found!");
                    status = ContractStatus.Failed;
                });
                break;
            default:
                Debug.LogError("Player role is not assigned correctly");
                return;
        }
        ContractManager.instance.CmdCheckContracts();
    }

    public void FinalizeContract()
    {
        if (!isLocalPlayer) return;

        if (PlayerManager.instance == null)
        {
            Debug.LogError("PlayerManager is null!");
            return;
        }

        GamePlayer localPlayer = PlayerManager.instance.GetLocalGamePlayer().GetValueOrDefault();
        if (PlayerManager.instance.GetLocalGamePlayer().GetValueOrDefault() == null)
        {
            Debug.LogError("Local GamePlayer not found!");
            return;
        }
        
        int sum = 0;

        currentContractItems.ForEach(item => sum += item.price * item.quantity);

        switch (ownerRole)
        {
            case PlayerRole.Factory:
                localPlayer.bankAccount.AddBalance(sum, "Contract");
                break;
            case PlayerRole.Shop:
                if(!localPlayer.bankAccount.RemoveBalance(sum, "Contract"))
                {
                    Game.instance.EndGame();
                    return;
                }

                ContractDelivery.instance.DeliverItems(currentContractItems);
                break;
            default:
                Debug.LogError("Player role is not assigned correctly!");
                return;
        }

        localPlayer.bankAccount.PayTaxes();
    }

    public bool SubmitItem(ItemType itemType)
    {
        foreach (ContractItem item in currentContractItems)
        {
            if (item.itemType != itemType) continue;
            if (item.fulfilled) continue;
            item.ItemSubmitted();
            ReloadRemainingContractDataUI();
            return true;
        }

        return false;
    }

    public void ReloadTimeUI(int contractTime, int passedTime)
    {
        if (ContractManager.instance.remainingContractItemsTimer == null) return;
        int remainingDuration = (contractTime - passedTime);
        ContractManager.instance.remainingContractItemsTimer.text = $"{(remainingDuration / 60):00}:{(remainingDuration % 60):00}";
    }

    public void ReloadRemainingContractDataUI()
    {
        if (ContractManager.instance.remainingContractItemPrefab == null) 
        { 
            Debug.LogError("Remaining Contract Item Prefab is null"); 
            return; 
        }
        if (ContractManager.instance.remainingContractItemsContainer == null) 
        { 
            Debug.LogError("Remaining Contract Item Container is null"); 
            return; 
        }

        GameObject container = ContractManager.instance.remainingContractItemsContainer;

        // Clear current records
        for (int i = 0; i < container.transform.childCount; i++)
        {
            Destroy(container.transform.GetChild(i).gameObject);
        }

        currentContractItems.ForEach(item => 
        {
            if (item.quantity == 0) return;
            GameObject newInstance = Instantiate(ContractManager.instance.remainingContractItemPrefab, ContractManager.instance.remainingContractItemsContainer.transform);
            RemainingContractData remainingContractData = newInstance.GetComponent<RemainingContractData>();
            if (remainingContractData == null)
            {
                Debug.LogError("Script was not found in the instance!");
                return;
            }
            if (PlayerManager.instance == null)
            {
                Debug.LogError("Player manager instance is null!");
                return;
            }
            if (PlayerManager.instance.GetLocalGamePlayer().GetValueOrDefault() == null)
            {
                Debug.LogError("Local player is null!");
                return;
            }
            remainingContractData.LoadData(item, PlayerManager.instance.GetLocalGamePlayer().GetValueOrDefault().playerRole);
        });

        GameObject instance = Instantiate (ContractManager.instance.remainingContractSummaryPrefab, ContractManager.instance.remainingContractItemsContainer.transform);
        RemainingContractSummary script = instance.GetComponent<RemainingContractSummary>();
        if (script == null || PlayerManager.instance == null) return;
        GamePlayer player = PlayerManager.instance.GetLocalGamePlayer().GetValueOrDefault();
        if (player == null) return;
        int total = 0;
        foreach (ContractItem item in currentContractItems) total += item.price * item.quantity;
        switch (player.playerRole)
        {
        case PlayerRole.Shop:
            script.LoadData("Total:", "<color=#FF5555>-" + total + "$");
            break;
        case PlayerRole.Factory:
            script.LoadData("Total:", "<color=#55FF55>+" + total + "$");
            break;
        default:
            Debug.Log("Player role is unassigned!");
            break;
        }
        
    }

    public void OnContractUIClose()
    {
        if (timer != null)
        {
            timer.Stop();
            timer = null;
        }
    }
}
