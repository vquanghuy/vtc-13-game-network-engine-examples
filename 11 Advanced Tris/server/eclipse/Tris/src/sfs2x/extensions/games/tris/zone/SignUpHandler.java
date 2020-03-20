package sfs2x.extensions.games.tris.zone;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.SQLException;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import com.smartfoxserver.v2.db.IDBManager;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;

public class SignUpHandler extends BaseClientRequestHandler
{
	public static final Pattern VALID_EMAIL_ADDRESS_REGEX = 
	    Pattern.compile("^[A-Z0-9._%+-]+@[A-Z0-9.-]+\\.[A-Z]{2,6}$", Pattern.CASE_INSENSITIVE);

	public static boolean isEmailValid(String email) {
        Matcher matcher = VALID_EMAIL_ADDRESS_REGEX .matcher(email);
        return matcher.find();
	}
	
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
			
			// Thông báo ngược lại cho user
			ISFSObject respObj = new SFSObject();
			respObj.putBool("success", true);
			
			send("signup", respObj, user);
		} catch (SQLException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
			
			ISFSObject respObj = new SFSObject();
			respObj.putBool("success", false);
			respObj.putText("message", e.getMessage());
			
			send("signup", respObj, user);
		}   
	}
}
