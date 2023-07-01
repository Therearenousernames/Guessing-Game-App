using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCore.Services
{
    public class GameRepositoryService : IGameRepositoryService
    {       
        public readonly static List<GameInstance> currentGames = new();

        public GameInstance NewGame()
        {
            GameInstance gameInstance = new();
            currentGames.Add(gameInstance);
            return gameInstance;
        }

        public GameInstance? GetGameInstanceById(int id)
        {
            return currentGames.FirstOrDefault(x => x.Id == id);

        }



    }
}
