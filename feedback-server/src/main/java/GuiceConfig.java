/**
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * Contributors:
 *    - Sebastian Proksch
 */
import java.io.File;
import java.io.IOException;
import java.util.HashMap;
import java.util.Map;

import kave.FeedbackService;
import kave.UniqueFileCreator;
import kave.UploadCleanser;

import org.codehaus.jackson.jaxrs.JacksonJsonProvider;
import org.codehaus.jackson.map.ObjectMapper;

import com.google.inject.Guice;
import com.google.inject.Injector;
import com.google.inject.Provides;
import com.google.inject.servlet.GuiceServletContextListener;
import com.sun.jersey.guice.JerseyServletModule;
import com.sun.jersey.guice.spi.container.servlet.GuiceContainer;
import com.sun.jersey.spi.container.servlet.ServletContainer;

public class GuiceConfig extends GuiceServletContextListener {
    @Override
    protected Injector getInjector() {
        return Guice.createInjector(new JerseyServletModule() {
            private File dataDir;
            private File tmpDir;
            private UniqueFileCreator tmpUfc;
            private UniqueFileCreator dataUfc;

            @Override
            protected void configureServlets() {

                dataDir = getPath("data");
                tmpDir = getPath("tmp");

                dataDir.mkdir();
                tmpDir.mkdir();
                tmpUfc = new UniqueFileCreator(tmpDir, "zip");
                dataUfc = new UniqueFileCreator(dataDir, "zip");

                ObjectMapper mapper = new ObjectMapper();
                bind(JacksonJsonProvider.class).toInstance(new JacksonJsonProvider(mapper));

                Map<String, String> params = new HashMap<String, String>();
                params.put(ServletContainer.JSP_TEMPLATES_BASE_PATH, "WEB-INF/jsp");
                filterRegex("/((?!css|js|images).)*").through(GuiceContainer.class, params);
            }

            private File getPath(String folderName) {
                // put a file into {TOMCAT-conf}/[engine]/[host]/[app=name].xml with the content:
                //
                // <Context path="" docBase="[app-name]">
                // <Parameter name="qualifier" value="[whatever you like]" override="false"/>
                // </Context>

                String qualifier = getServletContext().getInitParameter("qualifier");
                if (qualifier == null) {
                    return new File(folderName);
                } else {
                    return new File(folderName + "-" + qualifier);
                }
            }

            @Provides
            public UploadCleanser provideUploadCleanser() {
                return new UploadCleanser(tmpUfc);
            }

            @Provides
            public FeedbackService provideFeedbackService(UploadCleanser cleanser) throws IOException {
                return new FeedbackService(dataDir, tmpDir, cleanser, tmpUfc, dataUfc);
            }
        });
    }
}