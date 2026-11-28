using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;

namespace Task1
{
    public class GameSession
    {
        public string BaseWord { get; init; }
        public string Language { get; init; }
        public Player Player1 { get; init; }
        public Player Player2 { get; init; }
        public List<string> UsedWords { get; set; }
        public Player CurrentPlayer { get; set; }

        private const int TurnTimeLimitMs = 15000;

        public GameSession(string baseWord, string language, Player player1, Player player2)
        {
            BaseWord = baseWord;
            Language = language;
            Player1 = player1;
            Player2 = player2;
            UsedWords = new List<string>();
            CurrentPlayer = Player1;
        }

        public void Start()
        {
            while (true)
            {
                Console.WriteLine($"\n{Localization.GetText(Language, "WordsList")}: [{string.Join(", ", UsedWords)}]");
                Console.WriteLine($"{Localization.GetText(Language, "BaseWord")}: {BaseWord}");

                string? validWord = GetWordWithTimer();

                if (validWord == null)
                {
                    Player winner = (CurrentPlayer == Player1) ? Player2 : Player1;
                    CurrentPlayer.Losses++;
                    winner.Wins++;

                    Console.WriteLine(Localization.GetText(Language, "TimeOut"));
                    Console.WriteLine($"{Localization.GetText(Language, "Winner")} {winner.Name}!");
                    return;
                }

                UsedWords.Add(validWord);
                CurrentPlayer = (CurrentPlayer == Player1) ? Player2 : Player1;

                Console.WriteLine(Localization.GetText(Language, "Success"));
            }
        }

        private string? GetWordWithTimer()
        {
            bool isTimeOut = false;

            using var timer = new System.Timers.Timer(TurnTimeLimitMs);
            timer.AutoReset = false;
            timer.Elapsed += (_, _) => isTimeOut = true;
            timer.Start();

            while (!isTimeOut)
            {
                Console.WriteLine($"{Localization.GetText(Language, "EnterWord")} {CurrentPlayer.Name}");

                string? input = ReadLineWithTimer(timer);
                if (input == null) return null;

                string word = input.Trim().ToLower();

                if (word.StartsWith("/"))
                {
                    continue;
                }

                if (!IsValid(word))
                {
                    Console.WriteLine(Localization.GetText(Language, "WordNotValid"));
                    continue;
                }

                if (!CanConstructWord(word))
                {
                    Console.WriteLine(Localization.GetText(Language, "WordConstruct"));
                    continue;
                }

                if (!IsUnique(word))
                {
                    Console.WriteLine(Localization.GetText(Language, "WordExist"));
                    continue;
                }

                timer.Stop();
                return word;
            }

            return null;
        }

        private string? ReadLineWithTimer(System.Timers.Timer timer)
        {
            var buffer = new StringBuilder();
            while (timer.Enabled)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(intercept: true);
                    if (key.Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine();
                        return buffer.ToString();
                    }
                    if (key.Key == ConsoleKey.Backspace && buffer.Length > 0)
                    {
                        buffer.Length--;
                        Console.Write("\b \b");
                    }
                    else if (!char.IsControl(key.KeyChar))
                    {
                        buffer.Append(key.KeyChar);
                        Console.Write(key.KeyChar);
                    }
                }
                else
                {
                    Thread.Sleep(20);
                }
            }
            return null;
        }

        private bool IsValid(string word) =>
            !string.IsNullOrEmpty(word) && Language switch
            {
                "ru" => word.All(c => (c >= 'а' && c <= 'я') || c == 'ё'),
                "eng" => word.All(c => c >= 'a' && c <= 'z'),
                _ => false
            };

        private bool IsUnique(string word) =>
            word != BaseWord && !UsedWords.Contains(word);

        private bool CanConstructWord(string word) =>
            word.Length <= BaseWord.Length && word.Distinct().All(c => word.Count(x => x == c) <= BaseWord.Count(x => x == c));
    }
}