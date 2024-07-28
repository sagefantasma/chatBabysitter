using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace chatBabysitter
{
    public class TwitchManager(string activationString = "!")
    {
        internal JoinedChannel? JoinedChannel { get; set; } = null;
        public string ActivationString { get; set; } = activationString;

        private readonly TwitchClient _twitchClient = new TwitchClient();
        private readonly string _username = "chatBabysitter";
        private readonly string _twitchOAuth = "";

        public bool ConnectToTwitch()
        {
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(_username, _twitchOAuth);
            _twitchClient.SetConnectionCredentials(connectionCredentials);

            _twitchClient.OnMessageReceived += TwitchClient_OnMessageReceived;

            return _twitchClient.Connect();
        }

        private void TwitchClient_OnMessageReceived(object? sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            ReadMessage(e.ChatMessage);
        }

        public void ReadMessage(ChatMessage receivedMessage)
        {
            string message = receivedMessage.Message;

            if (message.StartsWith(ActivationString))
            {
                //TODO: stuff.
            }
        }

        public void WriteMessage(string messageToSend)
        {
            if(JoinedChannel == null)
            {
                throw new InvalidOperationException("Not connected to a chat, can't write any messages Sadge");
            }

            _twitchClient.SendMessage(JoinedChannel, messageToSend);
        }
    }
}
