package sfs2x.extensions.games.tris.room;

import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;

public class RestartHandler extends BaseClientRequestHandler
{
	@Override
	public void handleClientRequest(User user, ISFSObject params)
	{
		TrisRoomExtension gameExt = (TrisRoomExtension) getParentExtension();
		
		// Checks if two players are available and start game
		if (gameExt.getGameRoom().getSize().getUserCount() == 2)
			gameExt.startGame();
	}
}
