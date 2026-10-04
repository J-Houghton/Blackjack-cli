# Blackjack

A console blackjack table: you, up to six basic-strategy bots, the book's advice on every move, and
a live Hi-Lo card count.

## Play

```sh
dotnet run --project blackjack
```

Options go after `--`, for example `dotnet run --project blackjack -- --fast --seed 42`:

- `--fast` skips the pauses between bot moves and dealer draws
- `--seed <n>` shuffles from a fixed seed, so the same cards come out again
- `--keys` prints the key bindings and which file they came from
- `--keys-file <path>` reads and saves key bindings somewhere other than `keys.ini`

Setup asks for your name, whether to change the table rules, your starting chips, how many bots
to seat, and whether to change the keys. Every question has a default, so Enter all the way
through starts a game. The game wants a terminal at least 80 by 24; taller ones get bigger cards.

## The table

The dealer is at the top. Below are the other seats in the order they act, with a `▼ YOU` marker
where you sit. Your seat is the panel at the bottom with the count beside it, then the last few
things that happened, then a bar that always says what the keys do right now.

Default keys, all under the left hand:

| When | Key | Does |
| --- | --- | --- |
| Between hands | `Space` (or `Enter`) | deal, at the bet shown |
| | `Q` / `E` | lower / raise the bet by the minimum |
| | digits | type an exact bet (`Backspace` and `Esc` to fix it) |
| | `F` | autopilot: the book plays your seat while you watch |
| | `R` | skip to the next shoe |
| | `X` twice | cash out |
| Your move | `D` / `S` | hit / stand |
| Any time | `B` / `C` | show or hide the book's advice / the count |
| On autopilot | `Q` / `E` | slower / faster |
| | `R` | skip one more shoe |
| | any other key | stop after this hand |

The book's choice is highlighted on every move. If you go against it, the log says what the book
would have done, and the cash-out summary shows how often you followed it. On autopilot your seat
bets the table minimum. Keys pressed while the bots and dealer play are ignored, so a stray press
can't deal the next hand.

Default rules: 6 decks, dealer stands on soft 17, blackjack pays 3:2, minimum bet 10, reshuffle
after 75% of the shoe. A dealer blackjack ends the round for everyone; a player blackjack is paid
straight away.

## Key bindings

Keys live in `keys.ini` next to the solution (a copy beside the program covers running from Visual
Studio). Edit it, or choose "Change the key bindings?" in setup and press the new keys, which saves
the file. Each action takes one or more keys: a letter or symbol, or a name such as `Space`, `Tab`,
`Up` or `F5`. Enter, Escape, Backspace and the digits are kept for typing bets. A key only has to
differ from the other keys read at the same moment, so hit and lower-bet could share one.

## Layout

- `blackjack/Domain`: `Card`, `Hand`, `Player`, `Shoe`
- `blackjack/Engine`: `Game` (the round state machine), `Rules`, `CardCounter`
- `blackjack/Strategies`: `IPlayerStrategy`, `Move`, `BasicStrategyBot`
- `blackjack/Cli`: the Spectre.Console front end: `TableScreen` draws the table, `ConsoleStrategy`
  is your seat, `FastForward` is the autopilot, `KeyBindings` reads `keys.ini`
- `blackjack.Tests`: xUnit tests, using a stacked shoe to rig hands

```sh
dotnet test
```
