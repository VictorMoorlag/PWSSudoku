namespace SudokuWpf.Entities;
/// <summary>
/// This class represents a turn in the game
/// To set a turn fill the X (col) with the correct coordinate letter a->i,
/// Y (row) with the correct coordinate letter A->I,
/// and the Value properties with the value you want to assign to the cell from 1 -> 9
/// If you  want to claim a unique solution set the ClaimUniqueSolution property to true
/// If you only want to claim a unique solution set the ClaimUniqueSolution property to true but leave the Value property to 0
/// </summary>
public class Turn
{
    public Player Player { get; set; }
    public int Number { get; set; }
    public string X { get; set; }
    public string Y { get; set; }
    public int Value { get; set; }

    public bool IsValidMove { get; set; } = true;

    public bool ClaimUniqueSolution { get; set; }

    public bool? IsValidClaim { get; set; }

    public bool IsValid
    {
        get
        {
            return (IsValidMove && IsValidClaim is null) || (IsValidMove && IsValidClaim == true);
        }
    }

    public string TrunAsString
    {
        get
        {
            if (Value == 0)
            {
                return ClaimUniqueSolution ? "!" : "Error";
            }
            else
            {
                return $"{Y}{X}{Value}{(ClaimUniqueSolution ? '!' : string.Empty)}";
            }
        }
    }
}
