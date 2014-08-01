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

        public Thread EndThread { get; set; }
        public Thread[] Threads { get; set; }
        public Bitmap Map { get; set; }
        public Segment[] Segments { get; set; }
        public int WorkersDone { get; set; }
        public bool ContinousWork { get; set; }
        private readonly object StatusLock = new object();
        public Stopwatch Timer { get; set; }
        public long Interval { get; set; }
        delegate void WorkCallback(int id);
        delegate void EndCallback(long obj);

        public ParallelEngine()
        {
            Timer = new Stopwatch();
            Interval = 1000;
        }

        private void EndWork(object obj)
        {
            EndCallback ec = new EndCallback(EndWorkComplete);
            // finish updating and merge all segments
            Storage.Game.EndIteration();
            MergeAllSegments();
            Timer.Stop();
            long elapsed = Timer.ElapsedMilliseconds;
            int restTime = Convert.ToInt32(Interval - elapsed);
            if (restTime > 0)
            {
                Thread.Sleep(restTime);
            }
            Storage.MainForm.Invoke(ec, elapsed);
        }

        private void EndWorkComplete(long obj)
        {
            long elapsed = obj;
            Storage.MainForm.RefreshBoard(Map);
            if (ContinousWork)
            {
                Storage.SettingsForm.AddLogText("Simulation step finished in: " + elapsed + "ms");
                LaunchWorkers();
            }
            else
            {
                Storage.Game.GameState = Game.GameStates.Paused;
                Storage.SettingsForm.AddLogText("Task finished in: " + elapsed + "ms");
            }
            if (elapsed > Interval)
                Storage.SettingsForm.UpdateGameInfo(1000f / elapsed);
            else
                Storage.SettingsForm.UpdateGameInfo(1000f / Interval);
        }

        public void MergeAllSegments()
        {
            using (Graphics g = Graphics.FromImage(Map))
            {
                Rectangle rect = new Rectangle();
                rect.Y = 0;
                rect.Height = Map.Height;
                for (int i = 0; i < Segments.Length; i++)
                {
                    //g.DrawImageUnscaled(Segments[i].Map,Segments[i].MapOffset,0);
                    rect.X = Segments[i].MapOffset;
                    rect.Width = Segments[i].MapWidth;
                    g.DrawImage(Segments[i].Map, Segments[i].MapOffset, 0, rect, GraphicsUnit.Pixel);
                }
            }
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

        public void Init(int count)
        {
            // init all workers
            Threads = new Thread[count];

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
            for (int i = 0;i < count;i++)
            {
                Segment s = new Segment();
                s.ColumnsCount = segmentsCols[i];
                s.ColumnsOffset = colsOffsets[i];
                s.Map = new Bitmap(Map.Width, Map.Height);
                s.MapWidth = mapsWidth[i];
                s.MapOffset = mapsOffsets[i];
                Segments[i] = s;
            }
        }

        private void DoWork(object obj)
        {
            int workerId = (int)obj;
            WorkCallback wc = new WorkCallback(WorkComplete);
            //Storage.SettingsForm.AddLogText("Worker_" + workerId + " works in: " + Thread.CurrentThread.ManagedThreadId);
            Segment s = Segments[workerId];
            // logic
            Storage.Game.UpdateNewIteration(s.ColumnsOffset, s.ColumnsOffset + s.ColumnsCount - 1);

            // drawing
            using (Graphics g = Graphics.FromImage(s.Map))
            {
                Storage.Game.DrawIteration(g, s.Map.Width, s.Map.Height,
                    Storage.Game.NewIteration,
                    s.ColumnsOffset, s.ColumnsOffset + s.ColumnsCount - 1);
            }
            Storage.MainForm.Invoke(wc, workerId);
        }

        private void WorkComplete(int id)
        {
            Storage.SettingsForm.AddLogText("Worker_" + id + " finished in: " + Thread.CurrentThread.ManagedThreadId);
            int status;
            lock (StatusLock)
            {
                WorkersDone++;
                status = WorkersDone;
            }
            if (status == Threads.Length)
            {
                lock (StatusLock)
                {
                    WorkersDone = 0;
                }
                EndThread = new Thread(new ParameterizedThreadStart(EndWork));
                EndThread.Start();
            }              
        }

        public void LaunchWorkers()
        {
            Storage.SettingsForm.AddLogText("Engine thread ID: " + Thread.CurrentThread.ManagedThreadId);
            Storage.Game.GameState = Game.GameStates.Run;
            Storage.Game.StartNewIteration();
            Storage.SettingsForm.UpdateGameInfo(-1);
            lock(StatusLock)
            {
                WorkersDone = 0;
            }
            Timer.Restart();
            for(int i = 0;i < Threads.Length;i++)
            {
                Threads[i] = new Thread(new ParameterizedThreadStart(DoWork));
                Threads[i].Priority = ThreadPriority.BelowNormal;
                Threads[i].Start(i);
                Storage.SettingsForm.AddLogText("Worker_" + i + " started");
            }
        }

        public void RunSingleStep()
        {
            ContinousWork = false;
            Storage.SettingsForm.AddLogText("SINGLE STEP WORK START");
            LaunchWorkers();
        }

        public void RunSimulation()
        {
            ContinousWork = true;
            Storage.SettingsForm.AddLogText("SIMULATION STARTED");
            LaunchWorkers();
        }

        public void Stop()
        {
            ContinousWork = false;
        }

        public int[] CountOffsets(int[] tab)
        {
            int[] res = new int[tab.Length];
            int off = 0;
            for (int i = 0;i < tab.Length;i++)
            {
                res[i] = off;
                off += tab[i];
            }
            return res;
        }
    }
}
