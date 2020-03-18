import java.sql.*;
import java.util.Properties;

public class Demo {
	Connection conn;
	
	public Demo() throws SQLException {
		String URL = "jdbc:mysql://localhost/trisgame?serverTimezone=UTC";
		Properties info = new Properties( );
		info.put("user", "root");
		info.put("password", "");
		
		conn = DriverManager.getConnection(URL, info);
		System.out.println("Connected database successfully...");
	}
	
	public int insertUser(String name, String email, String password) throws SQLException {
		String sql = String.format(
				"INSERT INTO users (name, email, password) "
				+ "VALUES ('%s', '%s', '%s')", 
				name, email, password);
		PreparedStatement stmt = conn.prepareStatement(sql);
		return stmt.executeUpdate();
	}
	
	public void printUsers() throws SQLException {
		Statement stmt = conn.createStatement();

		String sql = "SELECT * FROM users";
		ResultSet rs = stmt.executeQuery(sql);
		
		while (rs.next()) {
			int id  = rs.getInt("id");
			String name = rs.getString("name");
			String email = rs.getString("email");
			String password = rs.getString("password");

			System.out.print("ID: " + id);
			System.out.print(", Name: " + name);
			System.out.print(", Email: " + email);
			System.out.println(", Password: " + password);
		}
		
		rs.close();
	}
	
	public static void main(String[] args) throws SQLException {
		try {
			Demo demoApp = new Demo();
			demoApp.insertUser("Huy", "huy.vuquang@stdio.vn", "123");
			demoApp.printUsers();
			
		} catch(Exception ex) {
			ex.printStackTrace();
		}
	}

}
