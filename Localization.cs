using System;

namespace Task1
{
    public static class Localization
    {
        public static string SelectLanguage()
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

        public static string GetText(string lang, string key) => (lang, key) switch
        {
            ("ru", "EnterBaseWord") => "Введите исходное слово (от 8 до 30 букв):",
            ("eng", "EnterBaseWord") => "Enter the base word (8 to 30 letters):",

            ("ru", "BaseWord") => "Базовое слово",
            ("eng", "BaseWord") => "Base word",

            ("ru", "InvalidBaseWord") => "Неверная длина",
            ("eng", "InvalidBaseWord") => "Invalid length",

            ("ru", "WordNotValid") => "Слово не корректно",
            ("eng", "WordNotValid") => "Word is not valid",

            ("ru", "EnterWord") => "Введите слово. Очередь игрока",
            ("eng", "EnterWord") => "Enter word. Player's turn",

            ("ru", "Success") => "Успех. Слово было добавлено",
            ("eng", "Success") => "Success. Word has been added",

            ("ru", "WordExist") => "Слово уже было введено",
            ("eng", "WordExist") => "Word has already been written",

            ("ru", "WordConstruct") => "Слово должно состоять из букв базового слова",
            ("eng", "WordConstruct") => "Word must consist of letters from base word",

            ("ru", "TimeOut") => "Время вышло!",
            ("eng", "TimeOut") => "Time is up!",

            ("ru", "Winner") => "Победил Игрок",
            ("eng", "Winner") => "Winner is Player",

            ("ru", "Score") => "Счет игрока",
            ("eng", "Score") => "Player score",

            ("ru", "TotalScore") => "Общий счет всех игроков",
            ("eng", "TotalScore") => "Total score of all players",

            ("ru", "ShowWords") => "Слова в текущей игре",
            ("eng", "ShowWords") => "Words in current game",

            _ => key
        };
    }
}