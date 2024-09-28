using System.Drawing.Drawing2D;

namespace Snake
{
    public partial class Form1 : Form
    {
        private const int CELL_SIZE_PIXELS = 30;                    
        private const int ROWS_NUMBER = 15;                         
        private const int COLS_NUMBER = 15;                         
        private const int FIELD_LEFT_OFFSET_PIXELS = 40;           
        private const int FIELD_TOP_OFFSET_PIXELS = 15;             
        private const int INITIAL_SNAKE_SPEED_INTERVAL = 300;       
        private const int SPEED_INCREMENT_BY = 5;                   

        private enum SnakeDirection
        {
            Left,
            Right,
            Up,
            Down
        }

        private SnakeDirection snakeDirection = SnakeDirection.Up;  
        private LinkedList<Point> snake = new LinkedList<Point>();  
        private Point food;                                         
        private Random rand = new Random();                         
        private bool isGameEnd;                                  
        public Form1()
        {
            InitializeComponent();
           
        }
        private void GameStart()
        {
            GenerateFood();
            StartSnake();
            isGameEnd = false;
            timer1.Start();
            timer1.Interval = INITIAL_SNAKE_SPEED_INTERVAL;
        }


        private void StartSnake()
        {
            snakeDirection = SnakeDirection.Up;
            snake.Clear();
            snake.AddFirst(new Point(ROWS_NUMBER - 1, COLS_NUMBER / 2 - 1));
        }

        private void DrawGrid(Graphics g)
        {
            for (int row = 0; row <= ROWS_NUMBER; row++)
            {
                g.DrawLine(Pens.Teal,
                    new Point(FIELD_LEFT_OFFSET_PIXELS, FIELD_TOP_OFFSET_PIXELS + row * CELL_SIZE_PIXELS),
                    new Point(FIELD_LEFT_OFFSET_PIXELS + CELL_SIZE_PIXELS * ROWS_NUMBER, FIELD_TOP_OFFSET_PIXELS + row * CELL_SIZE_PIXELS)
                );

                for (int col = 0; col <= COLS_NUMBER; col++)
                {
                    g.DrawLine(Pens.Teal,
                        new Point(FIELD_LEFT_OFFSET_PIXELS + col * CELL_SIZE_PIXELS, FIELD_TOP_OFFSET_PIXELS),
                        new Point(FIELD_LEFT_OFFSET_PIXELS + col * CELL_SIZE_PIXELS, FIELD_TOP_OFFSET_PIXELS + CELL_SIZE_PIXELS * COLS_NUMBER)
                    );
                }
            }
        }

        private void GenerateFood()
        {
            bool isFoodClashWithSnake;
            do
            {
                food = new Point(rand.Next(0, ROWS_NUMBER), rand.Next(0, COLS_NUMBER));
                isFoodClashWithSnake = false;
                foreach (Point p in snake)
                {
                    if (p.X == food.X && p.Y == food.Y)
                    {
                        isFoodClashWithSnake = true;
                        break;
                    }
                }
            } while (isFoodClashWithSnake);

            timer1.Interval -= SPEED_INCREMENT_BY;
        }

        private void DrawSnake(Graphics g)
        {
            foreach (Point p in snake)
            {
                g.FillRectangle(Brushes.Green, new Rectangle(
                    FIELD_LEFT_OFFSET_PIXELS + p.Y * CELL_SIZE_PIXELS + 1,
                    FIELD_TOP_OFFSET_PIXELS + p.X * CELL_SIZE_PIXELS + 1,
                    CELL_SIZE_PIXELS - 1,
                    CELL_SIZE_PIXELS - 1));
            }
        }

        private void DrawFood(Graphics g)
        {
            g.FillRectangle(Brushes.OrangeRed, new Rectangle(
                FIELD_LEFT_OFFSET_PIXELS + food.Y * CELL_SIZE_PIXELS + 1,
                FIELD_TOP_OFFSET_PIXELS + food.X * CELL_SIZE_PIXELS + 1,
                CELL_SIZE_PIXELS - 1,
                CELL_SIZE_PIXELS - 1));
        }

        private void MoveSnake()
        {
            LinkedListNode<Point> head = snake.First;
            Point newHead = new Point(0, 0);
            switch (snakeDirection)
            {
                case SnakeDirection.Left:
                    newHead = new Point(head.Value.X, head.Value.Y - 1);
                    break;
                case SnakeDirection.Right:
                    newHead = new Point(head.Value.X, head.Value.Y + 1);
                    break;
                case SnakeDirection.Down:
                    newHead = new Point(head.Value.X + 1, head.Value.Y);
                    break;
                case SnakeDirection.Up:
                    newHead = new Point(head.Value.X - 1, head.Value.Y);
                    break;
            }

            if (snake.Any(point => point.X == newHead.X && point.Y == newHead.Y))
            {
                // "Змейка" съела саму себя! Конец игры!
                Invalidate();
                GameOver();
                return;
            }

            snake.AddFirst(newHead);

            if (newHead.X == food.X && newHead.Y == food.Y)
            {
                GenerateFood();
            }
            else
            {
                snake.RemoveLast();
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            DrawGrid(g);
            DrawFood(g);
            DrawSnake(g);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DoubleBuffered = true;
            BackColor = Color.Black;
            GameStart();
        }

        
        private bool IsGameOver()
        {
            LinkedListNode<Point> head = snake.First;
            switch (snakeDirection)
            {
                case SnakeDirection.Left:
                    return head.Value.Y - 1 < 0;
                case SnakeDirection.Right:
                    return head.Value.Y + 1 >= COLS_NUMBER;
                case SnakeDirection.Down:
                    return head.Value.X + 1 >= ROWS_NUMBER;
                case SnakeDirection.Up:
                    return head.Value.X - 1 < 0;
            }
            return false;
        }

        private void GameOver()
        {
            isGameEnd = true;
            timer1.Stop();
            if (MessageBox.Show("Конец игры! Начать заново?", "Конец игры", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                GameStart();
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (IsGameOver())
            {
                GameOver();
            }
            else
            {
                MoveSnake();
                Invalidate();
            }
        }

        private void ChangeSnakeDirection(SnakeDirection restrictedDirection, SnakeDirection newDirection)
        {
            if (snakeDirection != restrictedDirection)
            {
                snakeDirection = newDirection;
            }
        }

        private void FrmSnakeGame_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                case Keys.A:
                    ChangeSnakeDirection(SnakeDirection.Right, SnakeDirection.Left);
                    break;
                case Keys.Right:
                case Keys.D:
                    ChangeSnakeDirection(SnakeDirection.Left, SnakeDirection.Right);
                    break;
                case Keys.Down:
                case Keys.S:
                    ChangeSnakeDirection(SnakeDirection.Up, SnakeDirection.Down);
                    break;
                case Keys.Up:
                case Keys.W:
                    ChangeSnakeDirection(SnakeDirection.Down, SnakeDirection.Up);
                    break;
                case Keys.Escape:
                    timer1.Stop();
                    Close();
                    break;
                case Keys.Space:
                    if (isGameEnd && !timer1.Enabled)
                    {
                        GameStart();
                    }
                    break;
            }
        }
    }
}
