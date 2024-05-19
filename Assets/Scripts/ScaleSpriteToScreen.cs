using UnityEngine;

public class ScaleSpriteToScreen : MonoBehaviour
{
    private float currentScreenWidth;
    private float currentScreenHeight;

    void Start()
    {
        currentScreenWidth = Screen.width;
        currentScreenHeight = Screen.height;
        ScaleSpriteToFitScreen();
    }

    private void Update()
    {
        if (Screen.width != currentScreenWidth || Screen.height != currentScreenHeight)
        {
            ScaleSpriteToFitScreen();
        }
    }

    void ScaleSpriteToFitScreen()
    {
        SpriteRenderer spriteRend = GetComponent<SpriteRenderer>();
        if (spriteRend == null) return;

        // Get screen width and screen height
        float screenHeight = Camera.main.orthographicSize * 2;
        float screenWidth = screenHeight * Camera.main.aspect;

        // Get the sprite's size
        float spriteHeight = spriteRend.sprite.bounds.size.y;
        float spriteWidth = spriteRend.sprite.bounds.size.x;

        // Calculate Scale Factors based on current sprite size
        Vector2 scale = transform.localScale;
        scale.x = screenWidth / spriteWidth;
        scale.y = screenHeight / spriteHeight;

        // Apply the scale
        transform.localScale = scale;
    }
}
