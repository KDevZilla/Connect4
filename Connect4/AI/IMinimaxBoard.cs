using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4.AI
{
    interface IMinimaxBoard
    {
        List<Object> GenerateMoves();
        void Move(Object moveInfo);
        int Evaluate();
        void Undo();

    }
}
