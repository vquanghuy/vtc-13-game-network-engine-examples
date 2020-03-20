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

		enableSignupUI(true);
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
		string name = nameInput.text;
		string email = emailInput.text;
		string password = passwordInput.text;

		if (name.Length == 0 || email.Length == 0 || password.Length == 0)
		{
			errorText.text = "Name / Email / Password must have at least 1 character";
			return;
		}

		SFSObject obj = new SFSObject();
		obj.PutText("name", name);
		obj.PutText("email", email);
		obj.PutText("password", password);

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
	public void OnExtensionResponse(BaseEvent evt)
	{
		string cmd = (string)evt.Params["cmd"];
		SFSObject dataObject = (SFSObject)evt.Params["params"];

		switch (cmd)
		{
			case "signup":
				{
					if (dataObject.GetBool("success"))
					{
						reset();
						sfs.Send(new LogoutRequest());
						SceneManager.LoadScene("Login");
					} else
					{
						errorText.text = dataObject.GetText("message");
					}
				}
				break;
		}
	}

	private void OnLogin(BaseEvent evt) {
		Debug.Log("Login as " + evt.Params["user"]);
	}
	
	private void OnLoginError(BaseEvent evt) {
		// Show error message
		errorText.text = "Login failed: " + (string) evt.Params["errorMessage"];
	}
}
