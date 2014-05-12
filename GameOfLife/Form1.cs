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
    public partial class Form1 : Form
    {
        private Game _game { get; set; }
        private CellPointer _leftClickLastPoint { get; set; }
        private CellPointer _leftClickTempPoint { get; set; }
        private bool _leftClickPainting { get; set; }
        private CellPointer _rightClickFirstPoint { get; set; }

        public Form1()
        {
            InitializeComponent();
            _leftClickLastPoint = new CellPointer();
            _rightClickFirstPoint = new CellPointer();
            _leftClickTempPoint = new CellPointer();
            _game = new Game(100, 100);
            _bitmap = new Bitmap(bufferedPanel1.Width, bufferedPanel1.Height);
            m_RefreshBoard(true);
        }

        private void m_RefreshBoard(bool newSize)
        {
            _game.m_SetBitmap(Graphics.FromImage(_bitmap), _bitmap.Width, _bitmap.Height);
            _game.m_DrawBoard(newSize);
            bufferedPanel1.BackgroundImage = _bitmap;
            bufferedPanel1.Invalidate();
        }



        private void bufferedPanel1_Resize(object sender, EventArgs e)
        {
            _bitmap = new Bitmap(bufferedPanel1.Width, bufferedPanel1.Height);
            m_RefreshBoard(true);
        }



        private void bufferedPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (_game._LastSelected.Count != 0)
                {
                    _game.m_Deselect(true);
                    _game.m_DrawBoard(false);
                    bufferedPanel1.Invalidate();
                }
                else
                {
                    _game.m_HitTest(e.X, e.Y);
                    if (_game._HitTestInfo._FieldHit)
                    {
                        _leftClickLastPoint._I = _game._HitTestInfo._I;
                        _leftClickLastPoint._J = _game._HitTestInfo._J;
                        _game.m_ToggleCellState(_game._HitTestInfo._I, _game._HitTestInfo._J);
                        bufferedPanel1.Invalidate();

                    }
                }               
            }
            else if (e.Button == MouseButtons.Right)
            {
                _game.m_HitTest(e.X, e.Y);
                if (_game._HitTestInfo._FieldHit)
                {
                    _game._Cells[_game._HitTestInfo._I, _game._HitTestInfo._J]._Selected = true;
                    _game.m_UpdateBoardField(_game._HitTestInfo._I, _game._HitTestInfo._J);
                    _game.m_DrawField(_game._HitTestInfo._I, _game._HitTestInfo._J);
                    _rightClickFirstPoint._I = _game._HitTestInfo._I;
                    _rightClickFirstPoint._J = _game._HitTestInfo._J;
                    bufferedPanel1.Invalidate();
                }
            }
        }

        

        private void bufferedPanel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {                
                _game.m_HitTest(e.X, e.Y);
                if (_game._HitTestInfo._FieldHit)
                {
                    //Debug.WriteLine("X: {0}, Y: {1},I: {2},J: {3}", e.X, e.Y, _game._HitTestInfo._I, _game._HitTestInfo._J);
                    _leftClickTempPoint._I = _game._HitTestInfo._I;
                    _leftClickTempPoint._J = _game._HitTestInfo._J;
                    if (!_leftClickLastPoint.f_Equals(_leftClickTempPoint))
                    {
                        _leftClickLastPoint.m_SetCord(_leftClickTempPoint);
                        _game.m_ToggleCellState(_game._HitTestInfo._I, _game._HitTestInfo._J);
                        //bufferedPanel1.BackgroundImage = _bitmap;
                        bufferedPanel1.Invalidate();
                    }                       
                }
                
            }
            else if (e.Button == MouseButtons.Right)
            {
                _game.m_HitTest(e.X, e.Y);
                if (_game._HitTestInfo._FieldHit)
                {
                    _game.m_Deselect(false);
                    _game.m_DrawBoard(false);
                    _game.m_SelectCellsInRange(_rightClickFirstPoint, new CellPointer(_game._HitTestInfo._I, _game._HitTestInfo._J));
                    _game.m_DrawBoard(false);
                    bufferedPanel1.Invalidate();
                }

            }
        }

        private void bufferedPanel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                _game.m_HitTest(e.X, e.Y);
                if (_game._HitTestInfo._FieldHit)
                {
                    _game.m_SelectCellsInRange(_rightClickFirstPoint, new CellPointer(_game._HitTestInfo._I, _game._HitTestInfo._J));
                    bufferedPanel1.Invalidate();
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            _game.m_Pause();
            bufferedPanel1.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _bitmap = _game._Snapshot;
            bufferedPanel1.BackgroundImage = _bitmap;
            //_game.m_SetGraphics(Graphics.FromImage(_bitmap), _bitmap.Width, _bitmap.Height);
            bufferedPanel1.Invalidate();
        }
    }
}
