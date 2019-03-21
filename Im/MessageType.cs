﻿namespace Sharp.Xmpp.Im
{
    /// <summary>
    /// Defines the possible types for Message stanzas.
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// The message is a single message that is sent outside the context of
        /// a one-to-one conversation or groupchat.
        /// </summary>
        Normal,

        /// <summary>
        /// The message is sent in the context of a one-to-one chat conversation.
        /// </summary>
        Chat,

        /// <summary>
        /// An error has occurred related to a previous message sent by the sender.
        /// </summary>
        Error,

        /// <summary>
        /// The message is sent in the context of a multi-user chat environment.
        /// </summary>
        Groupchat,

        /// <summary>
        /// The message is generated by an automated service that delivers or
        /// broadcasts content.
        /// </summary>
        Headline,

        /// <summary>
        /// The message is sent in the context of a management - Used in Rainbow Context
        /// </summary>
        Management
    }
}