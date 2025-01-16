using SudokuWpf.Helpers;
using System.Collections.ObjectModel;
using System.Windows;

namespace SudokuWpf.Entities;
public class Rules
{
    public bool? CheckCell(ObservableCollection<ObservableCollection<Cell>> sudokuGridData, Player currentPlayer, Turn turn)
    {
        bool? gameIsRunning = true;
        // Player wants to do a move
        if (turn.Value > 0)
        {
            int row = turn.Y[0] - 'A';
            int col = turn.X[0] - 'a';

            if (row < 0 || row >= 9 || col < 0 || col >= 9)
            {
                MessageBox.Show($"Invalid cell coordinates '{turn.Y},{turn.X}', Player {currentPlayer.Name} loses");
                gameIsRunning = null;
                return gameIsRunning;
            }

            if (turn.Value < 1 || turn.Value > 9)
            {
                MessageBox.Show($"Invalid value {turn.Value} for '{turn.Y},{turn.X}', Player {currentPlayer.Name} loses");
                gameIsRunning = null;
                return gameIsRunning;
            }

            var cell = sudokuGridData[row][col];

            if (cell.PossibleValues.Contains(turn.Value))
            {
                UpdateCell(sudokuGridData, turn, row, col, cell);

                if (turn.ClaimUniqueSolution)
                {
                    cell.UniqueSolutionClaimed = true;
                    //check in the claim is correct
                    gameIsRunning = null;
                    //make a copy of the board
                    var copysudokuGridData = DeepCloneSudokuGridData(sudokuGridData);

                    var startOver = false;
                    for (int x = 0; x < 9; x++)
                    {
                        for (int y = 0; y < 9; y++)
                        {
                            if (copysudokuGridData[x][y].FilteredPossibleValues.Count() == 1) //check if there is only one possible value left in (x,y)
                            {
                                // Fill in the only possible value
                                var onlyPossibleValue = copysudokuGridData[x][y].FilteredPossibleValues.First();
                                UpdateCell(copysudokuGridData, new Turn { X = x.ToColumnLetter(), Y = y.ToRowLetter(), Value = onlyPossibleValue }, x, y, copysudokuGridData[x][y]);

                                //start the for loop again
                                x = 0;
                                y = 0;
                                startOver = true;
                                break;
                            }
                        }
                        if (startOver)
                        {
                            startOver = false;
                            break;
                        }
                    }
                    //check if all cells are filled in
                    for (int x = 0; x < 9; x++)
                    {
                        for (int y = 0; y < 9; y++)
                        {
                            if (sudokuGridData[x][y].Value == 0)
                            {
                                turn.IsValidClaim = false;
                                MessageBox.Show($"The sudoku was not unique. Player {currentPlayer.Name} loses");
                                return gameIsRunning;
                            }
                            else
                            {
                                turn.IsValidClaim = true;
                                MessageBox.Show($"The sudoku was unique. Player {currentPlayer.Name} has won!");
                                return gameIsRunning;
                            }
                        }
                    }
                }

            }
            else
            {
                cell.IsValidMove = false;
                turn.IsValidMove = false;
                MessageBox.Show($"Value {turn.Value} in cell '{turn.Y},{turn.X}' is not valid, Player {currentPlayer.Name} loses\"");
                UpdateCell(sudokuGridData, turn, row, col, cell);
                gameIsRunning = null;
                return gameIsRunning;
            }
        }
        return gameIsRunning;
    }

    private void UpdateCell(ObservableCollection<ObservableCollection<Cell>> sudokuGridData, Turn turn, int row, int col, Cell cell)
    {
        cell.Value = turn.Value;
        if (cell.IsValidMove)
        {
            for (int i = 0; i < cell.PossibleValues.Count; i++)
            {
                cell.PossibleValues[i] = null;
            }
            //update possible values for all cells in the same row, column and square
            UpdateRowPossibleValues(sudokuGridData, row, turn.Value);
            UpdateColumnPossibleValues(sudokuGridData, col, turn.Value);
            UpdateSquarePossibleValues(sudokuGridData, row, col, turn.Value);
        }
    }

    private void UpdateRowPossibleValues(ObservableCollection<ObservableCollection<Cell>> sudokuGridData, int row, int value)
    {
        for (int col = 0; col < 9; col++)
        {
            var cell = sudokuGridData[row][col];
            if (cell.PossibleValues.Contains(value))
            {
                int index = cell.PossibleValues.IndexOf(value);
                cell.PossibleValues[index] = null;
            }
        }
    }

    private void UpdateColumnPossibleValues(ObservableCollection<ObservableCollection<Cell>> sudokuGridData, int col, int value)
    {
        for (int row = 0; row < 9; row++)
        {
            var cell = sudokuGridData[row][col];
            if (cell.PossibleValues.Contains(value))
            {
                int index = cell.PossibleValues.IndexOf(value);
                cell.PossibleValues[index] = null;
            }
        }
    }

    private void UpdateSquarePossibleValues(ObservableCollection<ObservableCollection<Cell>> sudokuGridData, int row, int col, int value)
    {
        int startRow = (row / 3) * 3;
        int startCol = (col / 3) * 3;

        for (int i = startRow; i < startRow + 3; i++)
        {
            for (int j = startCol; j < startCol + 3; j++)
            {
                var cell = sudokuGridData[i][j];
                if (cell.PossibleValues.Contains(value))
                {
                    int index = cell.PossibleValues.IndexOf(value);
                    cell.PossibleValues[index] = null;
                }
            }
        }
    }
    public ObservableCollection<ObservableCollection<Cell>> DeepCloneSudokuGridData(ObservableCollection<ObservableCollection<Cell>> sudokuGridData)
    {
        var newGridData = new ObservableCollection<ObservableCollection<Cell>>();

        foreach (var row in sudokuGridData)
        {
            var newRow = new ObservableCollection<Cell>();
            foreach (var cell in row)
            {
                var newCell = new Cell
                {
                    X = cell.X,
                    Y = cell.Y,
                    Value = cell.Value,
                    IsValidMove = cell.IsValidMove,
                    PossibleValues = new ObservableCollection<int?>(cell.PossibleValues)
                };
                newRow.Add(newCell);
            }
            newGridData.Add(newRow);
        }

        return newGridData;
    }

}
