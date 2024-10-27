using AmazingGameCLient.Services;
using Microsoft.Extensions.Configuration;

var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

IConfiguration config = configBuilder.Build();


LoginService loginService = new(config);
SessionService sessionService = new(config);
ShopValidatorService shopValidatorService = new();

var uiService = new UIService(loginService, sessionService, shopValidatorService);

await uiService.StartDialogAsync();
