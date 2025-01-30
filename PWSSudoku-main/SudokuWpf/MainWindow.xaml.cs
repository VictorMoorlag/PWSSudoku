using SudokuWpf.Entities;
using SudokuWpf.Helpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace SudokuWpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private int _turnNumber = 1;
    public ObservableCollection<ObservableCollection<Cell>> SudokuGridData { get; set; }

    public ObservableCollection<Turn> Turns { get; set; }

    public Rules Rules { get; set; }
    public Player Player1 { get; set; }
    public Player Player2 { get; set; }
    public Player CurrentPlayer { get; set; }

    public AlgorithmA AlgorithmA { get; set; }
    public AlgorithmB AlgorithmB { get; set; }

    /// <summary>
    /// True if the game is running, false if the game is paused, null if the game is over.
    /// </summary>
    private bool? _gameIsRunning = false;

    public MainWindow()
    {
        InitializeComponent();
        InitializePlayers();
        InitializeAlgoritms();
        InitializeSudokuGridData();
        InitializeTurns();
        CreateSudokuGrid();
        UpdateCurrentPlayerUI();
    }
    private async Task PlayGameAsync()
    {
        Player1Header.Header = Player1.Name;
        Player2Header.Header = Player2.Name;
        while (_gameIsRunning == true)
        {
            var turn = CurrentPlayer == Player1 ? await AlgorithmA.GetTurnAsync(DeepCloneTurns()) : await AlgorithmB.GetTurnAsync(DeepCloneTurns());

            //check again, game might have been paused in the mean time
            if (_gameIsRunning == true)
            {
                turn.Number = _turnNumber++;
                turn.Player = CurrentPlayer;
                Turns.Add(turn);
                EnterValueForCurrentPlayer(turn);
            }
        }
    }

    public void EnterValueForCurrentPlayer(Turn turn)
    {
        _gameIsRunning = Rules.CheckCell(SudokuGridData, CurrentPlayer, turn);
        if(_gameIsRunning is null)
        {
            controlButton.Content = "Reset Game";
        }
        else
        {
            SwitchTurn();
        }
    }

    private void InitializePlayers()
    {
        Player1 = new Player { Name = "AlgorithmA" };
        Player2 = new Player { Name = "AlgorithmB" };
        CurrentPlayer = Player1;
    }

    private void InitializeAlgoritms()
    {
        AlgorithmA = new AlgorithmA();
        AlgorithmB = new AlgorithmB();       
    }

    private void InitializeSudokuGridData()
    {
        Rules = new Rules();
        SudokuGridData = new ObservableCollection<ObservableCollection<Cell>>();
        for (int row = 0; row < 9; row++)
        {
            var rowData = new ObservableCollection<Cell>();
            for (int col = 0; col < 9; col++)
            {
                var cell = new Cell
                {
                    X = row.ToColumnLetter(),
                    Y = row.ToRowLetter(),
                    Value = 0
                };
                rowData.Add(cell);
            }
            SudokuGridData.Add(rowData);
        }
    }

    private void InitializeTurns()
    {
        Turns = new ObservableCollection<Turn>();
        TurnsControl.ItemsSource = Turns;
    }

    private void ResetSudokuGridData()
    {
        _turnNumber = 1;
        Turns.Clear();
        if(CurrentPlayer == Player2)
        {
            SwitchTurn();
        }
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                var cell = SudokuGridData[row][col];
                if (cell.Value > 0)
                {
                    cell.Value = 0;
                    cell.IsValidMove = true;
                }
                if (cell.FilteredPossibleValues.Count != 9)
                {
                    for (int i = 1; i <= 9; i++)
                    {
                        cell.PossibleValues[i-1] = i;
                    }
                }
            }
        }
    }

    private void SwitchTurn()
    {
        CurrentPlayer = CurrentPlayer == Player1 ? Player2 : Player1;
        UpdateCurrentPlayerUI();
    }

    private void UpdateCurrentPlayerUI()
    {
        lblCurrentPlayer.Content = $"Current Player: {CurrentPlayer.Name}";
    }

    private void CreateSudokuGrid()
    {
        for (int row = 0; row <= 9; row++)
        {
            for (int col = 0; col <= 9; col++)
            {                
                TextBox textBox = new TextBox
                {
                    MaxLength = 1,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    FontSize = 24,
                    BorderThickness = new Thickness(0),
                    IsReadOnly = row == 0 || col == 0
                };

                if (row == 0 && col > 0)
                {
                    textBox.Text = ((char)('a' + col - 1)).ToString();
                    textBox.FontWeight = FontWeights.Bold;
                }
                else if (col == 0 && row > 0)
                {
                    textBox.Text = ((char)('A' + row - 1)).ToString();
                    textBox.FontWeight = FontWeights.Bold;
                }
                else if (row > 0 && col > 0)
                {
                    // Allow only digits 1-9
                    textBox.PreviewTextInput += (sender, e) =>
                    {
                        e.Handled = !char.IsDigit(e.Text, 0) || e.Text == "0";
                    };

                    // Bind the TextBox to the Cell's Value property using the converter
                    Binding TextBinding = new Binding($"[{row - 1}][{col - 1}].Value")
                    {
                        Source = SudokuGridData,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Mode = BindingMode.TwoWay,
                        Converter = (IValueConverter)FindResource("ValueToStringConverter")
                    };
                    textBox.SetBinding(TextBox.TextProperty, TextBinding);
                    Binding visibilityBinding = new Binding($"[{row - 1}][{col - 1}].Value")
                    {
                        Source = SudokuGridData,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Mode = BindingMode.OneWay,
                        Converter = (IValueConverter)FindResource("ValueToVisibilityConverter")
                    };
                    textBox.SetBinding(TextBox.VisibilityProperty, visibilityBinding);
                    Binding foregroundBinding = new Binding($"[{row - 1}][{col - 1}].IsValidMove")
                    {
                        Source = SudokuGridData,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Mode = BindingMode.OneWay,
                        Converter = (IValueConverter)FindResource("IsValidMoveToForegroundConverter")
                    };
                    textBox.SetBinding(TextBox.ForegroundProperty, foregroundBinding);
                }
                Border border = new Border
                {
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(1)
                };

                // Make the borders thicker for every 3rd row and column
                if (col % 3 == 1)
                {
                    border.BorderThickness = new Thickness(3, border.BorderThickness.Top, border.BorderThickness.Right, border.BorderThickness.Bottom);
                }
                if (row % 3 == 1)
                {
                    border.BorderThickness = new Thickness(border.BorderThickness.Left, 3, border.BorderThickness.Right, border.BorderThickness.Bottom);
                }
                if (col == 9)
                {
                    border.BorderThickness = new Thickness(border.BorderThickness.Left, border.BorderThickness.Top, 3, border.BorderThickness.Bottom);
                }
                if (row == 9)
                {
                    border.BorderThickness = new Thickness(border.BorderThickness.Left, border.BorderThickness.Top, border.BorderThickness.Right, 3);
                }

                Grid threeByThreeGrid = new Grid();
                for (int i = 0; i < 3; i++)
                {
                    threeByThreeGrid.RowDefinitions.Add(new RowDefinition());
                    threeByThreeGrid.ColumnDefinitions.Add(new ColumnDefinition());
                }
                Grid.SetRowSpan(textBox, 3);
                Grid.SetColumnSpan(textBox, 3);
                threeByThreeGrid.Children.Add(textBox);

                for (int gridRow = 0; gridRow < 3; gridRow++)
                {
                    for (int gridCol = 0; gridCol < 3; gridCol++)
                    {
                        Label label = new Label
                        {
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            VerticalContentAlignment = VerticalAlignment.Center,
                            FontSize = 14,
                            Foreground = Brushes.Gray // Set text color to gray
                        };

                        Binding binding = new Binding($"[{row - 1}][{col - 1}].PossibleValues[{gridRow * 3 + gridCol}]")
                        {
                            Source = SudokuGridData,
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                            Mode = BindingMode.OneWay
                        };
                        label.SetBinding(Label.ContentProperty, binding);

                        Grid.SetRow(label, gridRow);
                        Grid.SetColumn(label, gridCol);
                        threeByThreeGrid.Children.Add(label);
                    }
                }
                border.Child = threeByThreeGrid;
                Grid.SetRow(border, row);
                Grid.SetColumn(border, col);

                SudokuGrid.Children.Add(border);
            }
        }
    }

    private async void controlButton_Click(object sender, RoutedEventArgs e)
    {

        if(_gameIsRunning is null)
        {
            //reset the game
            ResetSudokuGridData();
            _gameIsRunning = false;
            controlButton.Content = "Start Game";
        }
        else if(_gameIsRunning == false)
        {
            //continue the game
            _gameIsRunning = true;
            controlButton.Content = "Pause Game";
            await PlayGameAsync();
        }
        else
        {
            //pause the game
            _gameIsRunning = false;
            controlButton.Content = "Continue Game";
        }
    }
    private List<Turn> DeepCloneTurns()
    {
        var clonedTurns = new List<Turn>();
        foreach (var turn in Turns)
        {
            var clonedTurn = new Turn
            {
                Player = turn.Player,
                Number = turn.Number,
                X = turn.X,
                Y = turn.Y,
                Value = turn.Value,
                IsValidMove = turn.IsValidMove,
                ClaimUniqueSolution = turn.ClaimUniqueSolution
            };
            clonedTurns.Add(clonedTurn);
        }
        return clonedTurns;
    }
}
