using Spectre.Console;
using Spectre.Console.Rendering;
using Spectre.Console.Testing;

namespace blackjack.Tests
{
    // A test console whose keys only reach code that asks for one. IsKeyAvailable always says no,
    // so polling (autopilot stop checks, dropping keys typed during animations) never eats keys
    // meant for a later question.
    internal sealed class ScriptedConsole : IAnsiConsole
    {
        private readonly TestConsole inner;
        private readonly ScriptedInput input;

        public ScriptedConsole(TestConsole inner)
        {
            this.inner = inner;
            input = new ScriptedInput(inner.Input);
        }

        public string Output => inner.Output;
        public Profile Profile => inner.Profile;
        public IAnsiConsoleCursor Cursor => inner.Cursor;
        public IAnsiConsoleInput Input => input;
        public IExclusivityMode ExclusivityMode => inner.ExclusivityMode;
        public RenderPipeline Pipeline => inner.Pipeline;

        public void Clear(bool home) => inner.Clear(home);

        public void Write(IRenderable renderable) => inner.Write(renderable);

        public void WriteAnsi(Action<AnsiWriter> action) => inner.WriteAnsi(action);

        private sealed class ScriptedInput : IAnsiConsoleInput
        {
            private readonly TestConsoleInput keys;

            public ScriptedInput(TestConsoleInput keys)
            {
                this.keys = keys;
            }

            public bool IsKeyAvailable() => false;

            public ConsoleKeyInfo? ReadKey(bool intercept) => keys.ReadKey(intercept);

            public Task<ConsoleKeyInfo?> ReadKeyAsync(bool intercept, CancellationToken cancellationToken) =>
                keys.ReadKeyAsync(intercept, cancellationToken);
        }
    }
}
