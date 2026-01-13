/// @file hankpym.cs
/// @brief Manages joint anchor positions during object scaling.
/// @details This script preserves joint anchor configurations for child objects
///          while the parent object scales, disabling itself once max scale is reached.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Maintains joint anchor positions for child objects during scaling operations.
/// </summary>
/// <remarks>
/// Named after Hank Pym (Ant-Man) because of it's ability to change size at will, this script handles the physics joint corrections
/// needed when scaling objects. Automatically disables when max scale is reached.
/// Requires a MolScale component on the same GameObject.
/// 
/// Allows to insert big molecules in versy small space
/// </remarks>
public class hankpym : MonoBehaviour {

	/// <summary>
	/// Array of all child Transform components.
	/// </summary>
	public Transform[] children;

	/// <summary>
	/// Cached connected anchor positions for each child's joint.
	/// </summary>
	public Vector3[] _connectedAnchor;

	/// <summary>
	/// Cached anchor positions for each child's joint.
	/// </summary>
	public Vector3[] _anchor;

	/// <summary>
	/// Maximum scale threshold at which this script disables itself.
	/// </summary>
	float maxscale ;

	/// <summary>
	/// Initializes child references and caches joint anchor positions.
	/// </summary>
	/// <remarks>
	/// Disables auto-configuration of connected anchors to prevent unwanted behavior.
	/// </remarks>
	void Start()
	{
		children = transform.GetComponentsInChildren<Transform>();
		_connectedAnchor = new Vector3[children.Length];
		_anchor = new Vector3[children.Length];
		for (int i = 1; i < children.Length; i++)
		{
			if (children[i].GetComponent<Joint>() != null)
			{
				children[i].GetComponent<Joint>().autoConfigureConnectedAnchor = false; // /!\ script HankPym.cs will give bad results if Connected Anchor is auto configured
				_connectedAnchor[i] = children[i].GetComponent<Joint>().connectedAnchor;
				_anchor[i] = children[i].GetComponent<Joint>().anchor;
			}
		}
	}

	/// <summary>
	/// Updates joint anchors each frame and checks for max scale threshold.
	/// </summary>
	/// <remarks>
	/// Disables itself when the object reaches or exceeds the maximum scale.
	/// Continuously reapplies cached anchor positions to maintain joint integrity.
	/// </remarks>
	private void Update()
	{
		//check gameobject size and disable hankpym script if gameobject size is superior or equal to final size in 
		//

		MolScale thisGOscale = this.GetComponent<MolScale> ();
		maxscale = thisGOscale.maxScale.x;
		if (this.transform.localScale.x>=maxscale) {
			enabled = false;
		}

		for (int i = 1; i < children.Length; i++)
		{
            //Debug.Log("index " + i + " name " + children[i].name);
            if (children[i].GetComponent<Joint>() != null)
			{
                //Debug.Log("Doing something");
                children[i].GetComponent<Joint>().connectedAnchor = _connectedAnchor[i];
				children[i].GetComponent<Joint>().anchor = _anchor[i];
			}
		}
	}
}
