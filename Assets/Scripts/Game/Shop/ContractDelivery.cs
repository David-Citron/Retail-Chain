using System.Collections.Generic;
using UnityEngine;

public class ContractDelivery : MonoBehaviour
{
    public static ContractDelivery instance;

    [SerializeField] private GameObject vehicle;
    [SerializeField] private GameObject garageDoor;

    private float elapsedTime;
    private bool isMoving;
    private bool isActive;

    void Start()
    {
        instance = this;
    }

    void Update() {
        if (!isMoving) return;
        PlayTruckAnimation(isActive);
    }

    public void DeliverItems(List<ContractItem> items)
    {
        isActive = true;
        isMoving = true;

        Hint.Create("UNLOADING GOODS..", 2);
        foreach (var item in items)
        {
            if(item.quantity == 0) continue;
            StorageRack.instance.InsertItem(item.itemType, item.quantity);
        }

        new ActionTimer(() =>
        {
            isActive = false;
            isMoving = true;
        }, 8).Run();
    }

    private void PlayTruckAnimation(bool inAnimation)
    {
        if (vehicle == null || elapsedTime >= 5f)
        {
            isMoving = false;
            garageDoor.gameObject.SetActive(!inAnimation);
            if (!inAnimation) vehicle.SetActive(false);
            return;
        }

        if (inAnimation && !vehicle.activeSelf) vehicle.SetActive(true);
        if (!inAnimation && !garageDoor.activeSelf) garageDoor.SetActive(true);

        elapsedTime += Time.fixedDeltaTime;
        float t = elapsedTime / 50f;

        float newX = Mathf.Lerp(vehicle.transform.localPosition.x, inAnimation ? 5.85f : 7.75f, t);
        vehicle.transform.localPosition = new Vector3(newX, vehicle.transform.localPosition.y, vehicle.transform.localPosition.z);
    }
}
