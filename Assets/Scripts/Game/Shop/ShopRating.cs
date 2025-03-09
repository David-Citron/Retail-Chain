using UnityEngine;

public class ShopRating : MonoBehaviour
{

    public static ShopRating instance;

    public float rating;

    /// <summary>
    /// Rating is value between 1 & 5
    /// 
    /// </summary>

    void Start() { 
        instance = this;
        rating = 3f;
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
        Hint.Create("RATING CHANGED!", Color.green, 3);
    }
}
