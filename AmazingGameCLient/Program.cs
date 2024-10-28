using AmazingGameCLient.Services;
using Microsoft.Extensions.Configuration;

var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

IConfiguration config = configBuilder.Build();


LoginService loginService = new(config);
SessionService sessionService = new(config);

var uiService = new UIService(loginService, sessionService);

await uiService.StartDialogAsync();
