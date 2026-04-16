using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using DotNetEnv;

#pragma warning disable OPENAI001

// Load environment variables from .env file
Env.Load();

// Load configuration from environment variables
string deploymentName = Environment.GetEnvironmentVariable("OPENAI_DEPLOYMENT_NAME") ?? "gpt-5.2-chat";
string endpoint = Environment.GetEnvironmentVariable("OPENAI_ENDPOINT") ?? "https://imaheshnagar-4650-resource.openai.azure.com/openai/v1/";
string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? throw new InvalidOperationException("OPENAI_API_KEY environment variable is required");

 
ChatClient client = new(
    credential: new ApiKeyCredential(apiKey),
    model: deploymentName,
    options: new OpenAIClientOptions()
    {
        Endpoint = new($"{endpoint}"),
    }); 


ChatCompletion completion = client.CompleteChat(
     [
         new SystemChatMessage("You are a helpful assistant that talks like a pirate."),
         new UserChatMessage("can you help me?"),
         new AssistantChatMessage("What can I do for you?"),
         new UserChatMessage("What's the best way to train a parrot? reply in short and one sentence"),
     ]);

Console.WriteLine($"Model={completion.Model}");
foreach (ChatMessageContentPart contentPart in completion.Content)
{
    string message = contentPart.Text;
    Console.WriteLine($"Chat Role: {completion.Role}");
    Console.WriteLine("Message:");
    Console.WriteLine(message);
}
