using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles UI-driven prefab instantiation via button clicks and dropdown selection.
/// Allows spawning different prefabs based on dropdown value.
/// </summary>
/// <remarks>
/// Attach Button and Dropdown references in the Unity Inspector.
/// Dropdown value 0 spawns prefab1, value 1 spawns prefab2.
/// </remarks>
public class dragndrop_onclick : MonoBehaviour
{
	/// <summary>Button that triggers prefab instantiation when clicked.</summary>
	public Button m_YourFirstButton;
	
	/// <summary>Dropdown to select which prefab to spawn.</summary>
	public Dropdown m_Dropdown;
	
	//public Transform prefab;
	
	/// <summary>First prefab option (spawned when dropdown value is 0).</summary>
	public Transform prefab1;
	
	/// <summary>Second prefab option (spawned when dropdown value is 1).</summary>
	public Transform prefab2;
	
	/// <summary>Reference to the most recently spawned prefab instance.</summary>
	private Transform spawn;

	/// <summary>
	/// Initializes button and dropdown event listeners.
	/// Sets up click handler for spawning and dropdown change logging.
	/// </summary>
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
	/// Update is called once per frame (currently unused).
	/// </summary>
	void Update()
	{
		
	}

	/// <summary>
	/// Logs a message to the console. Called when dropdown value changes.
	/// </summary>
	/// <param name="message">The message to log (dropdown value as string).</param>
	void TaskWithParameters(string message)
	{
		Debug.Log(message);
	}

	/// <summary>
	/// Instantiates a prefab at the origin based on the dropdown selection.
	/// </summary>
	/// <param name="value">Dropdown index: 0 for prefab1, 1 for prefab2.</param>
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