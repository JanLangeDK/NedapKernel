
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
                    apiKey: "e78bafddedd441ea9a72e05f458dfc33", 
                    httpClient: new HttpClient
                    {
                        Timeout = TimeSpan.FromSeconds(300) // Set timeout to 5 minutes
                    })
                .Build();


            var cardIssuePlugin = new CardIssuePlugin();
            var nonUsedCardPlugin = new NonUsedCardPlugin();
            var fraudDetectionPlugin = new FraudDetectionPlugin(kernel); // Pass Kernel manually
            var predictiveMaintenancePlugin = new PredictiveMaintenancePlugin();

            var analyzer = new EventLogAnalyzer(
                kernel,
                cardIssuePlugin,
                nonUsedCardPlugin,
                fraudDetectionPlugin,
                predictiveMaintenancePlugin);

            var resultJson = await analyzer.AnalyzeLogsAsync();
            Console.WriteLine(resultJson);

            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

            var eventlogPlugin = new EventLogPlugin();
            kernel.Plugins.AddFromObject(eventlogPlugin, "Eventlog");

            var executionSettings = new OpenAIPromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };

            var chatHistory = new ChatHistory($"Today's date and time is {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
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