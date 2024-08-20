using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoriNoriSolver
{
    public class Utils
    {
        public static char[] MirrorNoriNori(char[] grid, Size size)
        {
            int width = size.Width;
            int height = size.Height;
            char[] mirrored = new char[width * height];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    mirrored[i * width + j] = grid[i * width + (width - 1 - j)];
                }
            }

            return mirrored;
        }

        public static char[] RotateNoriNori(char[] grid, Size size)
        {
            int width = size.Width;
            int height = size.Height;
            char[] rotated = new char[width * height];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    rotated[j * height + (height - 1 - i)] = grid[i * width + j];
                }
            }

            return rotated;
        }
    }
}
