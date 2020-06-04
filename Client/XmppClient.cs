﻿using Sharp.Xmpp.Extensions;
using Sharp.Xmpp.Im;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Xml;

namespace Sharp.Xmpp.Client
{
    /// <summary>
    /// Implements an XMPP client providing basic instant messaging (IM) and
    /// presence functionality as well as various XMPP extension functionality.
    /// </summary>
    /// <remarks>
    /// This provides most of the functionality exposed by the XmppIm class but
    /// simplifies some of the more complicated aspects such as privacy lists and
    /// roster management. It also implements various XMPP protocol extensions.
    /// </remarks>
    public class XmppClient : IDisposable
    {
        /// <summary>
        /// True if the instance has been disposed of.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// The instance of the XmppIm class used for implementing the basic messaging
        /// and presence functionality.
        /// </summary>
        private XmppIm im;

        /// <summary>
        /// Provides access to the 'Software Version' XMPP extension functionality.
        /// </summary>
        private SoftwareVersion version;

        /// <summary>
        /// Provides access to the 'Service Discovery' XMPP extension functionality.
        /// </summary>
        private ServiceDiscovery sdisco;

        /// <summary>
        /// Provides access to the 'Entity Capabilities' XMPP extension functionality.
        /// </summary>
        private EntityCapabilities ecapa;

        /// <summary>
        /// Provides access to the 'Ping' XMPP extension functionality.
        /// </summary>
        private Ping ping;

        /// <summary>
        /// Provides access to the 'Custom Iq Extension' functionality
        /// </summary>
        private CustomIqExtension cusiqextension;

        /// <summary>
        /// Provides access to the 'Attention' XMPP extension functionality.
        /// </summary>
        private Attention attention;

        /// <summary>
        /// Provides access to the 'Entity Time' XMPP extension functionality.
        /// </summary>
        private EntityTime time;

        /// <summary>
        /// Provides access to the 'Blocking Command' XMPP extension functionality.
        /// </summary>
        private BlockingCommand block;

        /// <summary>
        /// Provides access to the 'Personal Eventing Protocol' extension.
        /// </summary>
        private Pep pep;

        /// <summary>
        /// Provides access to the 'User Tune' XMPP extension functionality.
        /// </summary>
        private UserTune userTune;

        /// <summary>
        /// Provides access to the "Multi-User Chat" XMPP extension functionality.
        /// </summary>
        private MultiUserChat groupChat;

#if WINDOWSPLATFORM
		/// <summary>
		/// Provides access to the 'User Avatar' XMPP extension functionality.
		/// </summary>
		UserAvatar userAvatar;
#endif

        /// <summary>
        /// Provides access to the 'User Mood' XMPP extension functionality.
        /// </summary>
        private UserMood userMood;

        /// <summary>
        /// Provides access to the 'Data Forms' XMPP extension functionality.
        /// </summary>
        private DataForms dataForms;

        /// <summary>
        /// Provides access to the 'Feature Negotiation' XMPP extension.
        /// </summary>
        private FeatureNegotiation featureNegotiation;

        /// <summary>
        /// Provides access to the 'Stream Initiation' XMPP extension.
        /// </summary>
        private StreamInitiation streamInitiation;

        /// <summary>
        /// Provides access to the 'SI File Transfer' XMPP extension.
        /// </summary>
        private SIFileTransfer siFileTransfer;

        /// <summary>
        /// Provides access to the 'In-Band Bytestreams' XMPP extension.
        /// </summary>
        private InBandBytestreams inBandBytestreams;

        /// <summary>
        /// Provides access to the 'User Activity' XMPP extension.
        /// </summary>
        private UserActivity userActivity;

        /// <summary>
        /// Provides access to the 'Socks5 Bytestreams' XMPP extension.
        /// </summary>
        private Socks5Bytestreams socks5Bytestreams;

        /// <summary>
        /// Provides access to the 'Server IP Check' XMPP extension.
        /// </summary>
        private ServerIpCheck serverIpCheck;

        /// <summary>
        /// Provides access to the 'In-Band Registration' XMPP extension.
        /// </summary>
        private InBandRegistration inBandRegistration;

        /// <summary>
        /// Provides access to the 'Chat State Nofitications' XMPP extension.
        /// </summary>
        private ChatStateNotifications chatStateNotifications;

        /// <summary>
        /// Provides access to the 'Bits of Binary' XMPP extension.
        /// </summary>
        private BitsOfBinary bitsOfBinary;

        /// <summary>
        /// Provides vcard Based Avatar functionality
        /// </summary>
        private VCardAvatars vcardAvatars;

        /// <summary>
        /// Provides message delivery receipts functionality
        /// </summary>
        private MessageDeliveryReceipts msgDeliveryReceipt;

        /// <summary>
        /// Provides the Message Carbons extension
        /// </summary>
        private MessageCarbons messageCarbons;

        /// <summary>
        /// Provides the MaM extension
        /// </summary>
        private MessageArchiveManagment mam;

        /// <summary>
        /// Provides the Configuration extension
        /// </summary>
        private Configuration configuration;

        /// <summary>
        /// Provides the Conference extension
        /// </summary>
        private Conference conference;

        /// <summary>
        /// Provides the CallLog extension
        /// </summary>
        private CallLog callLog;

        /// <summary>
        /// Provides the Cap extension (Common Alert Protocol)
        /// </summary>
        private Cap cap;

        /// <summary>
        /// Provides the CallService extension
        /// </summary>
        private CallService callService;

        public IPEndPoint IPEndPoint
        {
            get
            {
                return im.IPEndPoint;
            }

            set
            {
                im.IPEndPoint = value;
            }
        }

        public Uri Proxy
        {
            get
            {
                return im.Proxy;
            }

            set
            {
                im.Proxy = value;
            }
        }

        /// <summary>
        /// Is web socket used - false by default
        /// </summary>
        public bool UseWebSocket
        {
            get
            {
                return im.UseWebSocket;
            }

            set
            {
                im.UseWebSocket = value;
            }
        }

        /// <summary>
        /// URI to use for web socket connection
        /// </summary>
        public string WebSocketUri
        {
            get
            {
                return im.WebSocketUri;
            }

            set
            {
                im.WebSocketUri = value;
            }
        }

        /// <summary>
        /// The hostname of the XMPP server to connect to.
        /// </summary>
        public string Hostname
        {
            get
            {
                return im.Hostname;
            }

            set
            {
                im.Hostname = value;
            }
        }

        /// <summary>
        /// The port number of the XMPP service of the server.
        /// </summary>
        public int Port
        {
            get
            {
                return im.Port;
            }

            set
            {
                im.Port = value;
            }
        }

        /// <summary>
        /// The username with which to authenticate. In XMPP jargon this is known
        /// as the 'node' part of the JID.
        /// </summary>
        public string Username
        {
            get
            {
                return im.Username;
            }

            set
            {
                im.Username = value;
            }
        }

        /// <summary>
        /// The password with which to authenticate.
        /// </summary>
        public string Password
        {
            get
            {
                return im.Password;
            }

            set
            {
                im.Password = value;
            }
        }

        /// <summary>
        /// If true the session will be TLS/SSL-encrypted if the server supports it.
        /// </summary>
        public bool Tls
        {
            get
            {
                return im.Tls;
            }

            set
            {
                im.Tls = value;
            }
        }

        /// <summary>
        /// A delegate used for verifying the remote Secure Sockets Layer (SSL)
        /// certificate which is used for authentication.
        /// </summary>
        public RemoteCertificateValidationCallback Validate
        {
            get
            {
                return im.Validate;
            }

            set
            {
                im.Validate = value;
            }
        }

        /// <summary>
        /// Determines whether the session with the server is TLS/SSL encrypted.
        /// </summary>
        public bool IsEncrypted
        {
            get
            {
                return im.IsEncrypted;
            }
        }

        /// <summary>
        /// The address of the Xmpp entity.
        /// </summary>
        public Jid Jid
        {
            get
            {
                return im.Jid;
            }
        }

