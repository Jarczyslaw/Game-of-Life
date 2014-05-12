using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    public class BoardField
    {
        public int _Width { get; set; }
        public int _Height { get; set; }
        public int _X { get; set; }
        public int _Y { get; set; }
        public Color _Color { get; set; }

        public BoardField()
        {
            _Color = Color.Red;
        }

        public BoardField(int x,int y) : this()
        {
            _Width = x;
            _Height = y;            
        }
    }
}
