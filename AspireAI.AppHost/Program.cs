var builder = DistributedApplication.CreateBuilder(args);

var AzureOrOpenAI = builder.Configuration["AI:AzureOrOpenAI"] ?? "Azure"; ;
var chatDeploymentName = builder.Configuration["AI:AzureChatDeploymentName"];
var openAiChatModel = builder.Configuration["AI:OpenAiChatModel"];

IResourceBuilder<IResourceWithConnectionString> openai;
if (AzureOrOpenAI.ToLower() == "azure") {
    openai = builder.ExecutionContext.IsPublishMode
        ? builder.AddAzureOpenAI("azureOpenAi")
        : builder.AddConnectionString("azureOpenAi");
} else {
    openai = builder.ExecutionContext.IsPublishMode
        ? builder.AddAzureOpenAI("openAi")
        : builder.AddConnectionString("openAi");
}

// Register the RazorPagesAI project and pass to it environment variables.
//  WithReference method passes connection info to client project
builder.AddProject<Projects.RazorPagesAI>("razor")
    .WithReference(openai)
    .WithEnvironment("AI__AzureChatDeploymentName", chatDeploymentName)
    .WithEnvironment("AI__AzureOrOpenAI", AzureOrOpenAI)
    .WithEnvironment("AI_OpenAiChatModel", openAiChatModel);

 // register the ConsoleAI project and pass to it environment variables
builder.AddProject<Projects.ConsoleAI>("console")
    .WithReference(openai)
    .WithEnvironment("AI__AzureChatDeploymentName", chatDeploymentName)
    .WithEnvironment("AI__AzureOrOpenAI", AzureOrOpenAI)
    .WithEnvironment("AI_OpenAiChatModel", openAiChatModel);

builder.Build().Run();
