using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using DotNetEnv;
using Azure.Identity;
using Azure.Core;

#pragma warning disable OPENAI001

// Load environment variables from .env file
Env.Load();

// Load configuration from environment variables
string deploymentName = Environment.GetEnvironmentVariable("OPENAI_DEPLOYMENT_NAME") ?? "gpt-5.2-chat";
string endpoint = Environment.GetEnvironmentVariable("OPENAI_ENDPOINT") ?? "https://imaheshnagar-4650-resource.openai.azure.com/openai/v1/";
string? apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

// Try to use Azure Identity first, fallback to API key
ApiKeyCredential credential;
try 
{
    if (string.IsNullOrEmpty(apiKey))
    {
        // Use Azure Identity to get a token
        var azureCredential = new DefaultAzureCredential();
        var tokenRequestContext = new TokenRequestContext(new[] { "https://cognitiveservices.azure.com/.default" });
        var token = await azureCredential.GetTokenAsync(tokenRequestContext);
        credential = new ApiKeyCredential(token.Token);
    }
    else
    {
        // Use API key from environment variable
        credential = new ApiKeyCredential(apiKey);
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Azure authentication failed: {ex.Message}");
    if (string.IsNullOrEmpty(apiKey))
    {
        throw new InvalidOperationException("Both Azure authentication and OPENAI_API_KEY environment variable failed. Please provide valid authentication.");
    }
    credential = new ApiKeyCredential(apiKey);
}

ChatClient client = new(
    credential: credential,
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
