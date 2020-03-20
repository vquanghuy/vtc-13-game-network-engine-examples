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
using Sfs2X.Entities.Data;
using Sfs2X.Requests;

public class SignupController : MonoBehaviour {
	//----------------------------------------------------------
	// Biến giao diện cho Sign Up
	//----------------------------------------------------------

	public InputField nameInput;
	public InputField emailInput;
	public InputField passwordInput;

	public Button signupButton;
	public Text errorText;

	//----------------------------------------------------------
	// Thuộc tính private
	//----------------------------------------------------------

	private SmartFox sfs;

	//----------------------------------------------------------
	// Hàm của Unity
	//----------------------------------------------------------

	void Awake() {
		Application.runInBackground = true;

		if (SmartFoxConnection.IsInitialized)
		{
			sfs = SmartFoxConnection.Connection;
		}
		else
		{
			SceneManager.LoadScene("Login");
			return;
		}
	}

	void Start()
	{
		sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);

		sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
		sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);

		sfs.Send(new LoginRequest(""));
	}

	// Update is called once per frame
	void Update() {
		if (sfs != null)
			sfs.ProcessEvents();
	}

	//----------------------------------------------------------
	// Public interface methods for UI
	//----------------------------------------------------------

	public void OnSignUpButtonClick()
	{
		SFSObject obj = new SFSObject();
		obj.PutText("name", "Huy1");
		obj.PutText("email", "abc@xyz.com");
		obj.PutText("password", "123");

		sfs.Send(new ExtensionRequest("signup", obj));
	}

	//----------------------------------------------------------
	// Private helper methods
	//----------------------------------------------------------
	
	private void enableSignupUI(bool enable) {
		nameInput.interactable = enable;
		emailInput.interactable = enable;
		passwordInput.interactable = enable;
		signupButton.interactable = enable;
		errorText.text = "";
	}
	
	private void reset() {
		// Remove SFS2X listeners
		// This should be called when switching scenes, so events from the server do not trigger code in this scene
		sfs.RemoveAllEventListeners();
		
		// Enable interface
		enableSignupUI(true);
	}

	//----------------------------------------------------------
	// SmartFoxServer event listeners
	//----------------------------------------------------------
	private void OnExtensionResponse(BaseEvent evt)
	{
	}
	
	private void OnLogin(BaseEvent evt) {
		Debug.Log("Login as " + evt.Params["user"]);
	}
	
	private void OnLoginError(BaseEvent evt) {
		// Show error message
		errorText.text = "Login failed: " + (string) evt.Params["errorMessage"];
	}
}
