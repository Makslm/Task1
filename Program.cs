using System.Timers;

namespace Task1
{
    public record GameState(
        string Language,
        string BaseWord,
        IReadOnlyList<string> UsedWords,
        int CurrentPlayer
    );

    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            string language = SelectLanguage();
            string baseWord = SetBaseWord(language);

            var currentState = new GameState(
                Language: language,
                BaseWord: baseWord,
                UsedWords: [],
                CurrentPlayer: 1
            );

            Move(currentState);
        }

        static string SelectLanguage()
        {
            Console.WriteLine("Select Language / Выберите язык: (eng / ru)");
            string language = (Console.ReadLine() ?? "").Trim().ToLower();

            return language switch
            {
                "ru" => "ru",
                "eng" => "eng",
                _ => SelectLanguage()
            };
        }

        static string GetText(string lang, string key) => (lang, key) switch
        {
            ("ru", "EnterBaseWord") => "Введите исходное слово (от 8 до 30 букв):",
            ("eng", "EnterBaseWord") => "Enter the base word (8 to 30 letters):",

            ("ru", "BaseWord") => "Базовое слово",
            ("eng", "BaseWord") => "Base word",

            ("ru", "InvalidBaseWord") => "Неверная длина",
            ("eng", "InvalidBaseWord") => "Invalid length",

            ("ru", "EnterWord") => "Введите слово. Очередь игрока",
            ("eng", "EnterWord") => "Enter word. Player's turn",

            ("ru", "Success") => "Успех. Слово было добавлено",
            ("eng", "Success") => "Success. Word has been added",

            ("ru", "WordExist") => "Слово уже было введено",
            ("eng", "WordExist") => "Word has already been written",

            ("ru", "WordsList") => "Список использованных слов",
            ("eng", "WordsList") => "List of used words",

            ("ru", "WordConstruct") => "Слово должно состоять из букв базового слова",
            ("eng", "WordConstruct") => "Word must consist of letters from base word",

            ("ru", "WordNotValid") => "Слово не корректно",
            ("eng", "WordNotValid") => "Word is not valid",

            ("ru", "TimeOut") => "Время вышло! Нажмите Enter...",
            ("eng", "TimeOut") => "Time is up! Press Enter...",

            ("ru", "Winner") => "Победил Игрок",
            ("eng", "Winner") => "Winner is Player",

            _ => key
        };

        static string SetBaseWord(string language)
        {
            Console.WriteLine(GetText(language, "EnterBaseWord"));
            string baseWord = (Console.ReadLine() ?? "").Trim().ToLower();

            if (baseWord.Length < 8 || baseWord.Length > 30)
            {
                Console.WriteLine(GetText(language, "InvalidBaseWord"));
                return SetBaseWord(language);
            }

            if (!IsValid(baseWord, language))
            {
                Console.WriteLine(GetText(language, "WordNotValid"));
                return SetBaseWord(language);
            }

            return baseWord;
        }

        static void Move(GameState state)
        {
            Console.WriteLine($"\n{GetText(state.Language, "WordsList")}: [{string.Join(", ", state.UsedWords)}]");
            Console.WriteLine($"{GetText(state.Language, "BaseWord")}: {state.BaseWord}");

            using var timer = new System.Timers.Timer(15000);
            timer.AutoReset = false;
            timer.Elapsed += (_, _) => Console.WriteLine(GetText(state.Language, "TimeOut"));
            timer.Start();

            string? validWord = TryGetValidWord(state, timer);
            timer.Stop();

            int nextPlayer = state.CurrentPlayer == 1 ? 2 : 1;

            if (validWord == null)
            {
                Console.WriteLine($"{GetText(state.Language, "Winner")} {nextPlayer}!");
                return;
            }

            var nextState = state with
            {
                UsedWords = state.UsedWords.Append(validWord).ToList(),
                CurrentPlayer = nextPlayer
            };

            Console.WriteLine(GetText(state.Language, "Success"));

            Move(nextState);
        }

        static string? TryGetValidWord(GameState state, System.Timers.Timer timer)
        {
            if (!timer.Enabled)
                return null;

            Console.WriteLine($"{GetText(state.Language, "EnterWord")} {state.CurrentPlayer}");
            string word = (Console.ReadLine() ?? "").Trim().ToLower();

            if (!timer.Enabled)
                return null;

            if (!IsValid(word, state.Language))
            {
                Console.WriteLine(GetText(state.Language, "WordNotValid"));
                return TryGetValidWord(state, timer);
            }

            if (!CanConstructWord(word, state.BaseWord))
            {
                Console.WriteLine(GetText(state.Language, "WordConstruct"));
                return TryGetValidWord(state, timer);
            }

            if (!IsUnique(word, state.BaseWord, state.UsedWords))
            {
                Console.WriteLine(GetText(state.Language, "WordExist"));
                return TryGetValidWord(state, timer);
            }

            return word;
        }

        static bool IsValid(string word, string lang) =>
            !string.IsNullOrEmpty(word) && lang switch
            {
                "ru" => word.All(c => (c >= 'а' && c <= 'я') || c == 'ё'),
                "eng" => word.All(c => c >= 'a' && c <= 'z'),
                _ => false
            };

        static bool IsUnique(string word, string baseWord, IReadOnlyList<string> usedWords) =>
            word != baseWord && !usedWords.Contains(word);

        static bool CanConstructWord(string word, string baseWord) =>
            word.Length <= baseWord.Length && word.Distinct().All(c => word.Count(x => x == c) <= baseWord.Count(x => x == c));
    }
}
