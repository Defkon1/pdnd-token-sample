namespace PDNDTokenSample
{
    using Microsoft.Extensions.Configuration;
    using PDNDTokenSample.Core.Services;
    using PDNDTokenSample.Core.Models;
    using Spectre.Console;
    using System.Reflection;

    internal class Program
    {
        static void Main(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args);

            IConfiguration config = builder.Build();
            
            /* output to console */
            AnsiConsole.Write(new Rule());
            AnsiConsole.Write(new FigletText("PDND Token Sample").LeftJustified().Color(Color.Green));
            AnsiConsole.Write(new Rule());
            AnsiConsole.WriteLine();

            try
            {
                var assertionTitle = new Rule("[green]Client assertion[/]");
                assertionTitle.RuleStyle("green dim");
                AnsiConsole.Write(assertionTitle);
                AnsiConsole.WriteLine();

                var clientSettings = config.GetRequiredSection("PDND").Get<PDNDTokenClientSettings>();

                var client = new PDNDTokenClient(clientSettings!);

                var assertion = client.GetClientAssertion();

                {
                    var table = new Table();

                    table.AddColumn("Key");
                    table.AddColumn("Value");

                    foreach (PropertyInfo propertyInfo in assertion.GetType().GetProperties())
                    {
                        table.AddRow($"[blue]{propertyInfo.Name}[/]", propertyInfo.GetValue(assertion, null)?.ToString() ?? string.Empty);
                    }

                    AnsiConsole.Write(table);
                }

                AnsiConsole.WriteLine();

                var tokenTitle = new Rule("[green]OAuth2 JWT Token[/]");
                tokenTitle.RuleStyle("green dim");
                AnsiConsole.Write(tokenTitle);
                AnsiConsole.WriteLine();

                var tokenInfo = client.GetToken(assertion.ClientAssertion).Result;

                {
                    var table = new Table();

                    table.AddColumn("Key");
                    table.AddColumn("Value");

                    foreach (PropertyInfo propertyInfo in tokenInfo.GetType().GetProperties())
                    {
                        table.AddRow($"[blue]{propertyInfo.Name}[/]", propertyInfo.GetValue(tokenInfo, null)?.ToString() ?? string.Empty);
                    }

                    AnsiConsole.Write(table);
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
            }

            Console.ReadLine();
        }
    }
}