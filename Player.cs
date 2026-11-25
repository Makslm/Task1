namespace Task1
{
    public class Player
    {
        public string Name { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }

        public Player(string name, int wins = 0, int losses = 0)
        {
            Name = name;
            Wins = wins;
            Losses = losses;
        }

        public override string ToString()
        {
            return $"{Name} - Побед: {Wins}, Поражений: {Losses}";
        }
    }
}