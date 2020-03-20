using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Logging;
using Sfs2X.Util;
using Sfs2X.Core;
using Sfs2X.Entities;

public class LoginController : MonoBehaviour {

	//----------------------------------------------------------
	// Thuộc tính public của editor
	//----------------------------------------------------------

	[Tooltip("IP address or domain name of the SmartFoxServer 2X instance")]
	public string Host = "127.0.0.1";

	[Tooltip("TCP port listened by the SmartFoxServer 2X instance; used for regular socket connection in all builds except WebGL")]
	public int TcpPort = 9933;

	[Tooltip("Name of the SmartFoxServer 2X Zone to join")]
	public string Zone = "TrisGame";

	//----------------------------------------------------------
	// Biến giao diện cho Login
	//----------------------------------------------------------

	public InputField loginNameInput;
	public Button loginButton;
	public Text loginErrorText;

	//----------------------------------------------------------
	// Biến giao diện cho Sign Up
	//----------------------------------------------------------

	public InputField signupNameInput;
	public InputField signupEmailInput;
	public InputField signupPasswordInput;
	public Button signupButton;
	public Text signupErrorText;

	//----------------------------------------------------------
	// Thuộc tính private
	//----------------------------------------------------------

	private SmartFox sfs;

	//----------------------------------------------------------
	// Hàm của Unity
	//----------------------------------------------------------

	void Awake() {
		Application.runInBackground = true;

		enableLoginUI(true);
	}

	void Start()
	{
		// Kết nối khi vừa vào scene
		ConfigData cfg = new ConfigData();
		cfg.Host = Host;
		cfg.Port = TcpPort;
		cfg.Zone = Zone;

		sfs = new SmartFox();

		sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
		sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);

		// Connect to SFS2X
		sfs.Connect(cfg);
	}
	
	// Update is called once per frame
	void Update() {
		if (sfs != null)
			sfs.ProcessEvents();
	}

	//----------------------------------------------------------
	// Public interface methods for UI
	//----------------------------------------------------------

	public void OnLoginButtonClick() {
		enableLoginUI(false);

		sfs.Send(new Sfs2X.Requests.LoginRequest(loginNameInput.text));
	}

	//----------------------------------------------------------
	// Private helper methods
	//----------------------------------------------------------
	
	private void enableLoginUI(bool enable) {
		loginNameInput.interactable = enable;
		loginButton.interactable = enable;
		loginErrorText.text = "";
	}
	
	private void reset() {
		// Remove SFS2X listeners
		// This should be called when switching scenes, so events from the server do not trigger code in this scene
		sfs.RemoveAllEventListeners();
		
		// Enable interface
		enableLoginUI(true);
	}

	//----------------------------------------------------------
	// SmartFoxServer event listeners
	//----------------------------------------------------------

	private void OnConnection(BaseEvent evt) {
		if ((bool)evt.Params["success"])
		{
			Debug.Log("SFS2X API version: " + sfs.Version);
			Debug.Log("Connection mode is: " + sfs.ConnectionMode);

			// Save reference to SmartFox instance; it will be used in the other scenes
			SmartFoxConnection.Connection = sfs;
		}
		else
		{
			// Remove SFS2X listeners and re-enable interface
			reset();

			// Show error message
			loginErrorText.text = "Connection failed; is the server running at all?";
		}
	}
	
	private void OnConnectionLost(BaseEvent evt) {
		// Remove SFS2X listeners and re-enable interface
		reset();

		string reason = (string) evt.Params["reason"];

		if (reason != ClientDisconnectionReason.MANUAL) {
			// Show error message
			loginErrorText.text = "Connection was lost; reason is: " + reason;
		}
	}
	
	private void OnLogin(BaseEvent evt) {
		// Remove SFS2X listeners and re-enable interface
		reset();

		// Load lobby scene
		//Application.LoadLevel("Lobby");
		SceneManager.LoadScene("Lobby");
	}
	
	private void OnLoginError(BaseEvent evt) {
		// Disconnect
		sfs.Disconnect();

		// Remove SFS2X listeners and re-enable interface
		reset();
		
		// Show error message
		loginErrorText.text = "Login failed: " + (string) evt.Params["errorMessage"];
	}
}
