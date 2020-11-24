using System.Threading.Tasks;
using Astor.Subscriptions.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Example.Application
{
    [Route("Greetings")]
    public class GreetingsController
    {
        [SubscribedOn("needs-greeting")]
        [HttpPost]
        public async Task<string> SayHelloAsync([FromBody]GreetingCandidate candidate)
        {
            Greeting.Phrase = $"Hello, {candidate.Name}";

            return Greeting.Phrase;
        }
    }
}