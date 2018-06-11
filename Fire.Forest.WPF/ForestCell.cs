using System.Windows.Media;

namespace Fire.Forest.WPF
{
    public partial class MainWindow
    {
        class ForestCell
        {
            public ForestCell(int row, int col)
            {
                this.row = row;
                this.column = col;
                this.state = 0;
            }
            public int column;
            public int row;
            public int state; // 1 - дерево, 2 - горит, 3 - пепел
            public ForestCell left;
            public ForestCell right;
            public ForestCell top;
            public ForestCell bot;

            public void CellNeighbors(int row, int col, ForestCell l, ForestCell r, ForestCell t, ForestCell b)
            {
                this.left = l;
                this.right = r;
                this.top = t;
                this.bot = b;
            }
        }
    }
}

