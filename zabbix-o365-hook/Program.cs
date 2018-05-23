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

        [Option('u', "url", Required = true, HelpText = "URL of the webhook to POST the message against")]
        public string Url { get; set; }
    }

    class Program
    {

        static int Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            return Parser.Default.ParseArguments<Options>(args)
                .MapResult(
                options => RunWithParameters(options),
                _ => 1
                );
        }
        /// <summary>
        ///     Main procedure - POST the message as an active card to the URL provided.
        /// </summary>
        /// <param name="opts"></param>
        /// <returns></returns>
        static int RunWithParameters(Options opts)
        {
            var card = ZabbixToCard(subject: opts.Subject, message: opts.Message);
            var content = new StringContent(card.ToJson(), Encoding.UTF8, "application/json");

            using(var client = new HttpClient())
            {
                var response = client.PostAsync(requestUri: opts.Url, content: content).Result;
                try
                {
                    response.EnsureSuccessStatusCode();
                    Console.WriteLine("Web Hook succeeded");
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("Caught exception sending webhook");
                    Console.WriteLine($"Message: {e.Message}");
                    return 2;
                }
                return 0;
            }
           
            

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