        /// <summary>
        /// Determines whether the instance is connected to the XMPP server.
        /// </summary>
        public bool Connected
        {
            get
            {
                if (im != null)
                {
                    return im.Connected;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Determines whether the instance has been authenticated.
        /// </summary>
        public bool Authenticated
        {
            get
            {
                return im.Authenticated;
            }
        }

        /// <summary>
        /// The default IQ Set Time out in Milliseconds. -1 means no timeout
        /// </summary>
        public int DefaultTimeOut
        {
            get { return im.DefaultTimeOut; }
            set { im.DefaultTimeOut = value; }
        }

        /// <summary>
        /// If true prints XML stanzas
        /// </summary>
        public bool DebugStanzas
        {
            get { return im.DebugStanzas; }
            set { im.DebugStanzas = value; }
        }

        /// <summary>
        /// Contains settings for configuring file-transfer options.
        /// </summary>
        public FileTransferSettings FileTransferSettings
        {
            get;
            private set;
        }

        /// <summary>
        /// The underlying XmppIm instance.
        /// </summary>
        public XmppIm Im
        {
            get
            {
                return im;
            }
        }

        /// <summary>
        /// A callback method to invoke when a request for a subscription is received
        /// from another XMPP user.
        /// </summary>
        /// <include file='Examples.xml' path='S22/Xmpp/Client/XmppClient[@name="SubscriptionRequest"]/*'/>
        public SubscriptionRequest SubscriptionRequest
        {
            get
            {
                return im.SubscriptionRequest;
            }

            set
            {
                im.SubscriptionRequest = value;
            }
        }

        /// <summary>
        /// A callback method to invoke when a request for voice is received
        /// from another XMPP user.
        /// </summary>
        public RegistrationCallback VoiceRequestedInGroupChat
        {
            get
            {
                return groupChat.VoiceRequested;
            }

            set
            {
                groupChat.VoiceRequested = value;
            }
        }

        /// <summary>
        /// The event that is raised when the connection status with the server is modified
        /// received.
        /// </summary>
        public event EventHandler<Sharp.Xmpp.Core.ConnectionStatusEventArgs> ConnectionStatus
        {
            add
            {
                im.ConnectionStatus += value;
            }
            remove
            {
                im.ConnectionStatus -= value;
            }
        }
        /// <summary>
        /// The event that is raised when a status notification has been received.
        /// </summary>
        public event EventHandler<StatusEventArgs> StatusChanged
        {
            add
            {
                im.Status += value;
            }
            remove
            {
                im.Status -= value;
            }
        }

        /// <summary>
        /// The event that is raised when a mood notification has been received.
        /// </summary>
        public event EventHandler<MoodChangedEventArgs> MoodChanged
        {
            add
            {
                userMood.MoodChanged += value;
            }
            remove
            {
                userMood.MoodChanged -= value;
            }
        }

        /// <summary>
        /// The event that is raised when an activity notification has been received.
        /// </summary>
        public event EventHandler<ActivityChangedEventArgs> ActivityChanged
        {
            add
            {
                userActivity.ActivityChanged += value;
            }
            remove
            {
                userActivity.ActivityChanged -= value;
            }
        }

        /// <summary>
        /// The event that is raised when a VCard has been updated.
        /// </summary>
        public event EventHandler<VCardChangedEventArgs> VCardChanged
        {
            add
            {
                vcardAvatars.VCardChanged += value;
            }
            remove
            {
                vcardAvatars.VCardChanged -= value;
            }
        }

#if WINDOWSPLATFORM
		/// <summary>
		/// The event that is raised when a contact has updated his or her avatar.
		/// </summary>
		public event EventHandler<AvatarChangedEventArgs> AvatarChanged {
			add {
				userAvatar.AvatarChanged += value;
			}
			remove {
				userAvatar.AvatarChanged -= value;
			}
		}
#endif

        /// <summary>
        /// The event that is raised when a contact has published tune information.
        /// </summary>
        public event EventHandler<TuneEventArgs> Tune
        {
            add
            {
                userTune.Tune += value;
            }
            remove
            {
                userTune.Tune -= value;
            }
        }

        /// <summary>
        /// The event that is raised when a chat message is received.
        /// </summary>
        public event EventHandler<Im.MessageEventArgs> Message
        {
            add
            {
                im.Message += value;
            }
            remove
            {
                im.Message -= value;
            }
        }

        /// <summary>
        /// The event that is raised when the subject is changed in a group chat.
        /// </summary>
        public event EventHandler<Im.MessageEventArgs> GroupChatSubjectChanged
        {
            add
            {
                groupChat.SubjectChanged += value;
            }
            remove
            {
                groupChat.SubjectChanged -= value;
            }
        }

        /// <summary>
        /// The event that is raised when a participant's presence is changed in a group chat.
        /// </summary>
        public event EventHandler<GroupPresenceEventArgs> GroupPresenceChanged
        {
            add
            {
                groupChat.PrescenceChanged += value;
            }
            remove
            {
                groupChat.PrescenceChanged -= value;
            }
        }

        /// <summary>
        /// The event that is raised when an invite to a group chat is received.
        /// </summary>
        public event EventHandler<GroupInviteEventArgs> GroupInviteReceived
        {
            add
            {
                groupChat.InviteReceived += value;
            }
            remove
            {
                groupChat.InviteReceived -= value;
            }
        }

        /// <summary>
        /// The event that is raised when an invite to a group chat is declined.
        /// </summary>
        public event EventHandler<GroupInviteDeclinedEventArgs> GroupInviteDeclined
        {
            add
            {
                groupChat.InviteWasDeclined += value;
            }
            remove
            {
                groupChat.InviteWasDeclined -= value;
            }
        }

        /// <summary>
        /// The event that is raised when the server responds with an error in relation to a group chat.
        /// </summary>
        public event EventHandler<GroupErrorEventArgs> GroupMucError
        {
            add
            {
                groupChat.MucErrorResponse += value;
            }
            remove
            {
                groupChat.MucErrorResponse -= value;
            }
        }

        /// <summary>
        /// The event that is raised periodically for every file-transfer operation to
        /// inform subscribers of the progress of the operation.
        /// </summary>
        public event EventHandler<FileTransferProgressEventArgs> FileTransferProgress
        {
            add
            {
                siFileTransfer.FileTransferProgress += value;
            }
            remove
            {
                siFileTransfer.FileTransferProgress -= value;
            }
        }

        /// <summary>
        /// The event that is raised when an on-going file-transfer has been aborted
        /// prematurely, either due to cancellation or error.
        /// </summary>
        public event EventHandler<FileTransferAbortedEventArgs> FileTransferAborted
        {
            add
            {
                siFileTransfer.FileTransferAborted += value;
            }
            remove
            {
                siFileTransfer.FileTransferAborted -= value;
            }
        }

        /// <summary>
        /// The event that is raised when the chat-state of an XMPP entity has
        /// changed.
        /// </summary>
        public event EventHandler<ChatStateChangedEventArgs> ChatStateChanged
        {
            add
            {
                chatStateNotifications.ChatStateChanged += value;
            }
            remove
            {
                chatStateNotifications.ChatStateChanged -= value;
            }
        }

        /// <summary>
        /// The event that is raised when the roster of the user has been updated,
        /// i.e. a contact has been added, removed or updated.
        /// </summary>
        public event EventHandler<RosterUpdatedEventArgs> RosterUpdated
        {
            add
            {
                im.RosterUpdated += value;
            }
            remove
            {
                im.RosterUpdated -= value;
            }
        }

        /// <summary>
        /// The event that is raised when a user or resource has unsubscribed from
        /// receiving presence notifications of the JID associated with this instance.
        /// </summary>
        public event EventHandler<UnsubscribedEventArgs> Unsubscribed
        {
            add
            {
                im.Unsubscribed += value;
            }
            remove
            {
                im.Unsubscribed -= value;
            }
        }

        /// <summary>
        /// The event that is raised when a subscription request made by the JID
        /// associated with this instance has been approved.
        /// </summary>
        public event EventHandler<SubscriptionApprovedEventArgs> SubscriptionApproved
        {
            add
            {
                im.SubscriptionApproved += value;
            }
            remove
            {
                im.SubscriptionApproved -= value;
            }
        }

        /// <summary>
        /// The event that is raised when a subscription request made by the JID
        /// associated with this instance has been refused.
        /// </summary>
        public event EventHandler<SubscriptionRefusedEventArgs> SubscriptionRefused
        {
            add
            {
                im.SubscriptionRefused += value;
            }
            remove
            {
                im.SubscriptionRefused -= value;
            }
        }

        /// <summary>
        /// The event that is raised when an unrecoverable error condition occurs.
        /// </summary>
        public event EventHandler<Im.ErrorEventArgs> Error
        {
            add
            {
                im.Error += value;
            }
            remove
            {
                im.Error -= value;
            }
        }

        /// <summary>
        /// The event that is raised when an user invitation occurs
        /// </summary>
        public event EventHandler<UserInvitationEventArgs> UserInvitation
        {
            add
            {
                configuration.UserInvitation += value;
            }
            remove
            {
                configuration.UserInvitation -= value;
            }
        }

        /// <summary>
        /// The event that is raised when an conversation has been created / updated / deleted
        /// </summary>
        public event EventHandler<ConversationManagementEventArgs> ConversationManagement
        {
            add
            {
                configuration.ConversationManagement += value;
            }
            remove
            {
                configuration.ConversationManagement -= value;
            }
        }

        /// <summary>
        /// The event that is raised when an alert message is received
        /// </summary>
        public event EventHandler<Sharp.Xmpp.Im.MessageEventArgs> AlertMessage
        {
            add
            {
                cap.AlertMessage += value;
            }
            remove
            {
                cap.AlertMessage -= value;
            }
        }

        /// <summary>
        /// The event that is raised when  call log item(s) has been deleted
        /// </summary>
        public event EventHandler<CallLogItemDeletedEventArgs> CallLogItemsDeleted
        {
            add
            {
                callLog.CallLogItemsDeleted += value;
            }
            remove
            {
                callLog.CallLogItemsDeleted -= value;
            }
        }

        /// <summary>
        /// The event that is raised when an call log item has been read
        /// </summary>
        public event EventHandler<CallLogReadEventArgs> CallLogRead
        {
            add
            {
                callLog.CallLogRead += value;
            }
            remove
            {
                callLog.CallLogRead -= value;
            }
        }

        /// <summary>
        /// The event that is raised when a call log item has been added
        /// </summary>
        public event EventHandler<CallLogItemEventArgs> CallLogItemAdded
        {
            add
            {
                callLog.CallLogItemAdded += value;
            }
            remove
            {
                callLog.CallLogItemAdded -= value;
            }
        }

        /// <summary>
        /// The event that is raised when a call log item has been retrieved
        /// </summary>
        public event EventHandler<CallLogItemEventArgs> CallLogItemRetrieved
        {
            add
            {
                callLog.CallLogItemRetrieved += value;
            }
            remove
            {
                callLog.CallLogItemRetrieved -= value;
            }
        }

        /// <summary>
        /// The event raised when we received the result of call log entries
        /// </summary>
        public event EventHandler<CallLogResultEventArgs> CallLogResult
        {
            add
            {
                callLog.CallLogResult += value;
            }
            remove
            {
                callLog.CallLogResult -= value;
            }
        }

        /// <summary>
        /// The event raised when the call forward has been updated
        /// </summary>
        public event EventHandler<CallForwardEventArgs> CallForwardUpdated
        {
            add
            {
                callService.CallForwardUpdated += value;
            }
            remove
            {
                callService.CallForwardUpdated -= value;
            }
        }

        /// <summary>
        /// The event raised when the nomadic status has been updated
        /// </summary>
        public event EventHandler<NomadicEventArgs> NomadicUpdated
        {
            add
            {
                callService.NomadicUpdated += value;
            }
            remove
            {
                callService.NomadicUpdated -= value;
            }
        }

        /// <summary>
        /// The event raised when the PBX Agent info is updated
        /// </summary>
        public event EventHandler<PbxAgentInfoEventArgs> PbxAgentInfoUpdated
        {
            add
            {
                callService.PbxAgentInfoUpdated += value;
            }
            remove
            {
                callService.PbxAgentInfoUpdated += value;
            }
        }

        /// <summary>
        /// The event that is raised when voice messages are updated
        /// </summary>
        public event EventHandler<VoiceMessagesEventArgs> VoiceMessagesUpdated
        {
            add
            {
                callService.VoiceMessagesUpdated += value;
            }
            remove
            {
                callService.VoiceMessagesUpdated += value;
            }
        }

        /// <summary>
        /// The event that is raised when a call service message not specifically managed is received
        /// </summary>
        public event EventHandler<Sharp.Xmpp.Extensions.MessageEventArgs> CallServiceMessageReceived
        {
            add
            {
                callService.MessageReceived += value;
            }
            remove
            {
                callService.MessageReceived += value;
            }
        }

        /// <summary>
        /// The event that is raised when we asked and have PBX calls in progress
        /// </summary>
        public event EventHandler<Sharp.Xmpp.Extensions.MessageEventArgs> PBXCallsInProgress
        {
            add
            {
                callService.PBXCallsInProgress += value;
            }
            remove
            {
                callService.PBXCallsInProgress += value;
            }
        }

        /// <summary>
        /// The event that is raised when an conversation has been created / updated / deleted
        /// </summary>
        public event EventHandler<FavoriteManagementEventArgs> FavoriteManagement
        {
            add
            {
                configuration.FavoriteManagement += value;
            }
            remove
            {
                configuration.FavoriteManagement -= value;
            }
        }


        /// <summary>
        /// The event that is raised when an room has been created / updated / deleted
        /// </summary>
        public event EventHandler<RoomManagementEventArgs> RoomManagement
        {
            add
            {
                configuration.RoomManagement += value;
            }
            remove
            {
                configuration.RoomManagement -= value;
            }
        }

        /// <summary>
        /// The event that is raised when a user is invited in a room
        /// </summary>
        public event EventHandler<RoomInvitationEventArgs> RoomInvitation
        {
            add
            {
                configuration.RoomInvitation += value;
            }
            remove
            {
                configuration.RoomInvitation -= value;
            }
        }

        /// <summary>
        /// The event that is raised when a voice mail has been created / deleted
        /// </summary>
        //public event EventHandler<VoiceMailManagementEventArgs> VoiceMailManagement
        //{
        //    add
        //    {
        //        configuration.VoiceMailManagement += value;
        //    }
        //    remove
        //    {
        //        configuration.VoiceMailManagement -= value;
        //    }
        //}

        /// <summary>
        /// The event that is raised when a file has been created / deleted / updated
        /// </summary>
        public event EventHandler<FileManagementEventArgs> FileManagement
        {
            add
            {
                configuration.FileManagement += value;
            }
            remove
            {
                configuration.FileManagement -= value;
            }
        }

        /// <summary>
        /// The event that is raised when a conference has been updated
        /// </summary>
        public event EventHandler<Sharp.Xmpp.Extensions.MessageEventArgs> ConferenceUpdated
        {
            add
            {
                conference.ConferenceUpdated += value;
            }
            remove
            {
                conference.ConferenceUpdated -= value;
            }
        }


        /// <summary>
        /// The event that is raised when we have ingo about image file
        /// </summary>
        public event EventHandler<ThumbnailEventArgs> ThumbnailManagement
        {
            add
            {
                configuration.ThumbnailManagement += value;
            }
            remove
            {
                configuration.ThumbnailManagement -= value;
            }
        }

        /// <summary>
        /// The event raised about user role/type updates in channel: subscribe / unsubscribe / add / remove / update
        /// </summary>
        public event EventHandler<ChannelManagementEventArgs> ChannelManagement
        {
            add
            {
                configuration.ChannelManagement += value;
            }
            remove
            {
                configuration.ChannelManagement -= value;
            }
        }

        /// <summary>
        /// The event raised when a ChannelItem is created, updated, deleted
        /// </summary>
        public event EventHandler<Sharp.Xmpp.Extensions.MessageEventArgs> ChanneItemManagement
        {
            add
            {
                configuration.ChanneItemManagement += value;
            }
            remove
            {
                configuration.ChanneItemManagement -= value;
            }
        }

        /// <summary>
        /// The event raised when a Group is created, updated, deleted but alos when a member is added / remove in a group
        /// </summary>
        public event EventHandler<Sharp.Xmpp.Extensions.MessageEventArgs> GroupManagement
        {
            add
            {
                configuration.GroupManagement += value;
            }
            remove
            {
                configuration.GroupManagement -= value;
            }
        }
        

        /// <summary>
        /// The event that is raised when the current user password has been updated
        /// </summary>
        public event EventHandler<EventArgs> PasswordUpdated
        {
            add
            {
                configuration.PasswordUpdated += value;
            }
            remove
            {
                configuration.PasswordUpdated -= value;
            }
        }

        /// <summary>
        /// The event that is raised when a message delivery is received
        /// </summary>
        public event EventHandler<MessageDeliveryReceivedEventArgs> MessageDeliveryReceived
        {
            add
            {
                msgDeliveryReceipt.MessageDeliveryReceived += value;
            }
            remove
            {
                msgDeliveryReceipt.MessageDeliveryReceived -= value;
            }
        }

        /// <summary>
        /// The event that is raised when all messages for a JID is read
        /// </summary>
        public event EventHandler<JidEventArgs> MessagesAllRead
        {
            add
            {
                msgDeliveryReceipt.MessagesAllRead += value;
            }
            remove
            {
                msgDeliveryReceipt.MessagesAllRead -= value;
            }
        }

        /// <summary>
        /// The event that is raised when a message arcchive has been found
        /// </summary>
        public event EventHandler<MessageArchiveEventArgs> MessageArchiveRetrieved
        {
            add
            {
                mam.MessageArchiveRetrieved += value;
            }
            remove
            {
                mam.MessageArchiveRetrieved -= value;
            }
        }

        public event EventHandler<JidEventArgs> MessagesAllDeleted
        {
            add
            {
                mam.MessagesAllDeleted += value;
            }
            remove
            {
                mam.MessagesAllDeleted -= value;
            }
        }
        


        /// <summary>
        /// The event that is raised when a result is donrecevied after asking list of messages archive
        /// </summary>
        public event EventHandler<MessageArchiveManagementResultEventArgs> MessageArchiveManagementResult
        {
            add
            {
                mam.MessageArchiveManagementResult += value;
            }
            remove
            {
                mam.MessageArchiveManagementResult -= value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the XmppClient class.
        /// </summary>
        /// <param name="address">The XMPP server IP address.</param>
        /// <param name="hostname">The hostname of the XMPP server to connect to.</param>
        /// <param name="username">The username with which to authenticate. In XMPP jargon
        /// this is known as the 'node' part of the JID.</param>
        /// <param name="password">The password with which to authenticate.</param>
        /// <param name="port">The port number of the XMPP service of the server.</param>
        /// <param name="tls">If true the session will be TLS/SSL-encrypted if the server
        /// supports TLS/SSL-encryption.</param>
        /// <param name="validate">A delegate used for verifying the remote Secure Sockets
        /// Layer (SSL) certificate which is used for authentication. Can be null if not
        /// needed.</param>
        /// <exception cref="ArgumentNullException">The hostname parameter or the
        /// username parameter or the password parameter is null.</exception>
        /// <exception cref="ArgumentException">The hostname parameter or the username
        /// parameter is the empty string.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The value of the port parameter
        /// is not a valid port number.</exception>
        /// <remarks>Use this constructor if you wish to connect to an XMPP server using
        /// an existing set of user credentials.</remarks>
        public XmppClient(string address, string hostname, string username, string password,
            int port = 5222, bool tls = true, RemoteCertificateValidationCallback validate = null)
        {
            im = new XmppIm(address, hostname, username, password, port, tls, validate);
            // Initialize the various extension modules.
            LoadExtensions();
        }

        /// <summary>
        /// Initializes a new instance of the XmppClient class.
        /// </summary>
        /// <param name="hostname">The hostname of the XMPP server to connect to.</param>
        /// <param name="username">The username with which to authenticate. In XMPP jargon
        /// this is known as the 'node' part of the JID.</param>
        /// <param name="password">The password with which to authenticate.</param>
        /// <param name="port">The port number of the XMPP service of the server.</param>
        /// <param name="tls">If true the session will be TLS/SSL-encrypted if the server
        /// supports TLS/SSL-encryption.</param>
        /// <param name="validate">A delegate used for verifying the remote Secure Sockets
        /// Layer (SSL) certificate which is used for authentication. Can be null if not
        /// needed.</param>
        /// <exception cref="ArgumentNullException">The hostname parameter or the
        /// username parameter or the password parameter is null.</exception>
        /// <exception cref="ArgumentException">The hostname parameter or the username
        /// parameter is the empty string.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The value of the port parameter
        /// is not a valid port number.</exception>
        /// <remarks>Use this constructor if you wish to connect to an XMPP server using
        /// an existing set of user credentials.</remarks>
        public XmppClient(string hostname, string username, string password,
            int port = 5222, bool tls = true, RemoteCertificateValidationCallback validate = null)
        {
            im = new XmppIm(hostname, username, password, port, tls, validate);
            // Initialize the various extension modules.
            LoadExtensions();
        }

        /// <summary>
        /// Initializes a new instance of the XmppClient class.
        /// </summary>
        /// <param name="hostname">The hostname of the XMPP server to connect to.</param>
        /// <param name="port">The port number of the XMPP service of the server.</param>
        /// <param name="tls">If true the session will be TLS/SSL-encrypted if the server
        /// supports TLS/SSL-encryption.</param>
        /// <param name="validate">A delegate used for verifying the remote Secure Sockets
        /// Layer (SSL) certificate which is used for authentication. Can be null if not
        /// needed.</param>
        /// <exception cref="ArgumentNullException">The hostname parameter is
        /// null.</exception>
        /// <exception cref="ArgumentException">The hostname parameter is the empty
        /// string.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The value of the port parameter
        /// is not a valid port number.</exception>
        /// <remarks>Use this constructor if you wish to register an XMPP account using
        /// the in-band account registration process supported by some servers.</remarks>
        public XmppClient(string hostname, int port = 5222, bool tls = true,
            RemoteCertificateValidationCallback validate = null)
        {
            im = new XmppIm(hostname, port, tls, validate);
            LoadExtensions();
        }

        /// <summary>
        /// Establishes a connection to the XMPP server.
        /// </summary>
        /// <param name="resource">The resource identifier to bind with. If this is null,
        /// a resource identifier will be assigned by the server.</param>
        /// <returns>The user's roster (contact list).</returns>
        /// <exception cref="System.Security.Authentication.AuthenticationException">An
        /// authentication error occured while trying to establish a secure connection, or
        /// the provided credentials were rejected by the server, or the server requires
        /// TLS/SSL and the Tls property has been set to false.</exception>
        /// <exception cref="System.IO.IOException">There was a failure while writing to or
        /// reading from the network. If the InnerException is of type SocketExcption, use
        /// the ErrorCode property to obtain the specific socket error code.</exception>
        /// <exception cref="ObjectDisposedException">The XmppClient object has been
        /// disposed.</exception>
        /// <exception cref="XmppException">An XMPP error occurred while negotiating the
        /// XML stream with the server, or resource binding failed, or the initialization
        /// of an XMPP extension failed.</exception>
        public void Connect(string resource = null)
        {
            im.Connect(resource);
        }

        /// <summary>
        /// Authenticates with the XMPP server using the specified username and
        /// password.
        /// </summary>
        /// <param name="username">The username to authenticate with.</param>
        /// <param name="password">The password to authenticate with.</param>
        /// <exception cref="ArgumentNullException">The username parameter or the
        /// password parameter is null.</exception>
        /// <exception cref="System.Security.Authentication.AuthenticationException">
        /// An authentication error occured while trying to establish a secure connection,
        /// or the provided credentials were rejected by the server, or the server requires
        /// TLS/SSL and the Tls property has been set to false.</exception>
        /// <exception cref="IOException">There was a failure while writing to or reading
        /// from the network. If the InnerException is of type SocketExcption, use the
        /// ErrorCode property to obtain the specific socket error code.</exception>
        /// <exception cref="ObjectDisposedException">The XmppIm object has been
        /// disposed.</exception>
        /// <exception cref="XmppException">An XMPP error occurred while negotiating the
        /// XML stream with the server, or resource binding failed, or the initialization
        /// of an XMPP extension failed.</exception>
        public void Authenticate(string username, string password)
        {
            im.Autenticate(username, password);
        }

        /// <summary>
        /// Sends a chat message with the specified content to the specified JID.
        /// </summary>
        /// <param name="to">ID of the message</param>
        /// <param name="to">The JID of the intended recipient.</param>
        /// <param name="body">The content of the message.</param>
        /// <param name="subject">The subject of the message.</param>
        /// <param name="thread">The conversation thread the message belongs to.</param>
        /// <param name="type">The type of the message. Can be one of the values from
        /// the MessagType enumeration.</param>
        /// <param name="language">The language of the XML character data of
        /// the stanza.</param>
        /// <exception cref="ArgumentNullException">The to parameter or the body parameter
        /// is null.</exception>
        /// <exception cref="ArgumentException">The body parameter is the empty
        /// string.</exception>
        /// <exception cref="IOException">There was a failure while writing to or reading
        /// from the network.</exception>
        /// <exception cref="InvalidOperationException">The XmppClient instance is not
        /// connected to a remote host, or the XmppClient instance has not authenticated with
        /// the XMPP server.</exception>
        /// <exception cref="ObjectDisposedException">The XmppClient object has been
        /// disposed.</exception>
        /// <include file='Examples.xml' path='S22/Xmpp/Client/XmppClient[@name="SendMessage-1"]/*'/>
        public void SendMessage(string id, Jid to, string body, string subject = null,
            string thread = null, MessageType type = MessageType.Normal,
            CultureInfo language = null, Dictionary<String, String> oobInfo = null)
        {
            AssertValid();
            to.ThrowIfNull("to");
            body.ThrowIfNull("body");
            im.SendMessage(id, to, body, subject, thread, type, language, oobInfo);
        }

        /// <summary>
        /// Sends a chat message with the specified content to the specified JID.
        /// </summary>
        /// <param name="to">The JID of the intended recipient.</param>
        /// <param name="bodies">A dictionary of message bodies. The dictionary
        /// keys denote the languages of the message bodies and must be valid
        /// ISO 2 letter language codes.</param>
        /// <param name="subjects">A dictionary of message subjects. The dictionary
        /// keys denote the languages of the message subjects and must be valid
        /// ISO 2 letter language codes.</param>
        /// <param name="thread">The conversation thread the message belongs to.</param>
        /// <param name="type">The type of the message. Can be one of the values from
        /// the MessagType enumeration.</param>
        /// <param name="language">The language of the XML character data of
        /// the stanza.</param>
        /// <exception cref="ArgumentNullException">The to parameter or the bodies
        /// parameter is null.</exception>
        /// <exception cref="IOException">There was a failure while writing to or reading
        /// from the network.</exception>
        /// <exception cref="InvalidOperationException">The XmppClient instance is not
        /// connected to a remote host, or the XmppClient instance has not authenticated with
        /// the XMPP server.</exception>
        /// <exception cref="ObjectDisposedException">The XmppClient object has been
        /// disposed.</exception>
        /// <remarks>
        /// An XMPP chat-message may contain multiple subjects and bodies in different
        /// languages. Use this method in order to send a message that contains copies of the
        /// message content in several distinct languages.
        /// </remarks>
        /// <include file='Examples.xml' path='S22/Xmpp/Client/XmppClient[@name="SendMessage-2"]/*'/>
        public void SendMessage(Jid to, IDictionary<string, string> bodies,
            IDictionary<string, string> subjects = null, string thread = null,
            MessageType type = MessageType.Normal, CultureInfo language = null, Dictionary<String, String> oobInfo = null)
        {
            AssertValid();
            to.ThrowIfNull("to");
            bodies.ThrowIfNull("bodies");
            im.SendMessage(to, bodies, subjects, thread, type, language, oobInfo);
        }

        /// <summary>
        /// Sends the specified chat message.
        /// </summary>
        /// <param name="message">The chat message to send.</param>
        /// <exception cref="ArgumentNullException">The message parameter is null.</exception>
        /// <exception cref="IOException">There was a failure while writing to or reading
        /// from the network.</exception>
        /// <exception cref="InvalidOperationException">The XmppClient instance is not
        /// connected to a remote host, or the XmppClient instance has not authenticated with
        /// the XMPP server.</exception>
        /// <exception cref="ObjectDisposedException">The XmppClient object has been
        /// disposed.</exception>
        public void SendMessage(Message message)
        {
            AssertValid();
            message.ThrowIfNull("message");
            im.SendMessage(message);
        }

        public void DeleteCallLog(String callId)
        {
            callLog?.DeleteCallLog(callId);
        }

        public void DeleteCallsLogForContact(String contactJid)
        {
            callLog?.DeleteCallsLogForContact(contactJid);
        }

        public void DeleteAllCallsLog()
        {
            callLog?.DeleteAllCallsLog();
        }

        public void MarkCallLogAsRead(String callId)
        {
            callLog?.MarkCallLogAsRead(callId);
        }

        /// <summary>
        /// Mark a message as read (XEP-0184: Message Delivery Receipts). Must be done only on message of type Chat
        /// </summary>
        /// <param name="jid">the JID who send the message</param>
        /// <param name="messageId">The ID of the message to mark as read</param>
        public void MarkMessageAsRead(Jid jid, string messageId, MessageType messageType)
        {
            Message message = new Message(jid);
            message.Type = messageType;

            XmlElement e = message.Data;

            XmlElement timestamp = e.OwnerDocument.CreateElement("timestamp", "urn:xmpp:receipts");
            timestamp.SetAttribute("value", DateTime.UtcNow.ToString("o"));
            e.AppendChild(timestamp);

            XmlElement received = e.OwnerDocument.CreateElement("received", "urn:xmpp:receipts");
            received.SetAttribute("entity", "client");
            received.SetAttribute("event", "read");
            received.SetAttribute("id", messageId);
            e.AppendChild(received);

            im.SendMessage(message);
        }

        /// <summary>
        /// Mark a message as received (XEP-0184: Message Delivery Receipts). Must be done only on message of type Chat
        /// </summary>
        /// <param name="jid">the JID who send the message</param>
        /// <param name="messageId">The ID of the message to mark as read</param>
        public void MarkMessageAsReceive(Jid jid, string messageId, MessageType messageType)
        {
            Message message = new Message(jid);
            message.Type = messageType;

            XmlElement e = message.Data;

            XmlElement timestamp = e.OwnerDocument.CreateElement("timestamp", "urn:xmpp:receipts");
            timestamp.SetAttribute("value", DateTime.UtcNow.ToString("o"));
            e.AppendChild(timestamp);

            XmlElement received = e.OwnerDocument.CreateElement("received", "urn:xmpp:receipts");
            received.SetAttribute("entity", "client");
            received.SetAttribute("event", "received");
            received.SetAttribute("id", messageId);
            e.AppendChild(received);

            im.SendMessage(message);
        }


        /// <summary>
        /// Sets the chat-state for the conversation with the XMPP user with the
        /// specified JID.
        /// </summary>
        /// <param name="jid">The JID of the XMPP user to set the chat-state
        /// for.</param>
        /// <param name="state">The new chat-state.</param>
        /// <exception cref="ArgumentNullException">The jid parameter is
        /// null.</exception>
        public void SetChatState(Jid jid, MessageType type, ChatState state)
        {
            chatStateNotifications.SetChatState(jid, type, state);
        }

        /// <summary>
        /// Sets the default availability status.
        /// </summary>
        /// <param name="availability">The availability state. Can be one of the
        /// values from the Availability enumeration, however not
        /// Availability.Offline.</param>
        /// <exception cref="ObjectDisposedException">The XmppClient object has been
        /// disposed.</exception>
        public void SetDefaultStatus(Availability availability)
        {
            im.SetDefaultStatus(availability);
        }

        /// <summary>
        /// Sets the availability status.
        /// </summary>
        /// <param name="availability">The availability state. Can be one of the
        /// values from the Availability enumeration, however not
        /// Availability.Offline.</param>
        /// <param name="message">An optional message providing a detailed
        /// description of the availability state.</param>
        /// <param name="priority">Provides a hint for stanza routing.</param>
        /// <param name="language">The language of the description of the
        /// availability state.</param>
        /// <exception cref="ArgumentException">The availability parameter has a
        /// value of Availability.Offline.</exception>
        /// <exception cref="IOException">There was a failure while writing to or reading
        /// from the network.</exception>
        /// <exception cref="InvalidOperationException">The XmppClient instance is not
        /// connected to a remote host, or the XmppClient instance has not authenticated with
        /// the XMPP server.</exception>
        /// <exception cref="ObjectDisposedException">The XmppClient object has been
        /// disposed.</exception>
        public void SetStatus(Availability availability, string message = null,
            sbyte priority = 0, XmlElement elementToAdd = null, CultureInfo language = null)
        {
            AssertValid();
            im.SetStatus(availability, message, priority, elementToAdd, language);
        }

        /// <summary>
        /// Sets the availability status.
        /// </summary>
        /// <param name="availability">The availability state. Can be one of the
        /// values from the Availability enumeration, however not
        /// Availability.Offline.</param>
        /// <param name="messages">A dictionary of messages providing detailed
        /// descriptions of the availability state. The dictionary keys denote
        /// the languages of the messages and must be valid ISO 2 letter language
        /// codes.</param>
        /// <param name="priority">Provides a hint for stanza routing.</param>
        /// <exception cref="ArgumentException">The availability parameter has a
        /// value of Availability.Offline.</exception>
        /// <exception cref="IOException">There was a failure while writing to or reading
        /// from the network.</exception>
        /// <exception cref="InvalidOperationException">The XmppClient instance is not
        /// connected to a remote host, or the XmppClient instance has not authenticated with
        /// the XMPP server.</exception>
        /// <exception cref="ObjectDisposedException">The XmppClient object has been
        /// disposed.</exception>
        public void SetStatus(Availability availability,
            Dictionary<string, string> messages, sbyte priority = 0)
        {
            AssertValid();
            im.SetStatus(availability, messages, priority);
        }

        /// <summary>
        /// Sets the availability status.
        /// </summary>
        /// <param name="status">An instance of the Status class.</param>
        /// <exception cref="ArgumentNullException">The status parameter is null.</exception>
        /// <exception cref="ArgumentException">The Availability property of the status
        /// parameter has a value of Availability.Offline.</exception>
        /// <exception cref="IOException">There was a failure while writing to or reading
        /// from the network.</exception>
        /// <exception cref="InvalidOperationException">The XmppClient instance is not
        /// connected to a remote host, or the XmppClient instance has not authenticated with
        /// the XMPP server.</exception>
        /// <exception cref="ObjectDisposedException">The XmppClient object has been
        /// disposed.</exception>
        public void SetStatus(Status status)
        {
            AssertValid();
            status.ThrowIfNull("status");
            im.SetStatus(status);
        }

        /// <summary>
        /// Retrieves the user's roster (contact list).
        /// </summary>
        /// <returns>The user's roster.</returns>
        /// <remarks>In XMPP jargon, the user's contact list is called a
        /// 'roster'.</remarks>
        /// <exception cref="IOException">There was a failure while writing to or reading
        /// from the network.</exception>
        /// <exception cref="InvalidOperationException">The XmppClient instance is not
        /// connected to a remote host, or the XmppClient instance has not authenticated with
        /// the XMPP server.</exception>
        /// <exception cref="ObjectDisposedException">The XmppClient object has been
        /// disposed.</exception>
        /// <exception cref="XmppErrorException">The server returned an XMPP error code.
        /// Use the Error property of the XmppErrorException to obtain the specific
        /// error condition.</exception>
        /// <exception cref="XmppException">The server returned invalid data or another
        /// unspecified XMPP error occurred.</exception>
        /// <include file='Examples.xml' path='S22/Xmpp/Client/XmppClient[@name="GetRoster"]/*'/>
        public Roster GetRoster()
        {
            AssertValid();
            return im.GetRoster();
        }

        /// <summary>
        /// Adds the contact with the specified JID to the user's roster.
        /// </summary>
        /// <param name="jid">The JID of the contact to add to the user's roster.</param>
        /// <param name="name">The nickname with which to associate the contact.</param>
        /// <param name="groups">An array of groups or categories the new contact
        /// will be added to.</param>
        /// <remarks>This method creates a new item on the user's roster and requests
        /// a subscription from the contact with the specified JID.</remarks>
        /// <exception cref="ArgumentNullException">The jid parameter is null.</exception>
        /// <exception cref="IOException">There was a failure while writing to or reading
        /// from the network.</exception>
        /// <exception cref="InvalidOperationException">The XmppClient instance is not
        /// connected to a remote host, or the XmppClient instance has not authenticated with
        /// the XMPP server.</exception>
        /// <exception cref="ObjectDisposedException">The XmppClient object has been
        /// disposed.</exception>
        /// <exception cref="XmppErrorException">The server returned an XMPP error code.
        /// Use the Error property of the XmppErrorException to obtain the specific
        /// error condition.</exception>
        /// <exception cref="XmppException">The server returned invalid data or another
        /// unspecified XMPP error occurred.</exception>
        public void AddContact(Jid jid, string name = null, params string[] groups)
        {
            AssertValid();
            jid.ThrowIfNull("jid");
            // Create a roster item for the new contact.
            im.AddToRoster(new RosterItem(jid, name, groups));
            // Request a subscription from the contact.
            im.RequestSubscription(jid);
        }

        /// <summary>
        /// Removes the item with the specified JID from the user's roster.
        /// </summary>
        /// <param name="jid">The JID of the roster item to remove.</param>
        /// <exception cref="ArgumentNullException">The jid parameter is null.</exception>
        /// <exception cref="IOException">There was a failure while writing to or reading
        /// from the network.</exception>
        /// <exception cref="InvalidOperationException">The XmppClient instance is not
        /// connected to a remote host, or the XmppClient instance has not authenticated with
        /// the XMPP server.</exception>
        /// <exception cref="ObjectDisposedException">The XmppClient object has been
        /// disposed.</exception>
        /// <exception cref="XmppErrorException">The server returned an XMPP error code.
        /// Use the Error property of the XmppErrorException to obtain the specific
        /// error condition.</exception>
        /// <exception cref="XmppException">The server returned invalid data or another
        /// unspecified XMPP error occurred.</exception>
        public void RemoveContact(Jid jid)
        {
            AssertValid();
            jid.ThrowIfNull("jid");
            // This removes the contact from the user's roster AND also cancels any
            // subscriptions.
            im.RemoveFromRoster(jid);
        }

        /// <summary>
        /// Removes the specified item from the user's roster.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <exception cref="ArgumentNullException">The item parameter is null.</exception>
        /// <exception cref="IOException">There was a failure while writing to or reading
        /// from the network.</exception>
        /// <exception cref="InvalidOperationException">The XmppClient instance is not
        /// connected to a remote host, or the XmppClient instance has not authenticated with
        /// the XMPP server.</exception>
        /// <exception cref="ObjectDisposedException">The XmppClient object has been
        /// disposed.</exception>
        /// <exception cref="XmppErrorException">The server returned an XMPP error code.
        /// Use the Error property of the XmppErrorException to obtain the specific
        /// error condition.</exception>
        /// <exception cref="XmppException">The server returned invalid data or another
        /// unspecified XMPP error occurred.</exception>
        public void RemoveContact(RosterItem item)
        {
            AssertValid();
            item.ThrowIfNull("item");
            im.RemoveFromRoster(item);
        }

#if WINDOWSPLATFORM
        /// <summary>
        /// Publishes the image located at the specified path as the user's avatar.
        /// </summary>
        /// <param name="filePath">The path to the image to publish as the user's
        /// avatar.</param>
        /// <exception cref="ArgumentNullException">The filePath parameter is
        /// null.</exception>
        /// <exception cref="ArgumentException">filePath is a zero-length string,
        /// contains only white space, or contains one or more invalid
        /// characters.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name,
        /// or both exceed the system-defined maximum length. For example, on
        /// Windows-based platforms, paths must be less than 248 characters, and
        /// file names must be less than 260 characters.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is
        /// invalid, (for example, it is on an unmapped drive).</exception>
        /// <exception cref="UnauthorizedAccessException">The path specified is
        /// a directory, or the caller does not have the required
        /// permission.</exception>
        /// <exception cref="FileNotFoundException">The file specified in
        /// filePath was not found.</exception>
        /// <exception cref="NotSupportedException">filePath is in an invalid
        /// format, or the server does not support the 'Personal Eventing
        /// Protocol' extension.</exception>
        /// <exception cref="XmppErrorException">The server returned an XMPP error code.
        /// Use the Error property of the XmppErrorException to obtain the specific
        /// error condition.</exception>
        /// <exception cref="XmppException">The server returned invalid data or another
        /// unspecified XMPP error occurred.</exception>
        /// <exception cref="InvalidOperationException">The XmppClient instance is not
        /// connected to a remote host, or the XmppClient instance has not authenticated with
        /// the XMPP server.</exception>
        /// <exception cref="ObjectDisposedException">The XmppClient object has been
        /// disposed.</exception>
        /// <remarks>
        /// The following file types are supported:
        ///  BMP, GIF, JPEG, PNG and TIFF.
        /// </remarks>
		public void SetAvatar(string filePath) {
			AssertValid();
			filePath.ThrowIfNull("filePath");
			userAvatar.Publish(filePath);
		}
#endif

        /// <summary>
        /// Publishes the image located at the specified path as the user's avatar using vcard based Avatars
        /// </summary>
        /// <param name="filePath">The path to the image to publish as the user's avatar.</param>
        public void SetvCardAvatar(string filePath)
        {
            AssertValid();
            filePath.ThrowIfNull("filePath");

            try
            {
                using (Stream s = File.OpenRead(filePath))
                {
                    vcardAvatars.SetAvatar(s);
                }
            }
            catch (IOException copyError)
            {
                System.Diagnostics.Debug.WriteLine(copyError.Message);
                //Fix??? Should throw a network exception
            }
        }

        /// <summary>
        /// Get the vcard based Avatar of user with Jid
        /// </summary>
        /// <param name="jid">The string jid of the user</param>
        /// <param name="filepath">The filepath where the avatar will be stored</param>
        /// <param name="callback">The action that will be executed after the file has been downloaded</param>
        public void GetvCardAvatar(string jid, string filepath, Action callback)
        {
            AssertValid();
            vcardAvatars.RequestAvatar(new Jid(jid), filepath, callback);
        }

        /// <summary>
        /// Requests a Custom Iq from the XMPP entinty Jid
        /// </summary>
        /// <param name="jid">The XMPP entity to request the custom IQ</param>
        /// <param name="str">The payload string to provide to the Request</param>
        /// <param name="callback">The callback method to call after the Request Result has being received. Included the serialised dat
        /// of the answer to the request</param>
        public void RequestCustomIq(Jid jid, string str, CustomIqRequestDelegate callback = null)
        {
            AssertValid();
            if (callback == null) cusiqextension.RequestCustomIq(jid, str);
            else
                cusiqextension.RequestCustomIqAsync(jid, str, callback);
        }

        public void RequestArchivedMessagesByDate(Jid toJid, Jid fromJid, Jid withJid, string queryId, DateTime startDate, DateTime endDate)
        {
            AssertValid();
            mam.RequestArchivedMessagesByDate(toJid, fromJid, withJid, queryId, startDate, endDate);
        }

        public void RequestArchivedMessages(Jid jid, string queryId, int maxNumber, bool isRoom, string before = null, string after = null)
        {
            AssertValid();
            mam.RequestArchivedMessages(jid, queryId, maxNumber, isRoom, before, after);
        }

        public void DeleteAllArchivedMessages(Jid jid, string queryId, bool isRoom)
        {
            AssertValid();
            mam.DeleteAllArchivedMessages(jid, queryId, isRoom);
        }


        public void RequestCallLogs(string queryId, int maxNumber, string before = null, string after = null)
        {
            AssertValid();
            callLog.RequestCustomIqAsync(queryId, maxNumber, before, after);
        }

        /// <summary>
        /// Ask PBX agent Agent info (iq request)
        /// </summary>
        /// <param name="to">The JID to send the request</param>
        public void AskPbxAgentInfo(String to)
        {
            AssertValid();
            callService.AskPbxAgentInfo(to);
        }

        /// <summary>
        /// Ask then number of voice messages
        /// </summary>
        /// <param name="to">The JID to send the request</param>
        public void AskVoiceMessagesNumber(String to)
        {
            AssertValid();
            callService.AskVoiceMessagesNumber(to);
        }

        /// <summary>
        /// To get PBX calls in progress (if any) of the specified device (MAIN or SECONDARY)
        /// </summary>
        /// <param name="to">The JID to send the request</param>
        /// <param name="onSecondary">To we want info about the SECONDARY device or not</param>
        public void AskPBXCallsInProgress(String to, Boolean onSecondary)
        {
            AssertValid();
            callService.AskPBXCallsInProgress(to, onSecondary);
        }

        /// <summary>
        /// Sets the user's mood to the specified mood value.
        /// </summary>
        /// <param name="mood">A value from the Mood enumeration to set the user's
        /// mood to.</param>
        /// <param name="description">A natural-language description of, or reason
        /// for, the mood.</param>
        /// <exception cref="InvalidOperationException">The XmppClient instance is not
        /// connected to a remote host, or the XmppClient instance has not authenticated with
        /// the XMPP server.</exception>
        /// <exception cref="ObjectDisposedException">The XmppClient object has been
        /// disposed.</exception>
        public void SetMood(Mood mood, string description = null)
        {
            AssertValid();
            userMood.SetMood(mood, description);
        }

        /// <summary>
        /// Sets the user's activity to the specified activity value(s).
        /// </summary>
        /// <param name="activity">A value from the GeneralActivity enumeration to
        /// set the user's general activity to.</param>
        /// <param name="specific">A value from the SpecificActivity enumeration
        /// best describing the user's activity in more detail.</param>
        /// <param name="description">A natural-language description of, or reason
        /// for, the activity.</param>
        /// <exception cref="InvalidOperationException">The XmppClient instance is not
        /// connected to a remote host, or the XmppClient instance has not authenticated with
        /// the XMPP server.</exception>
        /// <exception cref="ObjectDisposedException">The XmppClient object has been
        /// disposed.</exception>
        /// <include file='Examples.xml' path='S22/Xmpp/Client/XmppClient[@name="SetActivity"]/*'/>
        public void SetActivity(GeneralActivity activity, SpecificActivity specific =
            SpecificActivity.Other, string description = null)
        {
            AssertValid();
            userActivity.SetActivity(activity, specific, description);
        }

        /// <summary>
        /// Publishes the specified music information to contacts on the user's
        /// roster.
        /// </summary>
        /// <param name="title">The title of the song or piece.</param>
        /// <param name="artist">The artist or performer of the song or piece.</param>
        /// <param name="track">A unique identifier for the tune; e.g., the track number
        /// within a collection or the specific URI for the object (e.g., a
        /// stream or audio file).</param>
        /// <param name="length">The duration of the song or piece in seconds.</param>
        /// <param name="rating">The user's rating of the song or piece, from 1
        /// (lowest) to 10 (highest).</param>
        /// <param name="source">The collection (e.g., album) or other source
        /// (e.g., a band website that hosts streams or audio files).</param>
        /// <param name="uri">A URI or URL pointing to information about the song,
        /// collection, or artist</param>
        /// <exception cref="NotSupportedException">The server does not support the
        /// 'Personal Eventing Protocol' extension.</exception>
        /// <exception cref="XmppErrorException">The server returned an XMPP error code.
        /// Use the Error property of the XmppErrorException to obtain the specific
        /// error condition.</exception>
        /// <exception cref="XmppException">The server returned invalid data or another
        /// unspecified XMPP error occurred.</exception>
        /// <exception cref="InvalidOperationException">The XmppClient instance is not
        /// connected to a remote host, or the XmppClient instance has not authenticated with
        /// the XMPP server.</exception>
        /// <exception cref="ObjectDisposedException">The XmppClient object has been
        /// disposed.</exception>
        /// <remarks>Publishing no information (i.e. calling Publish without any parameters
        /// is considered a "stop command" to disable publishing).</remarks>
        public void SetTune(string title = null, string artist = null, string track = null,
            int length = 0, int rating = 0, string source = null, string uri = null)
        {
            AssertValid();
            userTune.Publish(title, artist, track, length, rating, source, uri);
        }

        /// <summary>
        /// Publishes the specified music information to contacts on the user's
        /// roster.
        /// </summary>
        /// <param name="tune">The tune information to publish.</param>
        /// <exception cref="ArgumentNullException">The tune parameter is
        /// null.</exception>
        /// <exception cref="NotSupportedException">The server does not support the
        /// 'Personal Eventing Protocol' extension.</exception>
        /// <exception cref="XmppErrorException">The server returned an XMPP error code.
        /// Use the Error property of the XmppErrorException to obtain the specific
        /// error condition.</exception>
        /// <exception cref="XmppException">The server returned invalid data or another
        /// unspecified XMPP error occurred.</exception>
        /// <exception cref="InvalidOperationException">The XmppClient instance is not
        /// connected to a remote host, or the XmppClient instance has not authenticated with
        /// the XMPP server.</exception>
        /// <exception cref="ObjectDisposedException">The XmppClient object has been
        /// disposed.</exception>
        /// <include file='Examples.xml' path='S22/Xmpp/Client/XmppClient[@name="SetTune"]/*'/>
        public void SetTune(TuneInformation tune)
        {
            AssertValid();
            userTune.Publish(tune);
        }

        /// <summary>
        /// A callback method to invoke when a request for a file-transfer is received
        /// from another XMPP user.
        /// </summary>
        public FileTransferRequest FileTransferRequest
        {
            get
            {
                return siFileTransfer.TransferRequest;
            }

            set
            {
                siFileTransfer.TransferRequest = value;
            }
        }

        /// <summary>
        /// A callback method to invoke when a Custom Iq Request is received
        /// from another XMPP user.
        /// </summary>
        public CustomIqRequestDelegate CustomIqDelegate
        {
            get
            {
                return im.CustomIqDelegate;
            }

            set
            {
                im.CustomIqDelegate = value;
            }
        }

        /// <summary>
        /// Offers the specified file to the XMPP user with the specified JID and, if
        /// accepted by the user, transfers the file.
        /// </summary>
        /// <param name="to">The JID of the XMPP user to offer the file to.</param>
        /// <param name="path">The path of the file to transfer.</param>
        /// <param name="cb">a callback method invoked once the other site has
        /// accepted or rejected the file-transfer request.</param>
        /// <param name="description">A description of the file so the receiver can
        /// better understand what is being sent.</param>
        /// <returns>Sid of the file transfer</returns>
        /// <exception cref="ArgumentNullException">The to parameter or the path
        /// parameter is null.</exception>
        /// <exception cref="ArgumentException">path is a zero-length string,
        /// contains only white space, or contains one or more invalid
        /// characters.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name,
        /// or both exceed the system-defined maximum length.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is
        /// invalid, (for example, it is on an unmapped drive).</exception>
        /// <exception cref="UnauthorizedAccessException">path specified a
        /// directory, or the caller does not have the required
        /// permission.</exception>
        /// <exception cref="FileNotFoundException">The file specified in path
        /// was not found.</exception>
        /// <exception cref="NotSupportedException">path is in an invalid
        /// format, or the XMPP entity with the specified JID does not support
        /// the 'SI File Transfer' XMPP extension.</exception>
        /// <exception cref="XmppErrorException">The server or the XMPP entity
        /// with the specified JID returned an XMPP error code. Use the Error
        /// property of the XmppErrorException to obtain the specific error
        /// condition.</exception>
        /// <exception cref="XmppException">The server returned invalid data or
        /// another unspecified XMPP error occurred.</exception>
        /// <exception cref="InvalidOperationException">The XmppClient instance is not
        /// connected to a remote host, or the XmppClient instance has not authenticated with
        /// the XMPP server.</exception>
        /// <exception cref="ObjectDisposedException">The XmppClient object has been
        /// disposed.</exception>
        public string InitiateFileTransfer(Jid to, string path,
            string description = null, Action<bool, FileTransfer> cb = null)
        {
            AssertValid();
            return siFileTransfer.InitiateFileTransfer(to, path, description, cb);
        }

        /// <summary>
        /// Offers the XMPP user with the specified JID the file with the specified
        /// name and, if accepted by the user, transfers the file using the supplied
        /// stream.
        /// </summary>
        /// <param name="to">The JID of the XMPP user to offer the file to.</param>
        /// <param name="stream">The stream to read the file-data from.</param>
        /// <param name="name">The name of the file, as offered to the XMPP user
        /// with the specified JID.</param>
        /// <param name="size">The number of bytes to transfer.</param>
        /// <param name="cb">A callback method invoked once the other site has
        /// accepted or rejected the file-transfer request.</param>
        /// <param name="description">A description of the file so the receiver can
        /// better understand what is being sent.</param>
        /// <returns>The Sid of the file transfer</returns>
        /// <exception cref="ArgumentNullException">The to parameter or the stream
        /// parameter or the name parameter is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The value of the size
        /// parameter is negative.</exception>
        /// <exception cref="NotSupportedException">The XMPP entity with the
        /// specified JID does not support the 'SI File Transfer' XMPP
        /// extension.</exception>
        /// <exception cref="XmppErrorException">The server or the XMPP entity
        /// with the specified JID returned an XMPP error code. Use the Error
        /// property of the XmppErrorException to obtain the specific error
        /// condition.</exception>
        /// <exception cref="XmppException">The server returned invalid data or
        /// another unspecified XMPP error occurred.</exception>
        /// <exception cref="InvalidOperationException">The XmppClient instance is not
        /// connected to a remote host, or the XmppClient instance has not authenticated with
        /// the XMPP server.</exception>
        /// <exception cref="ObjectDisposedException">The XmppClient object has been
        /// disposed.</exception>
        public string InitiateFileTransfer(Jid to, Stream stream, string name, long size,
            string description = null, Action<bool, FileTransfer> cb = null)
        {
            AssertValid();
            return siFileTransfer.InitiateFileTransfer(to, stream, name, size, description, cb);
        }

        /// <summary>
        /// Cancels the specified file-transfer.
        /// </summary>
        /// <param name="transfer">The file-transfer to cancel.</param>
        /// <exception cref="ArgumentNullException">The transfer parameter is
        /// null.</exception>
        /// <exception cref="ArgumentException">The specified transfer instance does
        /// not represent an active data-transfer operation.</exception>
        /// <exception cref="InvalidOperationException">The XmppClient instance is not
        /// connected to a remote host, or the XmppClient instance has not authenticated with
        /// the XMPP server.</exception>
        /// <exception cref="ObjectDisposedException">The XmppClient object has been
        /// disposed.</exception>
        public void CancelFileTransfer(FileTransfer transfer)
        {
            AssertValid();
            transfer.ThrowIfNull("transfer");
            siFileTransfer.CancelFileTransfer(transfer);
        }

        /// <summary>
        /// Cancels the specified file-transfer.
        /// </summary>
        /// <param name="from">From Jid</param>
        /// <param name="sid">Sid</param>
        /// <param name="to">To Jid</param>
        /// <exception cref="ArgumentNullException">The transfer parameter is
        /// null.</exception>
        /// <exception cref="ArgumentException">The specified transfer instance does
        /// not represent an active data-transfer operation.</exception>
        /// <exception cref="InvalidOperationException">The XmppClient instance is not
        /// connected to a remote host, or the XmppClient instance has not authenticated with
        /// the XMPP server.</exception>
        /// <exception cref="ObjectDisposedException">The XmppClient object has been
        /// disposed.</exception>
        public void CancelFileTransfer(string sid, Jid from, Jid to)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("XmppClient CancelFileTransfer, sid {0}, from {1}, to {2}", sid, from.ToString(), to.ToString());
#endif

            AssertValid();
            sid.ThrowIfNullOrEmpty("sid");
            from.ThrowIfNull("from");
            to.ThrowIfNull("to");

            siFileTransfer.CancelFileTransfer(sid, from, to);
        }

        /// <summary>
        /// Initiates in-band registration with the XMPP server in order to register
        /// a new XMPP account.
        /// </summary>
        /// <param name="callback">A callback method invoked to let the user
        /// enter any information required by the server in order to complete the
        /// registration.</param>
        /// <exception cref="ArgumentNullException">The callback parameter is
        /// null.</exception>
        /// <exception cref="NotSupportedException">The XMPP server with does not
        /// support the 'In-Band Registration' XMPP extension.</exception>
        /// <exception cref="XmppErrorException">The server returned an XMPP error code.
        /// Use the Error property of the XmppErrorException to obtain the specific
        /// error condition.</exception>
        /// <exception cref="XmppException">The server returned invalid data or another
        /// unspecified XMPP error occurred.</exception>
        /// <remarks>
        /// See the "Howto: Register an account" guide for a walkthrough on how to
        /// register an XMPP account through the in-band registration process.
        /// </remarks>
        public void Register(RegistrationCallback callback)
        {
            callback.ThrowIfNull("callback");
            inBandRegistration.Register(callback);
        }

        /// <summary>
        /// Retrieves the current time of the XMPP client with the specified JID.
        /// </summary>
        /// <param name="jid">The JID of the user to retrieve the current time
        /// for.</param>
        /// <returns>The current time of the XMPP client with the specified JID.</returns>
        /// <exception cref="ArgumentNullException">The jid parameter is null.</exception>
        /// <exception cref="InvalidOperationException">The XmppClient instance is
        /// not connected to a remote host.</exception>
        /// <exception cref="System.IO.IOException">There was a failure while writing to or
        /// reading from the network.</exception>
        /// <exception cref="NotSupportedException">The XMPP client of the
        /// user with the specified JID does not support the retrieval of the
        /// current time.</exception>
        /// <exception cref="ObjectDisposedException">The XmppClient object
        /// has been disposed.</exception>
        /// <exception cref="XmppErrorException">The server or the XMPP client of
        /// the user with the specified JID returned an XMPP error code. Use the
        /// Error property of the XmppErrorException to obtain the specific error
        /// condition.</exception>
        /// <exception cref="XmppException">The server returned invalid data or another
        /// unspecified XMPP error occurred.</exception>
        public DateTime GetTime(Jid jid)
        {
            AssertValid();
            return time.GetTime(jid);
        }

        /// <summary>
        /// Retrieves the software version of the XMPP client with the specified JID.
        /// </summary>
        /// <param name="jid">The JID of the user to retrieve version information
        /// for.</param>
        /// <returns>An initialized instance of the VersionInformation class providing
        /// the name and version of the XMPP client used by the user with the specified
        /// JID.</returns>
        /// <exception cref="ArgumentNullException">The jid parameter is null.</exception>
        /// <exception cref="InvalidOperationException">The XmppClient instance is not
        /// connected to a remote host, or the XmppCleint instance has not authenticated
        /// with the XMPP server.</exception>
        /// <exception cref="System.IO.IOException">There was a failure while writing to or
        /// reading from the network.</exception>
        /// <exception cref="NotSupportedException">The XMPP client of the
        /// user with the specified JID does not support the retrieval of version
        /// information.</exception>
        /// <exception cref="ObjectDisposedException">The XmppClient object
        /// has been disposed.</exception>
        /// <exception cref="XmppErrorException">The server or the XMPP client of
        /// the user with the specified JID returned an XMPP error code. Use the
        /// Error property of the XmppErrorException to obtain the specific error
        /// condition.</exception>
        /// <exception cref="XmppException">The server returned invalid data or another
        /// unspecified XMPP error occurred.</exception>
        public VersionInformation GetVersion(Jid jid)
        {
            AssertValid();
            return version.GetVersion(jid);
        }

        /// <summary>
        /// Returns an enumerable collection of XMPP features supported by the XMPP
        /// client with the specified JID.
        /// </summary>
        /// <param name="jid">The JID of the XMPP client to retrieve a collection of
        /// supported features for.</param>
        /// <returns>An enumerable collection of XMPP extensions supported by the
        /// XMPP client with the specified JID.</returns>
        /// <exception cref="ArgumentNullException">The jid parameter is null.</exception>
        /// <exception cref="InvalidOperationException">The XmppClient instance is
        /// not connected to a remote host.</exception>
        /// <exception cref="System.IO.IOException">There was a failure while writing to or
        /// reading from the network.</exception>
        /// <exception cref="NotSupportedException">The XMPP client of the
        /// user with the specified JID does not support the retrieval of feature
        /// information.</exception>
        /// <exception cref="ObjectDisposedException">The XmppClient object
        /// has been disposed.</exception>
        /// <exception cref="XmppErrorException">The server or the XMPP client of
        /// the user with the specified JID returned an XMPP error code. Use the
        /// Error property of the XmppErrorException to obtain the specific error
        /// condition.</exception>
        /// <exception cref="XmppException">The server returned invalid data or another
        /// unspecified XMPP error occurred.</exception>
        /// <include file='Examples.xml' path='S22/Xmpp/Client/XmppClient[@name="GetFeatures"]/*'/>
        public IEnumerable<Extension> GetFeatures(Jid jid)
        {
            AssertValid();
            return ecapa.GetExtensions(jid);
        }

        /// <summary>
        /// Buzzes the user with the specified JID in order to get his or her attention.
        /// </summary>
        /// <param name="jid">The JID of the user to buzz.</param>
        /// <param name="message">An optional message to send along with the buzz
        /// notification.</param>
        /// <exception cref="ArgumentNullException">The jid parameter is null.</exception>
        /// <exception cref="InvalidOperationException">The XmppClient instance is
        /// not connected to a remote host.</exception>
        /// <exception cref="System.IO.IOException">There was a failure while writing to or
        /// reading from the network.</exception>
        /// <exception cref="NotSupportedException">The XMPP client of the
        /// user with the specified JID does not support buzzing.</exception>
        /// <exception cref="ObjectDisposedException">The XmppClient object
        /// has been disposed.</exception>
        /// <exception cref="XmppErrorException">The server or the XMPP client of
        /// the user with the specified JID returned an XMPP error code. Use the
        /// Error property of the XmppErrorException to obtain the specific error
        /// condition.</exception>
        /// <exception cref="XmppException">The server returned invalid data or another
        /// unspecified XMPP error occurred.</exception>
        public void Buzz(Jid jid, string message = null)
        {
            AssertValid();
            attention.GetAttention(jid, message);
        }

        /// <summary>
        /// Pings the user with the specified JID.
        /// </summary>
        /// <param name="jid">The JID of the user to ping.</param>
        /// <returns>The time it took to ping the user with the specified
        /// JID.</returns>
        /// <exception cref="ArgumentNullException">The jid parameter is null.</exception>
        /// <exception cref="InvalidOperationException">The XmppClient instance is
        /// not connected to a remote host.</exception>
        /// <exception cref="System.IO.IOException">There was a failure while writing to or
        /// reading from the network.</exception>
        /// <exception cref="NotSupportedException">The XMPP client of the
        /// user with the specified JID does not support the 'Ping' XMPP protocol
        /// extension.</exception>
        /// <exception cref="ObjectDisposedException">The XmppClient object
        /// has been disposed.</exception>
        /// <exception cref="XmppErrorException">The server or the XMPP client of
        /// the user with the specified JID returned an XMPP error code. Use the
        /// Error property of the XmppErrorException to obtain the specific error
        /// condition.</exception>
        /// <exception cref="XmppException">The server returned invalid data or another
        /// unspecified XMPP error occurred.</exception>
        public TimeSpan Ping(Jid jid)
        {
            AssertValid();
            return ping.PingEntity(jid);
        }

        /// <summary>
        /// Blocks all communication to and from the XMPP entity with the specified JID.
        /// </summary>
        /// <param name="jid">The JID of the XMPP entity to block.</param>
        /// <exception cref="ArgumentNullException">The jid parameter is
        /// null.</exception>
        /// <exception cref="NotSupportedException">The server does not support the
        /// 'Blocking Command' extension and does not support privacy-list management.
        /// </exception>
        /// <exception cref="XmppErrorException">The server returned an XMPP error code.
        /// Use the Error property of the XmppErrorException to obtain the specific
        /// error condition.</exception>
        /// <exception cref="XmppException">The server returned invalid data or another
        /// unspecified XMPP error occurred.</exception>
        /// <exception cref="InvalidOperationException">The XmppClient instance is
        /// not connected to a remote host.</exception>
        /// <exception cref="ObjectDisposedException">The XmppClient object
        /// has been disposed.</exception>
        public void Block(Jid jid)
        {
            AssertValid();
            jid.ThrowIfNull("jid");
            // If our server supports the 'Blocking Command' extension, we can just
            // use that.
            if (block.Supported)
                block.Block(jid);
            else
            {
                // Privacy list blocking. If our server doesn't support privacy lists, we're
                // out of luck.
                PrivacyList privacyList = null;
                string name = im.GetDefaultPrivacyList();
                if (name != null)
                    privacyList = im.GetPrivacyList(name);
                // If no default list has been set, look for a 'blocklist' list.
                foreach (var list in im.GetPrivacyLists())
                {
                    if (list.Name == "blocklist")
                        privacyList = list;
                }
                // If 'blocklist' doesn't exist, create it and set it as default.
                if (privacyList == null)
                    privacyList = new PrivacyList("blocklist");
                privacyList.Add(new JidPrivacyRule(jid, false, 0), true);
                // Save the privacy list and activate it.
                im.EditPrivacyList(privacyList);
                im.SetDefaultPrivacyList(privacyList.Name);
                im.SetActivePrivacyList(privacyList.Name);
            }
        }

        /// <summary>
        /// Unblocks all communication to and from the XMPP entity with the specified
        /// JID.
        /// </summary>
        /// <param name="jid">The JID of the XMPP entity to unblock.</param>
        /// <exception cref="ArgumentNullException">The jid parameter is
        /// null.</exception>
        /// <exception cref="NotSupportedException">The server does not support the
        /// 'Blocking Command' extension and does not support privacy-list management.
        /// </exception>
        /// <exception cref="XmppErrorException">The server returned an XMPP error code.
        /// Use the Error property of the XmppErrorException to obtain the specific
        /// error condition.</exception>
        /// <exception cref="XmppException">The server returned invalid data or another
        /// unspecified XMPP error occurred.</exception>
        /// <exception cref="InvalidOperationException">The XmppClient instance is
        /// not connected to a remote host.</exception>
        /// <exception cref="InvalidOperationException">The XmppClient instance is
        /// not connected to a remote host.</exception>
        /// <exception cref="ObjectDisposedException">The XmppClient object
        /// has been disposed.</exception>
        public void Unblock(Jid jid)
        {
            AssertValid();
            jid.ThrowIfNull("jid");
            // If our server supports the 'Blocking Command' extension, we can just
            // use that.
            if (block.Supported)
                block.Unblock(jid);
            else
            {
                // Privacy list blocking. If our server doesn't support privacy lists, we're
                // out of luck.
                PrivacyList privacyList = null;
                string name = im.GetDefaultPrivacyList();
                if (name != null)
                    privacyList = im.GetPrivacyList(name);
                // If no default list has been set, look for a 'blocklist' list.
                foreach (var list in im.GetPrivacyLists())
                {
                    if (list.Name == "blocklist")
                        privacyList = list;
                }
                // No blocklist found.
                if (privacyList == null)
                    return;
                ISet<JidPrivacyRule> set = new HashSet<JidPrivacyRule>();
                foreach (var rule in privacyList)
                {
                    if (rule is JidPrivacyRule)
                    {
                        var jidRule = rule as JidPrivacyRule;
                        if (jidRule.Jid == jid && jidRule.Allow == false)
                            set.Add(jidRule);
                    }
                }
                foreach (var rule in set)
                    privacyList.Remove(rule);
                // Save the privacy list and activate it.
                if (privacyList.Count == 0)
                {
                    im.SetDefaultPrivacyList();
                    im.RemovePrivacyList(privacyList.Name);
                }
                else
                {
                    im.EditPrivacyList(privacyList);
                    im.SetDefaultPrivacyList(privacyList.Name);
                }
            }
        }

        /// <summary>
        /// Returns an enumerable collection of blocked contacts.
        /// </summary>
        /// <returns>An enumerable collection of JIDs which are on the client's
        /// blocklist.</returns>
        /// <exception cref="NotSupportedException">The server does not support the
        /// 'Blocking Command' extension and does not support privacy-list management.
        /// </exception>
        /// <exception cref="XmppErrorException">The server returned an XMPP error code.
        /// Use the Error property of the XmppErrorException to obtain the specific
        /// error condition.</exception>
        /// <exception cref="XmppException">The server returned invalid data or another
        /// unspecified XMPP error occurred.</exception>
        /// <exception cref="InvalidOperationException">The XmppClient instance is
        /// not connected to a remote host.</exception>
        /// <exception cref="ObjectDisposedException">The XmppClient object
        /// has been disposed.</exception>
        public IEnumerable<Jid> GetBlocklist()
        {
            AssertValid();
            if (block.Supported)
                return block.GetBlocklist();
            PrivacyList privacyList = null;
            string name = im.GetDefaultPrivacyList();
            if (name != null)
                privacyList = im.GetPrivacyList(name);
            foreach (var list in im.GetPrivacyLists())
            {
                if (list.Name == "blocklist")
                    privacyList = list;
            }
            var items = new HashSet<Jid>();
            if (privacyList != null)
            {
                foreach (var rule in privacyList)
                {
                    if (rule is JidPrivacyRule)
                        items.Add((rule as JidPrivacyRule).Jid);
                }
            }
            return items;
        }

        /// <summary>
        /// Returns a list of active public chat room messages.
        /// </summary>
        /// <param name="chatService">JID of the chat service (depends on server)</param>
        /// <returns>List of Room JIDs</returns>
        public IEnumerable<RoomInfoBasic> DiscoverRooms(Jid chatService)
        {
            AssertValid();
            return groupChat.DiscoverRooms(chatService);
        }


        /// <summary>
        /// Returns a list of active public chat room messages.
        /// </summary>
        /// <param name="chatRoom">Room Identifier</param>
        /// <returns>Information about room</returns>
        public RoomInfoExtended GetRoomInfo(Jid chatRoom)
        {
            AssertValid();
            return groupChat.GetRoomInfo(chatRoom);
        }

        /// <summary>
        /// Joins or creates new room using the specified room.
        /// </summary>
        /// <param name="chatRoom">Chat room</param>
        /// <param name="nickname">Desired nickname</param>
        /// <param name="password">(Optional) Password</param>
        public void JoinRoom(Jid chatRoom, string nickname, string password = null)
        {
            AssertValid();
            groupChat.JoinRoom(chatRoom, nickname, password);
        }

        /// <summary>
        /// Leaves the specified room.
        /// </summary>
        public void LeaveRoom(Jid chatRoom, string nickname)
        {
            AssertValid();
            groupChat.LeaveRoom(chatRoom, nickname);
        }

        /// <summary>
        /// Sends a request to get X previous messages.
        /// </summary>
        /// <param name="option">How long to look back</param>
        public void GetGroupChatLog(History option)
        {
            AssertValid();
            groupChat.GetMessageLog(option);
        }

        /// <summary>
        /// Requests a list of occupants within the specific room.
        /// </summary>
        public IEnumerable<Occupant> GetRoomAllOccupants(Jid chatRoom)
        {
            AssertValid();
            return groupChat.GetMembers(chatRoom);
        }

        /// <summary>
        /// Requests a list of non-members within the specified room.
        /// </summary>
        public IEnumerable<Occupant> GetRoomStrangers(Jid chatRoom)
        {
            AssertValid();
            return groupChat.GetMembers(chatRoom, Affiliation.None);
        }

        /// <summary>
        /// Requests a list of room members within the specified room.
        /// </summary>
        public IEnumerable<Occupant> GetRoomMembers(Jid chatRoom)
        {
            AssertValid();
            return groupChat.GetMembers(chatRoom, Affiliation.Member);
        }

        /// <summary>
        /// Requests a list of room owners within the specified room.
        /// </summary>
        public IEnumerable<Occupant> GetRoomOwners(Jid chatRoom)
        {
            AssertValid();
            return groupChat.GetMembers(chatRoom, Affiliation.Owner);
        }

        /// <summary>
        /// Requests a list of people banned within the specified room.
        /// </summary>
        public IEnumerable<Occupant> GetRoomBanList(Jid chatRoom)
        {
            AssertValid();
            return groupChat.GetMembers(chatRoom, Affiliation.Outcast);
        }

        /// <summary>
        /// Requests a list of visitors within the specified room.
        /// </summary>
        public IEnumerable<Occupant> GetRoomVisitors(Jid chatRoom)
        {
            AssertValid();
            return groupChat.GetMembers(chatRoom, Role.Visitor);
        }

        /// <summary>
        /// Requests a list of occupants with a voice privileges within the specified room.
        /// </summary>
        public IEnumerable<Occupant> GetRoomVoiceList(Jid chatRoom)
        {
            AssertValid();
            return groupChat.GetMembers(chatRoom, Role.Participant);
        }

        /// <summary>
        /// Requests a list of moderators within the specified room.
        /// </summary>
        public IEnumerable<Occupant> GetRoomModerators(Jid chatRoom)
        {
            AssertValid();
            return groupChat.GetMembers(chatRoom, Role.Moderator);
        }
        
        /// <summary>
        /// Allows moderators to kick an occupant from the room.
        /// </summary>
        /// <param name="chatRoom">chat room</param>
        /// <param name="nickname">user to kick</param>
        /// <param name="reason">reason for kick</param>
        public void KickGroupOccupant(Jid chatRoom, string nickname, string reason = null)
        {
            groupChat.KickOccupant(chatRoom, nickname, reason);
        }

        /// <summary>
        /// Allows a user to modify the configuration of a specified room.
        /// Only "Room Owners" may edit room config.
        /// </summary>
        /// <param name="room">JID of the room.</param>
        /// <param name="callback">Room Configuration callback.</param>
        public void ModifyRoomConfig(Jid room, RegistrationCallback callback)
        {
            groupChat.ModifyRoomConfig(room, callback);
        }

        /// <summary>
        /// Asks the chat service to invite the specified user to the chat room you specify.
        /// </summary>
        /// <param name="to">user you intend to invite to chat room.</param>
        /// <param name="message">message you want to send to the user.</param>
        /// <param name="chatRoom">Jid of the chat room.</param>
        /// <param name="password">Password if any.</param>
        public void SendInvite(Jid to, Jid chatRoom, string message, string password = null)
        {
            groupChat.SendInvite(to, chatRoom, message, password);
        }

        /// <summary>
        /// Responds to a group chat invitation with a decline message.
        /// </summary>
        /// <param name="invite">Original group chat invitation.</param>
        /// <param name="reason">Reason for declining.</param>
        public void DeclineInvite(Invite invite, string reason)
        {
            groupChat.DeclineInvite(invite, reason);
        }

        /// <summary>
        /// Allows visitors to request membership to a room.
        /// </summary>
        public void RequestVoice(Jid chatRoom)
        {
            groupChat.RequestPrivilige(chatRoom, Role.Participant);
        }

        /// <summary>
        /// Closes the connection with the XMPP server. This automatically disposes
        /// of the object.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The XmppClient object has been
        /// disposed.</exception>
        public void Close()
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().FullName);
            Dispose();
        }

        /// <summary>
        /// Releases all resources used by the current instance of the XmppClient class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases all resources used by the current instance of the XmppClient
        /// class, optionally disposing of managed resource.
        /// </summary>
        /// <param name="disposing">true to dispose of managed resources, otherwise
        /// false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                // Indicate that the instance has been disposed.
                disposed = true;
                // Get rid of managed resources.
                if (disposing)
                {
                    if (im != null)
                        im.Close();
                    im = null;
                }
                // Get rid of unmanaged resources.
            }
        }

