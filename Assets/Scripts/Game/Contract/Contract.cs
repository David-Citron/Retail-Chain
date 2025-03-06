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
        if (newValue == ContractStatus.Failed || newValue == ContractStatus.Success) 
        {
            Debug.Log("Hook caught! Sending command to check contracts");
            ContractManager.instance.CmdCheckContracts();
        } else
        {
            Debug.LogError("Hook caught but the ContractStatus is invalid!");
        }
    }

    private void CheckContractStatus()
    {
        switch (ownerRole)
        {
            case PlayerRole.Factory:
                bool allItemsFulfilled = true;
                currentContractItems.ForEach(item => {
                    if (!item.fulfilled)
                    {
                        allItemsFulfilled = false;
                        Debug.Log("Item not fulfilled! Item type: " + item.itemType + " - Remaining: " + item.quantityRemaining);
                    }
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
        Debug.Log("Local contract checked successfully! Sending command to check contracts");
        Debug.Log("Local contract status: " + status);
        ContractManager.instance.CmdCheckContracts();
    }

    public void FinalizeContract()
    {
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
        currentContractItems.ForEach(item => sum += item.price);
        switch (ownerRole)
        {
            case PlayerRole.Factory:
                localPlayer.bankAccount.AddBalance(sum);
                break;
            case PlayerRole.Shop:
                localPlayer.bankAccount.RemoveBalance(sum);
                ContractDelivery.instance.DeliverItems(currentContractItems);
                break;
            default:
                Debug.LogError("Player role is not assigned correctly!");
                return;
        }
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
        GameObject container = ContractManager.instance.remainingContractItemsContainer;

        // Clear current records
        for (int i = 0; i < container.transform.childCount; i++)
        {
            Destroy(container.transform.GetChild(i).gameObject);
        }

        if (ContractManager.instance.remainingContractItemPrefab == null) return;
        if (ContractManager.instance.remainingContractItemsContainer == null) return;

        currentContractItems.ForEach(item => 
        {
            if (item.quantity == 0) return;
            GameObject instance = Instantiate(ContractManager.instance.remainingContractItemPrefab, ContractManager.instance.remainingContractItemsContainer.transform);
            RemainingContractData script = instance.GetComponent<RemainingContractData>();
            if (script == null)
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
            script.LoadData(item, PlayerManager.instance.GetLocalGamePlayer().GetValueOrDefault().playerRole);
        });
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
