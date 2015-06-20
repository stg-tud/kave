/*
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
 *    - Sven Amann
 */

using System;

namespace KaVE.VS.FeedbackGenerator.MessageBus
{
    /// <summary>
    ///     A bus for exchanging messages.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Note that the bus is not subtyping-sensitive, i.e., you cannot subscribe for a supertype to receive messages of
    ///         a subtype and you will not receive messages that are of a subtype if they are published as an instance of a
    ///         supertype.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     You can subscribe for messages from a bus like this:
    ///     <code>
    /// IMessageBus bus = ...;
    /// bus.Subscribe&lt;string&gt;(msg => /* do something with the message */);
    /// </code>
    /// </example>
    /// <example>
    ///     You can send messages over a message bus like this:
    ///     <code>
    /// IMessageBus bus = ...;
    /// bus.Publish&lt;string&gt;("Hello World!");
    /// </code>
    /// </example>
    public interface IMessageBus
    {
        /// <summary>
        ///     Send messages to the bus.
        /// </summary>
        /// <typeparam name="TMessage">
        ///     The type of message you want to publish. Subscribers need to specify the exact same type to
        ///     receive messages. Subtyping is not considered.
        /// </typeparam>
        /// <param name="message">The message to send.</param>
        void Publish<TMessage>(TMessage message) where TMessage : class;

        /// <summary>
        ///     Subscribe to a bus to receive messages of one specific type.
        /// </summary>
        /// <typeparam name="TMessage">
        ///     The type of message you want to receive. You will receive messages that have been published
        ///     with exactly this type, exclusively.
        /// </typeparam>
        /// <param name="action">The action to perform on received messages.</param>
        /// <param name="filter">A message filter. Only messages that pass this filter are handed to the action.</param>
        void Subscribe<TMessage>(Action<TMessage> action, Func<TMessage, bool> filter = null) where TMessage : class;
    }
}