        /// <summary>
        /// Asserts the instance has not been disposed of and is connected to the
        /// XMPP server.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The XmppClient object has been
        /// disposed.</exception>
        /// <exception cref="InvalidOperationException">The XmppClient instance is not
        /// connected to a remote host.</exception>
        private void AssertValid()
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (!Connected)
                throw new InvalidOperationException("Not connected to XMPP server.");
            if (!Authenticated)
                throw new InvalidOperationException("Not authenticated with XMPP server.");
        }

        /// <summary>
        /// Initializes the various XMPP extension modules.
        /// </summary>
        private void LoadExtensions()
        {
            version = im.LoadExtension<SoftwareVersion>();
            sdisco = im.LoadExtension<ServiceDiscovery>();
            ecapa = im.LoadExtension<EntityCapabilities>();
            ping = im.LoadExtension<Ping>();
            attention = im.LoadExtension<Attention>();
            time = im.LoadExtension<EntityTime>();
            block = im.LoadExtension<BlockingCommand>();
            pep = im.LoadExtension<Pep>();
            userTune = im.LoadExtension<UserTune>();
#if WINDOWSPLATFORM
			ertguserAvatar = im.LoadExtension<UserAvatar>();
#endif
            userMood = im.LoadExtension<UserMood>();
            dataForms = im.LoadExtension<DataForms>();
            featureNegotiation = im.LoadExtension<FeatureNegotiation>();
            streamInitiation = im.LoadExtension<StreamInitiation>();
            userActivity = im.LoadExtension<UserActivity>();

            socks5Bytestreams = im.LoadExtension<Socks5Bytestreams>();
            inBandBytestreams = im.LoadExtension<InBandBytestreams>();
            siFileTransfer = im.LoadExtension<SIFileTransfer>();
            FileTransferSettings = new FileTransferSettings(socks5Bytestreams,
                siFileTransfer);
            
            serverIpCheck = im.LoadExtension<ServerIpCheck>();
            messageCarbons = im.LoadExtension<MessageCarbons>();
            inBandRegistration = im.LoadExtension<InBandRegistration>();
            chatStateNotifications = im.LoadExtension<ChatStateNotifications>();
            bitsOfBinary = im.LoadExtension<BitsOfBinary>();
            vcardAvatars = im.LoadExtension<VCardAvatars>();
            cusiqextension = im.LoadExtension<CustomIqExtension>();
            groupChat = im.LoadExtension<MultiUserChat>();
            mam = im.LoadExtension<MessageArchiveManagment>();

            configuration = im.LoadExtension<Configuration>();
            conference = im.LoadExtension<Conference>();
            callLog = im.LoadExtension<CallLog>();
            cap = im.LoadExtension<Cap>();
            msgDeliveryReceipt = im.LoadExtension<MessageDeliveryReceipts>();
            callService = im.LoadExtension<CallService>();
        }
    }
}