using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCore.Services
{
    public class TransientInstance : ITransient
    {
        readonly static Random rand = new();

        readonly static int random = rand.Next(1, 21);

        static int tries = 5;

        public int GetRandom()
        {
            return random;
        }

        public int GetTries()
        {
            return --tries;
        }
    }
}
