using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class ContractManager : NetworkBehaviour
{
    public static ContractManager instance;

    [SerializeField] private List<Contract> contracts;
    public Contract localContract = null;

    private int contractsPassed = 0;

    private const int CONTRACT_TIME = 300; //TODO Edit
    private const int CONTRACT_FIRST_TIME = 180; // default: 180
    private const int NEGOTIATION_TIME = 120; // default: 120

    [SerializeField] private List<ContractItem> initialContractItems = new List<ContractItem>() 
    {
        new ContractItem(ItemType.Book, 1, 50),
    };

    [SerializeField] private GameObject waitingTab;
    [SerializeField] private GameObject negotiationTab;
    [SerializeField] private GameObject infoRequiredTab;
    [SerializeField] private GameObject infoNegotiationTab;
    [SerializeField] private TMP_Text infoNegotiationLabel;
    [SerializeField] private List<GameObject> factoryButtons;
    [SerializeField] private List<GameObject> shopButtons;
    [SerializeField] private TMP_Text moneyText;

    private List<ContractItem> lastOfferContractItems;
    private ActionTimer negotiationTimer = null;
    private ActionTimer localTimer = null;

    [SerializeField] private OfferState negotiationState;

    private List<ContractItemData> currentItemData;
    [SerializeField] private GameObject itemDataParent;
    [SerializeField] private GameObject itemDataPrefab;

    public GameObject remainingContractItemsContainer;
    public GameObject remainingContractItemPrefab;
    public GameObject remainingContractSummaryPrefab;
    public TMP_Text remainingContractItemsTimer;

    [SerializeField] private Image waitingTimeImage;
    [SerializeField] private TMP_Text waitingTimeText;

    private bool negotiating = false;

    // Start is called before the first frame update
    void Awake()
    {
        //
        // Server + client:
        //

        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        syncDirection = SyncDirection.ServerToClient;
        negotiationState = OfferState.None;
        negotiating = false;
        contracts = new List<Contract>();
        currentItemData = new List<ContractItemData>();
        if (PlayerManager.instance == null)
        {
            Destroy(gameObject);
            return;
        }
        PlayerManager.instance.gamePlayers.ForEach(player =>
        {
            Contract contract = player.GetComponent<Contract>();
            contracts.Add(contract);
            if (!player.isLocalPlayer) return;
            localContract = contract;
        });
    }

    [Server]
    public void InitializeFirstContract()
    {
        RpcStartNewContract(initialContractItems, CONTRACT_FIRST_TIME); // Start default contract at the beginning of the game
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
        infoNegotiationTab.SetActive(false);
        infoRequiredTab.SetActive(true);
        localContract.StartNewContract(contractItems, time);
    }

    [Command (requiresAuthority = false)]
    public void CmdCheckContracts()
    {
        if (negotiating) return;
        bool contractNotFinished = false;
        bool contractSuccess = true;
        contracts.ForEach(contract =>
        {
            if (contract.status == ContractStatus.Unknown || contract.status == ContractStatus.Pending) contractNotFinished = true;
            if (contract.status != ContractStatus.Success) contractSuccess = false;
        });
        if (contractNotFinished) return;
        if (contractSuccess)
        {
            negotiating = true;
            StartNegotiation();
            return;
        }
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
        if (negotiationTimer != null) return;
        negotiationTimer = new ActionTimer(() => RpcEndGame(), NEGOTIATION_TIME).Run();
    }

    [ClientRpc]
    private void RpcShowNegotiationPanel()
    {
        MenuManager.instance.Open("Contract");
        if (PlayerManager.instance != null && moneyText != null)
            moneyText.text = "$" + PlayerManager.instance.GetLocalGamePlayer().GetValueOrDefault().bankAccount.GetBalance();

        negotiationState = OfferState.MakeOffer;
        ChangeNegotiationTab(negotiationState);

        if (localTimer != null) return;

        ReloadNegotiationTime(0f);
        localTimer = new ActionTimer(timer =>
        {
            ReloadNegotiationTime(timer.passedTime);
        }, null, null, NEGOTIATION_TIME, 1).Run();
    }

    [ClientRpc]
    private void RpcHideNegotiationPanel()
    {
        MenuManager.instance.Close("Contract");
        if (localTimer != null)
        {
            localTimer.Stop();
            localTimer = null;
        }
        localContract.OnContractUIClose();
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
        negotiationTab.SetActive(true);
        waitingTab.SetActive(false);

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
                    infoNegotiationTab.SetActive(true);
                    infoRequiredTab.SetActive(false);
                    MenuManager.instance.Close("Contract");
                }
                else if (localPlayer.playerRole == PlayerRole.Shop)
                {
                    infoNegotiationTab.SetActive(true);
                    infoRequiredTab.SetActive(false);
                    MenuManager.instance.Open("Contract");
                    InitializeEmptyNegotiation();
                }
            return;
            case OfferState.ShowOffer:
                if (localPlayer.playerRole == PlayerRole.Shop)
                {
                    infoNegotiationTab.SetActive(true);
                    infoRequiredTab.SetActive(false);
                    MenuManager.instance.Close("Contract");
                }
                else if (localPlayer.playerRole == PlayerRole.Factory)
                {
                    infoNegotiationTab.SetActive(true);
                    infoRequiredTab.SetActive(false);
                    MenuManager.instance.Open("Contract");
                    InitializeFilledNegotiation();
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
        RpcFinishContract();
        contractsPassed++;
        int contractTime = CONTRACT_TIME;
        if (contractsPassed < 3) contractTime = CONTRACT_FIRST_TIME;
        RpcStartNewContract(lastOfferContractItems, contractTime);
    }

    [Command(requiresAuthority = false)]
    public void DeclineContract()
    {
        RpcNextTab();
    }

    [Server]
    private void EndNegotiation()
    {
        negotiating = false;
        negotiationTimer.Stop();
        negotiationTimer = null;
        RpcHideNegotiationPanel();
    }

    public void SendCurrentOffer()
    {
        List<ContractItem> contractItems = new List<ContractItem>();
        currentItemData.ForEach(itemData => {
            ContractItem data = itemData.ReadData();
            if (data.quantity <= 0) return;
            contractItems.Add(data);
        });
        if (contractItems.Count == 0)
        {
            Debug.LogWarning("No contract items send!");
            // TODO: add warning to player?
            return;
        }
        CmdSendNegotiationOffer(contractItems);
    }

    [ClientRpc]
    private void RpcEndGame()
    {
        Debug.Log("RpcEndGame called!");
        if (negotiationTimer != null)
        {
            negotiationTimer.Stop();
            negotiationTimer = null;
        }
        if (Game.instance == null) return;
        Game.instance.EndGame();
    }

    private void ReloadNegotiationTime(float passedTime)
    {
        if (infoNegotiationLabel != null)
        {
            int remainingDuration = (NEGOTIATION_TIME - (int)passedTime);
            infoNegotiationLabel.text = $"{(remainingDuration / 60):00}:{(remainingDuration % 60):00}";
        }
        if (waitingTimeImage != null)
            waitingTimeImage.fillAmount = 1 - (float)(passedTime / NEGOTIATION_TIME);
        if (waitingTimeText != null)
        {
            int remainingDuration = (NEGOTIATION_TIME - (int)passedTime);
            waitingTimeText.text = $"{(remainingDuration / 60):00}:{(remainingDuration % 60):00}";
        }
    }

    public int GetContractedPassed() => contractsPassed;
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
