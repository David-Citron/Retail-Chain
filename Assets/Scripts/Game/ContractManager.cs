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
    private List<bool> fulfilled;
    private bool negotiated = false;

    // Start is called before the first frame update
    void Start()
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
        negotiated = false;
    }

    [Server]
    public void InitializeFirstContract()
    {
        Debug.Log("Calling StartNewContract");
        // TODO!!!!!
        // Test for ClientRPC parameters
        RpcStartNewContractTest1(new ContractItem(ItemType.GlueBarrel, 1, 200), 200);
        RpcStartNewContractTest2(new List<int> { 1, 2, 4, 6 }, 200);
        // RpcStartNewContract(initialContractItems, CONTRACT_TIME); // Start default contract at the beginning of the game
        RpcTest();
        
        //new ActionTimer(() => { RpcTest(); }, 5, 1).Run();
        //StartCoroutine(delay());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator delay()
    {
        yield return new WaitForSecondsRealtime(5);
        Debug.Log(NetworkClient.spawned);
        RpcTest();
    }

    [ClientRpc]
    private void RpcTest()
    {
        Debug.Log("THIS WORKS");
    }

    // Try:
    // 1. List<int>
    // 2. ContractItem
    //
    [ClientRpc]
    private void RpcStartNewContract(List<ContractItem> contractItems, int time)
    {
        try
        {
            Debug.Log("StartNewContract Called");
            localContract.StartNewContract(contractItems, time);
        }
        catch (Exception e)
        {
            Debug.LogError("Error: " + e.Message);
        }
    }
    
    [ClientRpc]
    private void RpcStartNewContractTest1(ContractItem contractItems, int time)
    {
        try
        {
            Debug.Log("StartNewContract Called");
            localContract.StartNewContract(new List<ContractItem>() { contractItems }, time);
        }
        catch (Exception e)
        {
            Debug.LogError("Error TEST 1: " + e.Message);
        }
    }

    [ClientRpc]
    private void RpcStartNewContractTest2(List<int> ids, int time)
    {
        try
        {
            Debug.Log("StartNewContract Called");
            ids.Add(10);
            //localContract.StartNewContract(new List<ContractItem>() { contractItems }, time);
        }
        catch (Exception e)
        {
            Debug.LogError("Error TEST 2: " + e.Message);
        }
    }

    [Command (requiresAuthority = false)]
    public void CmdCheckContracts()
    {
        if (!isServer)
        {
            Debug.LogError("Checking contract's status called on non-server");
            return;
        }
        bool contractNotFinished = false;
        bool contractSuccess = true;
        contracts.ForEach(contract =>
        {
            Debug.Log("Current ContractStatus: " + contract.status);
            if (contract.status == ContractStatus.Unknown || contract.status == ContractStatus.Pending) contractNotFinished = true;
            if (contract.status != ContractStatus.Success) contractSuccess = false;
        });
        if (contractNotFinished)
        {
            Debug.Log("Contract is not finished yet");
            return;
        }
        if (contractSuccess)
        {
            Debug.Log("Contract was finished successfully!");
            return;
        }
        Debug.Log("Contract WAS NOT finished successfully!");
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
