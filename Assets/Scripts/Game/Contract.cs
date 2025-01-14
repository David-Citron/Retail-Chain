using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Contract : NetworkBehaviour
{
    public static Contract instance;

    private const int CONTRACT_TIME = 10;
    private const int NEGOTIATION_TIME = 120;

    [SyncVar]
    private List<ContractItem> currentContractItems;
    [SerializeField] private List<ContractItem> firstContractItems = new List<ContractItem>(){ new ContractItem(ItemType.Book, 1, 50), new ContractItem(ItemType.GlueCanister, 2, 60), new ContractItem(ItemType.Paper, 2, 100) };

    private ActionTimer actionTimer;

    [SerializeField] private GameObject negotiationPanel;

    private List<ContractItem> lastOfferContractItems;
    private List<bool> fulfilled;
    private bool negotiated = false;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        if (negotiationPanel != null) negotiationPanel.SetActive(false);
        currentContractItems = new List<ContractItem>();
        negotiated = false;
        // Server side only
        if (!isServer) return;
        StartNewContractCycle(firstContractItems, CONTRACT_TIME);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Sets new contract data which will be used for the next time period.
    /// </summary>
    /// <param name="newContractItems">List of ContractItems for the new contract</param>
    /// <param name="newContractTime">Set time for the new contract (in seconds)</param>
    private void StartNewContractCycle(List<ContractItem> newContractItems, int newContractTime)
    {
        if (!isServer) {
            Debug.LogError("Trying to start contract cycle on client side");
            return;
        }
        actionTimer = new ActionTimer(() =>
        {
            CheckContract();
            actionTimer = null;
        }, newContractTime, 1).Run();
    }

    private void CheckContract()
    {
        if (!isServer)
        {
            Debug.LogError("Checking contract's status called on non-server");
            return;
        }

        PlayerManager.instance.gamePlayers.ForEach(player =>
        {
            TargetContractFulfilledRequest(player.connectionToClient);
        });

        new ActionTimer(() => fulfilled.Count < 2 || fulfilled.Count == 2 && (!fulfilled[0] || !fulfilled[1]), null, () => { Debug.Log("Contract failed"); ContractFailed(); }, () => { Debug.Log("Contract successfuly completed"); StartNegotiation(); }, 15, .1f);
    }

    private void StartNegotiation()
    {
        if (!isServer)
        {
            Debug.LogError("Trying to initialize negotiation panel on client side");
            return;
        }
        fulfilled = new List<bool>();
        RpcInitializeNegotiation();
        new ActionTimer(() => !negotiated, null /* ADD TIMER IN UI */, () => StartNewContractCycle(lastOfferContractItems, CONTRACT_TIME), () => ContractNotNegotiated(), NEGOTIATION_TIME, 1).Run();
    }

    [ClientRpc]
    private void RpcInitializeNegotiation()
    {
        if (negotiationPanel != null) negotiationPanel.SetActive(true);
    }

    [TargetRpc]
    private void TargetContractFulfilledRequest(NetworkConnectionToClient target)
    {
        CmdContractFulfilledAnswer(IsLocalContractFinished());
    }

    [Command]
    private void CmdContractFulfilledAnswer(bool result)
    {
        fulfilled.Add(result);
    }

    [Command]
    private void CmdLoadOffer(List<ContractItem> offeredContractItems)
    {
        if (!isServer)
        {
            Debug.LogError("Trying to load offer on client side");
            return;
        }
        Optional<NetworkConnectionToClient> targetNetworkConnection = Optional<NetworkConnectionToClient>.Empty();

        PlayerManager.instance.gamePlayers.ForEach(player =>
        {
            if (player.playerRole == PlayerRole.Factory) Optional<NetworkConnectionToClient>.Of(player.connectionToClient);
        });

        targetNetworkConnection.IfPresentOrElse(factoryConnection =>
        {
            lastOfferContractItems = offeredContractItems;
            TargetShowOffer(factoryConnection, lastOfferContractItems);
        }, () =>
        {
            Debug.LogError("Factory player was not found");
            return;
        });
    }

    [TargetRpc]
    private void TargetShowOffer(NetworkConnectionToClient target, List<ContractItem> offeredContractItems)
    {
        // TODO: show offer in local player's UI
    }

    [Command]
    private void CmdOfferConfirmation(bool accepted)
    {
        StartNewContractCycle(lastOfferContractItems, CONTRACT_TIME);
    }

    public void SendOffer()
    {
        if (!isClient)
        {
            Debug.Log("Tryting to send an offer on non-client side");
            return;
        }
        // TODO: load client data from UI and invoke CmdLoadOffer on server
    }
   
    private bool IsLocalContractFinished()
    {
        bool contractNotFinished = false;
        PlayerManager.instance.GetLocalGamePlayer().IfPresentOrElse(localPlayer => {
            if (localPlayer.playerRole == PlayerRole.Factory)
            {
                for (int i = 0; i < currentContractItems.Count; i++)
                {
                    if (currentContractItems[i].fulfilled) continue;
                    contractNotFinished = true;
                }
            }else if (localPlayer.playerRole == PlayerRole.Shop)
            {
                int contractSum = 0;
                currentContractItems.ForEach(contractItem =>
                {
                    contractSum += contractItem.price;
                });
                if (localPlayer.bankAccount.GetBalance() < contractSum)
                {
                    contractNotFinished = true;
                }
            }
        }, () => Debug.LogError("Local player was not found"));
        Debug.Log("Local contract check, finished: " + !contractNotFinished);
        return !contractNotFinished;
    }

    /// <summary>
    /// Is called when the previous contract wasn't fulfilled. Acts accordingly.
    /// </summary>
    private void ContractFailed()
    {
        // TODO: Game ends
        Debug.LogWarning("GAME ENDS!!!!");
    }

    private void ContractNotNegotiated()
    {
        // TODO: Game ends
        Debug.LogWarning("GAME ENDS!!!!");
    }
}
