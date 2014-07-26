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

import static org.junit.Assert.assertArrayEquals;
import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertTrue;
import kave.Result.State;

import org.junit.Test;

public class ResultTest {

    private static final String SOME_STRING = "a";

    @Test
    public void equalsAndHashCode() {
        Result a = new Result();
        Result b = new Result();
        assertEquals(a, b);
        assertTrue(a.hashCode() == b.hashCode());
    }

    @Test
    public void ok_2() {
        Result actual = Result.ok();
        Result expected = new Result();
        expected.status = State.OK;
        expected.message = null;
        assertEquals(expected, actual);
    }

    @Test
    public void fail() {
        Result actual = Result.fail(SOME_STRING);
        Result expected = new Result();
        expected.status = State.FAIL;
        expected.message = SOME_STRING;
        assertEquals(expected, actual);
    }

    @Test
    public void fail_2() {
        Result actual = Result.fail(mockThrowable());
        Result expected = new Result();
        expected.status = State.FAIL;
        expected.message = getFailString();
        assertEquals(expected, actual);
    }

    @Test
    public void argumentIsNull() {
        Object o = null;
        Result actual = Result.fail("", o);
        assertEquals("", actual.message);
    }

    @Test
    public void argumentIsEmptyArray() {
        Object[] o = new Object[0];
        Result actual = Result.fail("", o);
        assertEquals("", actual.message);
    }

    @Test
    public void stringIsFormatted() {
        Result actual = Result.fail("a %s", "b");
        assertEquals("a b", actual.message);
    }

    private String getFailString() {
        return "java.lang.RuntimeException: MESSAGE<br />\n\tat CLASS.METHOD1(Unknown Source)<br />\n\tat CLASS.METHOD2(Unknown Source)<br />\n";
    }

    private Throwable mockThrowable() {
        StackTraceElement[] es = new StackTraceElement[2];
        es[0] = mockStacktraceElement(1);
        es[1] = mockStacktraceElement(2);

        Throwable t = new RuntimeException("MESSAGE");
        t.setStackTrace(es);

        return t;
    }

    private StackTraceElement mockStacktraceElement(int id) {
        return new StackTraceElement("CLASS", "METHOD" + id, null, 0);
    }

    @Test
    public void enumCoverage() {
        State[] actuals = Result.State.values();
        State[] expecteds = new State[] { State.OK, State.FAIL };
        assertArrayEquals(expecteds, actuals);

        State actual = Result.State.valueOf("OK");
        State expected = State.OK;
        assertEquals(expected, actual);
    }
}