using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RetroShareApi.Connection;
using RetroShareApi.Request;
using System.Collections.Concurrent;
using System.Threading;
using IrcDotNet;
using System.Text.RegularExpressions;
using BotHost.Markov;

namespace BotHost
{
    class Program
    {
        static BlockingCollection<ChatMessage> msgqueue = new BlockingCollection<ChatMessage>();
        static MarkovChainTextBot bot;
        static string ChatCommandPrefix = "!";
        private static readonly Regex commandPartsSplitRegex = new Regex("(?<! /.*) ", RegexOptions.None);

        static void Main(string[] args)
        {
            IConnection con;
            if (args.Length > 0)
                con = new HTTPConnection(new Uri(args[0]));
            else
                con = new HTTPConnection(new Uri(@"http://localhost:9090"));

            Dictionary<string, RetroShareApi.Request.Chat.Messages> requests = new Dictionary<string, RetroShareApi.Request.Chat.Messages>();

            Util.con = con;
            bot = new MarkovChainTextBot();

            Thread t = new Thread(Consume);
            t.Start();

            while (true)
            {
                RetroShareApi.Request.Chat.Lobbies lobbies = new RetroShareApi.Request.Chat.Lobbies();
                foreach (RetroShareApi.Response.Chat.Lobby lobby in lobbies.Execute(con).data)
                {
                    if (!lobby.subscribed || lobby.is_broadcast) continue;              
                    RetroShareApi.Request.Chat.Messages msgreq;
                    bool found = requests.TryGetValue(lobby.id, out msgreq);
                    if (!found)
                    {
                        msgreq = new RetroShareApi.Request.Chat.Messages(lobby.chat_id);
                        requests.Add(lobby.id, msgreq);
                    }

                    foreach (RetroShareApi.Response.Chat.Message msg in msgreq.Execute(con).data)
                    {
                        BotHost.ChatMessage chatmsg = new ChatMessage();
                        chatmsg.nick = msg.author_name;
                        chatmsg.chatid = lobby.chat_id;
                        chatmsg.text = msg.msg;
                        chatmsg.time = (new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).AddSeconds(msg.send_time).ToLocalTime();
                        chatmsg.incoming = msg.incoming;
                        msgqueue.Add(chatmsg);
                    }
                }
                Thread.Sleep(1000);
            }
        }

        private static void Consume()
        {
            while (true)
            {
                ChatMessage msg = msgqueue.Take();
                ProcessMessage(msg);
            }
        }

        private static void ProcessMessage(ChatMessage msg)
        {
            // Check if given message represents chat command.
            var line = msg.text;
            if (line.Length > 1 && line.StartsWith(ChatCommandPrefix))
            {
                //Do not respond to commands from previous runs
                if (msg.time <= (DateTime.Now - new TimeSpan(0, 1, 0))) return;

                // Process command.
                var parts = commandPartsSplitRegex.Split(line.Substring(1)).Select(p => p.TrimStart('/')).ToArray();
                var command = parts.First();
                var parameters = parts.Skip(1).ToArray();

                if (command.Equals("talk")) bot.ProcessChatCommandTalk(msg.chatid, command, parameters);
                if (command.Equals("stats")) bot.ProcessChatCommandStats(msg.chatid, command, parameters);

            }
            else
            {
                if(msg.incoming)
                    bot.OnChannelMessageReceived(msg.chatid, msg.nick, msg.text);
            }
        }
    }

    public class ChatMessage
    {
        public string nick;
        public string chatid;
        public DateTime time;
        public string text;
        public bool incoming;

        public override string ToString()
        {
            return time.ToString() + " " + nick + ": " + text;
        }
    }
}
