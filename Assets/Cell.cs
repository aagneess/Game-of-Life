using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool alive;
    public int generation;

    SpriteRenderer spriteRenderer;

    public void UpdateStatus()
    {
        spriteRenderer ??= GetComponent<SpriteRenderer>();

        if (alive)
        {
            generation++;
        }
        else
        {
            generation = 0;
        }

        spriteRenderer.enabled = alive;

        UpdateColor();
    }

	private void UpdateColor()
	{
		Color startColor = HexToColor("#674188");
		Color endColor = HexToColor("#F7EFE5");   

		int maxGenerations = 5;
		float t = Mathf.Clamp01((float)generation / maxGenerations);

		spriteRenderer.color = Color.Lerp(startColor, endColor, t);
	}

	private Color HexToColor(string hex)
	{
		Color newColor;
		if (ColorUtility.TryParseHtmlString(hex, out newColor))
		{
			return newColor;
		}
		else
		{
			return Color.white;
		}
	}
}