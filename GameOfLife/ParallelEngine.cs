using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace GameOfLife
{
    public class ParallelEngine
    {
        public class Segment
        {
            public Bitmap Map { get; set; }
            public int ColumnsCount { get; set; }
            public int ColumnsOffset { get; set; }
            public int MapWidth { get; set; }
            public int MapOffset { get; set; }
        }

        public BackgroundWorker[] Workers { get; set; }
        public BackgroundWorker LaunchWorker { get; set; }
        public Bitmap Map { get; set; }
        public Segment[] Segments { get; set; }
        public bool ContinousWork { get; set; }
        public Stopwatch Timer { get; set; }
        public long Interval { get; set; }
        ManualResetEvent[] Finished { get; set; }
        public int ThreadsCount  { get; set; }
        public delegate void LogTextCallback(string txt);
        public delegate void ResultCallback(Bitmap bmp);
        public delegate void InfoUpdateCallback(float f);

        public ParallelEngine()
        {
            Timer = new Stopwatch();
            Interval = 1000;
        }

        public void SetEventVal(bool val)
        {
            for (int i = 0; i < ThreadsCount; i++)
                Finished[i] = new ManualResetEvent(val);
        }

        public void Init(int count)
        {
            ThreadsCount = count;
            Workers = new BackgroundWorker[count];
            Finished = new ManualResetEvent[count];
            SetEventVal(false);
            LaunchWorker = new BackgroundWorker();
            LaunchWorker.DoWork += new DoWorkEventHandler(ThreadsExecution);
            LaunchWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LaunchComplete);
            for (int i = 0; i < count;i++ )
            {
                Workers[i] = new BackgroundWorker();
                Workers[i].DoWork += new DoWorkEventHandler(DoWork);
            }
            // init graphics
            Map = Storage.Game.GenFixedBoard(1, 1);
            Segments = new Segment[count];
            // count segments columns width and offsets
            int[] segmentsCols = new int[count];
            int cols = Storage.Game.GameCols / count;
            for (int i = 0; i < count - 1; i++)
            {
                segmentsCols[i] = cols;
            }
            segmentsCols[count - 1] = Storage.Game.GameCols - (count - 1) * cols;
            int[] colsOffsets = CountOffsets(segmentsCols);
            // count segments map widths and offsets
            int startX = 0;
            int endX = 0;
            int[] mapsWidth = new int[count];
            for (int i = 0; i < Segments.Length; i++)
            {
                startX = Storage.Game.Board[0, colsOffsets[i]].X - Storage.Game.BorderSize;
                endX = Storage.Game.Board[0, colsOffsets[i] + segmentsCols[i] - 1].X + Storage.Game.Board[0, colsOffsets[i] + segmentsCols[i] - 1].Width;
                mapsWidth[i] = endX - startX;
                if (i == Segments.Length - 1)
                    mapsWidth[i] += Storage.Game.BorderSize;
            }
            int[] mapsOffsets = CountOffsets(mapsWidth);
            // finally create segments;
            for (int i = 0; i < count; i++)
            {
                Segment s = new Segment();
                s.ColumnsCount = segmentsCols[i];
                s.ColumnsOffset = colsOffsets[i];
                s.Map = new Bitmap(mapsWidth[i], Map.Height);
                s.MapWidth = mapsWidth[i];
                s.MapOffset = mapsOffsets[i];
                Segments[i] = s;
            }
        }

        public void RunSingleStep()
        {
            ContinousWork = false;
            Storage.SettingsForm.AddLogText("SINGLE STEP WORK START");
            LaunchThreads();
        }

        public void RunSimulation()
        {
            ContinousWork = true;
            Storage.SettingsForm.AddLogText("SIMULATION STARTED");
            LaunchThreads();
        }

        public void Stop()
        {
            ContinousWork = false;
        }

        public void LaunchThreads()
        {
            Storage.SettingsForm.AddLogText("New interation started");
            LaunchWorker.RunWorkerAsync();
        }

        public void ThreadsExecution(object sender, DoWorkEventArgs e)
        {
            Storage.Game.GameState = Game.GameStates.Run;
            Storage.Game.StartNewIteration();
            Timer.Restart();
            for (int i = 0; i < ThreadsCount; i++)
            {
                Workers[i].RunWorkerAsync(i);
            }
            WaitHandle.WaitAll(Finished);
            // drawing
            using (Graphics g = Graphics.FromImage(Map))
            {
                GraphicsExtensions.ToLowQuality(g);
                Storage.Game.DrawIteration(g, Map.Width, Map.Height,
                    Storage.Game.NewIteration,
                    0, Storage.Game.GameCols - 1);
            }
            Storage.Game.EndIteration();
            //MergeAllSegments();
            Timer.Stop();
            long elapsed = Timer.ElapsedMilliseconds;
            int restTime = Convert.ToInt32(Interval - elapsed);
            if (restTime > 0)
            {
                Thread.Sleep(restTime);
            }
            e.Result = elapsed;
        }

        private void LaunchComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            Storage.MainForm.RefreshBoard(Map);
            long elapsed = (long)e.Result;
            float ips = 0;
            Storage.SettingsForm.AddLogText("All threads finished in: " + elapsed);
            SetEventVal(false);
            if (Interval - elapsed > 0)
            {
                ips = 1000f / Interval;
            }
            else
            {
                ips = 1000f / elapsed;
            }
            if (ContinousWork)
            {
                LaunchThreads();
            }
            else
            {
                Storage.Game.GameState = Game.GameStates.Paused;
            }
            Storage.SettingsForm.UpdateGameInfo(ips);
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            int tId = (int)e.Argument;
            //Storage.MainForm.Invoke(log, "Thread_" + tId + " in: " + Thread.CurrentThread.ManagedThreadId + " started");
            //Storage.SettingsForm.AddLogText("Worker_" + workerId + " works in: " + Thread.CurrentThread.ManagedThreadId);
            Segment s = Segments[tId];
            // logic
            Storage.Game.UpdateNewIteration(s.ColumnsOffset, s.ColumnsOffset + s.ColumnsCount - 1);

            //// drawing
            //using (Graphics g = Graphics.FromImage(s.Map))
            //{
            //    GraphicsExtensions.ToLowQuality(g);
            //    Storage.Game.DrawIteration(g, s.Map.Width, s.Map.Height,
            //        Storage.Game.NewIteration,
            //        s.ColumnsOffset, s.ColumnsOffset + s.ColumnsCount - 1);
            //}
            Finished[tId].Set();
        }

        public void MergeAllSegments()
        {
            using (Graphics g = Graphics.FromImage(Map))
            {
                GraphicsExtensions.ToLowQuality(g);
                for (int i = 0; i < Segments.Length; i++)
                {
                    g.DrawImageUnscaled(Segments[i].Map,Segments[i].MapOffset,0);
                }
            }
        }

        public int[] CountOffsets(int[] tab)
        {
            int[] res = new int[tab.Length];
            int off = 0;
            for (int i = 0; i < tab.Length; i++)
            {
                res[i] = off;
                off += tab[i];
            }
            return res;
        }

        public void AddRandomIter()
        {
            Storage.Game.StartNewIteration();
            Random r = new Random();
            for (int i = 0; i < Storage.Game.GameRows; i++)
            {
                for (int j = 0; j < Storage.Game.GameCols; j++)
                {
                    Storage.Game.NewIteration[i, j].State = r.Next(0, 2);
                }
            }
            using (Graphics g = Graphics.FromImage(Map))
            {
                Storage.Game.DrawIteration(g, Map.Width, Map.Height,
                    Storage.Game.NewIteration,
                    0, Storage.Game.GameCols - 1);
            }
            Storage.Game.EndIteration();
            Storage.MainForm.RefreshBoard(Map);
        }
    }
}
