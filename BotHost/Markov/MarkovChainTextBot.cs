using BotHost.Markov;
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

        public MarkovChainTextBot()
            : base()
        {
            this.markovChain = new MarkovChain<string>();

            this.launchTime = DateTime.Now;
            this.numTrainingMessagesReceived = 0;
            this.numTrainingWordsReceived = 0;
        }

        public void OnChannelMessageReceived(string chatid, string nick, string message)
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
                    lastWord = w;
                    this.numTrainingWordsReceived++;
                }
                markovChain.Train(lastWord, null);
            }
            this.numTrainingMessagesReceived++;
            
        }

        //protected override void InitializeChatCommandProcessors()
        //{
        //    this.ChatCommandProcessors.Add("talk", ProcessChatCommandTalk);
        //    this.ChatCommandProcessors.Add("stats", ProcessChatCommandStats);
        //}

        #region Chat Command Processors

        public void ProcessChatCommandTalk(string chatid, string command, IList<string> parameters)
        {
            // Send reply containing random message text (generated from Markov chain).
            int numSentences = -1;
            if (parameters.Count >= 1)
                numSentences = int.Parse(parameters[0]);
            string higlightNickName = null;
            if (parameters.Count >= 2)
                higlightNickName = parameters[1] + ": ";
            SendRandomMessage(chatid,
                higlightNickName, numSentences);
        }

        public void ProcessChatCommandStats(string chatid, string command, IList<string> parameters)
        {
            Util.SendNotice(chatid, "Bot launch time: {0:f} ({1:g} ago)",
                this.launchTime,
                DateTime.Now - this.launchTime);
            Util.SendNotice(chatid, "Number of training messages received: {0:#,#0} ({1:#,#0} words)",
                this.numTrainingMessagesReceived,
                this.numTrainingWordsReceived);
            Util.SendNotice(chatid, "Number of unique words in vocabulary: {0:#,#0}",
                this.markovChain.Nodes.Count);
        }

        #endregion

        private void SendRandomMessage(string chatid, string textPrefix,
            int numSentences = -1)
        {
            if (this.markovChain.Nodes.Count == 0)
            {
                Util.SendNotice(chatid, "Bot has not yet been trained.");
                return;
            }

            var textBuilder = new StringBuilder();
            if (textPrefix != null)
                textBuilder.Append(textPrefix);

            // Use Markov chain to generate random message, composed of one or more sentences.
            if (numSentences == -1)
                numSentences = this.random.Next(1, 4);
            for (int i = 0; i < numSentences; i++)
                textBuilder.Append(GenerateRandomSentence());

            Util.SendMessage(chatid, textBuilder.ToString());
        }

        private string GenerateRandomSentence()
        {
            // Generate sentence by using Markov chain to produce sequence of random words.
            // Note: There must be at least three words in sentence.
            int trials = 0;
            string[] words;
            do
            {
                words = this.markovChain.GenerateSequence().ToArray();
            }
            while (words.Length < 3 && trials++ < 10);

            return string.Join(" ", words) + ". ";
        }



    }
}
