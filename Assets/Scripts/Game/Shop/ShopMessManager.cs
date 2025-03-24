using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class ShopMessManager : MonoBehaviour
{

    public static ShopMessManager instance;

    [SerializeField] private List<GameObject> messes;

    void Start()
    {
        instance = this;
        StartCoroutine(StartMessTimer());
    }

    void Update() {}

    private void SpawnMess()
    {
        Random random = new Random();

        var availableMesses = GetAvailablePlaces();
        var count = availableMesses.Count - 1;
        if (count < 0) return;
        GetAvailablePlaces()[random.Next(count)].GetComponent<ShopMess>().ShowMess();
    }


    private IEnumerator StartMessTimer()
    {
        yield return new WaitForSecondsRealtime(2f);
        Random random = new Random();
        new ActionTimer(() =>
        {
            if (gameObject == null) return;
            SpawnMess();
            StartCoroutine(StartMessTimer());
        }, random.Next(25, 80)).Run();
    }

    private List<GameObject> GetAvailablePlaces() => messes.FindAll(place => !place.activeSelf);
}
