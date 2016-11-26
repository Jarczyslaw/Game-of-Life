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
        public Cell[,] Current { get; set; }
        public List<Cell[,]> Generations { get; set; }
        public int Iterations { get; set; }
        
        public Dictionary<byte,CellState> CellStates { get; set; }
        public int GameRows { get; set; }
        public int GameCols { get; set; }
        public int BorderSize { get; set; }

        public Game(int r, int c)
        {
            GameRows = r;
            GameCols = c;
            BorderSize = 1;
            Iterations = 0;
           
            GameState = GameStates.Paused;
            Generations = new List<Cell[,]>();
            
            Current = Storage.InitializeArray<Cell>(GameRows, GameCols);
            CellStates = new Dictionary<byte,CellState>();
            CellStates.Add(0,new CellState("Dead",Color.White));
            CellStates.Add(1, new CellState("Alive", Color.BlueViolet));
        }

        public void StartNewIteration()
        {
            Current = Storage.InitializeArray<Cell>(GameRows, GameCols); 
        }

        public void UpdateNewIteration(int startCol,int endCol)
        {
            Cell[,] Previous = Generations.Last();
            if (startCol == -1 || endCol == -1)
            {
                startCol = 0;
                endCol = GameCols - 1;
            }    
            for (int i = 0; i < GameRows; i++)
            {
                for (int j = startCol; j <= endCol; j++)
                {
                    int num = 0;
                    int[] temp = Neightbourhood(Previous, i, j);
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
                    if (Previous[i, j].State == 1)
                    {
                        if (num == 2 || num == 3)
                        {
                            Current[i, j].State = 1;
                        }
                        else
                        {
                            Current[i, j].State = 0;
                        }
                    }
                    else
                    {
                        if (num == 3)
                        {
                            Current[i, j].State = 1;
                        }
                        else
                        {
                            Current[i, j].State = 0;
                        }
                    }
                }
            }
        }

        public void EndIteration()
        {
            Iterations++;
            Generations.Add(Current);
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

        

        
        
        

        

        
    }
}
