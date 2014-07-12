/**
 * Copyright (c) 2010, 2011 Darmstadt University of Technology.
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution, and is available at
 * http://www.eclipse.org/legal/epl-v10.html
 * 
 * Contributors:
 *     Sebastian Proksch - initial API and implementation
 */

import org.eclipse.jetty.server.Server;
import org.eclipse.jetty.webapp.WebAppContext;

import com.google.inject.servlet.GuiceFilter;

public class run_platform_old {
	
	private static Server server;
	
	public static void main(String[] args) throws Exception {
		
		server = new Server(8080);
		
		WebAppContext webAppContext = new WebAppContext();
		webAppContext.setContextPath("/");
		webAppContext.setResourceBase("src/main/webapp");
		webAppContext.setParentLoaderPriority(true);
		webAppContext.addEventListener(new GuiceConfig());
		webAppContext.addFilter(GuiceFilter.class, "/*", null);
		
		server.setHandler(webAppContext);
		server.start();
		server.join();
	}
}