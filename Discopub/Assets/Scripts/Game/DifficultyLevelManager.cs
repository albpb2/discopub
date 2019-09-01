using Assets.Scripts.Importers;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Game
{
    public class DifficultyLevelManager
    {
        private List<GameDifficulty> _difficultyLevels;

        public DifficultyLevelManager()
        {
            ImportDifficultyLevels();
        }

        public GameDifficulty GetDifficultyLevel(int roundNumber)
        {
            return _difficultyLevels.Single(d =>
               (!d.MinRound.HasValue || roundNumber >= d.MinRound.Value)
               && (!d.MaxRound.HasValue || roundNumber <= d.MaxRound.Value));
        }

        private void ImportDifficultyLevels()
        {
            _difficultyLevels = GameDifficultyImporter.ImportGamedifficultyLevels("Config/DifficultyLevels", true).ToList();
        }
    }
}
