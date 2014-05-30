using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    public class Game
    {
        public enum GameStates { Paused,Run}
        public GameStates GameState{ get; set; }
        public Cell[,] NewIteration { get; set; }
        public List<Cell[,]> Iterations { get; set; }
        public Rectangle[,] Board { get; set; }
        public List<CellState> CellStates { get; set; }
        public int GameRows { get; set; }
        public int GameCols { get; set; }
        public int BorderSize { get; set; }
        public HitTestInfo HitTest { get; set; }
        public List<CellPointer> SelectedCells { get; set; }

        public class HitTestInfo
        {
            public CellPointer Coord { get; set; }
            public bool IsFieldHit { get; set; }
            public HitTestInfo()
            {
                IsFieldHit = false;
                Coord = new CellPointer();
            }
        }

        public Game(int r, int c)
        {
            GameRows = r;
            GameCols = c;
            BorderSize = 1;
            HitTest = new HitTestInfo();
            SelectedCells = new List<CellPointer>();
            GameState = GameStates.Paused;
            Iterations = new List<Cell[,]>();
            Board = InitializeArray<Rectangle>(GameRows, GameCols);
            CellStates = new List<CellState>() { 
                new CellState("Dead", Color.White),
                new CellState("Alive", Color.Blue)
            };
        }

        public void StartNewIteration()
        {
            NewIteration = InitializeArray<Cell>(GameRows, GameCols);
        }

        public void UpdateNewIteration(int startCol,int endCol)
        {
            if (startCol == -1 || endCol == -1)
            {
                startCol = 0;
                endCol = GameCols - 1;
            }    
            Cell[,] old = Iterations.Last();
            for (int i = 0; i < GameRows; i++)
            {
                for (int j = startCol; j <= endCol; j++)
                {
                    int num = 0;
                    int[] temp = Neightbourhood(old, i, j);
                    for (int k = 0; k < temp.Length; k++)
                    {
                        if (temp[k] != -1)
                        {
                            if (temp[k] == 1)
                            {
                                num++;
                            }
                        }
                    }
                    if (old[i, j].State == 1)
                    {
                        if (num == 2 || num == 3)
                        {
                            NewIteration[i, j].State = 1;
                        }
                        else
                        {
                            NewIteration[i, j].State = 0;
                        }
                    }
                    else
                    {
                        if (num == 3)
                        {
                            NewIteration[i, j].State = 1;
                        }
                        else
                        {
                            NewIteration[i, j].State = 0;
                        }
                    }
                }
            }
        }

        public void DrawIteration(Graphics g,int width,int height, Cell[,] cells,int startCol, int endCol)
        {
            if (startCol == -1 || endCol == -1)
            {
                startCol = 0;
                endCol = GameCols - 1;
            }
            DrawBackground(g, width,height);
            // because SolidBrush can't be shared between thread there's a need to create local deep copy od cell states pallete
            SolidBrush[] pallete = new SolidBrush[CellStates.Count];
            for (int i = 0; i < CellStates.Count;i++)
            {
                pallete[i] = new SolidBrush(CellStates[i].BoardColor);
            }
            for (int i = 0; i < GameRows; i++)
            {
                for (int j = startCol; j <= endCol; j++)
                {
                    DrawField(g, pallete, cells[i, j], Board[i,j]);
                }
            }
            
        }

        public void EndIteration()
        {
            Iterations.Add(NewIteration);
            NewIteration = null;
        }

        public int[] Neightbourhood(Cell[,] cells,int i,int j)
        {
            int[] res = new int[9];
            int counter = 0;
            for (int k = i - 1; k <= i + 1;k++ )
            {
                for(int l = j - 1;l <= j + 1;l++)
                {
                    if (k >= 0 && (k <= GameRows - 1) && l >= 0 && (l <= GameCols - 1) && !(k == i && l == j))
                    {
                        res[counter] = cells[k, l].State;
                    }
                    else
                    {
                        res[counter] = -1;
                    }
                    counter++;
                }
            }
            return res;
        }



        public void Pause()
        {
            GameState = GameStates.Paused;
            //f_TakeSnapshot();          
            //SolidBrush shadow = new SolidBrush(Color.FromArgb(128,136,136,136));
            //Rectangle rect = new Rectangle(0, 0, m_graphicsWidth, m_graphicsHeight);
            //m_g.FillRectangle(shadow,rect);
                     
        }

        //public void Deselect(bool all)
        //{
        //    if (all)
        //    {
        //        for (int i = 0;i < GameRows;i++)
        //        {
        //            for (int j = 0;j < GameCols;j++)
        //            {
        //                Cells[i, j].IsSelected = false;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        foreach(CellPointer cp in SelectedCells)
        //        {
        //            Cells[cp.I, cp.J].IsSelected = false;
        //        }
        //    }
        //}

        //public void SelectCellsInRange(CellPointer start, CellPointer end)
        //{
        //    SelectedCells.Clear();   
        //    int startI,endI;
        //    int startJ,endJ;
        //    if (end.I - start.I > 0)
        //    {
        //        startI = start.I;
        //        endI = end.I;
        //    }
        //    else
        //    {
        //        startI = end.I;
        //        endI = start.I;
        //    }
        //    if (end.J - start.J > 0)
        //    {
        //        startJ = start.J;
        //        endJ = end.J;
        //    }
        //    else
        //    {
        //        startJ = end.J;
        //        endJ = start.J;
        //    }
        //    for (int i = startI;i <= endI;i++)
        //    {
        //        for(int j = startJ;j <= endJ;j++)
        //        {
        //            SelectedCells.Add(new CellPointer(i, j));
        //            Cells[i, j].IsSelected = true;
        //        }
        //    }
        //}

        //public void ToggleCellState(int i,int j)
        //{
        //    if (Cells[i, j].State == 0)
        //    {
        //        Cells[i, j].State = 1;
        //    }
        //    else if (Cells[i, j].State == 1)
        //    {
        //        Cells[i, j].State = 0;
        //    }
        //}

        public void DoHitTest(int x,int y)
        {
            Rectangle bf;
            HitTest.Coord.SetCord(-1, -1);
            HitTest.IsFieldHit = false;
            for (int i = 0; i < GameRows; i++)
            {
                bf = Board[i, 0];
                if (y >= bf.Y && y <= (bf.Y + bf.Height))
                {
                    for (int j = 0;j < GameCols;j++)
                    {
                        bf = Board[i, j];
                        if (x >= bf.X && x <= (bf.X + bf.Width))
                        {
                            HitTest.Coord.SetCord(i, j);
                            HitTest.IsFieldHit = true;
                            i = GameRows;
                            break;
                        }
                    }
                }
            }
        }

        public void DrawBackground(Graphics g,int width, int height)
        {
            SolidBrush background = new SolidBrush(Color.Black);
            g.FillRectangle(background, new Rectangle(0, 0, width, height));
        }

        //public void Reset()
        //{
        //    for (int i = 0;i < GameRows;i++)
        //    {
        //        for (int j = 0;j < GameCols;j++)
        //        {
        //            Cells[i, j].State = 0;
        //        }
        //    }
        //    DrawBoard();
        //}

        //public void DrawSelectionRectangle(CellPointer cp1, CellPointer cp2)
        //{
        //    int startI,endI;
        //    int startJ,endJ;
        //    Rectangle rect = new Rectangle();
        //    if (cp1.I >= cp2.I)
        //    {
        //        startI = cp2.I;
        //        endI = cp1.I;
        //    }
        //    else
        //    {
        //        startI = cp1.I;
        //        endI = cp2.I;
        //    }
        //    if (cp1.J >= cp2.J)
        //    {
        //        startJ = cp2.J;
        //        endJ = cp1.J;
        //    }
        //    else
        //    {
        //        startJ = cp1.J;
        //        endJ = cp2.J;
        //    }
        //    rect.Width = (Cells[endI, endJ].Sizes.X + Cells[endI, endJ].Sizes.Width + BorderSize) - (Cells[startI, startJ].Sizes.X - BorderSize);
        //    rect.Height = (Cells[endI, endJ].Sizes.Y + Cells[endI, endJ].Sizes.Height + BorderSize) - (Cells[startI, startJ].Sizes.Height - BorderSize);
        //    rect.X = Cells[startI, startJ].Sizes.X - BorderSize;
        //    rect.Y = Cells[startI, startJ].Sizes.Y - BorderSize;
        //    SolidBrush shadow = new SolidBrush(Color.FromArgb(128, 136, 136, 136));
        //    BaseGraphics.FillRectangle(shadow, rect);
        //}

        public void DrawField(Graphics g, SolidBrush[] b, Cell c, Rectangle r)
        {
            g.FillRectangle(b[c.State], r);
        }    
       
        public Bitmap GenFitBoard(int boardWidth,int boardHeight)
        {
            int[] widths = FitFields(boardWidth, GameCols, BorderSize);
            int[] heights = FitFields(boardHeight, GameRows, BorderSize);
            int currWidth = BorderSize;
            int currHeight = BorderSize;
            for (int i = 0; i < GameRows; i++)
            {
                currWidth = BorderSize;
                for (int j = 0;j < GameCols;j++)
                {
                    Board[i,j] = new Rectangle(currWidth, currHeight, widths[j], heights[i]);
                    currWidth += widths[j] + BorderSize;
                }
                currHeight += heights[i] + BorderSize;
            }
            Bitmap bmp = new Bitmap(boardWidth, boardHeight);
            using(Graphics g = Graphics.FromImage(bmp))
            {
                DrawBackground(g, boardWidth, boardHeight);
            }
            return bmp;
        }

        public Bitmap GenFixedBoard(int cellWidth, int cellHeight)
        {
            int currWidth = BorderSize;
            int currHeight = BorderSize;
            for (int i = 0;i < GameRows;i++)
            {
                currWidth = BorderSize;
                for (int j = 0;j < GameCols;j++)
                {
                    Board[i, j] = new Rectangle(currWidth, currHeight, cellWidth, cellHeight);
                    currWidth += cellWidth + BorderSize;
                }
                currHeight += cellHeight + BorderSize;
            }
            int boardWidth = (cellWidth + BorderSize) * GameCols + BorderSize;
            int boardHeight = (cellHeight + BorderSize) * GameRows + BorderSize;
            Bitmap bmp = new Bitmap(boardWidth, boardHeight);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                DrawBackground(g, boardWidth, boardHeight);
            }
            return bmp;
        }

        public T[,] InitializeArray<T>(int rows, int cols) where T : new()
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

        public int[] FitFields(int imageWidth,int fieldsCount,int borderSize)
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
