using AmazingGameCLient.Services;

LoginService loginService = new ();
SessionService sessionService = new();
ShopValidatorService shopValidatorService = new();
ShopService shopService = new();

var uiService = new UIService(loginService, sessionService, shopValidatorService, shopService);

await uiService.StartDialogAsync();
