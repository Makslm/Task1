using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Task1
{
    public class ScoreRepository
    {
        private readonly string _filePath = "scores.json";
        private Dictionary<string, Player> _players = new Dictionary<string, Player>();

        private void Load()
        {
            if (!File.Exists(_filePath)) return;

            try
            {
                string json = File.ReadAllText(_filePath);
                var loadedPlayers = JsonSerializer.Deserialize<Dictionary<string, Player>>(json);
                if (loadedPlayers != null) _players = loadedPlayers;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
            }
        }

        public void Save()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(_players, options);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении: {ex.Message}");
            }
        }

        public Player GetOrCreatePlayer(string name)
        {
            if (!_players.ContainsKey(name))
            {
                _players[name] = new Player(name);
            }
            return _players[name];
        }

        public IEnumerable<Player> GetAllPlayers()
        {
            return _players.Values;
        }
    }
}
