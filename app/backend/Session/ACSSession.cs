using System.Diagnostics;

namespace CustomerSupportServiceSample.Session
{
    public class ACSSession
    {
        private static readonly ActivitySource MyActivitySource = new("MyActivitySource");

        private static Activity? _session = null;
        private static Activity? _botChat = null;
        private static Activity? _botVoiceChat = null;

        public static void StartUserSession()
        {
            if (_session == null)
            {
                _session = MyActivitySource.StartActivity("user-session");
            }
        }

        public static void StopUserSession()
        {
            _session?.Stop();
        }

        public static void StartBotChat()
        {
            if (_session != null && _botChat == null)
            {
                _botChat = MyActivitySource.StartActivity("chat with bot", ActivityKind.Internal, _session.Context);
            }
        }

        public static void LogBotChatMessage(string sender, string message)
        {
            if (_botChat != null)
            {
                var tags = new ActivityTagsCollection
                {
                    { "sender", sender },
                    { "message", message }
                };
                var e = new ActivityEvent("message", DateTimeOffset.Now, tags);
                _botChat.AddEvent(e);
            }
        }

        public static void StopBotChat()
        {
            if (_botChat != null)
            {
                _botChat.Stop();
            }
        }

        public static void StartBotVoiceChat()
        {
            if (_session != null && _botVoiceChat == null)
            {
                _botVoiceChat = MyActivitySource.StartActivity("voice chat with bot", ActivityKind.Internal, _session.Context);
            }
        }

        public static void LogBotVoiceChatMessage(string speaker, string message)
        {
            if (_botVoiceChat != null)
            {
                var tags = new ActivityTagsCollection
                {
                    { "speaker", speaker },
                    { "message", message }
                };
                var e = new ActivityEvent("message", DateTimeOffset.Now, tags);
                _botVoiceChat.AddEvent(e);
            }
        }

        public static void StopBotVoiceChat(string reason)
        {
            if (_botVoiceChat != null)
            {
                _botVoiceChat.SetTag("end_reason", reason);
                _botVoiceChat.Stop();
            }
        }

        public static void SendSms(string customerPhoneNumber, string message)
        {
            if (_session != null)
            {
                var tags = new ActivityTagsCollection
                {
                    { "customerPhoneNumber", customerPhoneNumber },
                    { "message", message }
                };
                using var sms = MyActivitySource.StartActivity("send sms", ActivityKind.Internal, _session.Context, tags);
            }
        }

        public static void SendEmail(string customerEmail, string message)
        {
            if (_session != null)
            {
                var tags = new ActivityTagsCollection
                {
                    { "customerEmail", customerEmail },
                    { "message", message }
                };
                using var email = MyActivitySource.StartActivity("send email", ActivityKind.Internal, _session.Context, tags);
            }
        }
    }
}
