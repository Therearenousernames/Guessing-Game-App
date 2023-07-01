using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCore.Services
{
    public interface ITransient
    {
        public int GetTries();

        public int GetRandom();
    }
}
