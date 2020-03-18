package sfs2x.extensions.games.tris.zone;

import com.smartfoxserver.v2.extensions.SFSExtension;

public class TrisZoneExtension extends SFSExtension {

	@Override
	public void init() {
		// TODO Auto-generated method stub
		addRequestHandler("signup", SignUpHandler.class);
	}

}
