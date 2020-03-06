using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
using Sfs2X.Requests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Complete;

public class GameNetworkManager : MonoBehaviour
{
    public GameObject tankPrefab;
	public Color[] playerColors = { Color.red, Color.green, Color.blue };


	private SmartFox sfs;

    private GameObject localPlayer;
	private Complete.TankMovement localPlayerController;
	private int localPlayerColor;

	private Color playerColor;
    private Dictionary<SFSUser, GameObject> remotePlayers = new Dictionary<SFSUser, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        if (!SmartFoxConnection.IsInitialized)
        {
            SceneManager.LoadScene("Lobby");
            return;
        }

        sfs = SmartFoxConnection.Connection;
		Debug.Log("Player ID: " + sfs.MySelf.Id);

		sfs.AddEventListener(SFSEvent.OBJECT_MESSAGE, OnObjectMessage);
		sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
		sfs.AddEventListener(SFSEvent.USER_VARIABLES_UPDATE, OnUserVariableUpdate);
		sfs.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserExitRoom);
		sfs.AddEventListener(SFSEvent.USER_ENTER_ROOM, OnUserEnterRoom);

		SpawnLocalPlayer();
	}

	// Update is called once per frame
	void FixedUpdate()
    {
		if (sfs != null)
		{
			sfs.ProcessEvents();

			// If we spawned a local player, send position if movement is dirty
			if (localPlayer != null && localPlayerController != null && localPlayerController.m_MovementDirty)
			{
				List<UserVariable> userVariables = new List<UserVariable>();
				userVariables.Add(new SFSUserVariable("x", (double)localPlayer.transform.position.x));
				userVariables.Add(new SFSUserVariable("y", (double)localPlayer.transform.position.y));
				userVariables.Add(new SFSUserVariable("z", (double)localPlayer.transform.position.z));
				userVariables.Add(new SFSUserVariable("rot", (double)localPlayer.transform.rotation.eulerAngles.y));
				sfs.Send(new SetUserVariablesRequest(userVariables));
				localPlayerController.m_MovementDirty = false;
			}
		}
	}

	void OnApplicationQuit()
	{
		// Before leaving, lets notify the others about this client dropping out
		RemoveLocalPlayer();
	}

	public void OnObjectMessage(BaseEvent evt)
	{
		// The only messages being sent around are remove messages from users that are leaving the game
		ISFSObject dataObj = (SFSObject)evt.Params["message"];
		SFSUser sender = (SFSUser)evt.Params["sender"];

		if (dataObj.ContainsKey("cmd"))
		{
			switch (dataObj.GetUtfString("cmd"))
			{
				case "rm":
					Debug.Log("Removing player unit " + sender.Id);
					RemoveRemotePlayer(sender);
					break;
			}
		}
	}

	public void OnConnectionLost(BaseEvent evt)
	{
		// Reset all internal states so we kick back to login screen
		sfs.RemoveAllEventListeners();
		SceneManager.LoadScene("Lobby");
	}

	public void OnUserVariableUpdate(BaseEvent evt)
	{
		List<string> changedVars = (List<string>)evt.Params["changedVars"];

		SFSUser user = (SFSUser)evt.Params["user"];

		if (user == sfs.MySelf) return;

		if (!remotePlayers.ContainsKey(user))
		{
			// New client just started transmitting - lets create remote player
			Vector3 pos = new Vector3(0, 0, 0);
			if (user.ContainsVariable("x") && user.ContainsVariable("y") && user.ContainsVariable("z"))
			{
				pos.x = (float)user.GetVariable("x").GetDoubleValue();
				pos.y = (float)user.GetVariable("y").GetDoubleValue();
				pos.z = (float)user.GetVariable("z").GetDoubleValue();
			}
			float rotAngle = 0;
			if (user.ContainsVariable("rot"))
			{
				rotAngle = (float)user.GetVariable("rot").GetDoubleValue();
			}
			
			int colorId = 0;
			if (user.ContainsVariable("color"))
			{
				colorId = user.GetVariable("color").GetIntValue();
			}
			SpawnRemotePlayer(user, colorId, pos, Quaternion.Euler(0, rotAngle, 0));
		}

		// Check if the remote user changed his position or rotation
		if (changedVars.Contains("x") && changedVars.Contains("y") && changedVars.Contains("z") && changedVars.Contains("rot"))
		{
			// Move the character to a new position...
			remotePlayers[user].transform.position = new Vector3((float)user.GetVariable("x").GetDoubleValue(), (float)user.GetVariable("y").GetDoubleValue(), (float)user.GetVariable("z").GetDoubleValue());
			remotePlayers[user].transform.rotation = Quaternion.Euler(0, (float)user.GetVariable("rot").GetDoubleValue(), 0);
		}
	}

	public void OnUserExitRoom(BaseEvent evt)
	{
		// Someone left - lets make certain they are removed if they didn't nicely send a remove command
		SFSUser user = (SFSUser)evt.Params["user"];
		RemoveRemotePlayer(user);
	}

	public void OnUserEnterRoom(BaseEvent evt)
	{
		// User joined - and we might be standing still (not sending position updates); so let's send him our position
		if (localPlayer != null)
		{
			List<UserVariable> userVariables = new List<UserVariable>();
			userVariables.Add(new SFSUserVariable("x", (double)localPlayer.transform.position.x));
			userVariables.Add(new SFSUserVariable("y", (double)localPlayer.transform.position.y));
			userVariables.Add(new SFSUserVariable("z", (double)localPlayer.transform.position.z));
			userVariables.Add(new SFSUserVariable("rot", (double)localPlayer.transform.rotation.eulerAngles.y));
			userVariables.Add(new SFSUserVariable("color", localPlayerColor));
			sfs.Send(new SetUserVariablesRequest(userVariables));
		}
	}

	private void SpawnLocalPlayer()
	{
		Vector3 pos;
		Quaternion rot;

		// See if there already exists a model - if so, take its pos+rot before destroying it
		if (localPlayer != null)
		{
			pos = localPlayer.transform.position;
			rot = localPlayer.transform.rotation;
			Camera.main.transform.parent = null;
			Destroy(localPlayer);
		}
		else
		{
			pos = new Vector3(0, 0, 0);
			rot = Quaternion.identity;
		}
		
		// Lets spawn our local player model
		localPlayer = GameObject.Instantiate(tankPrefab) as GameObject;
		localPlayer.tag = "Player";
		localPlayer.transform.position = pos;
		localPlayer.transform.rotation = rot;

		// Assign starting material
		MeshRenderer[] renderers = localPlayer.GetComponentsInChildren<MeshRenderer>();
		localPlayerColor = Random.Range(0, playerColors.Length);
		playerColor = playerColors[localPlayerColor];

		// Go through all the renderers...
		for (int i = 0; i < renderers.Length; i++)
		{
			// ... set their material color to the color specific to this tank.
			renderers[i].material.color = playerColor;
		}

		// Lets set the model and material choice and tell the others about it
		List<UserVariable> userVariables = new List<UserVariable>();
		userVariables.Add(new SFSUserVariable("color", localPlayerColor));
		sfs.Send(new SetUserVariablesRequest(userVariables));

		// Store controller
		localPlayerController = localPlayer.GetComponent<Complete.TankMovement>();
	}

	private void RemoveLocalPlayer()
	{
		// Someone dropped off the grid. Lets remove him
		SFSObject obj = new SFSObject();
		obj.PutUtfString("cmd", "rm");
		sfs.Send(new ObjectMessageRequest(obj, sfs.LastJoinedRoom));
	}

	private void SpawnRemotePlayer(SFSUser user, int colorId, Vector3 pos, Quaternion rot)
	{
		// See if there already exists a model so we can destroy it first
		if (remotePlayers.ContainsKey(user) && remotePlayers[user] != null)
		{
			Destroy(remotePlayers[user]);
			remotePlayers.Remove(user);
		}

		// Lets spawn our remote player model
		GameObject remotePlayer = GameObject.Instantiate(tankPrefab) as GameObject;
		remotePlayer.transform.position = pos;
		remotePlayer.transform.rotation = rot;

		// Color and name
		MeshRenderer[] renderers = localPlayer.GetComponentsInChildren<MeshRenderer>();
		playerColor = playerColors[colorId];

		for (int i = 0; i < renderers.Length; i++)
		{
			// ... set their material color to the color specific to this tank.
			renderers[i].material.color = playerColor;
		}

		// Lets track the dude
		remotePlayers.Add(user, remotePlayer);
	}

	private void RemoveRemotePlayer(SFSUser user)
	{
		if (user == sfs.MySelf) return;

		if (remotePlayers.ContainsKey(user))
		{
			Destroy(remotePlayers[user]);
			remotePlayers.Remove(user);
		}
	}
}
