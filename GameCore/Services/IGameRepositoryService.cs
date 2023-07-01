using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCore.Services
{
    public interface IGameRepositoryService
    {

        public GameInstance? GetGameInstanceById(int id);

        public GameInstance NewGame();


    }
}
