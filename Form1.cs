﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class Form1 : Form
    {
        private int _currentGeneration;
        private Graphics _graphics;
        private int _resolution;
        private bool[,] _field;
        private int _rows;
        private int _cols;

        public Form1()
        {
            InitializeComponent();
        }

        private void StartGame()
        {
            if (timer1.Enabled)
                return;

            _currentGeneration = 0;
            Text = $"Generation {_currentGeneration}";

            nudResolution.Enabled = false;
            nudDensity.Enabled = false;
            _resolution = (int)nudResolution.Value;
            _rows = pictureBox1.Height / _resolution;
            _cols = pictureBox1.Width / _resolution;
            _field = new bool[_cols, _rows];

            Random random = new Random();
            for (int x = 0; x < _cols; x++) 
            {
                for (int y = 0; y < _rows; y++)
                {
                    _field[x, y] = random.Next((int)nudDensity.Value) == 0;
                }
            }

            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            _graphics = Graphics.FromImage(pictureBox1.Image);
            timer1.Start();
            // _graphics.FillRectangle(Brushes.Crimson, 0, 0, _resolution, _resolution);
        }

        private int CountNeighbours(int x, int y)
        {
            int count = 0;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    int col = (x + i + _cols) % _cols;
                    int row = (y + j + _rows) % _rows;

                    bool isSelfChecking = col == x && row == y;
                    bool hasLife = _field[col, row];

                    if (hasLife && !isSelfChecking)
                        count++;
                }
            }
            return count;
        }

        private void StopGame()
        {
            if (!timer1.Enabled)
                return;

            timer1.Stop();
            nudResolution.Enabled = true;
            nudDensity.Enabled = true;
        }

        private void NextGeneration()
        {
            _graphics.Clear(Color.Black);

            bool[,] newField = new bool[_cols, _rows];

            for (int x = 0; x < _cols; x++)
            {
                for (int y = 0; y < _rows; y++)
                {
                    var neighboursCount = CountNeighbours(x, y);
                    var hasLife = _field[x, y];

                    if (!hasLife && neighboursCount == 3)
                        newField[x, y] = true;
                    else if (hasLife && (neighboursCount < 2 || neighboursCount > 3))
                        newField[x, y] = false;
                    else
                        newField[x, y] = _field[x, y];
                    
                    if (hasLife)
                        // subtract 1 in height and width to get border
                        _graphics.FillRectangle(Brushes.Crimson, x * _resolution, y * _resolution,
                            _resolution - 1, _resolution - 1);
                }
            }
            _field = newField;
            pictureBox1.Refresh();
            Text = $"Generation {++_currentGeneration}";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void bStop_Click(object sender, EventArgs e)
        {
            StopGame();
        }

        private void bStart_Click(object sender, EventArgs e)
        {
            StartGame();
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!timer1.Enabled)
                return;

            if (e.Button == MouseButtons.Left)
            {
                var x = e.Location.X / _resolution;
                var y = e.Location.Y / _resolution;
                if (ValidateMousePosition(x, y))
                    _field[x, y] = true;
            }

            if (e.Button == MouseButtons.Right)
            {
                var x = e.Location.X / _resolution;
                var y = e.Location.Y / _resolution;
                if (ValidateMousePosition(x, y))
                    _field[x, y] = false;
            }
        }
        private bool ValidateMousePosition(int x, int y)
        {
            return x >= 0 && y >= 0 && x < _cols && y < _rows;
        }
    }
    
}
