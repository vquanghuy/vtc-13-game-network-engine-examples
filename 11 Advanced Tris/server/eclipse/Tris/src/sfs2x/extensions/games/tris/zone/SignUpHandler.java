package sfs2x.extensions.games.tris.zone;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.SQLException;

import com.smartfoxserver.v2.db.IDBManager;
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
		
		IDBManager dbManager = getParentExtension().getParentZone().getDBManager();
		Connection connection;	
        
    	try {
			connection = dbManager.getConnection();
			String sql = String.format(
					"INSERT INTO users (name, email, password) "
					+ "VALUES ('%s', '%s', '%s')", 
					name, email, password);
			PreparedStatement stmt = connection.prepareStatement(sql);
			int affectedRow = stmt.executeUpdate();
			trace("Insert row: ", affectedRow);
		} catch (SQLException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}        
	}
}
