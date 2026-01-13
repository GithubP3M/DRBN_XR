/// @file MolScale.cs
/// @brief Handles smooth scaling of molecular objects over time.
/// @details Provides coroutine-based linear interpolation for scaling GameObjects
///          from their initial size to a target size over a specified duration.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages smooth scaling transitions for molecular GameObjects.
/// </summary>
/// <remarks>
/// Uses coroutines to lerp between scale values over time.
/// Works in conjunction with hankpym scripts to maintain joint integrity during scaling.
/// </remarks>
public class MolScale : MonoBehaviour {

	/// <summary>
	/// The initial scale of the object captured at start.
	/// </summary>
	Vector3 minScale;

	/// <summary>
	/// The target scale to reach after scaling completes.
	/// </summary>
	public Vector3 maxScale;

	/// <summary>
	/// Speed multiplier for the scaling operation.
	/// </summary>
	public float speed = 2f;

	/// <summary>
	/// Duration in seconds for the scaling operation.
	/// </summary>
	public float duration = 50f;

	/// <summary>
	/// Initializes scaling parameters and starts the scale coroutine.
	/// </summary>
	/// <returns>IEnumerator for coroutine execution.</returns>
	IEnumerator Start () {
		minScale = transform.localScale;
		maxScale = new Vector3(0.001f,0.001f,0.001f);
		yield return ScaleLerp (minScale, maxScale, duration);
	}

	/// <summary>
	/// Smoothly interpolates the object's scale between two values over time.
	/// </summary>
	/// <param name="a">The starting scale vector.</param>
	/// <param name="b">The target scale vector.</param>
	/// <param name="time">Duration of the scaling transition in seconds.</param>
	/// <returns>IEnumerator for coroutine execution.</returns>
	public IEnumerator ScaleLerp(Vector3 a, Vector3 b, float time){
		float i = 0.0f;
		//float rate = (1.0f / time) * speed; //trying a slower rate 
        float rate = (0.5f / time) * speed;
        while (i < 1.0f){
			i += Time.deltaTime * rate;
			transform.localScale = Vector3.Lerp (a, b, i);
			yield return null;
		}
	}
}
