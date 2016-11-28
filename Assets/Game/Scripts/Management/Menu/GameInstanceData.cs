using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameInstanceData : MonoBehaviour {

	private static GameInstanceData _vehicleManager;

    public bool exists = false;

	// Use the static object pattern to guarantee that this object is correctly assigned and pressent in the scene.
	public static GameInstanceData Instance
	{
		get
		{
			if (!_vehicleManager)
			{
				_vehicleManager = FindObjectOfType(typeof(GameInstanceData)) as GameInstanceData;

				if (!_vehicleManager)
				{
					Debug.LogError("You need to have at least one active Game Instance data script in your scene.");
				}
			}
			return _vehicleManager;
		}
	}
		
	// Single Player Input Field Variables
	// ###################################
	[SerializeField] private int trackNo;
	[SerializeField] private int noOfAI;
	[SerializeField] private int noOfSelectedTokens;
    [SerializeField]
    private int numLaps = 1;

    [SerializeField] private Dropdown trackDropdown;
	[SerializeField] private Dropdown AIDropdown;
	[SerializeField] private Dropdown tokenDropdown;

	// Create Server Input Field Variables
	// ###################################
	[SerializeField] private string gameName;

	[SerializeField] private InputField create_server_gameName; 
	[SerializeField] private Dropdown create_server_trackDropdown;
	[SerializeField] private Dropdown create_server_AIDropdown;

	// Connect to Server Input Field Variables
	// ###################################
	[SerializeField] private string IPAddress;

	[SerializeField] private InputField connect_server_ipAddress; 

    public static int TrackNumber { get { return Instance.trackNo; } }
    public static int NumLaps { get { return Instance.numLaps; } }

    void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}

	void Start() {
        if (Instance.exists)
        {
            Object.Destroy(Instance.gameObject);
        }

		// Single Player Input Field Options
		// #################################
		trackDropdown.onValueChanged.AddListener(delegate {
			trackDropdownValueChangedHandler(trackDropdown);
		});

		AIDropdown.onValueChanged.AddListener(delegate {
			aiDropdownValueChangedHandler(AIDropdown);
		});

		tokenDropdown.onValueChanged.AddListener(delegate {
			tokenDropdownValueChangedHandler(tokenDropdown);
		});


		// Create Server Input Field Options
		// #################################
		create_server_trackDropdown.onValueChanged.AddListener(delegate {
			create_server_trackDropdownValueChangedHandler(create_server_trackDropdown);
		});

		create_server_AIDropdown.onValueChanged.AddListener(delegate {
			create_server_aiDropdownValueChangedHandler(create_server_AIDropdown);
		});

		create_server_gameName.onValueChanged.AddListener(delegate {
			create_server_gameNameValueChangedHandler(create_server_gameName);
		});


		// Connect to Server Input Field Options
		// #####################################
		connect_server_ipAddress.onValueChanged.AddListener(delegate {
			connect_server_ipAddressValueChangedHandler(connect_server_ipAddress);
		});

        _vehicleManager.exists = true;
    }



	// Single Player Input Field Options
	// #################################
	private void trackDropdownValueChangedHandler(Dropdown target) {
		trackNo = target.value;
	}
	private void aiDropdownValueChangedHandler(Dropdown target) {
		noOfAI = target.value;
	}
	private void tokenDropdownValueChangedHandler(Dropdown target) {
		numLaps = target.value + 1;
	}



	// Create Server Input Field Options
	// #################################
	private void create_server_trackDropdownValueChangedHandler(Dropdown target) {
		trackNo = target.value;
	}
	private void create_server_aiDropdownValueChangedHandler(Dropdown target) {
		noOfAI = target.value;
	}
	private void create_server_gameNameValueChangedHandler(InputField target) {
		gameName = target.text;
	}



	// Connect to Server Input Field Options
	// #####################################
	private void connect_server_ipAddressValueChangedHandler(InputField target) {
		IPAddress = target.text;
	}

	public void SetDropdownIndex(int index) {
		trackDropdown.value = index;
	}

    public static int GetNumAI()
    {
        return Instance.noOfAI;
    }
}
