using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SudokuWpf.Entities;
public class Cell : INotifyPropertyChanged
{
    private int _value;
    private bool _uniqueSolutionClaimed;
    public string X { get; set; }
    public string Y { get; set; }

    private ObservableCollection<int?> _possibleValues = new ObservableCollection<int?>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    private bool _isValidMove = true;

    public int Value
    {
        get => _value;
        set
        {
            if (_value != value)
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }
    }

    public bool IsValidMove
    {
        get => _isValidMove;
        set
        {
            _isValidMove = value;
            OnPropertyChanged(nameof(IsValidMove));
        }
    }

    public ObservableCollection<int?> PossibleValues { get => _possibleValues; set => _possibleValues = value; }

    public List<int> FilteredPossibleValues { get => _possibleValues.Where(pv => pv.HasValue).Select(pv => pv.Value).ToList(); }
    public bool UniqueSolutionClaimed { get => _uniqueSolutionClaimed; set => _uniqueSolutionClaimed = value; }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

