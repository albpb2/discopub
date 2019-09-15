using Assets.Scripts.Extensions;
using Assets.Scripts.Importers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Game
{
    public class ActionsManager : Singleton<ActionsManager>
    {
        private List<Action> _actions;
        private List<Action> _drinks;

        public ActionsManager()
        {
            ImportActions();
            ImportDrinks();
        }

        public List<Action> GetSuffledActionsList()
        {
            return GetShuffledActionsList(_actions);
        }

        public List<Action> GetShuffledDrinksList()
        {
            return GetShuffledActionsList(_drinks);
        }

        private List<Action> GetShuffledActionsList(List<Action> list)
        {
            var serializedActions = JsonConvert.SerializeObject(list);
            var clonedActions = JsonConvert.DeserializeObject<List<Action>>(serializedActions);
            clonedActions.Shuffle();

            return clonedActions;
        }

        private void ImportActions()
        {
            _actions = ActionImporter.ImportActions("Config/Actions", true).ToList();
        }

        private void ImportDrinks()
        {
            _drinks = DrinkImporter.ImportDrinks("Config/Drinks", true).ToList();
        }
    }
}
