using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    public class GameCell
    {
        public Game.CellStates _State { get; set; }
        public bool _Selected { get; set; }

        public GameCell()
        {
            _Selected = false;
            _State = Game.CellStates.DISABLED;
        }
    }
}
