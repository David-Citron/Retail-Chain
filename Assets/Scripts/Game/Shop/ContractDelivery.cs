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
        isMoving = true;
        isActive = true;

        Hint.Create("UNLOADING GOODS..", 5);
        int lastUnloaded = 0;
        new ActionTimer(() =>
        {
            if (lastUnloaded > items.Count) return;
            ContractItem item = items[lastUnloaded];
            StorageRack.instance.InsertItem(item.itemType, item.quantity);

            isMoving = false;
            isActive = false;
        }, () => Hint.Create("GOODS ARE IN STORAGE", 2), items.Count, 1);
    }

    private void PlayTruckAnimation(bool inAnimation)
    {
        if (vehicle == null || elapsedTime >= 3.5f)
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

        float newZ = Mathf.Lerp(vehicle.transform.localPosition.z, inAnimation ? -5.8f : -8, t);
        vehicle.transform.localPosition = new Vector3(vehicle.transform.localPosition.x, vehicle.transform.localPosition.y, newZ);
    }
}
