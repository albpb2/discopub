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

        public ActionsManager()
        {
            ImportActions();
        }

        public List<Action> GetShuffledActionsList()
        {
            var serializedActions = JsonConvert.SerializeObject(_actions);
            var clonedActions = JsonConvert.DeserializeObject<List<Action>>(serializedActions);
            clonedActions.Shuffle();

            return clonedActions;
        }

        private void ImportActions()
        {
            _actions = ActionImporter.ImportActions("Config/Actions", true).ToList();
        }
    }
}
