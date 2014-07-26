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
package kave;

import static java.lang.String.format;

import org.apache.commons.lang3.builder.EqualsBuilder;
import org.apache.commons.lang3.builder.HashCodeBuilder;
import org.apache.commons.lang3.exception.ExceptionUtils;

public class Result {

    public State status;
    public String message;

    public static Result fail(String message, Object... args) {
        Result res = new Result();
        res.status = State.FAIL;
        res.message = format(message, args);
        return res;
    }

    public static Result fail(Throwable t) {
    	return fail(ExceptionUtils.getStackTrace(t).replaceAll("\n", "<br />\n"));
    }

    public static Result ok() {
        Result res = new Result();
        res.status = State.OK;
        return res;
    }

    @Override
    public boolean equals(Object obj) {
        return EqualsBuilder.reflectionEquals(this, obj);
    }

    @Override
    public int hashCode() {
        return HashCodeBuilder.reflectionHashCode(this);
    }
    
    @Override
    public String toString() {
        return String.format("%s: %s", status, message);
    }

    public enum State {
        OK, FAIL
    }
}