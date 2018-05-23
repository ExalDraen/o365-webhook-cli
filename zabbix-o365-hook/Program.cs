using AdaptiveCards;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace zabbix_o365_hook
{
    class Options
    {
        [Option('m', "message", Required = true, HelpText = "Main message text for the webhook")]
        public string Message { get; set; }

        [Option('s', "subject", Required = true, HelpText = "Subject text for the webhook")]
        public string Subject { get; set; }
    }

    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            var subj = "test subject";
            var msg = "test msg";

            var cardText = ZabbixToCard(subject: subj, message: msg).ToJson();

            CommandLine.Parser.Default.ParseArguments<Options>(args)
                .WithNotParsed(errs => HandleParseError(errs))
                .WithParsed(opts => RunWithParameters(opts));
            Console.WriteLine("Hello World!");
            Console.WriteLine(cardText);
        }

        static void RunWithParameters(Options opts)
        {
            var card = ZabbixToCard(subject: opts.Subject, message: opts.Message);
            var content = new StringContent(card.ToJson(), Encoding.UTF8, "application/json");


            var response = client.PostAsync("", content);
        }

        static void HandleParseError(IEnumerable<Error> errors)
        {
            // exit
        }

        /// <summary>
        ///     Turns a zabbix message into an adaptive card
        /// </summary>
        /// <param name="subject">The subject of the message</param>
        /// <param name="message">The message itself</param>
        /// <returns></returns>
        static AdaptiveCard ZabbixToCard(string subject, string message)
        {
            var card = new AdaptiveCard();

            // Text Block for the title
            var headerBlock = new AdaptiveTextBlock()
            {
                Text = $"Zabbix Alert: {subject}",
                Weight = AdaptiveTextWeight.Bolder,
                Size = AdaptiveTextSize.Large
            };

            // Text block for the actual message
            var messageBlock = new AdaptiveTextBlock()
            {
                Text = message,
                Weight = AdaptiveTextWeight.Default,
                Size = AdaptiveTextSize.Default
            };

            card.Body.Add(new AdaptiveContainer()
            {
                Items = new List<AdaptiveElement> { headerBlock, messageBlock }
            });

            card.Body.Add(new AdaptiveImage()
            {
                Url = new Uri("http://adaptivecards.io/content/cats/1.png")
            });

            return card;
        }
    }
}
