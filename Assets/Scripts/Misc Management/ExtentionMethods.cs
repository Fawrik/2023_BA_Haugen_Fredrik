using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtentionMethods
{
	
	/// <summary>
	/// Rounds Vector3.
	/// </summary>
	/// <param name="vector2"></param>
	/// <param name="decimalPlaces"></param>
	/// <returns></returns>
	public static Vector2 Round(this Vector2 vector2, int decimalPlaces)
	{
		float multiplier = 1;
		for (int i = 0; i < decimalPlaces; i++)
		{
			multiplier *= 10f;
		}
		return new Vector2(
			Mathf.Round(vector2.x * multiplier) / multiplier,
			Mathf.Round(vector2.y * multiplier) / multiplier);
	}
	
}
