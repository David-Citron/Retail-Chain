using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbiesListManager : MonoBehaviour
{

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
        if (!LayoutManager.instance.lobbiesMenu.activeSelf) yield break;

        SteamLobby.instance.GetLobbiesList();
        yield return new WaitForSecondsRealtime(3);
        StartCoroutine(UpdateLobbyList());
    }

    public void DisplayLobbies(List<CSteamID> lobbyIds, LobbyDataUpdate_t update)
    {

        if(noLobbiesToJoin.activeSelf) noLobbiesToJoin.SetActive(false);
        for (int i = 0; i < lobbyIds.Count; i++)
        {
            if (lobbyIds[i].m_SteamID != update.m_ulSteamIDLobby) continue;
            var lobbyId = new CSteamID(lobbyIds[i].m_SteamID);

            Debug.Log(lobbyId);

            GameObject createdItem = Instantiate(lobbyDataItemPrefab);

            var component = createdItem.GetComponent<LobbyDataEntry>();

            component.lobbyId = lobbyId;
            component.hostId = PlayerSteamUtils.StringToCSteamID(SteamMatchmaking.GetLobbyData(lobbyId, SteamLobby.HostCSteamIDKey));
            component.hostUsername = SteamMatchmaking.GetLobbyData(lobbyId, "host-name");
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
