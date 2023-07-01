using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GameCore.Services
{
    public class GameInstance
    {
       
        public GameInstance()
        {
           
            
            RandomNumber = new Random().Next(1, 21);
            Tries = 5;
        }

        public int Id { get; set; }

       

        public int RandomNumber { get; }

        public int Tries { get; private set;}


        public int DecrementTries()
        {
            return --Tries;
        }

    }
}
