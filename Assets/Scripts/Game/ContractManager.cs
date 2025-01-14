using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ContractManager : NetworkBehaviour
{
    public static ContractManager instance;

    private List<Contract> contracts;
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

        //
        // Server side only:
        //

        if (!isServer) return;
        localContract.StartNewContract(initialContractItems, CONTRACT_TIME); // Start default contract at the beginning of the game
    }

    // Update is called once per frame
    void Update()
    {

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
