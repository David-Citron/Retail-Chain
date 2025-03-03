using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Net;
using TMPro;
using UnityEngine.UI;

public class ContractManager : NetworkBehaviour
{
    public static ContractManager instance;

    [SerializeField] private List<Contract> contracts;
    public Contract localContract = null;
    [SerializeField] private Contract serverContract = null;

    private const int CONTRACT_TIME = 30;
    private const int NEGOTIATION_TIME = 120;

    [SerializeField] private List<ContractItem> initialContractItems = new List<ContractItem>() 
    {
        new ContractItem(ItemType.Book, 1, 50),
    };

    [SerializeField] private GameObject waitingTab;
    [SerializeField] private GameObject negotiationTab;
    [SerializeField] private List<GameObject> factoryButtons;
    [SerializeField] private List<GameObject> shopButtons;

    private List<ContractItem> lastOfferContractItems;
    private ActionTimer negotiationTimer = null;
    private ActionTimer localTimer = null;

    [SerializeField] private OfferState negotiationState;

    private List<ContractItemData> currentItemData;
    [SerializeField] private GameObject itemDataParent;
    [SerializeField] private GameObject itemDataPrefab;

    public GameObject remainingContractItemsContainer;
    public GameObject remainingContractItemPrefab;
    public TMP_Text remainingContractItemsTimer;

    [SerializeField] private Image waitingTimeImage;
    [SerializeField] private TMP_Text waitingTimeText;

    // Start is called before the first frame update
    void Awake()
    {
        //
        // Server + client:
        //

        instance = this;
        syncDirection = SyncDirection.ServerToClient;
        negotiationState = OfferState.None;
        contracts = new List<Contract>();
        currentItemData = new List<ContractItemData>();
        PlayerManager.instance.gamePlayers.ForEach(player =>
        {
            Contract contract = player.GetComponent<Contract>();
            contracts.Add(contract);
            if (!player.isLocalPlayer) return;
            localContract = contract;
            if (player.isServer) serverContract = contract;
        });
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
        negotiationState = OfferState.None;
        if (localContract == null)
        {
            Debug.LogError("Local contract is null"); 
            return;
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
            Debug.Log("Contract is not finished yet");
            return;
        }
        if (contractSuccess)
        {
            Debug.Log("Contract was finished successfully!");
            RpcFinishContract();
            StartNegotiation();
            return;
        }
        Debug.LogWarning("Contract was NOT finished successfully!");
        RpcEndGame();
    }

    [ClientRpc]
    private void RpcFinishContract()
    {
        localContract.FinalizeContract();
    }

    [Server]
    private void StartNegotiation()
    {
        RpcShowNegotiationPanel();
        negotiationTimer = new ActionTimer(() => RpcEndGame(), NEGOTIATION_TIME).Run();
    }

    [ClientRpc]
    private void RpcShowNegotiationPanel()
    {
        ReloadNegotiationTime(0f);
        localTimer = new ActionTimer(timer => 
        {
            ReloadNegotiationTime(timer.passedTime);
        }, null, null, NEGOTIATION_TIME, 1).Run();
        if (!GameLayoutManager.instance.IsEnabled(LayoutType.Contract))
            GameLayoutManager.instance.ToggleUI(LayoutType.Contract);
        negotiationState = OfferState.MakeOffer;
        ChangeNegotiationTab(negotiationState);
    }

    [ClientRpc]
    private void RpcHideNegotiationPanel()
    {
        if (GameLayoutManager.instance.IsEnabled(LayoutType.Contract))
            GameLayoutManager.instance.ToggleUI(LayoutType.Contract);
        if (localTimer != null)
        {
            localTimer.Stop();
            localTimer = null;
        }
    }

    [ClientRpc]
    private void RpcNextTab()
    {
        NextNegotiationTab();
    }

    private void NextNegotiationTab()
    {
        negotiationState = (negotiationState == OfferState.MakeOffer ? OfferState.ShowOffer : OfferState.MakeOffer);
        ChangeNegotiationTab(negotiationState);
    }

