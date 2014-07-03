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
package kave;

import static java.lang.String.format;

import org.apache.commons.lang3.builder.EqualsBuilder;
import org.apache.commons.lang3.builder.HashCodeBuilder;
import org.apache.commons.lang3.exception.ExceptionUtils;

public class Result<T> {

    public State status;
    public String message;
    public T data;

    public static <T> Result<T> fail(String message, Object... args) {
        Result<T> res = new Result<T>();
        res.status = State.FAIL;
        res.message = format(message, args);
        return res;
    }

    public static <T> Result<T> fail(Throwable t) {
    	return fail(ExceptionUtils.getStackTrace(t).replaceAll("\n", "<br />\n"));
    }

    public static <T> Result<T> ok(T data) {
        Result<T> res = new Result<T>();
        res.status = State.OK;
        res.data = data;
        return res;
    }

    public static Result<Void> ok() {
        Result<Void> res = new Result<Void>();
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