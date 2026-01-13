/// @file dragndrop_onclick.cs
/// @brief Handles drag and drop functionality triggered by UI button clicks.
/// @details This script manages prefab instantiation based on dropdown selection
///          when a button is clicked in the Unity UI.

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages drag and drop prefab instantiation via button click and dropdown selection.
/// </summary>
/// <remarks>
/// Attach this script to a GameObject and assign the required UI elements in the Inspector.
/// </remarks>
public class dragndrop_onclick : MonoBehaviour
{
	/// <summary>
	/// The button that triggers prefab instantiation.
	/// </summary>
	/// <remarks>Make sure to attach this Button in the Inspector.</remarks>
	public Button m_YourFirstButton;

	/// <summary>
	/// The dropdown used to select which prefab to instantiate.
	/// </summary>
	/// <remarks>Make sure to attach this Dropdown in the Inspector.</remarks>
	public Dropdown m_Dropdown;

	//public Transform prefab;

	/// <summary>
	/// The first prefab option to instantiate (dropdown value 0).
	/// </summary>
	public Transform prefab1;

	/// <summary>
	/// The second prefab option to instantiate (dropdown value 1).
	/// </summary>
	public Transform prefab2;

	/// <summary>
	/// Reference to the most recently spawned prefab instance.
	/// </summary>
	private Transform spawn;

	/// <summary>
	/// Initializes button and dropdown event listeners.
	/// </summary>
	/// <remarks>
	/// Sets up click listener on the button to instantiate prefabs and
	/// value changed listener on the dropdown for debug logging.
	/// </remarks>
	void Start()
	{
		Button btn1 = m_YourFirstButton.GetComponent<Button>();
		Dropdown derp = m_Dropdown.GetComponent<Dropdown>();

		//Calls the TaskOnClick/TaskWithParameters method when you click the Button
		btn1.onClick.AddListener(delegate {
			Instantiate_Prefab(derp.value); 
		});

		derp.onValueChanged.AddListener (delegate {
			TaskWithParameters (derp.value.ToString());
		});
	}

	/// <summary>
	/// Called once per frame.
	/// </summary>
	void Update()
	{
		
	}


	/// <summary>
	/// Logs a debug message to the console.
	/// </summary>
	/// <param name="message">The message string to log.</param>
	void TaskWithParameters(string message)
	{
		Debug.Log(message);
	}

	/// <summary>
	/// Instantiates a prefab based on the dropdown selection value.
	/// </summary>
	/// <param name="value">The dropdown index determining which prefab to spawn.
	/// 0 = prefab1, 1 = prefab2.</param>
	void Instantiate_Prefab(int value)
	{
		if (value == 0) {
			spawn = Instantiate (prefab1, new Vector3 (0, 0, 0), Quaternion.identity);
		}
		if (value == 1) {
			spawn = Instantiate (prefab2, new Vector3 (0, 0, 0), Quaternion.identity);
		}
	}
}