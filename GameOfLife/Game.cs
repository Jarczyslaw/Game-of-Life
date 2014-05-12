using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    public class Game
    {
        public enum CellStates { DISABLED = 0,ENABLED = 1}
        public BoardField[,] _Board { get; set; }
        public GameCell[,] _Cells { get; set; }
        public Bitmap _Canvas { get; set; }
        public Bitmap _Snapshot { get; set; }
        public int _GameRows { get; set; }
        public int _GameCols { get; set; }
        public int _GraphicsWidth { get; set; }
        public int _GraphicsHeight { get; set; }
        public int _BorderSize { get; set; }
        public HitTestInfo _HitTestInfo { get; set; }
        public List<CellPointer> _LastSelected { get; set; }

        public class HitTestInfo
        {
            public int _I { get; set; }
            public int _J { get; set; }
            public bool _FieldHit { get; set; }
            public HitTestInfo()
            {
                _FieldHit = false;
                _I = -1;
                _J = -1;
            }
        }

        public void m_TakeSnapshot()
        {
            
        }

        public Game(int gameRows,int gameCols)
        {
            _GameRows = gameRows;
            _GameCols = gameCols;
            _BorderSize = 1;
            _HitTestInfo = new HitTestInfo();
            _LastSelected = new List<CellPointer>();
            _Board = f_InitializeArray<BoardField>(_GameRows, _GameCols);
            _Cells = f_InitializeArray<GameCell>(_GameRows, _GameCols);
        }

        public void m_SetBitmap(Bitmap bmp)
        {
            _Canvas = bmp;
            _GraphicsWidth = x;
            _GraphicsHeight = y;
        }

        public void m_Pause()
        {
            m_TakeSnapshot();
            SolidBrush shadow = new SolidBrush(Color.FromArgb(128,136,136,136));
            Rectangle rect = new Rectangle(0, 0, _GraphicsWidth, _GraphicsHeight);
            _G.FillRectangle(shadow,rect);
        }

        public void m_Deselect(bool all)
        {
            if (all)
            {
                for (int i = 0;i < _GameRows;i++)
                {
                    for (int j = 0;j < _GameCols;j++)
                    {
                        _Cells[i, j]._Selected = false;
                    }
                }
            }
            else
            {
                foreach(CellPointer cp in _LastSelected)
                {
                    _Cells[cp._I, cp._J]._Selected = false;
                }
            }
        }

        public void m_SelectCellsInRange(CellPointer start, CellPointer end)
        {
            _LastSelected.Clear();   
            int startI,endI;
            int startJ,endJ;
            if (end._I - start._I > 0)
            {
                startI = start._I;
                endI = end._I;
            }
            else
            {
                startI = end._I;
                endI = start._I;
            }
            if (end._J - start._J > 0)
            {
                startJ = start._J;
                endJ = end._J;
            }
            else
            {
                startJ = end._J;
                endJ = start._J;
            }
            for (int i = startI;i <= endI;i++)
            {
                for(int j = startJ;j <= endJ;j++)
                {
                    _LastSelected.Add(new CellPointer(i, j));
                    _Cells[i, j]._Selected = true;
                }
            }
        }

        public void m_ToggleCellState(int i, int j)
        {
            if (_Cells[i, j]._State == Game.CellStates.ENABLED)
            {
                _Cells[i, j]._State = Game.CellStates.DISABLED;
            }
            else if (_Cells[i, j]._State == Game.CellStates.DISABLED)
            {
                _Cells[i, j]._State = Game.CellStates.ENABLED;
            }
            m_UpdateBoardField(i, j);
            m_DrawField(i, j);
        }

        public void m_UpdateBoardField(int i,int j)
        {
            if (_Cells[i, j]._Selected)
            {
                _Board[i, j]._Color = Color.Teal;
            }
            else
            {
                if (_Cells[i, j]._State == CellStates.ENABLED)
                {
                    _Board[i, j]._Color = Color.Green;
                }
                else if (_Cells[i, j]._State == CellStates.DISABLED)
                {
                    _Board[i, j]._Color = Color.Red;
                }
            }          
        }

        public void m_HitTest(int x,int y)
        {
            BoardField bf;
            _HitTestInfo._I = -1;
            _HitTestInfo._J = -1;
            _HitTestInfo._FieldHit = false;
            for (int i = 0; i < _GameRows; i++)
            {
                bf = _Board[i,0];
                if (y >= bf._Y && y <= (bf._Y + bf._Height))
                {
                    for (int j = 0;j < _GameCols;j++)
                    {
                        bf = _Board[i,j];
                        if (x >= bf._X && x <= (bf._X + bf._Width))
                        {                          
                            _HitTestInfo._I = i;
                            _HitTestInfo._J = j;
                            _HitTestInfo._FieldHit = true;
                            i = _GameRows;
                            break;
                        }
                    }
                }
            }
        }

        public void m_DrawBoard(bool newSize)
        {            
            if (newSize)
            {
                m_GenBoard();
            }                    
            SolidBrush background = new SolidBrush(Color.Black);
            _G.FillRectangle(background, new Rectangle(0, 0, _GraphicsWidth, _GraphicsHeight));
            for (int i = 0; i < _GameRows; i++)
            {
                for (int j = 0; j < _GameCols; j++)
                {
                    m_UpdateBoardField(i, j);
                    m_DrawField(i, j);
                }
            }
        }

        public void m_DrawField(int i,int j)
        {
            Rectangle rect = f_GetFieldRect(i, j);
            BoardField bd = _Board[i, j];
            _G.FillRectangle(new SolidBrush(bd._Color), rect);         
        }

        public Rectangle f_GetFieldRect(int i,int j)
        {
            Rectangle rect = new Rectangle();
            BoardField bd = _Board[i, j];
            rect.Width = bd._Width;
            rect.Height = bd._Height;
            rect.X = bd._X;
            rect.Y = bd._Y;
            return rect;
        }

        public void m_GenBoard()
        {
            int[] widths = f_FitFields(_GraphicsWidth, _GameCols, _BorderSize);
            int[] heights = f_FitFields(_GraphicsHeight, _GameRows, _BorderSize);
            int currWidth = _BorderSize;
            int currHeight = _BorderSize;
            for (int i = 0; i < _GameRows; i++)
            {
                currWidth = _BorderSize;
                for (int j = 0;j < _GameCols;j++)
                {
                    _Board[i, j]._Width = widths[j];
                    _Board[i, j]._Height = heights[i];
                    _Board[i, j]._X = currWidth;
                    _Board[i, j]._Y = currHeight;
                    currWidth += widths[j] + _BorderSize;
                }
                currHeight += heights[i] + _BorderSize;
            }
        }

        T[,] f_InitializeArray<T>(int rows,int cols) where T : new()
        {
            T[,] array = new T[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols;j++ )
                {
                    array[i, j] = new T();
                }             
            }
            return array;
        }

        public int[] f_FitFields(int imageWidth,int fieldsCount,int borderSize)
        {
            int[] widths = new int[fieldsCount];
            int fieldsWidth = imageWidth - (fieldsCount + 1) * borderSize;
            int basicWidth = fieldsWidth / fieldsCount;
            float additionalWidth = (float)fieldsWidth / fieldsCount - (float)Math.Floor((float)fieldsWidth / fieldsCount);
            float rest = 0;
            int summWidth = borderSize;
            int tempWidth;
            for (int i = 0;i < fieldsCount;i++)
            {
                if (i == fieldsCount - 1)
                {
                    tempWidth = imageWidth - borderSize - summWidth;
                }
                else
                {
                    if (rest >= 1)
                    {
                        tempWidth = basicWidth + 1;
                        rest--;
                    }
                    else
                    {
                        tempWidth = basicWidth;
                    }
                    rest += additionalWidth;
                }                
                widths[i] = tempWidth;
                summWidth += tempWidth + borderSize;                
            }
            return widths;
        }
    }
}
