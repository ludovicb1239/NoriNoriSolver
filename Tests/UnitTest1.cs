using NoriNoriSolver;
using System.Diagnostics;
using System.Drawing;
using System.Media;
namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Easy()
        {
            Size size = new Size(6, 6);
            List<int> grid =
            [
                0, 0, 0, 0, 1, 1,
                0, 2, 2, 1, 1, 1,
                0, 3, 3, 3, 3, 1,
                4, 4, 4, 4, 3, 3,
                4, 4, 5, 5, 5, 5,
                4, 4, 5, 6, 6, 6,
            ];
            TryPuzzle(grid.ToArray(), size);

        }
        [Test]
        public void Medium()
        {
            Size size = new Size(10,10);
            List<int> grid =
            [
                1, 1, 2, 2, 2, 2, 3, 3, 3, 3,
                1, 0, 0, 2, 2, 2, 3, 4, 4, 3,
                6, 0, 0, 7, 8, 2, 2, 4, 4, 3,
                6, 6, 7, 7, 8, 9, 2, 5, 5, 5,
                10, 6, 10, 10, 8, 9, 5, 5, 18, 19,
                10, 10, 10, 8, 8, 9, 9, 9, 18, 19,
                11, 10, 14, 14, 14, 9, 15, 15, 18, 19,
                11, 12, 12, 14, 9, 9, 15, 17, 17, 19,
                11, 12, 12, 14, 14, 9, 16, 17, 17, 19,
                13, 13, 13, 14, 9, 9, 16, 16, 16, 19
            ];
            TryPuzzle(grid.ToArray(), size);

        }
        [Test]
        public void Reading()
        {
            Bitmap img = (Bitmap)Bitmap.FromFile("test.png");
            Size size = new Size(9,9);
            int[] grid = Player.FromBitmap(img, size);
            TryPuzzle(grid, size);
        }
        void TryPuzzle(int[] grid, Size size)
        {
            Board b = new Board(grid.ToArray(), size);
            Console.Write(b.ToString());

            Stopwatch sw = new Stopwatch();
            sw.Start();
            bool solved = b.Solve();
            sw.Stop();

            if (solved)
            {
                Console.WriteLine($"Solved in {sw.ElapsedMilliseconds} ms");
            }
            Console.WriteLine(b.ToString());
            Bitmap bmp = b.Draw();
            // Save the image to a file
            bmp.Save("nori.png");
            Assert.IsTrue(solved);
        }
    }
}