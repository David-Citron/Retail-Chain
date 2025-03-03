using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class ShopMessManager : MonoBehaviour
{

    [SerializeField] private List<GameObject> messPlaces = new List<GameObject>();
    [SerializeField] private GameObject messGameObject;

    private ActionTimer currenTimer;

    void Start()
    {
        StartCoroutine(StartMessTimer());
    }

    void Update() {}

    private void SpawnMess()
    {
        Random random = new Random();

        var availablePlaces = GetAvailablePlaces();
        var available = availablePlaces.Count - 1;
        if (available < 0) return;
        var selectedPlace = GetAvailablePlaces()[random.Next(available)];

        GameObject mess = Instantiate(messGameObject);
        mess.transform.parent = selectedPlace.transform;
        mess.transform.localPosition = Vector3.zero;
        mess.AddComponent<ShopMess>();
    }


    private IEnumerator StartMessTimer()
    {
        yield return new WaitForSecondsRealtime(2f);
        Random random = new Random();
        currenTimer = new ActionTimer(() =>
        {
            if (gameObject == null) return;
            SpawnMess();
            StartCoroutine(StartMessTimer());
        }, random.Next(10, 15)).Run();
    }

    private void OnDestroy()
    {
        currenTimer.Stop();
    }

    private List<GameObject> GetAvailablePlaces() => messPlaces.FindAll(place => place.transform.childCount == 0);
}
