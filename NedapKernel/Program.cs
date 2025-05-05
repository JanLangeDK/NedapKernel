
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using NedapKernel.Models;
using NedapKernel.Plugins;

namespace NedapKernel
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var kernel = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(
                    deploymentName: "gpt-4o-mini",
                    endpoint: "https://60087-openai.openai.azure.com",
                    apiKey: "e78bafddedd441ea9a72e05f458dfc33")
                .Build();

            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

            var eventlogPlugin = new EventLogPlugin();
            kernel.Plugins.AddFromObject(eventlogPlugin, "Eventlog");

            var executionSettings = new OpenAIPromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };

            // Create an instance of the eventlog class to call the non-static method
            //var eventLogInstance = new eventlog();
            //List<eventlog> eventlogs = await eventLogInstance.GetEventLogsAsync(
            //    "Server=AZEDKNedap01;Database=aeosdb;Integrated Security=True;TrustServerCertificate=True;", 10);

            var chatHistory = new ChatHistory($"You are a agent that can answer to questionsToday's date and time is {DateTime.Now:yyyy-MM-dd HH:mm:ss} localtime. localtime it utc + 1");
            Console.WriteLine("Chat started. Type 'exit' to quit.");
            while (true)
            {
                Console.Write("User > ");
                var userInput = Console.ReadLine();

                if (userInput?.ToLower() == "exit") break;

                chatHistory.AddUserMessage(userInput);

                var response = await chatCompletionService.GetChatMessageContentAsync(
                    chatHistory,
                    executionSettings,
                    kernel);

                Console.WriteLine("Assistant > " + response.Content);
                chatHistory.AddAssistantMessage(response.Content);
            }
        }
    }
}