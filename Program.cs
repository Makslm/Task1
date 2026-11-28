using System;

namespace Task1
{
    internal class Program
    {
        private const int MinLengthBaseWord = 8;
        private const int MaxLengthBaseWord = 30;

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            string language = Localization.SelectLanguage();
            var repository = new ScoreRepository();

            Console.WriteLine("Введите имя первого игрока:");
            string p1Name = (Console.ReadLine() ?? "").Trim();
            if (string.IsNullOrWhiteSpace(p1Name)) p1Name = "Player 1";

            Console.WriteLine("Введите имя второго игрока:");
            string p2Name = (Console.ReadLine() ?? "").Trim();
            if (string.IsNullOrWhiteSpace(p2Name)) p2Name = "Player 2";

            Player player1 = repository.GetOrCreatePlayer(p1Name);
            Player player2 = repository.GetOrCreatePlayer(p2Name);

            string baseWord = SetBaseWord(language);

            var session = new GameSession(baseWord, language, player1, player2);

            Console.WriteLine($"\nИгра началась! {player1.Name} VS {player2.Name}");

            session.Start();

            repository.Save();
            Console.WriteLine("\nИгра окончена. Статистика сохранена. Нажмите любую клавишу...");
            Console.ReadKey();
        }

        static string SetBaseWord(string language)
        {
            Console.WriteLine(Localization.GetText(language, "EnterBaseWord"));
            string baseWord = (Console.ReadLine() ?? "").Trim().ToLower();

            if (baseWord.Length < MinLengthBaseWord || baseWord.Length > MaxLengthBaseWord)
            {
                Console.WriteLine(Localization.GetText(language, "InvalidBaseWord"));
                return SetBaseWord(language);
            }

            return baseWord;
        }
    }
}