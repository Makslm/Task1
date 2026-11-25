namespace Task1
{
    internal class GameSession
    {
        public string BaseWord { get; init; }
        public string Language { get; init; }
        public Player Player1 { get; init; }
        public Player Player2 { get; init; }

        public List<string> UsedWords { get; set; }
        public Player CurrentPlayer { get; set; }
       

        public GameSession(string baseWord, string language, Player player1, Player player2)
        {
            BaseWord = baseWord;
            Language = language;
            Player1 = player1;
            Player2 = player2;
            UsedWords = new List<string>();
            CurrentPlayer = Player1;
        }
    }
}
