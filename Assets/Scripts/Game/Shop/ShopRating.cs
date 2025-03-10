using TMPro;
using UnityEngine;

public class ShopRating : MonoBehaviour
{

    public static ShopRating instance;

    [SerializeField] private TMP_Text text;
    public float rating;

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

    public static float GetRating() => instance.rating;

    private void VerifyRating()
    {
        rating = Mathf.Clamp(rating, 1, 5);
        text.text = rating.ToString();
    }
}
