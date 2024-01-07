using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4.AI
{
    class MoveScore
    {
        public double Score { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        public MoveScore(double pScore)
        {
            Score = pScore;
            Row = -1;
            Col = -1;
        }
        public MoveScore()
        {

        }
        public Position GetPosition()
        {
            return new Position(this.Row, this.Col);
        }
        public MoveScore(double Score, int Row, int Col)
        {
            this.Score = Score;
            this.Row = Row;
            this.Col = Col;
        }
    }
}
