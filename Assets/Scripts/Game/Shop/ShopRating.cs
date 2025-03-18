using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopRating : MonoBehaviour
{

    public static ShopRating instance;

    [SerializeField] private TMP_Text text;
    public float rating;

    [SerializeField] private Image starsImage; // Stars image whose fill needs to be changed dynamically

    /// <summary>
    /// Rating is value between 1 & 5
    /// 
    /// </summary>

    void Start() { 
        instance = this;
        rating = 3f;
        VerifyRating();
    }

    void Update() {}

    public void IncreaseRating(float by)
    {
        rating += by;
        VerifyRating();
    }

    public void DecreaseRating(float by)
    {
        rating -= by;
        VerifyRating();
    }

    public static float GetRating() => instance == null ? 3 : instance.rating;

    private void VerifyRating()
    {
        rating = Mathf.Clamp(rating, 0, 5);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (text != null) text.text = rating.ToString();
        if (starsImage != null) starsImage.fillAmount = rating / 5;
    }
}
