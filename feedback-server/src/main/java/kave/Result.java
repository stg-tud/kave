/**
 * Copyright (c) 2011-2014 Darmstadt University of Technology.
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution, and is available at
 * http://www.eclipse.org/legal/epl-v10.html
 * 
 * Contributors:
 *     Sebastian Proksch - initial API and implementation
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

    public enum State {
        OK, FAIL
    }
}