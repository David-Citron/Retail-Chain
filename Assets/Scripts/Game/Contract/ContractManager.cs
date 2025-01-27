using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Threading;
using System;

public class ContractManager : NetworkBehaviour
{
    public static ContractManager instance;

    [SerializeField] private List<Contract> contracts;
    [SerializeField] private Contract localContract = null;
    [SerializeField] private Contract serverContract = null;

    private const int CONTRACT_TIME = 10;
    private const int NEGOTIATION_TIME = 120;

    [SerializeField] private List<ContractItem> initialContractItems = new List<ContractItem>() 
    {
        new ContractItem(ItemType.Book, 1, 50), 
        new ContractItem(ItemType.GlueCanister, 2, 60), 
        new ContractItem(ItemType.Paper, 2, 100) 
    };

    [SerializeField] private GameObject negotiationPanel;

    private List<ContractItem> lastOfferContractItems;
    private ActionTimer negotiationTimer = null;

    // Start is called before the first frame update
    void Awake()
    {
        //
        // Server + client:
        //

        instance = this;
        syncDirection = SyncDirection.ServerToClient;
        contracts = new List<Contract>();
        PlayerManager.instance.gamePlayers.ForEach(player =>
        {
            Contract contract = player.GetComponent<Contract>();
            contracts.Add(contract);
            if (!player.isLocalPlayer) return;
            localContract = contract;
            if (player.isServer) serverContract = contract;
        });
        if (negotiationPanel != null) negotiationPanel.SetActive(false);
    }

    [Server]
    public void InitializeFirstContract()
    {
        Debug.Log("Calling StartNewContract");
        RpcStartNewContract(initialContractItems, CONTRACT_TIME); // Start default contract at the beginning of the game
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    [ClientRpc]
    private void RpcStartNewContract(List<ContractItem> contractItems, int time)
    {
        if (localContract == null)
        {
            Debug.LogError("Local contract is null"); return;
        }
        localContract.StartNewContract(contractItems, time);
    }

    [Command (requiresAuthority = false)]
    public void CmdCheckContracts()
    {
        bool contractNotFinished = false;
        bool contractSuccess = true;
        contracts.ForEach(contract =>
        {
            if (contract.status == ContractStatus.Unknown || contract.status == ContractStatus.Pending) contractNotFinished = true;
            if (contract.status != ContractStatus.Success) contractSuccess = false;
        });
        if (contractNotFinished)
        {
            Debug.LogError("Contract is not finished yet");
            return;
        }
        if (contractSuccess)
        {
            Debug.Log("Contract was finished successfully!");
            StartNegotiation();
            return;
        }
        Debug.LogError("Game over!");
        // TODO
    }

    [Server]
    private void StartNegotiation()
    {
        RpcShowNegotiationPanel();
        negotiationTimer = new ActionTimer(() =>
        {
            // TODO: game ends
            Debug.Log("Game ends! Not negotiated!");
        }, NEGOTIATION_TIME).Run();
    }

    [ClientRpc]
    private void RpcShowNegotiationPanel()
    {
        if (negotiationPanel == null)
        {
            Debug.LogWarning("Negotiation panel is null");
            return;
        }
        negotiationPanel.SetActive(true);
    }

    [ClientRpc]
    private void RpcHideNegotiationPanel()
    {
        if (negotiationPanel == null)
        {
            Debug.LogWarning("Negotiation panel is null");
            return;
        }
        negotiationPanel.SetActive(false);
    }

    [Command(requiresAuthority = false)]
    public void CmdSendNegotiationOffer(List<ContractItem> contractItems)
    {
        // TODO
        RpcShowNegotiationOffer(contractItems);
    }

    [ClientRpc]
    public void RpcShowNegotiationOffer(List<ContractItem> contractItems)
    {
        // TODO
        lastOfferContractItems = contractItems;
    }

    [Command(requiresAuthority = false)]
    public void AcceptContract()
    {
        EndNegotiation();
        RpcStartNewContract(lastOfferContractItems, CONTRACT_TIME);
    }

    [Server]
    private void EndNegotiation()
    {
        negotiationTimer.Stop();
        RpcHideNegotiationPanel();
    }
}

public static class ContractItemReaderWriter
{
    public static void WriteContractItem(this NetworkWriter writer, ContractItem contractItem)
    {
        writer.WriteByte((byte)contractItem.itemType);
        writer.WriteInt(contractItem.quantity);
        writer.WriteInt(contractItem.price);
    }

    public static ContractItem ReadContractItem(this NetworkReader reader)
    {
        return new ContractItem((ItemType)reader.ReadByte(), reader.ReadInt(), reader.ReadInt());
    }
}
