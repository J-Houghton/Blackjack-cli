using System.Text;
using blackjack.Cli;
using Spectre.Console;

// Usage: blackjack [--fast] [--seed <n>] [--keys] [--keys-file <path>]
//   --fast              no pauses between bot moves and dealer draws
//   --seed <n>          shuffle the shoe from a fixed seed, to replay the same cards
//   --keys              print the key bindings and where they're read from, then exit
//   --keys-file <path>  read and save key bindings in this file instead of keys.ini

Console.OutputEncoding = Encoding.UTF8;

if (args.Contains("--help") || args.Contains("-h"))
{
    Console.WriteLine("Usage: blackjack [--fast] [--seed <n>] [--keys] [--keys-file <path>]");
    return 0;
}

var keysPath = KeyBindings.ResolvePath(Value("--keys-file"));
var warnings = new List<string>();
var keys = KeyBindings.Load(keysPath, warnings);

if (args.Contains("--keys"))
{
    var table = new Table().Border(TableBorder.Rounded).AddColumn("Action").AddColumn("Keys").AddColumn("In the file as");
    foreach (var action in Enum.GetValues<KeyAction>())
    {
        table.AddRow(
            KeyBindings.Describe(action),
            Markup.Escape(string.Join(", ", keys.Get(action).Select(k => k.Label))),
            KeyBindings.IniName(action));
    }

    AnsiConsole.Write(table);
    AnsiConsole.MarkupLine(File.Exists(keysPath)
        ? $"Read from [bold]{Markup.Escape(keysPath)}[/]"
        : $"No {KeyBindings.FileName} found, so these are the defaults. Changing them in setup saves to [bold]{Markup.Escape(keysPath)}[/]");
    foreach (var warning in warnings)
    {
        AnsiConsole.MarkupLine($"[yellow]{Markup.Escape(warning)}[/]");
    }

    return 0;
}

if (!AnsiConsole.Profile.Capabilities.Interactive)
{
    Console.Error.WriteLine("Blackjack needs an interactive terminal.");
    return 1;
}

var pause = args.Contains("--fast") ? TimeSpan.Zero : TimeSpan.FromMilliseconds(700);
var random = int.TryParse(Value("--seed"), out var seed) ? new Random(seed) : new Random();

new BlackjackApp(AnsiConsole.Console, pause, random, keys, keysPath, warnings).Run();
return 0;

string? Value(string flag)
{
    var at = Array.IndexOf(args, flag);
    return at >= 0 && at + 1 < args.Length ? args[at + 1] : null;
}
