using UnityEngine;
using UnityEngine.UI;

public class ButtonOnCLick : MonoBehaviour
{
    public Button button;
    public Sprite defaultSprite;
    public Sprite clickedSprite;
    private Image buttonImage;
    private RectTransform buttonRectTransform;
    private Vector2 originalPosition;

    void Start()
    {
        buttonImage = button.GetComponent<Image>();
        buttonRectTransform = button.GetComponent<RectTransform>();
        buttonImage.sprite = defaultSprite;
        button.onClick.AddListener(ChangeSpriteOnClick);
        originalPosition = buttonRectTransform.anchoredPosition;
    }

    void ChangeSpriteOnClick()
    {
        buttonRectTransform.anchoredPosition = originalPosition + new Vector2(0, -12);
        if (buttonImage.sprite == defaultSprite)
        {
            buttonImage.sprite = clickedSprite;
        }
        else
        {
            buttonImage.sprite = defaultSprite;
        }
    }
}