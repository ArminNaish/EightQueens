using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

const char queen = 'Q';
const char empty = 'X';

var board = MakeBoard(8, 8);
var (solved, solution) = Solve(board);
if (solved)
    Print(solution);
else
    Console.WriteLine("There is no solution.");


/// <summary>
/// Get the diagonals by adding an increasing/decreasing buffer to the start/end of each row, 
/// get the columns of this buffered grid, then remove the buffer on each column afterwards
/// </summary>
T[][] GetDiagonals<T>(T[][] board, Direction direction = Direction.Forward) where T : struct
{
    T padCharacter = default;
    var buffer = new T[board.Length][];

    var j = board[0].Length - 1;
    for (var i = 0; i < board.Length; i++, j--)
    {
        var padLeft = Enumerable.Repeat(padCharacter, direction == Direction.Forward ? i : j);
        var padRight = Enumerable.Repeat(padCharacter, direction == Direction.Forward ? j : i);
        buffer[i] = padLeft
           .Concat(board[i])
           .Concat(padRight)
           .ToArray();
    }

    var cols = Enumerable
        .Range(0, buffer[0].Length)
        .Select(col => Enumerable
            .Range(0, buffer.Length)
            .Select(row => buffer[row][col])
            .Where(cell => !Equals(cell, padCharacter))
            .ToArray())
        .ToArray();

    return cols;
}

(bool, char[][]) Solve(char[][] board, int col = 0)
{
    // base case
    if (col >= board[0].Length)
        return (true, board);

    for (int row = 0; row < board.Length; row++)
    {
        board[row][col] = queen;
        if (AllQueensSafe(board))
        {
            var (solved, solution) = Solve(board, col + 1);
            if (solved)
            {
                return (true, solution);
            }
        }
        // undo queen placement
        board[row][col] = empty;
    }

    return (false, null);
}

bool AllQueensSafe(char[][] board)
{
    var r = GetRows(board);
    var oneQueenPerRow = GetRows(board).All(row => row.Count(cell => cell == queen) <= 1);
    if (!oneQueenPerRow)
        return false;

    var oneQueenPerColumn = GetColumns(board).All(col => col.Count(cell => cell == queen) <= 1);
    if (!oneQueenPerColumn)
        return false;

    var oneQueenPerForwardDiagonal = GetDiagonals(board, Direction.Forward).All(diag => diag.Count(cell => cell == queen) <= 1);
    if (!oneQueenPerForwardDiagonal)
        return false;

    var oneQueenPerBackwardDiagonal = GetDiagonals(board, Direction.Backward).All(diag => diag.Count(cell => cell == queen) <= 1);
    if (!oneQueenPerBackwardDiagonal)
        return false;

    return true;
}

char[][] MakeBoard(int row, int col)
{
    var board = new char[row][];
    for (int i = 0; i < board.Length; i++)
    {
        board[i] = new char[col];
        for (int j = 0; j < board[0].Length; j++)
        {
            board[i][j] = empty;
        }
    }
    return board;
}

void Print(char[][] board)
{
    var builder = new StringBuilder();
    builder.AppendLine("  a b c d e f g h");
    for (int i = 0; i < board.Length; i++)
    {
        builder.Append($"{board.Length - i} ");
        for (int j = 0; j < board[0].Length; j++)
        {
            builder.Append($"{board[i][j]} ");
        }
        builder.AppendLine();
    }
    Console.WriteLine(builder.ToString());
}

T[][] GetColumns<T>(T[][] board)
{
    return Enumerable
       .Range(0, board[0].Length)
       .Select(col => GetColumn(board, col))
       .ToArray();
}

T[] GetColumn<T>(T[][] board, int col)
{
    return Enumerable.Range(0, board.Length).Select(row => board[row][col]).ToArray();
}

T[][] GetRows<T>(T[][] board)
{
    return Enumerable
        .Range(0, board.Length)
        .Select(row => GetRow(board, row))
        .ToArray();
}

T[] GetRow<T>(T[][] board, int row)
{
    return Enumerable.Range(0, board[0].Length).Select(col => board[row][col]).ToArray();
}

enum Direction
{
    Forward,
    Backward
}