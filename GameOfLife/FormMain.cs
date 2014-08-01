using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class FormMain : CustomForm
    {
        public CellPointer LeftClickLastPoint { get; set; }
        public CellPointer LeftClickTempPoint { get; set; }
        public CellPointer RightClickFirstPoint { get; set; }
        public CellPointer RightClickLastPoint { get; set; }

        public FormMain()
        {
            Debug.WriteLine("Main ID: " + System.Threading.Thread.CurrentThread.ManagedThreadId);
            Storage.MainForm = this;
            LeftClickLastPoint = new CellPointer();
            RightClickFirstPoint = new CellPointer();
            LeftClickTempPoint = new CellPointer();
            RightClickLastPoint = new CellPointer();
            Game MainGame = new Game(500, 500);
            MainGame.GenFixedBoard(1,1);
            Storage.Game = MainGame;

            FormSettings fs = new FormSettings();
            fs.InitForm();
            fs.Show();
            Storage.SettingsForm = fs;
            

            UiConfig();

            ParallelEngine e = new ParallelEngine();
            e.Init(2);
            e.AddRandomIter();
            Storage.Engine = e;


            Storage.SettingsForm.UpdateGameInfo(0);
            
        }

        public void UiConfig()
        {
            InitializeComponent();
            zoomBox1.SetUp();
            WindowState = FormWindowState.Maximized;
        }

        //private void Timer_Tick(object sender, EventArgs e)
        //{
        //    long temp = 1000;
        //    if (MainStopwatch.IsRunning)
        //    {
        //        MainStopwatch.Stop();
        //        temp = MainStopwatch.ElapsedMilliseconds;
        //    }
        //    MainStopwatch.Restart();
        //    MainTimer.Interval = TimerInterval;
        //    MainGame.NextStep();
        //    zoomBox1.LoadImg();
        //    UpdateTitle((1000f / temp).ToString("0.00"));                     
        //}

        public void RefreshBoard(Bitmap bmp)
        {
            zoomBox1.SetSource(bmp);
            zoomBox1.LoadImg();
        }

        //private void bufferedPanel1_MouseDown(object sender, MouseEventArgs e)
        //{
        //    if (MainGame.GameState == Game.GameStates.Paused)
        //    {
        //        if (e.Button == MouseButtons.Left)
        //        {
        //            if (MainGame.SelectedCells.Count != 0)
        //            {
        //                //MainGame.Deselect(true);
        //                MainGame.DrawBoard();
        //                //bufferedPanel1.Invalidate();
        //            }
        //            else
        //            {
        //                MainGame.DoHitTest(e.X, e.Y);
        //                if (MainGame.HitTest.IsFieldHit)
        //                {
        //                    //LeftClickLastPoint.SetCord(MainGame.HitTest.Coord);
        //                    //MainGame.ToggleCellState(MainGame.HitTest.Coord.I, MainGame.HitTest.Coord.J);
        //                    //MainGame.DrawField(MainGame.Board[MainGame.HitTest.Coord.I, MainGame.HitTest.Coord.J]);
        //                    //bufferedPanel1.Invalidate();

        //                }
        //            }
        //        }
        //        else if (e.Button == MouseButtons.Right)
        //        {
        //            MainGame.DoHitTest(e.X, e.Y);
        //            if (MainGame.HitTest.IsFieldHit)
        //            {
        //                RightClickFirstPoint.SetCord(MainGame.HitTest.Coord);
        //                RightClickLastPoint.SetCord(MainGame.HitTest.Coord);
        //                //MainGame.TakeSnapshot();
        //                //MainGame.DrawSelectionRectangle(RightClickFirstPoint, RightClickFirstPoint);
        //                //bufferedPanel1.Invalidate();
        //            }
        //        }
        //    }         
        //}

        

        //private void bufferedPanel1_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (MainGame.GameState == Game.GameStates.Paused)
        //    {
        //        if (e.Button == MouseButtons.Left)
        //        {
        //            MainGame.DoHitTest(e.X, e.Y);
        //            if (MainGame.HitTest.IsFieldHit)
        //            {
        //                //Debug.WriteLine("X: {0}, Y: {1},I: {2},J: {3}", e.X, e.Y, _game._HitTestInfo._I, _game._HitTestInfo._J);
        //                LeftClickTempPoint.SetCord(MainGame.HitTest.Coord);
        //                if (!LeftClickLastPoint.IsEqual(LeftClickTempPoint))
        //                {
        //                    //LeftClickLastPoint.SetCord(LeftClickTempPoint);
        //                    //MainGame.ToggleCellState(MainGame.HitTest.Coord.I, MainGame.HitTest.Coord.J);
        //                    //MainGame.DrawField(MainGame.Board[MainGame.HitTest.Coord.I, MainGame.HitTest.Coord.J]);
        //                    //bufferedPanel1.Invalidate();
        //                }
        //            }

        //        }
        //        else if (e.Button == MouseButtons.Right)
        //        {
        //            MainGame.DoHitTest(e.X, e.Y);
        //            if (MainGame.HitTest.IsFieldHit)
        //            {
        //                if (!RightClickLastPoint.IsEqual(MainGame.HitTest.Coord))
        //                {
        //                    //MainGame.RestoreSnapshot();
        //                    //MainGame.DrawSelectionRectangle(RightClickFirstPoint, MainGame.HitTest.Coord);
        //                    //bufferedPanel1.Invalidate();
        //                }
        //                RightClickLastPoint.SetCord(MainGame.HitTest.Coord);
        //                //bufferedPanel1.Invalidate();
        //            }

        //        }
        //    }
            
        //}

        //private void bufferedPanel1_MouseUp(object sender, MouseEventArgs e)
        //{
        //    if (MainGame.GameState == Game.GameStates.Paused)
        //    {
        //        if (e.Button == MouseButtons.Right)
        //        {
        //            MainGame.DoHitTest(e.X, e.Y);
        //            if (MainGame.HitTest.IsFieldHit)
        //            {
        //               // MainGame.SelectCellsInRange(RightClickFirstPoint, MainGame.HitTest.Coord);
        //               // MainGame.RestoreSnapshot();
        //            }
        //        }
        //    }         
        //}

        public void CreateContextMenu(int x,int y)
        {
            ContextMenu m = new ContextMenu();
            MenuItem it = new MenuItem("Game settings"); it.Click += new EventHandler(ContextMenu_ShowSettings); m.MenuItems.Add(it);
            m.MenuItems.Add("-");
            m.MenuItems.Add(new MenuItem("Display settings"));
            it = new MenuItem("Zoom in"); it.Click += new EventHandler(ContextMenu_ZoomIn); m.MenuItems[m.MenuItems.Count-1].MenuItems.Add(it);
            it = new MenuItem("Zoom out"); it.Click += new EventHandler(ContextMenu_ZoomOut); m.MenuItems[m.MenuItems.Count - 1].MenuItems.Add(it);
            it = new MenuItem("Reset zoom"); it.Click += new EventHandler(ContextMenu_ResetZoom); m.MenuItems[m.MenuItems.Count - 1].MenuItems.Add(it);
            it = new MenuItem("Fit board to screen"); it.Click += new EventHandler(ContextMenu_FitToScreen); m.MenuItems[m.MenuItems.Count - 1].MenuItems.Add(it);
            it = new MenuItem("Set default cell size"); it.Click += new EventHandler(ContextMenu_SetCellSize); m.MenuItems[m.MenuItems.Count - 1].MenuItems.Add(it);
            m.MenuItems.Add("-");
            m.MenuItems.Add(new MenuItem("Simulation"));
            it = new MenuItem("Next step"); it.Click += new EventHandler(ContextMenu_NextStep); m.MenuItems[m.MenuItems.Count - 1].MenuItems.Add(it);
            it = new MenuItem("Start"); it.Click += new EventHandler(ContextMenu_Start); m.MenuItems[m.MenuItems.Count - 1].MenuItems.Add(it);
            it = new MenuItem("Stop"); it.Click += new EventHandler(ContextMenu_Stop); m.MenuItems[m.MenuItems.Count - 1].MenuItems.Add(it);
            m.MenuItems.Add("-");
            it = new MenuItem("Close"); it.Click += new EventHandler(ContextMenu_Close); m.MenuItems.Add(it);
            m.Show(this, new Point(x, y));
        }

        private void ContextMenu_ShowSettings(object sender, EventArgs e)
        {
            Storage.SettingsForm.Show();
            Storage.SettingsForm.Focus();
        }

        private void ContextMenu_ZoomIn(object sender, EventArgs e)
        {
            zoomBox1.ZoomIn();
            zoomBox1.LoadImg();
        }

        private void ContextMenu_ZoomOut(object sender, EventArgs e)
        {
            zoomBox1.ZoomOut();
            zoomBox1.LoadImg();
        }

        private void ContextMenu_ResetZoom(object sender, EventArgs e)
        {
            zoomBox1.Zoom = 1;
            zoomBox1.LoadImg();
        }
        
        private void ContextMenu_FitToScreen(object sender, EventArgs e)
        {
        }

        private void ContextMenu_SetCellSize(object sender, EventArgs e)
        {
        }

        private void ContextMenu_NextStep(object sender, EventArgs e)
        {
            Storage.Engine.RunSingleStep();
        }

        private void ContextMenu_Start(object sender, EventArgs e)
        {
            Storage.Engine.RunSimulation();
        }

        private void ContextMenu_Stop(object sender, EventArgs e)
        {
            Storage.Engine.Stop();
        }

        private void ContextMenu_Close(object sender, EventArgs e)
        {
            Close() ;
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void FormMain_Load(object sender, EventArgs e)
        {

        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            Storage.SettingsForm.Show();
            Storage.SettingsForm.Focus();
        }

        
    }
}
