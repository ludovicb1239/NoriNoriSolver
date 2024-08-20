using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace NoriNoriSolver
{
    public enum CellState
    {
        NotDefined,
        Marked,
        Empty
    }
    public class Board
    {
        readonly int[] zones;
        public CellState[] grid;
        public readonly Size size;
        int[] zonesCountMarked; //One for each zone, keeps track of the number of checked
        int[] zonesCountEmpty; //One for each zone, keeps track of the number of unchecked
        readonly int[] zonesCount;  //One for each zone, keeps track of the number of cell in each zone
        readonly List<int[]> directions;
        public Board(int[] grid, Size size)
        {
            if (grid.Length != size.Width * size.Height)
            {
                throw new Exception("Grid is not the right lenght");
            }
            this.grid = new CellState[grid.Length];
            this.zones = grid;
            this.size = size;
            List<int> zonesCountList = new();
            for (int i = 0; i < grid.Length; i++)
            {
                this.grid[i] = CellState.NotDefined;
                if (zonesCountList.Count <= grid[i])
                {
                    while (zonesCountList.Count <= grid[i])
                        zonesCountList.Add(0);
                }
                zonesCountList[grid[i]]++;

            }
            zonesCount = zonesCountList.ToArray();
            zonesCountMarked = new int[zonesCount.Length];
            zonesCountEmpty = new int[zonesCount.Length];

            directions = new();
            for (int i = 0; i < grid.Length; i++)
            {
                List<int> list = new List<int>();
                if (i % size.Width != 0) //West
                    list.Add(i - 1);
                if ((i+1) % size.Width != 0) //West
                    list.Add(i + 1);
                if (i >= size.Width)
                    list.Add(i - size.Width);
                if (i < grid.Length - size.Width)
                    list.Add(i + size.Width);
                directions.Add(list.ToArray());
            }
        }
        public bool Solve()
        {
            //First pass
            ReducePossibilities();
            Console.WriteLine("First pass done");
            if (!CheckConnection(0))
                return false;
            Console.WriteLine("Should be all done");
            return Done();
        }
        bool CheckConnection(int pos)
        {
            if (pos == size.Width * size.Height) return Done();
            if (grid[pos] != CellState.NotDefined) return CheckConnection(pos + 1);

            int zone = zones[pos];

            //Try with empty cell
            SetCell(pos, CellState.Empty);
            if (Verify())
            {
                if (CheckConnection(pos + 1))
                    return true;
            }
            SetCell(pos, CellState.NotDefined);

            if (!HasColoredNeighbor(pos))
            {
                //Try with marked cell
                if (directions[pos].Contains(pos + 1))
                {
                    int otherPos = pos + 1;
                    if (TrySpread(pos, otherPos))
                        return true;
                }
                if (directions[pos].Contains(pos + size.Width))
                {
                    int otherPos = pos + size.Width;
                    if (TrySpread(pos, otherPos))
                        return true;
                }
            }
            return false;
        }
        bool TrySpread(int pos, int otherPos)
        {
            if (grid[otherPos] == CellState.NotDefined && !HasColoredNeighbor(otherPos))
            {
                Dictionary<int, CellState> before = new();
                foreach (int neightPos in directions[pos])
                {
                    before[neightPos] = grid[neightPos];
                    SetCell(neightPos, CellState.Empty);
                }
                foreach (int neightPos in directions[otherPos])
                {
                    before[neightPos] = grid[neightPos];
                    SetCell(neightPos, CellState.Empty);
                }
                SetCell(otherPos, CellState.Marked);
                SetCell(pos, CellState.Marked);

                // Try this cell and the right one
                if (Verify())
                {
                    if (CheckConnection(pos + 1))
                        return true;
                }

                foreach (KeyValuePair<int, CellState> pair in before)
                {
                    SetCell(pair.Key, pair.Value);
                }
                SetCell(otherPos, CellState.NotDefined);
                SetCell(pos, CellState.NotDefined);
            }
            return false;
        }
        void SetCell(int pos, CellState state)
        {
            int zone = zones[pos];
            switch (grid[pos])
            {
                case CellState.Empty:
                    zonesCountEmpty[zone]--;
                    break;
                case CellState.Marked:
                    zonesCountMarked[zone]--;
                    break;
            }
            grid[pos] = state;
            switch (state)
            {
                case CellState.Empty:
                    zonesCountEmpty[zone]++;
                    break;
                case CellState.Marked:
                    zonesCountMarked[zone]++;
                    break;
            }
        }
        bool Done()
        {
            for (int i = 0; i < zonesCount.Length; i++)
            {
                if (zonesCountMarked[i] != 2)
                    return false;
            }
            return true;
        }
        void ReducePossibilities()
        {
            for (int pos = 0; pos < grid.Length; pos++)
            {
                int zone = zones[pos];
                if (zonesCount[zone] == 2 && zonesCountMarked[zone] == 0)
                {
                    if (zones[pos +1] == zone)
                    {
                        int otherPos = pos + 1;

                        Dictionary<int, CellState> before = new();
                        foreach (int neightPos in directions[pos])
                        {
                            before[neightPos] = grid[neightPos];
                            SetCell(neightPos, CellState.Empty);
                        }
                        foreach (int neightPos in directions[otherPos])
                        {
                            before[neightPos] = grid[neightPos];
                            SetCell(neightPos, CellState.Empty);
                        }
                        SetCell(otherPos, CellState.Marked);
                        SetCell(pos, CellState.Marked);
                    }
                    else
                    {
                        int otherPos = pos + size.Width;
                        Dictionary<int, CellState> before = new();
                        foreach (int neightPos in directions[pos])
                        {
                            before[neightPos] = grid[neightPos];
                            SetCell(neightPos, CellState.Empty);
                        }
                        foreach (int neightPos in directions[otherPos])
                        {
                            before[neightPos] = grid[neightPos];
                            SetCell(neightPos, CellState.Empty);
                        }
                        SetCell(otherPos, CellState.Marked);
                        SetCell(pos, CellState.Marked);
                    }
                }
            }
        }
        bool HasColoredNeighbor(int pos)
        {
            bool hasColoredNeighbor = false;
            foreach (int neightPos in directions[pos])
            {
                if (grid[neightPos] == CellState.Marked)
                {
                    hasColoredNeighbor = true;
                    break;
                }
            }
            return hasColoredNeighbor;
        }
        bool Verify()
        {
            for (int i = 0; i < zonesCount.Length; i++)
            {
                if (zonesCountMarked[i] > 2)
                    return false;
                if (zonesCount[i] - zonesCountEmpty[i] < 2)
                    return false;
            }
            return true;
        }
        public override string ToString()
        {
            List<char> str = new();
            for (int i = 0; i < grid.Length; i++)
            {
                if (i % size.Width == 0)
                    str.Add('\n');
                str.Add(zones[i].ToString()[0]);
            }
            str.Add('\n');
            for (int i = 0; i < grid.Length; i++)
            {
                if (i % size.Width == 0)
                    str.Add('\n');
                switch (grid[i])
                {
                    case CellState.NotDefined:
                        str.Add('.');
                        break;
                    case CellState.Marked:
                        str.Add('M');
                        break;
                    case CellState.Empty:
                        str.Add('X');
                        break;
                }
            }
            str.Add('\n');
            return new string(str.ToArray());
        }
        public Bitmap Draw()
        {
            int cellSize = 40;
            Size imageSize = new Size(cellSize * size.Width , cellSize * size.Height);
            Bitmap bitmap = new Bitmap(imageSize.Width, imageSize.Height);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // Set background color to white
                graphics.Clear(Color.White);

                for (int i = 0; i < grid.Length; i++)
                {
                    if (grid[i] != 0)
                    {
                        int row = i / size.Width;
                        int col = i % size.Width;
                        if (grid[i] == CellState.Marked)
                        {
                            // Fill the cell with a color (e.g., light gray)
                            Brush brush = new SolidBrush(Color.LightBlue);
                            graphics.FillRectangle(brush, col * cellSize, row * cellSize, cellSize, cellSize);
                        }
                    }
                }
                // Draw the grid
                Pen gridPen = new Pen(Color.Gray, 2);

                // Draw the grid lines
                for (int i = 0; i <= size.Width; i++)
                {
                    graphics.DrawLine(gridPen, i * cellSize, 0, i * cellSize, imageSize.Height);
                }
                for (int i = 0; i <= size.Height; i++)
                {
                    graphics.DrawLine(gridPen, 0, i * cellSize, imageSize.Width, i * cellSize);
                }

                gridPen = new Pen(Color.DarkRed, 5);
                gridPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                gridPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

                for (int pos = 0; pos < grid.Length; pos++)
                {
                    int otherPos = pos + 1;
                    if (directions[pos].Contains(otherPos) && zones[pos] != zones[otherPos])
                    {
                        int row = pos / size.Width;
                        int col = pos % size.Width;
                        graphics.DrawLine(gridPen, col * cellSize + cellSize, row * cellSize, col * cellSize + cellSize, row * cellSize + cellSize);
                    }
                }
                for (int pos = 0; pos < grid.Length; pos++)
                {
                    int otherPos = pos + size.Width;
                    if (directions[pos].Contains(otherPos) && zones[pos] != zones[otherPos])
                    {
                        int row = pos / size.Width;
                        int col = pos % size.Width;
                        graphics.DrawLine(gridPen, col * cellSize, row * cellSize + cellSize, col * cellSize + cellSize, row * cellSize + cellSize);
                    }
                }

                graphics.DrawLine(gridPen, 0, 0, imageSize.Width, 0);
                graphics.DrawLine(gridPen, 0, 0, 0, imageSize.Height);
                graphics.DrawLine(gridPen, 0, imageSize.Height, imageSize.Width, imageSize.Height);
                graphics.DrawLine(gridPen, imageSize.Width, 0, imageSize.Width, imageSize.Height);

            }
            return bitmap;
        }
    }
}
