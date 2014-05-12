using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    public class CellPointer
    {
        public int _I { get; set; }
        public int _J { get; set; }

        public CellPointer()
        {

        }

        public CellPointer(int i,int j)
        {
            _I = i;
            _J = j;
        }

        public bool f_Equals(CellPointer cp)
        {
            if (_I == cp._I && _J == cp._J)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void m_SetCord(CellPointer cp)
        {
            _I = cp._I;
            _J = cp._J;
        }

    }
}
