using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4.AI
{
    public class Board
    {
        public enum CellValue
        {
            Black=-1,
            Empty=0,
            White=1
        }
        public enum Turn
        {
            Black=CellValue.Black ,
            White=CellValue.White 
        }
        public enum GameResult
        {
            BlackWon,
            WhiteWon,
            NotDecideYet,
            Draw
        };
        public Board()
        {
            Initial();
        }
        public GameResult Result { get; private set; } = GameResult.NotDecideYet;
        public Turn CurrentTurn { get; private set; } = Turn.Black;
        public CellValue[,] Matrix { get; private set; }
        public Dictionary<String, AI.Position> dicWhiteDisk { get; private set; } = new Dictionary<String, AI.Position>();
        public Dictionary<String, AI.Position> dicBlackDisk { get; private set; } = new Dictionary<String, AI.Position>();

        public List<int> listHistory = new List<int>();
        public bool CanUndo => listHistory != null && listHistory.Count > 0;
        public int Rows { get; private set; } = 7;
        public int Cols { get; private set; } = 6;
        public void SwitchTurn()
        {
            if(CurrentTurn == Turn.Black)
            {
                CurrentTurn = Turn.White;
                return;
            }
            CurrentTurn = Turn.Black;
        }


        public void Undo()
        {
            int LastIndex = listHistory.Count - 1;
            int column = listHistory[LastIndex];
              listHistory.RemoveAt(LastIndex);
            int topRow = this.TopRow(column);
            RemoveDisk(new Position(topRow, column));

            this.SwitchTurn();

        }
        public void RemoveDisk(Position position)
        {


            if (dicBlackDisk.ContainsKey(position.PositionString()))
            {
                dicBlackDisk.Remove(position.PositionString());
            }
            else
            {
                if (dicWhiteDisk.ContainsKey(position.PositionString()))
                {
                    dicWhiteDisk.Remove(position.PositionString());
                }
                else
                {
                    throw new Exception(String.Format("Postion is not correct {0},{1}", position.Row, position.Col));
                }

            }

            Matrix[position.Row, position.Col] = 0;

        }
        public List<int> genereateAllPossiblePutColumns()
        {
            List<int> list = new List<int>();
            int i = 0;
            for (i = 0; i < Cols; i++)
            {
                int avilableMove = this.GetRowAvilable(i);
                if(!IsValidRow(avilableMove))
                {
                    continue;
                }

                list.Add(i);
            }
            return list;
        }
        public int GetRowAvilable(int columnNumber)
        {
            if(columnNumber >= Cols)
            {
                throw new Exception($"columun number is invalid {columnNumber}");
            }

            int rowAvilable = 6;
            int topRow = TopRow(columnNumber);

            
            rowAvilable = topRow > -1 && IsValidRow(topRow)
                ? topRow - 1
                : rowAvilable;
                
            return rowAvilable;
        }
        public int TopRow(int columnNumber)
        {
            if(Matrix [Rows -1,columnNumber]== CellValue.Empty)
            {
                return -1;
            }

            for (int i =Rows -1; i >=0; i--)
            {
                if (Matrix[i, columnNumber] == CellValue.Empty)
                {
                    return i+1;
                }
            }
            return 0;
        }
        public bool IsValidRow(int row)
        {
            if(row < 0 || row >= Rows){
                return false;
            }
            return true;
        }
        public bool IsValidCol(int col)
        {
            if (col < 0 || col >= this.Cols)
            {
                return false;
            }
            return true;
        }
        private Dictionary<String, Position> GetHshByCellValue(CellValue cellValue)
        {
            if (cellValue == CellValue.White)
            {
                return dicWhiteDisk;
            }
            return dicBlackDisk;
        }
        public int PutDiskAtColumn(int columnNumber, CellValue cellValue)
        {
            int rowAvilable = GetRowAvilable(columnNumber);

                if (!IsValidRow(rowAvilable))
                {
                    throw new Exception($"{rowAvilable} is not valid row");
                }

            Matrix[rowAvilable, columnNumber] = cellValue;
            Position newPosition = new Position(rowAvilable, columnNumber);
            GetHshByCellValue(cellValue).Add(newPosition.PositionString(), newPosition);
            listHistory.Add(columnNumber);
            return rowAvilable;
        }
        public int PutDiskAtColumn(int columnNumber)
        {
            if (!IsValidCol(columnNumber))
            {
                throw new ArgumentException($"{columnNumber} is not valid column");
            }
           return  PutDiskAtColumn(columnNumber, (CellValue)CurrentTurn);
        }
        public int PutDiskAtColumnThenSwitchTurn(int columnNumber)
        {
           int rowCanPut= PutDiskAtColumn(columnNumber);
            SwitchTurn();
            return rowCanPut;
        }
        public Boolean IsFull
        {
            get
            {
                return dicBlackDisk.Count + dicWhiteDisk.Count == this.Rows * this.Cols ;
            }
        }

        public GameResult CheckWinStatus()
        {

            foreach (Position pos in dicWhiteDisk.Values)
            {

                if (IsThere4inRow(pos))
                {
                    this.Result = GameResult.WhiteWon;
                    return this.Result;
                   // return GameResult.WhiteWon;
                }
            }
            foreach (Position pos in dicBlackDisk.Values)
            {
                if (IsThere4inRow(pos))
                {
                    this.Result = GameResult.BlackWon;
                    return this.Result;
                    //return GameResult.BlackWon;
                }
            }
            if (IsFull)
            {
                this.Result = GameResult.Draw;
                return this.Result;
                //return GameResult.Draw;
            }
            this.Result = GameResult.NotDecideYet;
            return this.Result;
            //foreach ()
            //return GameResult.NotDecideYet;
        }
        public Boolean IsThere4inRow(Position pos)
        {
            int i;
            int j;
            Board.CellValue cellValue = this.Matrix[pos.Row, pos.Col];
            if (cellValue == 0)
            {
                return false;
            }

            for (i = -1; i <= 1; i++)
            {

                for (j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        continue;
                    }
                    int NoofRepeating = 0;

                    int SameCellValueInDirectionCount = 0;
                    for (NoofRepeating = 1; NoofRepeating <= 3; NoofRepeating++)
                    {
                        Position CheckPosition = new Position(pos.Row + (i * NoofRepeating),
                            pos.Col + (j * NoofRepeating));
                        if (CheckPosition.Row < 0
                            || CheckPosition.Row > Rows - 1
                            || CheckPosition.Col < 0
                            || CheckPosition.Col > Cols - 1)
                        {
                            break;
                        }

                        if (Matrix[CheckPosition.Row, CheckPosition.Col] != cellValue)
                        {
                            break;
                        }
                        SameCellValueInDirectionCount++;
                        if (SameCellValueInDirectionCount >= 3)
                        {
                            return true;
                        }
                    }

                    // Position posCheck


                }
            }
            return false;
        }


        public Board(Board board)
        {
            // int[,] matrixToCopy = board.Matrix;
            Matrix = new CellValue[board.Matrix.GetLength(0), board.Matrix.GetLength(1)];
            dicWhiteDisk = new Dictionary<string, Position>();
            dicBlackDisk = new Dictionary<string, Position>();
         //   dicNeighbor = new Dictionary<string, Position>();

            this.Matrix = board.Matrix.Clone() as CellValue[,];
            this.dicWhiteDisk = new Dictionary<string, Position>(board.dicWhiteDisk);
            this.dicBlackDisk = new Dictionary<string, Position>(board.dicBlackDisk);
         //   this.dicNeighbor = new Dictionary<string, Position>(board.dicNeighbor);
            this.listHistory = new List<int>(board.listHistory);
            // this.BoardSize = board.BoardSize;
            this.Rows = 7;
            this.Cols = 6;
            this.CurrentTurn = board.CurrentTurn;

        }
        public Board Clone()
        {
            return new Board(this);
        }
        /*
        public void CheckGameResult()
        {
            int i;
            int j;
            for (i = 0; i < Rows; i++)
            {
                int countWhiteConsecutive = 0;
                int countBlackConsecutive = 0;

                for (j = 0; j < Cols; j++)
                {
                   switch (Matrix[i, j])
                    {
                        case CellValue.Black:
                            countBlackConsecutive++;
                            countWhiteConsecutive = 0;
                            break;
                        case CellValue.White:
                            countWhiteConsecutive++;
                            countBlackConsecutive = 0;
                            break;
                        case CellValue.Empty:
                            countBlackConsecutive = 0;
                            countWhiteConsecutive = 0;
                            break;
                    }
                }
                if(countBlackConsecutive >= 4)
                {
                    Result = GameResult.BlackWon;
                }
                if (countWhiteConsecutive >= 4)
                {
                    Result = GameResult.WhiteWon;
                }
            }
        }
        */
        public void Initial()
        {
            int i;
            int j;
            Matrix = new CellValue[Rows, Cols];

            for (i = 0; i < Rows; i++)
            {
                for (j = 0; j < Cols; j++)
                {
                    Matrix[i, j] = CellValue.Empty;
                }
            }
        }

        public List<Object> GenerateMoves()
        {
            int i;
            int j;
            List<object> listAvilableMove = new List<object>();
            for (j = 0; j < Cols; j++)
            {
                for (i = 0; i < Rows; i++)
                {
                    if(Matrix [i,j] != CellValue.Empty)
                    {
                        int avilableMoveColun = i - 1;
                        if(avilableMoveColun < 0)
                        {
                            continue;
                        }
                        listAvilableMove.Add(avilableMoveColun);
                    }
                }
            }
            return listAvilableMove;
           // throw new NotImplementedException();
        }

        public void Move(Object moveInfo)
        {
            int iColumnMove = (int)moveInfo;
            this.PutDiskAtColumnThenSwitchTurn(iColumnMove);

            //throw new NotImplementedException();
        }


    }
}
