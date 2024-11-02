using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

public class FiveMSensitivityBot
{
    private const double FivemMagicConstant = 1678106.125;

    // Map game names to their specific sensitivity constants
    private static readonly Dictionary<string, double> Games = new()
    {
        { "apex legends", 41563.0 },
        { "counter-strike", 41563.0 },
        { "valorant", 13062.86 },
        { "overwatch", 138545.0 },
        { "battlebit", 1828800.0 },
        { "battlefield", 61381.89 },
        { "fortnite", 164608.46 },
    };

    private readonly DiscordSocketClient _client;

    public FiveMSensitivityBot()
    {
        _client = new DiscordSocketClient();
        _client.Log += LogAsync;
        _client.Ready += ReadyAsync;
        _client.SlashCommandExecuted += SlashCommandHandler;
    }

    public static async Task Main(string[] args)
    {
        var bot = new FiveMSensitivityBot();
        await bot.RunBotAsync();
    }

    private async Task RunBotAsync()
    {
        // Get the bot token from the environment variables
        var token = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN");
        if (string.IsNullOrWhiteSpace(token))
        {
            Console.WriteLine("Error: No bot token found in environment variables. Please set DISCORD_BOT_TOKEN.");
            return;
        }

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        // Keep the bot running forever
        await Task.Delay(Timeout.Infinite);
    }

    private async Task ReadyAsync()
    {
        Console.WriteLine("Bot is connected!");
        await RegisterCommandsAsync();
    }

    private async Task RegisterCommandsAsync()
    {
        // Set up the /sens command with game, sens, and DPI options
        var sensCommand = new SlashCommandBuilder()
            .WithName("sens")
            .WithDescription("Calculate CM360 from game, sensitivity, and DPI")
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("game")
                .WithDescription("The game you are playing")
                .WithType(ApplicationCommandOptionType.String)
                .WithRequired(true)
                // Add choices for the 'game' option
                .AddChoice("apex legends", "apex legends")
                .AddChoice("counter-strike", "counter-strike")
                .AddChoice("valorant", "valorant")
                .AddChoice("overwatch", "overwatch")
                .AddChoice("battlebit", "battlebit")
                .AddChoice("battlefield", "battlefield")
                .AddChoice("fortnite", "fortnite")
            )
            .AddOption("sens", ApplicationCommandOptionType.Number, "Your in-game sensitivity", true)
            .AddOption("dpi", ApplicationCommandOptionType.Integer, "Your mouse DPI value", true)
            .Build();

        // Set up the /cm360 command with CM360 and DPI options
        var cm360Command = new SlashCommandBuilder()
            .WithName("cm360")
            .WithDescription("Calculate Profile_MouseOnFootScale directly from CM360 and DPI")
            .AddOption("cm360", ApplicationCommandOptionType.Number, "The CM360 value", true)
            .AddOption("dpi", ApplicationCommandOptionType.Integer, "Your mouse DPI value", true)
            .Build();

        var commands = new[] { sensCommand, cm360Command };

        foreach (var command in commands)
        {
            try
            {
                // Register the command globally
                await _client.Rest.CreateGlobalCommand(command);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: Failed to create command '{command.Name}' - {ex}");
            }
        }
    }

    private async Task SlashCommandHandler(SocketSlashCommand command)
    {
        switch (command.Data.Name)
        {
            case "sens":
                await HandleSensCommand(command);
                break;
            case "cm360":
                await HandleCM360Command(command);
                break;
        }
    }

    private async Task HandleSensCommand(SocketSlashCommand command)
    {
        // Turn the options into a dictionary for easier access
        var options = command.Data.Options.ToDictionary(opt => opt.Name, opt => opt.Value);

        // Get the 'game' option and make sure it's a valid string
        if (!options.TryGetValue("game", out var gameValueObj) || gameValueObj is not string game || string.IsNullOrWhiteSpace(game))
        {
            await command.RespondAsync("Error: 'game' option is missing or invalid.", ephemeral: true);
            return;
        }

        var sens = Convert.ToDouble(options["sens"]);
        var dpi = Convert.ToInt32(options["dpi"]);

        if (dpi <= 0)
        {
            await command.RespondAsync("Error: DPI must be greater than 0.", ephemeral: true);
            return;
        }

        if (!Games.TryGetValue(game, out var gameValue))
        {
            await command.RespondAsync($"Error: Invalid game name '{game}'. Please select a valid game.", ephemeral: true);
            return;
        }

        var cm360 = ComputeCM360(gameValue, sens, dpi);

        if (cm360 < 20 || cm360 > 80)
        {
            await command.RespondAsync("Error: Please enter valid values for CM360 (20-80).", ephemeral: true);
            return;
        }

        var profileMouseOnFootScale = ComputeProfileMouseOnFootScale(cm360, dpi);

        if (profileMouseOnFootScale < -60 || profileMouseOnFootScale > 120)
        {
            await command.RespondAsync("Error: The calculated Profile_MouseOnFootScale is out of range (-60 to 120). Please adjust your DPI.", ephemeral: true);
            return;
        }

        await command.RespondAsync($"Profile_MouseOnFootScale {Math.Round(profileMouseOnFootScale)}");
    }

    private async Task HandleCM360Command(SocketSlashCommand command)
    {
        // Turn the options into a dictionary for easier access
        var options = command.Data.Options.ToDictionary(opt => opt.Name, opt => opt.Value);

        var cm360 = Convert.ToDouble(options["cm360"]);
        var dpi = Convert.ToInt32(options["dpi"]);

        if (dpi <= 0)
        {
            await command.RespondAsync("Error: DPI must be greater than 0.", ephemeral: true);
            return;
        }

        if (cm360 < 20 || cm360 > 80)
        {
            await command.RespondAsync("Error: Please enter valid values for CM360 (20-80).", ephemeral: true);
            return;
        }

        var profileMouseOnFootScale = ComputeProfileMouseOnFootScale(cm360, dpi);

        if (profileMouseOnFootScale < -60 || profileMouseOnFootScale > 120)
        {
            await command.RespondAsync("Error: The calculated Profile_MouseOnFootScale is out of range (-60 to 120). Please adjust your DPI.", ephemeral: true);
            return;
        }

        await command.RespondAsync($"Profile_MouseOnFootScale {Math.Round(profileMouseOnFootScale)}");
    }

    // Compute CM360 using the game's value, sensitivity, and DPI
    private static double ComputeCM360(double gameValue, double sens, int dpi)
    {
        return Math.Round(gameValue / (sens * dpi));
    }

    // Compute Profile_MouseOnFootScale from CM360 and DPI
    private static double ComputeProfileMouseOnFootScale(double cm360, int dpi)
    {
        return Math.Round((FivemMagicConstant / (dpi * cm360)) - 60.25);
    }

    // Log Discord client messages to the console
    private Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log);
        return Task.CompletedTask;
    }
}
