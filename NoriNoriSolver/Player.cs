using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NoriNoriSolver
{
    public class Player
    {
        public static bool FromBitmap(Bitmap bmp, Size size, out int[] grid)
        {
            int cellW = bmp.Width / size.Width;
            int cellH = bmp.Height / size.Height;
            float edge = 0.85f; // 0.5 to 1.0
            float edgeInv = 2f - edge;
            float[] countRight = new float[size.Width * size.Height];
            float[] countDown = new float[size.Width * size.Height];
            grid = new int[size.Width * size.Height];
            for (int i = 0; i < size.Width * size.Height; i++)
            {
                int row = i / size.Width;
                int col = i % size.Height;

                if (col < size.Width - 1)
                {
                    countRight[i] = 0f;
                    for (int n = col * cellW + (int)(cellW * edge); n < col * cellW + (int)(cellW * edgeInv); n++)
                    {
                        float bright = bmp.GetPixel(n, row * cellH + cellH / 2).GetBrightness();
                        if (bright != 1f) //Pixel is black
                        {
                            countRight[i] += 1f - bright;
                        }
                        bmp.SetPixel(n, row * cellH + cellH / 2, Color.Red);
                    }
                    if (countRight[i] == 0f)
                        return false;
                }
                if (row < size.Height - 1)
                {
                    countDown[i] = 0f;
                    for (int n = row * cellH + (int)(cellH * edge); n < row * cellH + (int)(cellH * edgeInv); n++)
                    {
                        float bright = bmp.GetPixel(col * cellW + cellW / 2, n).GetBrightness();
                        if (bright != 1f) //Pixel is black
                        {
                            countDown[i] += 1f - bright;
                        }
                        bmp.SetPixel(col * cellW + cellW / 2, n, Color.Red);
                    }
                    if (countDown[i] == 0f)
                        return false;
                }
            }
            bool[] canRight = new bool[size.Width * size.Height];
            bool[] canDown = new bool[size.Width * size.Height];
            float total = countDown.Sum() + countRight.Sum();
            float avg = total / (countDown.Length + countRight.Length);
            for(int i = 0; i < countDown.Length; i++)
            {
                canDown[i] = countDown[i] < avg;
            }
            for (int i = 0; i < countRight.Length; i++)
            {
                canRight[i] = countRight[i] < avg; //You can go if the line is lighter
            }


            // Create and label regions
            for (int i = 0; i < grid.Length; i++)
                grid[i] = -1;
            int currentRegion = 0;

            for (int i = 0; i < size.Width * size.Height; i++)
            {
                int row = i / size.Width;
                int col = i % size.Width;

                if (grid[i] == -1) // If not yet labeled
                {
                    FloodFill(grid, size, row, col, currentRegion, canRight, canDown);
                    currentRegion++;
                }
            }
            return true;
        }

        public static void Play(Board board, Rectangle rect)
        {
            Size size = board.size;
            for (int i = 0; i < size.Width * size.Height; i++)
            {
                if (board.grid[i] == CellState.Marked)
                {
                    int row = i / size.Width;
                    int col = i % size.Height;

                    int cellW = (int)rect.Size.Width / size.Width;
                    int cellH = (int)rect.Size.Height / size.Height;

                    int x = (int)rect.Location.X + cellW * col + cellW / 2;
                    int y = (int)rect.Location.Y + cellH * row + cellH / 2;

                    SimulateMouseClick(x, y);

                    Thread.Sleep(1);
                }
            }
        }


        private static void FloodFill(int[] grid, Size size, int row, int col, int region, bool[] canRight, bool[] canDown)
        {
            int index = row * size.Width + col;
            grid[index] = region;

            // Check Right
            if (col < size.Width - 1 && grid[index + 1] == -1 && canRight[index])
            {
                FloodFill(grid, size, row, col + 1, region, canRight, canDown);
            }

            // Check Down
            if (row < size.Height - 1 && grid[index + size.Width] == -1 && canDown[index])
            {
                FloodFill(grid, size, row + 1, col, region, canRight, canDown);
            }

            // Check Left
            if (col > 0 && grid[index - 1] == -1 && canRight[index - 1])
            {
                FloodFill(grid, size, row, col - 1, region, canRight, canDown);
            }

            // Check Up
            if (row > 0 && grid[index - size.Width] == -1 && canDown[index - size.Width])
            {
                FloodFill(grid, size, row - 1, col, region, canRight, canDown);
            }
        }


        // Constants for mouse event flags
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        public static void SimulateMouseClick(int x, int y)
        {
            // Move the cursor to the specified coordinates
            SetCursorPos(x, y);

            // Simulate mouse click
            mouse_event(MOUSEEVENTF_LEFTDOWN, (uint)x, (uint)y, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetCursorPos(int x, int y);
    }
}
