using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using MetaCraft;
using MetaCraft.Commands;

var builder = new CommandLineBuilder()
    .EnablePosixBundling()
    .UseExceptionHandler((ex, context) =>
    {
        if (ex is InteractiveException iex)
        {
            // Interactive exceptions are those with no cause and thrown because of invalid
            // user action.
            Application.PrintError(iex.Message);
            context.ExitCode = 1;
            return;
        }
        
        Application.PrintError(ex);
        context.ExitCode = 1;
    })
    .UseHelp()
    .UseVersionOption()
    .RegisterWithDotnetSuggest()
    .UseParseErrorReporting(2);

var scope = Application.InitializeScope();

builder.Command.Description = "MetaCraft local package manager";
builder.Command.AddCommand(InspectCommand.Create());
builder.Command.AddCommand(new InstallCommand(scope.Container).Create());
builder.Command.AddCommand(new RemoveCommand(scope).Create());

var cli = builder.Build();
return cli.Invoke(args);