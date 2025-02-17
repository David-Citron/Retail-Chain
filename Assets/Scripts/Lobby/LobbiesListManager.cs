using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbiesListManager : MonoBehaviour {

    public static LobbiesListManager instance;

    public List<GameObject> listOfLobbies = new List<GameObject>();

    public GameObject lobbyDataItemPrefab;
    public GameObject lobbyListContent;
    public GameObject noLobbiesToJoin;

    private void Awake()
    {
        if(instance == null) instance = this;
    }

    public IEnumerator UpdateLobbyList()
    {
        if (!LayoutManager.Instance().GetValueOrDefault().lobbiesMenu.activeSelf) yield break; //If Lobbies List menu is disabled, then list should not be updated

        SteamLobby.instance.GetLobbiesList();
        yield return new WaitForSecondsRealtime(1.5f);
        StartCoroutine(UpdateLobbyList());
    }

    public void DisplayLobbies(List<CSteamID> lobbyIds, LobbyDataUpdate_t update)
    {

        if(noLobbiesToJoin.activeSelf) noLobbiesToJoin.SetActive(false);
        for (int i = 0; i < lobbyIds.Count; i++)
        {
            if (lobbyIds[i].m_SteamID != update.m_ulSteamIDLobby) continue;
            var lobbyId = new CSteamID(lobbyIds[i].m_SteamID);

            GameObject createdItem = Instantiate(lobbyDataItemPrefab);

            var component = createdItem.GetComponent<LobbyDataEntry>();

            component.lobbyId = lobbyId;
            component.hostId = PlayerSteamUtils.StringToCSteamID(SteamMatchmaking.GetLobbyData(lobbyId, SteamLobby.HostCSteamIDKey));
            component.UpdateList();

            createdItem.transform.SetParent(lobbyListContent.transform);
            createdItem.transform.localScale = Vector3.one;

            listOfLobbies.Add(createdItem);
        }
    }

    public void UpdateNoLobbyInfo(bool anyLobbies)
    {
        noLobbiesToJoin.SetActive(!anyLobbies);
    }

    public void DestroyLobbies()
    {
        foreach (GameObject item in listOfLobbies)
        {
            Destroy(item);
        }
        listOfLobbies.Clear();
        noLobbiesToJoin.SetActive(true);
    }

}
