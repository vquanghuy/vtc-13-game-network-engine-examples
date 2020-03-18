package sfs2x.extensions.games.tris.zone;

import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;

public class SignUpHandler extends BaseClientRequestHandler
{
	@Override
	public void handleClientRequest(User user, ISFSObject params)
	{
		String name = params.getText("name");
		String email = params.getText("email");
		String password = params.getText("password");
		
		trace("Sign Up Info");
		trace("name: " + name);
		trace("email: " + email);
		trace("password: " + password);
	}
}
