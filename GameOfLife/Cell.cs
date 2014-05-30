using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    public class Cell
    {
        public int State { get; set; }

        public Cell()
        {
            State = 0;
        }
    }
}
