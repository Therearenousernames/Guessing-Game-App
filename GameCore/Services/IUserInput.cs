using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCore.Services
{
    public interface IUserInput
    {
        string? ReadLine();

        ConsoleKeyInfo ReadKey();

    }
}