    // Changes the tabs depending on the contract state
    private void ChangeNegotiationTab(OfferState state)
    {
        GamePlayer localPlayer = PlayerManager.instance.GetLocalGamePlayer().GetValueOrDefault();
        if (localPlayer == null)
        {
            Debug.LogError("Local player is not set");
            return;
        }
        else if (localPlayer.playerRole == PlayerRole.Unassigned)
        {
            Debug.LogError("Local player's role is set to Unassigned!");
            return;
        }
        switch (state)
        {
            case OfferState.MakeOffer:
                if (localPlayer.playerRole == PlayerRole.Factory)
                {
                    negotiationTab.SetActive(false);
                    waitingTab.SetActive(true);
                }
                else if (localPlayer.playerRole == PlayerRole.Shop)
                {
                    InitializeEmptyNegotiation();
                    negotiationTab.SetActive(true);
                    waitingTab.SetActive(false);
                }
            return;
            case OfferState.ShowOffer:
                if (localPlayer.playerRole == PlayerRole.Shop)
                {
                    negotiationTab.SetActive(false);
                    waitingTab.SetActive(true);
                }
                else if (localPlayer.playerRole == PlayerRole.Factory)
                {
                    negotiationTab.SetActive(true);
                    InitializeFilledNegotiation();
                    waitingTab.SetActive(false);
                }
            return;
            default:
                Debug.LogError("Offer state not set!");
            return;
        }
    }

    private void InitializeEmptyNegotiation()
    {
        factoryButtons.ForEach(button => button.SetActive(false));
        shopButtons.ForEach(button => button.SetActive(true));
        List<ItemData> itemData = ItemManager.GetAllSellableItemData();
        currentItemData.ForEach(i => Destroy(i.gameObject));
        currentItemData.Clear();
        itemData.ForEach(i => {
            GameObject prefabInstance = Instantiate(itemDataPrefab, itemDataParent.transform);
            ContractItemData script = prefabInstance.GetComponent<ContractItemData>();
            if (script == null)
            {
                Debug.LogError("Script is set to null!");
                return;
            }
            currentItemData.Add(script);
            script.LoadData(i.itemType);
        });
    }

    private void InitializeFilledNegotiation()
    {
        factoryButtons.ForEach(button => button.SetActive(true));
        shopButtons.ForEach(button => button.SetActive(false));
        currentItemData.ForEach(i => Destroy(i.gameObject));
        currentItemData.Clear();
        lastOfferContractItems.ForEach(i => {
            GameObject prefabInstance = Instantiate(itemDataPrefab, itemDataParent.transform);
            ContractItemData script = prefabInstance.GetComponent<ContractItemData>();
            currentItemData.Add(script);
            script.LoadData(i);
        });
    }

    [Command(requiresAuthority = false)]
    public void CmdSendNegotiationOffer(List<ContractItem> contractItems)
    {
        RpcShowNegotiationOffer(contractItems);
    }

    [ClientRpc]
    public void RpcShowNegotiationOffer(List<ContractItem> contractItems)
    {
        lastOfferContractItems = contractItems;
        NextNegotiationTab();
    }

    [Command(requiresAuthority = false)]
    public void AcceptContract()
    {
        EndNegotiation();
        RpcStartNewContract(lastOfferContractItems, CONTRACT_TIME);
    }

    [Command(requiresAuthority = false)]
    public void DeclineContract()
    {
        RpcNextTab();
    }

    [Server]
    private void EndNegotiation()
    {
        negotiationTimer.Stop();
        RpcHideNegotiationPanel();
    }

    public void SendCurrentOffer()
    {
        List<ContractItem> contractItems = new List<ContractItem>();
        currentItemData.ForEach(itemData => {
            contractItems.Add(itemData.ReadData());
        });
        CmdSendNegotiationOffer(contractItems);
    }

    [ClientRpc]
    private void RpcEndGame()
    {
        Debug.Log("RpcEndGame called!");
        if (negotiationTimer != null)
            negotiationTimer.Stop();
        if (Game.instance == null) return;
        Game.instance.EndGame();
    }

    private void ReloadNegotiationTime(float passedTime)
    {
        if (waitingTimeImage != null)
            waitingTimeImage.fillAmount = 1 - (float)(passedTime / NEGOTIATION_TIME);
        if (waitingTimeText != null)
        {
            int remainingDuration = (NEGOTIATION_TIME - (int)passedTime);
            waitingTimeText.text = $"{(remainingDuration / 60):00}:{(remainingDuration % 60):00}";
        }
    }

    private void OnDestroy()
    {
        if (negotiationTimer != null) negotiationTimer.Stop();
        if (localTimer != null) localTimer.Stop();
    }
}

enum OfferState
{
    None,
    MakeOffer,
    ShowOffer
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
