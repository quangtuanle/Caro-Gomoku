using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gomoku_1312659_Ver4
{
    public class ChessBoard
    {
        const int ROW = 12;
        const int COL = 12;

        public List<ChessSquare> Squares { get; private set; }

        public Command<ChessSquare> SquareClickCommand { get; private set; }

        public ChessBoard()
        {
            Squares = new List<ChessSquare>();

            for (int i = 0; i < ROW; i++)
            {
                for (int j = 0; j < COL; j++)
                {
                    Squares.Add(new ChessSquare() { Row = i, Column = j, Static = (i + j) % 2 });
                }
            }

            SquareClickCommand = new Command<ChessSquare>(OnSquareClick);
        }

        public ChessBoard(int[][] arrSquare)
        {
            Squares = new List<ChessSquare>();

            for (int i = 0; i < ROW; i++)
            {
                for (int j = 0; j < COL; j++)
                {
                    Squares.Add(new ChessSquare() { Row = i, Column = j, Static = arrSquare[i][j] });
                }
            }

            SquareClickCommand = new Command<ChessSquare>(OnSquareClick);
        }

        private void OnSquareClick(ChessSquare square)
        {
            ClickSquare(square.Row, square.Column);
        }

        public delegate void OnClickSquare(int row, int col);
        public event OnClickSquare ClickSquare;
    }

    public class Command<T> : ICommand
    {
        public Action<T> Action { get; set; }

        public void Execute(object parameter)
        {
            if (Action != null && parameter is T)
                Action((T)parameter);
        }

        public bool CanExecute(object parameter)
        {
            return IsEnabled;
        }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                if (CanExecuteChanged != null)
                    CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler CanExecuteChanged;

        public Command(Action<T> action)
        {
            Action = action;
        }
    }

    public class ChessSquare
    {
        public int Row { get; set; }

        public int Column { get; set; }

        public int Static { get; set; }
    }
}
