using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Web;
using TwitchLib.Api;
using TwitchLib.Api.Auth;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace chatBabysitter
{
    public class TwitchAuthenticationModel
    {
        public string Token { get; set; }
        public string Scopes { get; set; }
    }

    public class TwitchManager(string activationString = "!")
    {
        internal JoinedChannel? JoinedChannel { get; set; } = null;
        public string ActivationString { get; set; } = activationString;

        private readonly TwitchClient _twitchClient = new TwitchClient();
        private readonly TwitchAPI _twitchAPI = new TwitchAPI();
        private readonly string _redirectUri = "http://localhost:3000";
        private readonly string _username = "chatBabysitter";
        private readonly string _clientId = "wu5zcrdobt95ejkucmpt0owgjz6knp";
        private static string _state = "";
        private static string _DASECRET = "";
        private readonly HttpListener authListener = new HttpListener();

        public async Task<bool> ConnectToTwitchAsync()
        {
            //TODO: verify
            string authToken = await GetAccessTokenAsync();
            ConnectionCredentials connectionCredentials = new ConnectionCredentials(_username, authToken);
            _twitchClient.SetConnectionCredentials(connectionCredentials);

            _twitchClient.OnMessageReceived += TwitchClient_OnMessageReceived;

            return _twitchClient.Connect();
        }

        private async Task ConnectToChatAsync(string chatChannel)
        {
            //TODO: verify
            if (!_twitchClient.IsConnected)
                await ConnectToTwitchAsync();
            _twitchClient.JoinChannel(chatChannel);
        }

        private TwitchAuthenticationModel GetModel(string token, string scopes)
        {
            return new TwitchAuthenticationModel { Token = token, Scopes = scopes };
        }

        private string GetResponse()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("<html>");
            builder.Append(Environment.NewLine);
            builder.Append("<head>");
            builder.Append(Environment.NewLine);
            builder.Append("<title>AivaBot Twitch Oauth</title>");
            builder.Append(Environment.NewLine);
            builder.Append("<script language=\"JavaScript\">");
            builder.Append(Environment.NewLine);
            builder.Append("if(window.location.hash) {");
            builder.Append(Environment.NewLine);
            builder.Append("window.location.href = window.location.href.replace(\"/#\",\"?=\");");
            builder.Append(Environment.NewLine);
            builder.Append("}");
            builder.Append(Environment.NewLine);
            builder.Append("</script>");
            builder.Append(Environment.NewLine);
            builder.Append("</head>");
            builder.Append(Environment.NewLine);
            builder.Append("<body>You can close this tab</body>");
            builder.Append(Environment.NewLine);
            builder.Append("</html>");

            return builder.ToString();
        }

        private async Task<AuthCodeResponse> GetOAuthTokenAsync()
        {
            //TODO: verify. currently using Aiva(https://github.com/Moerty/Aiva/tree/master) for reference on how to do the oauth handshake
            List<AuthScopes> authScopes = new List<AuthScopes> { AuthScopes.Any };
            _state = new Random(DateTime.UtcNow.Microsecond + DateTime.UtcNow.Millisecond + DateTime.UtcNow.Second).Next().ToString();
            string authCodeUrl = _twitchAPI.Auth.GetAuthorizationCodeUrl(_redirectUri, authScopes, false, _state, _clientId);
            Process.Start(authCodeUrl);
            authListener.Prefixes.Add(_redirectUri + "/");
            authListener.Start();

            TwitchAuthenticationModel Values = null;
            while (authListener.IsListening)
            {
                var context = authListener.GetContext();

                if (context.Request.QueryString.HasKeys())
                {
                    if (context.Request.RawUrl.Contains("access_token"))
                    {
                        Uri myUri = new Uri(context.Request.Url.OriginalString);
                        string scope = HttpUtility.ParseQueryString(myUri.Query).Get("scope");
                        string access_token = HttpUtility.ParseQueryString(myUri.Query).Get(0).Replace("access_token=", "");

                        if (!String.IsNullOrEmpty(scope) && !String.IsNullOrEmpty(access_token))
                        {
                            Values = GetModel(access_token, scope);
                        }
                    }
                }

                byte[] b = Encoding.UTF8.GetBytes(GetResponse());
                context.Response.StatusCode = 200;
                context.Response.KeepAlive = false;
                context.Response.ContentLength64 = b.Length;

                var output = context.Response.OutputStream;
                output.Write(b, 0, b.Length);
                context.Response.Close();

                if (Values != null)
                {
                    authListener.Stop();
                }
            }


            return await _twitchAPI.Auth.GetAccessTokenFromCodeAsync(Values.Token, _DASECRET, _redirectUri, _clientId);
        }

        private async Task<string> GetAccessTokenAsync()
        {
            AuthCodeResponse response = await GetOAuthTokenAsync();
            return response.AccessToken;
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
