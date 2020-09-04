using System.Threading.Tasks;
using Astor.Subscriptions.Abstractions;

namespace Example.Application
{
    public class GreetingsController
    {
        [SubscribedOn("needs-greeting")]
        public async Task SayHelloAsync(GreetingCandidate candidate)
        {
            Greeting.Phrase = $"Hello, {candidate.Name}";
        }
    }
}