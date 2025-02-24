using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Contract : NetworkBehaviour
{
    public List<ContractItem> currentContractItems { get; private set; }
    [SerializeField] private PlayerRole ownerRole = PlayerRole.Unassigned;
    private PlayerBank ownerBank;
    [SyncVar(hook = nameof(HookStatus))]
    public ContractStatus status = ContractStatus.Unknown;

    private void Start()
    {
        /*if ((isLocalPlayer && !isServer) || (!isLocalPlayer && isServer))*/ syncDirection = SyncDirection.ClientToServer; // !!! Maybe it can be set ClientToServer everytime
        currentContractItems = new List<ContractItem>();

        // Local player only:
        if (!isLocalPlayer) return;
        PlayerManager.instance.GetLocalGamePlayer().IfPresentOrElse(localPlayer =>
        {
            ownerRole = localPlayer.playerRole;
            ownerBank = localPlayer.bankAccount;
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
        new ActionTimer(() => 
        {
            CheckContractStatus();
        }, newContractTime, 1).Run();
    }

    public void HookStatus(ContractStatus oldValue, ContractStatus newValue)
    {
        if (newValue == ContractStatus.Failed || newValue == ContractStatus.Success) 
        {
            Debug.Log("Hook caught! Sending command to check contracts");
            ContractManager.instance.CmdCheckContracts();
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
                Debug.Log("SHOP price sum: " + sum);
                Debug.Log("Balance: " + ownerBank.GetBalance());
                if (ownerBank.GetBalance() >= sum) status = ContractStatus.Success;
                else status = ContractStatus.Failed;
                break;
            default:
                Debug.LogError("Player role is not assigned correctly");
                return;
        }
        Debug.Log("Local contract checked successfully! Sending command to check contracts");
        Debug.Log("Local contract status: " + status);
        ContractManager.instance.CmdCheckContracts();
    }

    public bool SubmitItem(ItemType itemType)
    {
        foreach (ContractItem item in currentContractItems)
        {
            if (item.itemType != itemType) continue;
            if (item.fulfilled) continue;
            item.ItemSubmitted();
            return true;
        }

        return false;
    }
}
