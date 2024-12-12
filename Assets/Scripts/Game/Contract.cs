using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Contract : NetworkBehaviour
{
    public static Contract instance;

    private List<ContractItem> currentContractItems = new List<ContractItem>();
    private int timeRemaining; // In seconds
    public int timePassed { private set; get; }

    [SerializeField] private GameObject negotiationPanel;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        if (negotiationPanel != null) negotiationPanel.SetActive(false);
        currentContractItems = new List<ContractItem>();
        ResetTimer();
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
    void StartNewContractCycle(List<ContractItem> newContractItems, int newContractTime)
    {
        currentContractItems = newContractItems;
        timeRemaining = newContractTime;
        StartCoroutine(ContractCycle());
    }

    /// <summary>
    /// When running, timer is decremented each second until it counts down to zero.
    /// Once it hits zero, checks if the contract was finished successfuly and handles next actions accordingly.
    /// </summary>
    private IEnumerator ContractCycle()
    {
        while (timeRemaining > 0)
        {
            yield return new WaitForSecondsRealtime(1);
            TimeTick();
        }
        // Contract time expired:
        if (IsContractFinished()) StartNegotiation();
        else ContractFailed();
    }

    private void TimeTick()
    {
        timeRemaining--;
        timePassed++;
    }

    private void ResetTimer()
    {
        timeRemaining = 0;
        timePassed = 0;
    }

    /// <summary>
    /// Shows the negotiation panel.
    /// </summary>
    private void StartNegotiation()
    {
        if (negotiationPanel != null) negotiationPanel.SetActive(true);
    }

    /// <summary>
    /// Checks if contract is fulfilled.
    /// </summary>
    /// <returns>Current status of contract</returns>
    private bool IsContractFinished()
    {
        bool contractNotFinished = false;
        for (int i = 0; i < currentContractItems.Count; i++)
        {
            if (currentContractItems[i].fulfilled) continue;
            contractNotFinished = true;
        }
        return !contractNotFinished;
    }

    /// <summary>
    /// Is called when the previous contract wasn't fulfilled. Acts accordingly.
    /// </summary>
    private void ContractFailed()
    {
        // TODO: Game ends
    }
}
