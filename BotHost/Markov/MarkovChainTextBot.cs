using BotHost.Markov;
using RetroShareApi.Services.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IrcDotNet
{
    public class MarkovChainTextBot
    {
        private const string quitMessage = "Andrey Markov, 1856 - 1922";

        // Bot statistics
        private DateTime launchTime;
        private int numTrainingMessagesReceived;
        private int numTrainingWordsReceived;

        // Markov chain training and generation
        private readonly Random random = new Random();
        private readonly char[] sentenceSeparators = new[] {
            '.', '!', '?', ',', ';', ':' };
        private readonly Regex cleanWordRegex = new Regex(
            @"[()\[\]{}'""`~]");

        // Markov chain object
        private MarkovChain<string> markovChain;
        private Dictionary<string, MarkovChain<string>> markovChainLocal;

        public MarkovChainTextBot()
            : base()
        {
            this.markovChain = new MarkovChain<string>();
            this.markovChainLocal = new Dictionary<string, MarkovChain<string>>();

            this.launchTime = DateTime.Now;
            this.numTrainingMessagesReceived = 0;
            this.numTrainingWordsReceived = 0;
        }

        private MarkovChain<string> chainForUser(string nick)
        {
            if (string.IsNullOrEmpty(nick))
                return markovChain;

            nick = nick.ToLower();
            MarkovChain<string> chain;
            if(!markovChainLocal.TryGetValue(nick, out chain))
            {
                chain = new MarkovChain<string>();
                markovChainLocal.Add(nick, chain);
            }
            return chain;
        }

        public void OnChannelMessageReceived(IChat source, string nick, string message)
        { 
            // Train Markov generator from received message text.
            // Assume it is composed of one or more coherent sentences that are themselves are composed of words.
            var sentences = message.Split(sentenceSeparators);
            foreach (var s in sentences)
            {
                string lastWord = null;
                foreach (var w in s.Split(' ').Select(w => cleanWordRegex.Replace(w, string.Empty)))
                {
                    if (w.Length == 0)
                        continue;
                    //// Ignore word if it is first in sentence and same as nick name.
                    //if (lastWord == null && channel.Users.Any(cu => cu.User.NickName.Equals(w,
                    //    StringComparison.OrdinalIgnoreCase)))
                    //    break;

                    markovChain.Train(lastWord, w);
                    chainForUser(nick).Train(lastWord, w);
                    lastWord = w;
                    this.numTrainingWordsReceived++;
                }
                markovChain.Train(lastWord, null);
                chainForUser(nick).Train(lastWord, null);
            }
            this.numTrainingMessagesReceived++;
            
        }

        //protected override void InitializeChatCommandProcessors()
        //{
        //    this.ChatCommandProcessors.Add("talk", ProcessChatCommandTalk);
        //    this.ChatCommandProcessors.Add("stats", ProcessChatCommandStats);
        //}

        #region Chat Command Processors

        public void ProcessChatCommandTalk(IChat chatid, string command, IList<string> parameters)
        {
            string higlightNickName = null;
            int numSentences = -1;
            foreach(string param in parameters)
            {
                int i;
                if (int.TryParse(param, out i))
                    numSentences = i;
                else
                    higlightNickName = param;
            }
            SendRandomMessage(chatid,
                higlightNickName, numSentences);
        }

        public void ProcessChatCommandStats(IChat chatid, string command, IList<string> parameters)
        {
            string nick = "";
            if (parameters.Count > 0)
                nick = parameters[0];

            Util.SendNotice(chatid, "Bot launch time: {0:f} ({1:g} ago)",
                this.launchTime,
                DateTime.Now - this.launchTime);
            Util.SendNotice(chatid, "Number of training messages received: {0:#,#0} ({1:#,#0} words)",
                this.numTrainingMessagesReceived,
                this.numTrainingWordsReceived);
            Util.SendNotice(chatid, "Number of unique words in vocabulary: {0:#,#0}",
                chainForUser(nick).Nodes.Count);
        }

        #endregion

        private void SendRandomMessage(IChat chatid, string nick,
            int numSentences = -1)
        {
            if (chainForUser(nick).Nodes.Count == 0)
            {
                Util.SendNotice(chatid, "Bot has not yet been trained.");
                return;
            }

            var textBuilder = new StringBuilder();
            if (nick != null)
                textBuilder.Append(nick + ": ");

            // Use Markov chain to generate random message, composed of one or more sentences.
            if (numSentences == -1)
                numSentences = this.random.Next(1, 4);
            for (int i = 0; i < numSentences; i++)
                textBuilder.Append(GenerateRandomSentence(nick));

            Util.SendMessage(chatid, textBuilder.ToString());
        }

        private string GenerateRandomSentence(string nick)
        {
            // Generate sentence by using Markov chain to produce sequence of random words.
            // Note: There must be at least three words in sentence.
            int trials = 0;
            string[] words;
            do
            {
                words = chainForUser(nick).GenerateSequence().ToArray();
            }
            while (words.Length < 3 && trials++ < 10);

            return string.Join(" ", words) + ". ";
        }



    }
}
