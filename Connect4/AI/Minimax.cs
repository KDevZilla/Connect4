using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4.AI
{
    public class MinimaxNew
    {



        private Board board;

        private int WonScore = 100000000;
        private int GuraranteeToWinScore = 1000000;

        public MinimaxNew(Board board)
        {
            this.board = board;

        }

        private void Log(String message)
        {
            /*
            if (this.log == null)
            {
                return;
            }
            log.Log("MiniMaxNew::" + message);
            */
        }




        public double evaluateBoardForWhite(Board board, Boolean blacksTurn)
        {


            double blackScore = CalcualteScore(board, true, blacksTurn);
            double whiteScore = CalcualteScore(board, false, blacksTurn);

            if (blackScore == 0) blackScore = 1.0;


            if (blacksTurn)
            {
                return blackScore / whiteScore;
            }

            return whiteScore / blackScore;
        }

        public int CalcualteScore(Board board, Boolean IsThisScoreforBlack, Boolean IsCurrentTurnblacks)
        {

            Board.CellValue[,] boardMatrix = board.Matrix;
            int horizontalScore = EvaluateHorizontal(boardMatrix, IsThisScoreforBlack, IsCurrentTurnblacks);
            int verticalScore = EvaluateVertical(boardMatrix, IsThisScoreforBlack, IsCurrentTurnblacks);
            int diagonalScore = EvaluateDiagonal(boardMatrix, IsThisScoreforBlack, IsCurrentTurnblacks);

            return horizontalScore +
                    verticalScore +
                    diagonalScore;
        }
        private bool IsBoardEmpty(Board.CellValue[,] boardMatrix)
        {
            int i;
            int j;

            for (i = 0; i <= boardMatrix.GetUpperBound(0); i++)
            {
                for (j = 0; j < boardMatrix.GetUpperBound(1); j++)
                {
                    if (boardMatrix[i, j] !=  Board.CellValue.Empty )
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public int NoofNodes = 0;
        private static readonly Random getrandom = new Random();

        public static int GetRandomNumber(int min, int max)
        {
            lock (getrandom) // synchronize
            {
                return getrandom.Next(min, max);
            }
        }
        public int GetNextMove(int depth)
        {

            MoveScore botWinningMove = new MoveScore();

            botWinningMove = searchBotWinningPosition(board);
            if (botWinningMove.Col  != -1)
            {
                return botWinningMove.GetPosition().Col ;
            }

            MoveScore OpponenetWinningMove = new MoveScore();
            OpponenetWinningMove = searchOpponentWinningPosition(board);
            if (OpponenetWinningMove.Col  != -1)
            {

                return OpponenetWinningMove.GetPosition().Col ;

            }
            MoveScore botMoveScore = new MoveScore();
            if (IsBoardEmpty(board.Matrix))
            {

                botMoveScore.Col =GetRandomNumber(0, board.Cols );
                return botMoveScore.Col;

            }


            NoofNodes = 0;
            miniMaxParameter Para = new miniMaxParameter();
            Para.Depth = depth;

            Para.board = board.Clone();
            Para.IsMax = true;
            Para.Alpha = -1.0;
            Para.Beta = WonScore;

            NoofNodeInEachLevel = new List<int>();
            int i;
            for (i = 0; i <= depth; i++)
            {
                NoofNodeInEachLevel.Add(0);
            }
         
            botMoveScore = minimaxSearchAB(Para.Depth, Para.board, Para.IsMax, Para.Alpha, Para.Beta);



            return botMoveScore.GetPosition().Col ;
        }




        public class miniMaxParameter
        {
            public int Depth { get; set; }
            public Board board { get; set; }
            public Boolean IsMax { get; set; }
            public double Alpha = 0;
            public double Beta = 0;
            public miniMaxParameter Clone()
            {
                miniMaxParameter CloneObject = new miniMaxParameter();
                CloneObject.Depth = this.Depth;
                CloneObject.board = this.board.Clone();
                CloneObject.IsMax = this.IsMax;
                CloneObject.Alpha = this.Alpha;
                CloneObject.Beta = this.Beta;
                return CloneObject;
            }
        }

        private List<int> NoofNodeInEachLevel = null;
        private MoveScore minimaxSearchAB(int depth, Board board, Boolean IsMax, double Alpha, double Beta)
        {
            NoofNodes++;
            NoofNodeInEachLevel[depth]++;


            MoveScore movescore = new MoveScore();

            if (depth == 0)
            {
                movescore = new MoveScore(evaluateBoardForWhite(board, !IsMax));
                return movescore;
            }

            List<int> allPossibleMove = board.genereateAllPossiblePutColumns();

            bool IsNothingLeftToSearch = (allPossibleMove.Count == 0);
            if (IsNothingLeftToSearch)
            {
                movescore = new MoveScore(evaluateBoardForWhite(board, !IsMax));
                return movescore;



            }


            MoveScore bestMove = new MoveScore();
            int depthChild = 0;
            Boolean isMaxChild = false;
            depthChild = depth - 1;
            isMaxChild = !IsMax;
            if (IsMax)
            {
                bestMove.Score = -1.0;
            }
            else
            {
                bestMove.Score = 100000000.0;
                bestMove.Col = allPossibleMove[0];
            
            }

            foreach (int moveColumn in allPossibleMove)
            {

                    board.PutDiskAtColumnThenSwitchTurn(moveColumn);
                    movescore = minimaxSearchAB(depthChild, board, isMaxChild, Alpha, Beta);

                    if (board.IsFull)
                    {
                        movescore = new MoveScore(evaluateBoardForWhite(board, !IsMax));
                        return movescore;

                    }
                    board.Undo();
               

                if (IsMax)
                {
                    Alpha = Math.Max(Alpha, movescore.Score);
                    if (movescore.Score >= Beta)
                    {
                        return movescore;
                    }

                    if (movescore.Score > bestMove.Score)
                    {
                        bestMove = movescore;
                        bestMove.Col = moveColumn;

                    }
                }
                else
                {
                    Beta = Math.Min(Beta, movescore.Score);

                    if (movescore.Score > Alpha)
                    {
                        return movescore;
                    }
                    if (movescore.Score < bestMove.Score)
                    {
                        bestMove = movescore;
                    }
                }
            }
            return bestMove;

        }
       


        private MoveScore searchBotWinningPosition(Board board)
        {

            List<int> allPossibleMoves = board.genereateAllPossiblePutColumns();
            MoveScore winningPosition = new MoveScore();
            winningPosition.Score = -1;
            winningPosition.Row = -1;
            winningPosition.Col = -1;

          
            MoveScore maxMoveScore = new MoveScore(int.MinValue, -1, -1);

            foreach (int move in allPossibleMoves)
            {

                Board dummyBoard = new Board(board);

                dummyBoard.PutDiskAtColumn(move);

                int Score = CalcualteScore(dummyBoard, false, false);
                if (Score > maxMoveScore.Score)
                {
                    maxMoveScore = new MoveScore(Score, -1, move);
                }

            }

            if (maxMoveScore.Score >= WonScore)
            {
                return maxMoveScore;
            }
            return winningPosition;
        }


        private MoveScore searchOpponentWinningPosition(Board board)
        {

            List<int> allPossiblePutColumns = board.genereateAllPossiblePutColumns();
            MoveScore winningPosition = new MoveScore();
            winningPosition.Score = -1;
            winningPosition.Row = -1;
            winningPosition.Col = -1;


            int maxScore = int.MinValue;
            MoveScore maxMoveScore = new MoveScore(int.MinValue, -1, -1);

            foreach (int column in allPossiblePutColumns)
            {

                Board dummyBoard = new Board(board);

                dummyBoard.SwitchTurn();
                dummyBoard.PutDiskAtColumn(column);
                int Score = CalcualteScore(dummyBoard, true, true);

                if (Score > maxMoveScore.Score)
                {

                    maxMoveScore = new MoveScore(Score, -1, column);
                }

            }

            if (maxMoveScore.Score >= 10000)
            {
                return maxMoveScore;

            }
            return winningPosition;
        }
        public int EvaluateHorizontal(Board.CellValue[,] boardMatrix, Boolean isForBlackTurn, Boolean playersTurn)
        {

            int NumberOfConsecutiveDisk = 0;

            BlockType blockType = BlockType.BothSideBlock;
            int score = 0;

            for (int i = 0; i < boardMatrix.GetLength(0); i++)
            {
                NumberOfConsecutiveDisk = 0;
                blockType = BlockType.BothSideBlock;
                for (int j = 0; j < boardMatrix.GetLength(1); j++)
                {
                     
                    if (boardMatrix[i, j] == (isForBlackTurn 
                        ? Board.CellValue.Black  
                        : Board.CellValue.White ))
                    {
                        NumberOfConsecutiveDisk++;
                    }
                    else if (boardMatrix[i, j] == Board.CellValue.Empty)
                    {

                        if (NumberOfConsecutiveDisk > 0)
                        {
                            blockType--;

                           
                            score += getConsecutiveSetScore(NumberOfConsecutiveDisk, blockType, isForBlackTurn == playersTurn);
                            NumberOfConsecutiveDisk = 0;

                            blockType = BlockType.OneSideBLock;
                        }
                        else
                        {
                            blockType = BlockType.OneSideBLock;

                        }
                    }
                    else if (NumberOfConsecutiveDisk > 0)
                    {

                        score += getConsecutiveSetScore(NumberOfConsecutiveDisk, blockType , isForBlackTurn == playersTurn);
                        NumberOfConsecutiveDisk = 0;

                        blockType = BlockType.BothSideBlock;
                    }
                    else
                    {

                        blockType = BlockType.BothSideBlock;
                    }
                }
               
                if (NumberOfConsecutiveDisk > 0)
                {
                    score += getConsecutiveSetScore(NumberOfConsecutiveDisk, blockType, isForBlackTurn == playersTurn);
                }


            }

            return score;
        }

        public int EvaluateVertical(Board.CellValue[,] boardMatrix, Boolean forBlack, Boolean playersTurn)
        {

            int NumberOfConsecutiveDisk = 0;

            BlockType blockType = BlockType.BothSideBlock;
            int score = 0;

            for (int j = 0; j < boardMatrix.GetLength(1); j++)
            {
                for (int i = 0; i < boardMatrix.GetLength(0); i++)
                {
                    if (boardMatrix[i, j] == (forBlack 
                        ? Board.CellValue.Black 
                        : Board.CellValue.White ))
                    {
                        NumberOfConsecutiveDisk++;
                    }
                    else if (boardMatrix[i, j] ==  Board.CellValue.Empty)
                    {
                        if (NumberOfConsecutiveDisk > 0)
                        {
                            blockType--;
                            score += getConsecutiveSetScore(NumberOfConsecutiveDisk, blockType, forBlack == playersTurn);
                            NumberOfConsecutiveDisk = 0;
                        
                            blockType = BlockType.OneSideBLock;
                        }
                        else
                        {
                            blockType = BlockType.OneSideBLock;
                        }
                    }
                    else if (NumberOfConsecutiveDisk > 0)
                    {
                        score += getConsecutiveSetScore(NumberOfConsecutiveDisk, blockType, forBlack == playersTurn);
                        NumberOfConsecutiveDisk = 0;

                        blockType = BlockType.BothSideBlock;
                    }
                    else
                    {

                        blockType = BlockType.BothSideBlock;
                    }
                }
                if (NumberOfConsecutiveDisk > 0)
                {
                    score += getConsecutiveSetScore(NumberOfConsecutiveDisk, blockType, forBlack == playersTurn);

                }
                NumberOfConsecutiveDisk = 0;
                //Noofblocks = 2;
                blockType = BlockType.BothSideBlock;

            }
            return score;
        }

        public int EvaluateDiagonal(Board.CellValue[,] boardMatrix, Boolean forBlack, Boolean playersTurn)
        {

            int NumberOfConsecutiveDisk = 0;

            BlockType blockType = BlockType.BothSideBlock;
            int score = 0;

            // https://stackoverflow.com/questions/20420065/loop-diagonally-through-two-dimensional-array
            for (int k = 0; k <= 2 * 7; k++)
            {
                int iStart = Math.Max(0, k - boardMatrix.GetLength(0) + 1);
                int iEnd = Math.Min(boardMatrix.GetLength(0) - 1, k);
                NumberOfConsecutiveDisk = 0;

                blockType = BlockType.BothSideBlock;
                for (int i = iStart; i <= iEnd; ++i)
                {
                    int j = k - i;
                    if(!board.IsValidRow (i) ||
                        !board.IsValidCol(j))
                    {

                        break;
                    }
                    if (boardMatrix[i, j] == (forBlack 
                                ? Board.CellValue.Black  
                                : Board.CellValue.White) )
                    {
                        NumberOfConsecutiveDisk++;
                    }
                    else if (boardMatrix[i, j] ==  Board.CellValue.Empty)
                    {
                        if (NumberOfConsecutiveDisk > 0)
                        {

                            blockType--;
                            score += getConsecutiveSetScore(NumberOfConsecutiveDisk, blockType, forBlack == playersTurn);
                            NumberOfConsecutiveDisk = 0;

                            blockType = BlockType.OneSideBLock;
                        }
                        else
                        {

                            blockType = BlockType.OneSideBLock;
                        }
                    }
                    else if (NumberOfConsecutiveDisk > 0)
                    {
                        score += getConsecutiveSetScore(NumberOfConsecutiveDisk, blockType, forBlack == playersTurn);
                        NumberOfConsecutiveDisk = 0;

                        blockType = BlockType.BothSideBlock;
                    }
                    else
                    {

                        blockType = BlockType.BothSideBlock;
                    }

                }
                if (NumberOfConsecutiveDisk > 0)
                {
                    score += getConsecutiveSetScore(NumberOfConsecutiveDisk, blockType, forBlack == playersTurn);

                }

            }

            for (int k = 1 - boardMatrix.GetLength(0); k < boardMatrix.GetLength(0); k++)
            {
                int iStart = Math.Max(0, k);
                int iEnd = Math.Min(boardMatrix.GetLength(0) + k - 1, boardMatrix.GetLength(0) - 1);
                NumberOfConsecutiveDisk = 0;

                blockType = BlockType.BothSideBlock;
                for (int i = iStart; i <= iEnd; ++i)
                {
                    int j = i - k;
                    if (!board.IsValidRow(i) ||
                       !board.IsValidCol(j))
                    {

                        break;
                    }
                    if (boardMatrix[i, j] == (forBlack
                                ? Board.CellValue.Black
                                : Board.CellValue.White))
                    {
                        NumberOfConsecutiveDisk++;
                    }
                    else if (boardMatrix[i, j] ==  Board.CellValue.Empty)
                    {
                        if (NumberOfConsecutiveDisk > 0)
                        {

                            blockType--;
                            score += getConsecutiveSetScore(NumberOfConsecutiveDisk, blockType, forBlack == playersTurn);
                            NumberOfConsecutiveDisk = 0;

                            blockType = BlockType.OneSideBLock;
                        }
                        else
                        {

                            blockType = BlockType.OneSideBLock;
                        }
                    }
                    else if (NumberOfConsecutiveDisk > 0)
                    {
                        score += getConsecutiveSetScore(NumberOfConsecutiveDisk, blockType, forBlack == playersTurn);
                        NumberOfConsecutiveDisk = 0;

                        blockType = BlockType.BothSideBlock;
                    }
                    else
                    {

                        blockType = BlockType.BothSideBlock;
                    }

                }
                if (NumberOfConsecutiveDisk > 0)
                {
                    score += getConsecutiveSetScore(NumberOfConsecutiveDisk, blockType, forBlack == playersTurn);

                }

            }
            return score;
        }

        public enum BlockType
        {
            NoBlockBothSize = 0,
            OneSideBLock = 1,
            BothSideBlock = 2
        }
        public int getConsecutiveSetScore(int noOfConsectiveDisk, BlockType blockType, Boolean IscurrentTurn)
        {


            bool isUseLess = (blockType == BlockType.BothSideBlock  &&
                noOfConsectiveDisk < 4);
            if (isUseLess)
            {
                return 0;
            }



            if (noOfConsectiveDisk > 4)
            {
                return WonScore * 3;
            }


            switch (noOfConsectiveDisk)
            {
                case 4:
                    {
                        return WonScore;
                    }
                case 3:
                    {
                        if (blockType ==  BlockType.NoBlockBothSize)
                        {
                            return GuraranteeToWinScore;
                        }

                        if (IscurrentTurn)
                        {

                            return GuraranteeToWinScore / 2;

                        }
                        else
                        {

                            return 200;
                        }
                    }
                case 2:
                    {

                        if (blockType ==  BlockType.NoBlockBothSize)
                        {
                            if (IscurrentTurn)
                            {
                                return 50000;
                            }
                            else
                            {
                                return 10000;
                            }

                        }
                        else
                        {

                            if (IscurrentTurn)
                            {
                                return 10;
                            }
                            else
                            {
                                return 5;
                            }
                        }
                    }

                case 1:
                    {
                        return 1;
                    }
            }


            return WonScore * 2;
        }

    }
}
