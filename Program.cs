using System;
using System.Collections.Generic;
using System.Linq;
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
        // 1. Магические числа вынесены в константы
        private const int MinLengthBaseWord = 8;
        private const int MaxLengthBaseWord = 30;
        private const int TurnTimeLimitMs = 15000;

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            string language = SelectLanguage();
            string baseWord = SetBaseWord(language);

            var currentState = new GameState(
                Language: language,
                BaseWord: baseWord,
                UsedWords: Array.Empty<string>(),
                CurrentPlayer: 1
            );

            PlayGame(currentState);
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
            
            ("ru", "TimeOut") => "Время вышло!",
            ("eng", "TimeOut") => "Time is up!",
            
            ("ru", "Winner") => "Победил Игрок",
            ("eng", "Winner") => "Winner is Player",
            
            _ => key
        };

        static string SetBaseWord(string language)
        {
            Console.WriteLine(GetText(language, "EnterBaseWord"));
            string baseWord = (Console.ReadLine() ?? "").Trim().ToLower();

            if (baseWord.Length < MinLengthBaseWord || baseWord.Length > MaxLengthBaseWord)
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

        static void PlayGame(GameState state)
        {
            while (true)
            {
                Console.WriteLine($"\n{GetText(state.Language, "WordsList")}: [{string.Join(", ", state.UsedWords)}]");
                Console.WriteLine($"{GetText(state.Language, "BaseWord")}: {state.BaseWord}");
                Console.WriteLine($"{GetText(state.Language, "EnterWord")} {state.CurrentPlayer}");

                string? validWord = GetWordForTurn(state);
                int nextPlayer = state.CurrentPlayer == 1 ? 2 : 1;

                if (validWord == null)
                {
                    Console.WriteLine(GetText(state.Language, "TimeOut"));
                    Console.WriteLine($"{GetText(state.Language, "Winner")} {nextPlayer}!");
                    return;
                }

                state = state with
                {
                    UsedWords = state.UsedWords.Append(validWord).ToList(),
                    CurrentPlayer = nextPlayer
                };

                Console.WriteLine(GetText(state.Language, "Success"));
            }
        }

        static string? GetWordForTurn(GameState state)
        {
            using var timer = new System.Timers.Timer(TurnTimeLimitMs);
            timer.AutoReset = false;
            timer.Start();

            while (timer.Enabled)
            {
                string? input = ReadInputWithTimer(timer);

                if (input == null) return null;

                string word = input.Trim().ToLower();

                if (!IsValid(word, state.Language))
                {
                    Console.WriteLine(GetText(state.Language, "WordNotValid"));
                    continue;
                }

                if (!CanConstructWord(word, state.BaseWord))
                {
                    Console.WriteLine(GetText(state.Language, "WordConstruct"));
                    continue;
                }

                if (!IsUnique(word, state.BaseWord, state.UsedWords))
                {
                    Console.WriteLine(GetText(state.Language, "WordExist"));
                    continue;
                }

                timer.Stop();
                return word;
            }

            return null;
        }

        static string? ReadInputWithTimer(System.Timers.Timer timer)
        {
            string word = (Console.ReadLine() ?? "").Trim().ToLower();

            if (!timer.Enabled) return null;

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