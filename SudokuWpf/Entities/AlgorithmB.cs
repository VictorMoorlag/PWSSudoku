using SudokuWpf.Helpers;

namespace SudokuWpf.Entities;
/// <summary>
/// This class represents a algorithm in the game
/// To set a turn fill the X (col) with the correct coordinate letter a->i,
/// Y (row) with the correct coordinate letter A->I,
/// and the Value properties with the value you want to assign to the cell from 1 -> 9
/// If you  want to claim a unique solution set the ClaimUniqueSolution property to true
/// If you only want to claim a unique solution set the ClaimUniqueSolution property to true but leave the Value property to 0
/// The turns parameter contains all the previous turns
/// </summary>
public class AlgorithmB
{
    public async Task<Turn> GetTurnAsync(List<Turn> turns)
    {
        await Task.Delay(1000); // Simulate some delay

        // cell and value are randomly generated
        // Algoritm should be implemented here
        Random random = new Random();
        return new Turn()
        {
            X = random.Next(0, 8).ToColumnLetter(),
            Y = random.Next(0, 8).ToRowLetter(),
            Value = random.Next(1, 10),
            ClaimUniqueSolution = random.Next(0, 10) == 1
        };
    }
}
