using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Fire.Forest.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowModel model;
        System.Windows.Threading.DispatcherTimer dispatcherTimer;

        const int width = 5;
        const int height = 5;

        const int treesHorizontally = 100;
        const int treesVertically = 100;

        int fireCentersCount = 0;

        ForestCell[,] Forest;
        Path[,] CellPathes;

        public MainWindow()
        {
            InitializeComponent();
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += timer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);

            model = DataContext as MainWindowModel;
            Forest = InitForestModel();
            CellPathes = InitForestViewModel(Brushes.Green, ForestCanvas);
            model.AddDynamicsPoint(treesVertically * treesHorizontally, 0, 0);
            //model.AddFireCenterDynamics(fireCentersCount);
        }

        private void StartModeling(object sender, EventArgs e)
        {
            if (dispatcherTimer.IsEnabled == true)
            {
                dispatcherTimer.Stop();
                model.SetFireCenterDynamics = NumFSpawn.AsQueryable().OrderBy(x => x).Reverse().ToArray();
            }
            else dispatcherTimer.Start();
        }

        private Path[,] InitForestViewModel(Brush color, Panel canvas)
        {
            var pathes = new Path[treesHorizontally, treesVertically];
            for (int i = 0; i < treesHorizontally; i++)
            {
                for (int j = 0; j < treesVertically; j++)
                {
                    RectangleGeometry myRectangleGeometry = new RectangleGeometry
                    {
                        Rect = new Rect(i * width, j * height, width, height)
                    };

                    Path myPath = new Path
                    {
                        Fill = color,
                        Data = myRectangleGeometry
                    };
                    pathes[i, j] = myPath;
                    canvas.Children.Add(myPath);
                }
            }
            return pathes;
        }

        private ForestCell[,] InitForestModel()
        {
            var forest = new ForestCell[treesVertically, treesHorizontally];
            for (int i = 0; i < treesVertically; i++)
            {
                for (int j = 0; j < treesHorizontally; j++)
                {
                    forest[i, j] = new ForestCell(i, j);
                }
            }
            for (int i = 0; i < treesVertically; i++)
            {
                for (int j = 0; j < treesHorizontally; j++)
                {
                    if (i == 0 && j == 0)
                        forest[i, j].CellNeighbors(i, j, null, forest[i, j + 1], null, forest[i + 1, j]);
                    else if (i == 0 && j == 99)
                        forest[i, j].CellNeighbors(i, j, forest[i, j - 1], null, null, forest[i + 1, j]);
                    else if (i == 99 && j == 0)
                        forest[i, j].CellNeighbors(i, j, null, forest[i, j + 1], forest[i - 1, j], null);
                    else if (i == 0)
                        forest[i, j].CellNeighbors(i, j, forest[i, j - 1], forest[i, j + 1], null, forest[i + 1, j]);
                    else if (j == 0)
                        forest[i, j].CellNeighbors(i, j, null, forest[i, j + 1], forest[i - 1, j], forest[i + 1, j]);
                    else if (i == 99 && j == 99)
                        forest[i, j].CellNeighbors(i, j, forest[i, j - 1], null, forest[i - 1, j], null);
                    else if (i == 99)
                        forest[i, j].CellNeighbors(i, j, forest[i, j - 1], forest[i, j + 1], forest[i - 1, j], null);
                    else if (j == 99)
                        forest[i, j].CellNeighbors(i, j, forest[i, j - 1], null, forest[i - 1, j], forest[i + 1, j]);
                    else forest[i, j].CellNeighbors(i, j, forest[i, j - 1], forest[i, j + 1], forest[i - 1, j], forest[i + 1, j]);
                }
            }

            return forest;
        }
        int[,] states = new int[treesVertically, treesVertically];
        List<int> NumFSpawn = new List<int>();


        private void timer_Tick(object sender, EventArgs e)
        {
            Random rand = new Random();
            bool TickDone = false;
            double prob;
            int treeCount = 0;
            int firesCount = 0;
            int ashesCount = 0;
            for (int i = 0; i < treesVertically; i++)
                for (int j = 0; j < treesHorizontally; j++)
                    states[i, j] = Forest[i, j].state;

            for (int i = 0; i < treesVertically; i++)
            {
                for (int j = 0; j < treesHorizontally; j++)
                {
                    prob = rand.NextDouble();
                    if (states[i, j] == -1)
                    {
                        if (prob < model.P)
                        {
                            Forest[i, j].state = 0;
                        }
                        TickDone = true;
                    }
                    if (TickDone == false && states[i, j] > 0)
                    {
                        Forest[i, j].state = -1;
                        TickDone = true;
                    }
                    if (TickDone == false && states[i, j] == 0 
                        && (Forest[i, j].left == null || states[i, j - 1] <= 0)
                        && (Forest[i, j].right == null || states[i, j + 1] <= 0)
                        && (Forest[i, j].top == null || states[i - 1, j] <= 0)
                        && (Forest[i, j].bot == null || states[i + 1, j] <= 0))
                    {
                        if (prob < model.F)
                        {
                            Forest[i, j].state = fireCentersCount;
                            fireCentersCount++;
                            NumFSpawn.Add(1);
                        }
                        TickDone = true;
                    }
                    if (TickDone == false && Forest[i, j].state == 0)
                    {
                        if (prob < 1 - model.G)
                        {
                            if (Forest[i, j].left != null && states[i, j - 1] > 0)
                            {
                                Forest[i, j].state = states[i, j - 1];
                            }
                            else if (Forest[i, j].right != null && states[i, j + 1] > 0)
                            {
                                Forest[i, j].state = states[i, j + 1];
                            }
                            else if (Forest[i, j].top != null && states[i - 1, j] > 0)
                            {
                                Forest[i, j].state = states[i - 1, j];
                            }
                            else if (Forest[i, j].bot != null && states[i + 1, j] > 0)
                            {
                                Forest[i, j].state = states[i + 1, j];
                            }
                        }
                    }
                    if (Forest[i, j].state == 0)
                    {
                        treeCount++;
                        DrawCell(i, j, Brushes.Green);
                    }
                    if (Forest[i, j].state > 0)
                    {
                        firesCount++;
                        DrawCell(i, j, Brushes.OrangeRed);
                    }
                    if (Forest[i, j].state == -1)
                    {
                        ashesCount++;
                        DrawCell(i, j, Brushes.Gray);
                    }
                    TickDone = false;
                }
            }

            model.AddDynamicsPoint(treeCount, firesCount, ashesCount);

            for (int i = 0; i < treesVertically; i++)
                for (int j = 0; j < treesHorizontally; j++)
                {
                    if (Forest[i, j].state > 0)
                    {
                        NumFSpawn[Forest[i, j].state]++;
                    }
                }
        }

        private void DrawCell(int i, int j, Brush color)
        {
            CellPathes[i, j].Fill = color;
        }
    }
}
