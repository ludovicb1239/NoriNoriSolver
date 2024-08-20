using System.Numerics;

namespace NoriNoriSolver
{
    public partial class Form1 : Form
    {
        static Thread th;
        public Form1()
        {
            InitializeComponent();
        }
        void Solve()
        {
            for (int n = 0; n < 2; n++)
            {
                for (int i = 0; i < 10; i++)
                {
                    Size size = new Size(8, 8);
                    int cellSize = 47;
                    Rectangle rect = new Rectangle(new Point(1335, 440), size * cellSize);
                    Bitmap bmp = TakeScreenshotRegion(rect);
                    if (SolveAndPlay(rect, size, bmp))
                    {
                        i = 0;
                        n = 0;
                        continue;
                    }

                    size = new Size(9, 9);
                    rect = new Rectangle(new Point(1311, 440), size * cellSize);
                    bmp = TakeScreenshotRegion(rect);
                    if (SolveAndPlay(rect, size, bmp))
                    {
                        i = 0;
                        n = 0;
                        continue;
                    }

                    size = new Size(10, 10);
                    rect = new Rectangle(new Point(1286, 440), size * cellSize);
                    bmp = TakeScreenshotRegion(rect);
                    if (SolveAndPlay(rect, size, bmp))
                    {
                        i = 0;
                        n = 0;
                        continue;
                    }

                    Thread.Sleep(100);
                }

                Thread.Sleep(200);
                Player.SimulateMouseClick(1432, 602);
                Thread.Sleep(200);
            }

        }

        bool SolveAndPlay(Rectangle rect, Size size, Bitmap bitmap)
        {
            Console.WriteLine("Trying " + size.ToString() + "\n\n\n");
            if (!Player.FromBitmap(bitmap, size, out int[] grid))
            {
                OutputImageBox.Invoke(new Action(() =>
                {
                    InputImageBox.Image = bitmap;
                    OutputImageBox.Image = null;
                    //outLabel.Text = "Solved puzzle";
                }));
                return false;
            }

            Board board = new Board(grid, size);

            Bitmap notSolvedImage = board.Draw();
            Console.WriteLine("Scanned Board" + board.ToString());

            bool solved = board.Solve();

            if (solved)
            {
                Console.WriteLine("Solved" + board.ToString());

                Bitmap solvedImage = board.Draw();
                OutputImageBox.Invoke(new Action(() =>
                {
                    InputImageBox.Image = bitmap;
                    OutputImageBox.Image = solvedImage;
                    //outLabel.Text = "Solved puzzle";
                }));

                Player.Play(board, rect);
                Thread.Sleep(100);
                Player.SimulateMouseClick(1346, 760);
                Thread.Sleep(400);
            }
            else
            {
                Console.WriteLine("Did not solve");
                OutputImageBox.Invoke(new Action(() =>
                {
                    InputImageBox.Image = bitmap;
                    OutputImageBox.Image = null;
                    //outLabel.Text = "Solved puzzle";
                }));
            }
            return solved;
        }

        static Bitmap TakeScreenshotRegion(Rectangle rect)
        {
            Point topLeft = rect.Location;
            int width = rect.Width;
            int height = rect.Height;
            Bitmap bitmap = new Bitmap(width, height);
            // Use the Graphics object to copy the pixel from the screen
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen((int)topLeft.X, (int)topLeft.Y, 0, 0, rect.Size);
            }
            return bitmap;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (th == null || th.ThreadState != ThreadState.Running) {
                th = new Thread(Solve);
                th.Start();
            }
        }
    }
}
