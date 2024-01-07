using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Connect4
{
    public partial class FormConnect4 : Form
    {
        public FormConnect4()
        {
            InitializeComponent();
        }
        bool IsPlayWithBot = true;
        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Invalidate();
        }
        AI.Board board = new AI.Board();
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Icon = Resource1.Connect4Icon;
            pictureBox1.Paint += PictureBox1_Paint;
            pictureBox1.Width = OffSetX * 2 + (board.Cols * CellWidth);
            pictureBox1.MouseMove += PictureBox1_MouseMove;
            pictureBox1.Click += PictureBox1_Click;
            this.MouseMove += Form1_MouseMove;
            recTable = new Rectangle[board.Rows, board.Cols];
            int i;
            int j;
            for (i = 0; i < board.Rows; i++)
            {
                for (j = 0; j < board.Cols; j++)
                {
                    Rectangle rec = new Rectangle(OffSetX + j * CellWidth,
                              OffSetY + i * CellWidth,
                              CellWidth - 10,
                              CellWidth - 10);
                    recTable[i, j] = rec;
                   

                }
            }
            this.Width = 500;
            this.Player1Completed += Form1_Player1Completed;
            this.Player2Completed += Form1_Player2Completed;
        }

        private void Form1_Player2Completed(object sender, EventArgs e)
        {
            // throw new NotImplementedException();
            CheckResult();

        }

        private void Form1_Player1Completed(object sender, EventArgs e)
        {
            // throw new NotImplementedException();
            //  MinimizeBox minimax = new MinimizeBox(this.board);
            CheckResult();

            if(board.Result != AI.Board.GameResult.NotDecideYet)
            {
                return;
            }

            if (!IsPlayWithBot)
            {
                return;
            }
            AI.MinimaxNew minimax = new AI.MinimaxNew(this.board);
            int depth = 6;
            int botColumnChoosen = minimax.GetNextMove(depth);
            ColHover = botColumnChoosen;
            PutCell(botColumnChoosen );

        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            ColHover = -1;
            pictureBox1.Invalidate();
           // throw new NotImplementedException();
        }

        Timer timerPutCell = null;
        int rowAvilable = -1;
        int currentRow = 0;
        bool IsPuttingDisk = false;

        private void PictureBox1_Click(object sender, EventArgs e)
        {

            if (IsPlayWithBot && board.CurrentTurn == AI.Board.Turn.White)
            {
                return;
            }
            if (board.Result != AI.Board.GameResult.NotDecideYet)
            {
                return;
            }
            if (IsPuttingDisk)
           {
                return;
           }

           if(ColHover == -1)
            {
                return;
            }
           

            PutCell(ColHover);

            
        }


        private int ColumnNeedtoPutCell = -1;
        private void PutCell(int column)
        {
            int rowAvialble = board.GetRowAvilable(column);
            if (rowAvialble == -1)
            {
                return;
            }

          

            this.rowAvilable = rowAvialble;
            currentRow = 0;
            if (timerPutCell != null)
            {
                timerPutCell.Tick -= TimerPutCell_Tick;
                timerPutCell.Enabled = false;
            }
            IsPuttingDisk = true;
            ColumnNeedtoPutCell = column;
            timerPutCell = new Timer();
            timerPutCell.Interval = 30;
            timerPutCell.Tick += TimerPutCell_Tick;
            timerPutCell.Enabled = true;
        }
        private event EventHandler Player2Completed;
        private event EventHandler Player1Completed;
        private void CheckResult()
        {
            var result = board.CheckWinStatus();
            if(result == AI.Board.GameResult.NotDecideYet)
            {
                return;
            }

            String message = "";

            
                switch (result)
                {
                    case AI.Board.GameResult.BlackWon:
                        message = "Playser 1 Won.";
                        break;
                    case AI.Board.GameResult.WhiteWon:
                    message = IsPlayWithBot
                    ? "Bot Won."
                    : "Player 2 Won";
                        break;
                    case AI.Board.GameResult.Draw:
                        message = "Draw.";
                        break;
                }
            
            MessageBox.Show(message);

        }
        private void TimerPutCell_Tick(object sender, EventArgs e)
        {
            if(currentRow == this.rowAvilable)
            {
                IsPuttingDisk = false;
                board.PutDiskAtColumnThenSwitchTurn(ColumnNeedtoPutCell);
                ColumnNeedtoPutCell = -1;
                ColHover = -1;
                currentRow = 0;
                timerPutCell.Enabled = false;

                EventHandler playerComplete = board.CurrentTurn == AI.Board.Turn.White
                                            ? Player1Completed
                                            : Player2Completed;

                playerComplete?.Invoke(this, null);
                /*
                if(board.CurrentTurn == AI.Board.Turn.White)
                {
                    playerComplete = Player2Completed;
                    Player1Completed?.Invoke(this, null);
                } else
                {
                    Player2Completed?.Invoke(this, null;)
                }
                */
                return;
            }
            currentRow++;
            pictureBox1.Invalidate();
            //throw new NotImplementedException();
        }

        private int ColHover = -1;
        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if(IsPlayWithBot && board.CurrentTurn == AI.Board.Turn.White)
            {
                return;
            }
            if(board.Result != AI.Board.GameResult.NotDecideYet )
            {
                return;
            }
            if (IsPuttingDisk)
            {
                return;
            }
            int i;
            int j;
            ColHover = (e.X -OffSetX ) / CellWidth;
            pictureBox1.Invalidate();
           // int X=e.X + ddddddddddddddddddddddd
           // throw new NotImplementedException();
        }

        private int CellWidth = 70;
        int OffSetX = 20;
        int OffSetY = 20;
        Rectangle[,] recTable = null;
        private Color GetColorByTurn(AI.Board.CellValue cellValue)
        {
            switch (cellValue)
            {
                case AI.Board.CellValue.White:
                    return Color.Yellow; // Color.FromArgb (255,255,128)  ;

                case AI.Board.CellValue.Black:
                    return Color.Blue              ;

                case AI.Board.CellValue.Empty:
                    return Color.White;
                default:
                    throw new Exception($"{nameof(cellValue)} is not valid  :{cellValue}");
            }
        }
        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            //throw new NotImplementedException();
           // AI.Board board = new AI.Board();
            int i;
            int j;
            Graphics g = e.Graphics;
            g.Clear(Color.LightSeaGreen  );
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;


            Rectangle r = new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height);
            System.Drawing.Drawing2D.GraphicsPath RouncedRectangle = new System.Drawing.Drawing2D.GraphicsPath();
            int d = 50;
            RouncedRectangle.AddArc(r.X, r.Y, d, d, 180, 90);
            RouncedRectangle.AddArc(r.X + r.Width - d, r.Y, d, d, 270, 90);
            RouncedRectangle.AddArc(r.X + r.Width - d, r.Y + r.Height - d, d, d, 0, 90);
            RouncedRectangle.AddArc(r.X, r.Y + r.Height - d, d, d, 90, 90);
            pictureBox1.Region = new Region(RouncedRectangle);


            for (i = 0; i < board.Rows; i++)
            {
                for (j = 0; j < board.Cols; j++)
                {
                    Color colorCell = Color.White;
                    bool IsTopRowofHoverColor = (i == currentRow && j == ColHover);
                    if (IsTopRowofHoverColor)
                    {
                        colorCell = GetColorByTurn((AI.Board.CellValue ) board.CurrentTurn);
                    }
                    else
                    {
                        colorCell = GetColorByTurn(board.Matrix[i, j]);
                    }

                    using (Brush b=new SolidBrush (colorCell ))
                    {
                        Rectangle rec = new Rectangle(OffSetX + j * CellWidth , 
                                                      OffSetY + i * CellWidth , 
                                                      CellWidth -10, 
                                                      CellWidth -10);
                        //g.DrawEllipse(pen, rec);
                        g.FillEllipse(b, rec);
                    }

                }
            }

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form_Resize(object sender, EventArgs e)
        {
          //  this.Text = this.Width.ToString();

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show ("Do you want to exit", "", MessageBoxButtons.OKCancel ) != DialogResult.OK)
            {
                return;
            }
            Application.Exit();

        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            board = new AI.Board();
            this.IsPlayWithBot = this.toolStripMenuItemPlayWithBot.Checked;
            this.pictureBox1.Invalidate();

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.toolStripMenuItemPlayWithBot.Checked = !this.toolStripMenuItemPlayWithBot.Checked;

        }
    }
}
