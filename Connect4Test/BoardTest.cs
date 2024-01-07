using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Connect4.AI;
namespace Connect4Test
{
    [TestClass]
    public class BoardTest
    {
        [TestMethod]
        public void TestMethod1()
        {
        }
        [TestMethod]
        public void Initial()
        {
            Board board = new Board();

            int i;
            int j;
            for (i = 0; i < board.Rows; i++)
            {
                for (j = 0; j < board.Cols; j++)
                {
                    Assert.AreEqual(board.Matrix[i, j], Board.CellValue.Empty);
                }
            }
            Assert.AreEqual(board.CurrentTurn, Board.Turn.Black);
        }
        [TestMethod]
        public void PutDiskAtColumn()
        {
            Board board = new Board();


            Assert.AreEqual(board.TopRow(0), -1);
            board.PutDiskAtColumn(0);
            Assert.AreEqual(board.Matrix[6, 0], Board.CellValue.Black );
            Assert.AreEqual(board.TopRow(0), 6);

            try
            {
                board.PutDiskAtColumn(-1);
                Assert.Fail("-1 is not valid column");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual(board.Matrix[5, 0], Board.CellValue.Empty);
            }

            try
            {
                board.PutDiskAtColumn(7);
                Assert.Fail("7 is not valid column");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(board.Matrix[5, 0], Board.CellValue.Empty);
            }
            board.PutDiskAtColumn(0);
            Assert.AreEqual(board.TopRow(0), 5);

            board.PutDiskAtColumn(0);
            board.PutDiskAtColumn(0);
            board.PutDiskAtColumn(0);
            board.PutDiskAtColumn(0);
            board.PutDiskAtColumn(0);

            for(int i = 0; i <= 6; i++)
            {
                Assert.AreEqual(board.Matrix[i, 0] , Board.CellValue.Black);
            }

            int avilableRow = board.GetRowAvilable(0);
            Assert.AreEqual(avilableRow, -1);
            try
            {
                board.PutDiskAtColumn(0);
                Assert.Fail(" At this point the column 0 is full, it must not allow to put");
            }catch (Exception ex)
            {

            }
           
        }

        

        [TestMethod]
        public void PutDiskAtColumnThenSwitchTurn()
        {
            Board board = new Board();
            Assert.AreEqual(board.CurrentTurn, Board.Turn.Black);
            int rowCanPut = board.PutDiskAtColumnThenSwitchTurn(0);
           
        
            Assert.AreEqual(rowCanPut, 6);
            Assert.AreEqual(board.Matrix[6, 0], Board.CellValue.Black );
            Assert.AreEqual(board.CurrentTurn, Board.Turn.White);

        }



        [TestMethod]
        public void WinStatusWestAndEast()
        {
            Board board = new Board();
            board.PutDiskAtColumn(0, Board.CellValue.Black);
            board.PutDiskAtColumn(1, Board.CellValue.Black);
            board.PutDiskAtColumn(2, Board.CellValue.Black);
            board.PutDiskAtColumn(3, Board.CellValue.Black);

            var winStatus = board.CheckWinStatus();
            Assert.IsTrue(winStatus == Board.GameResult.BlackWon);


        }

        [TestMethod]
        public void WinStatusWhiteNorthAndSouth()
        {
            Board board = new Board();
            board.PutDiskAtColumn(0, Board.CellValue.Black);
            board.PutDiskAtColumn(0, Board.CellValue.Black);
            board.PutDiskAtColumn(0, Board.CellValue.Black);
            board.PutDiskAtColumn(0, Board.CellValue.Black);

            var winStatus = board.CheckWinStatus();
            Assert.IsTrue(winStatus == Board.GameResult.BlackWon);


        }

        [TestMethod]
        public void WinStatusWhiteNorthWestAndSouthEast()
        {
            Board board = new Board();
            board.PutDiskAtColumn(1, Board.CellValue.White);
            board.PutDiskAtColumn(2, Board.CellValue.White);
            board.PutDiskAtColumn(2, Board.CellValue.White);
            board.PutDiskAtColumn(3, Board.CellValue.White);
            board.PutDiskAtColumn(3, Board.CellValue.White);
            board.PutDiskAtColumn(3, Board.CellValue.White);

            board.PutDiskAtColumn(0, Board.CellValue.Black);
            board.PutDiskAtColumn(1, Board.CellValue.Black);
            board.PutDiskAtColumn(2, Board.CellValue.Black);
            board.PutDiskAtColumn(3, Board.CellValue.Black);

            var winStatus = board.CheckWinStatus();
            Assert.IsTrue(winStatus == Board.GameResult.BlackWon);



        }


        [TestMethod]
        public void WinStatusWhiteNorthEastAndSouthWest()
        {
            Board board = new Board();
            board.PutDiskAtColumn(4, Board.CellValue.White);
            board.PutDiskAtColumn(3, Board.CellValue.White);
            board.PutDiskAtColumn(3, Board.CellValue.White);
            board.PutDiskAtColumn(2, Board.CellValue.White);
            board.PutDiskAtColumn(2, Board.CellValue.White);
            board.PutDiskAtColumn(2, Board.CellValue.White);

            board.PutDiskAtColumn(5, Board.CellValue.Black);
            board.PutDiskAtColumn(4, Board.CellValue.Black);
            board.PutDiskAtColumn(3, Board.CellValue.Black);
            board.PutDiskAtColumn(2, Board.CellValue.Black);

            var winStatus = board.CheckWinStatus();
            Assert.IsTrue(winStatus == Board.GameResult.BlackWon);



        }
       
    }
}
