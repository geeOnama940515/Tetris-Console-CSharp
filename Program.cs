using System;
using System.Threading;

class Tetris
{
    const int Width = 10; // Width of the game board
    const int Height = 20; // Height of the game board
    static char[,] board = new char[Height, Width]; // Represents the game board
    static (int x, int y)[] currentPiece; // The active falling tetromino
    static (int x, int y)[] nextPiece; // The upcoming tetromino
    static (int x, int y) piecePosition; // Position of the current piece on the board
    static Random random = new Random(); // Random generator for selecting tetrominoes
    static bool gameOver = false; // Indicates if the game is over
    static int score = 0; // Player's score
    static bool pieceDropped = false; // Indicates if a piece has been placed

    static (int x, int y)[][] Tetrominoes = new (int x, int y)[][]
    {
        new (int, int)[] { (0, 0), (1, 0), (0, 1), (1, 1) }, // Square
        new (int, int)[] { (0, 0), (-1, 0), (1, 0), (0, 1) }, // T-shape
        new (int, int)[] { (0, 0), (1, 0), (0, 1), (0, 2) }, // L-shape
        new (int, int)[] { (0, 0), (-1, 0), (0, 1), (0, 2) }, // J-shape
        new (int, int)[] { (0, 0), (1, 0), (0, 1), (-1, 1) }, // S-shape
        new (int, int)[] { (0, 0), (-1, 0), (0, 1), (1, 1) }, // Z-shape
        new (int, int)[] { (0, 0), (1, 0), (2, 0), (3, 0) }, // Line
    };

    static void Main()
    {
        Console.CursorVisible = false;

        do
        {
            StartGame();
        } while (PromptForNewGame());
    }

    /// <summary>
    /// Starts a new game session by initializing the board and running the game loop.
    /// </summary>
    static void StartGame()
    {
        InitializeBoard();
        nextPiece = Tetrominoes[random.Next(Tetrominoes.Length)];
        score = 0;
        gameOver = false;

        Thread inputThread = new Thread(HandleInput);
        inputThread.Start();

        DateTime lastDrop = DateTime.Now;
        SpawnPiece();

        while (!gameOver)
        {
            if ((DateTime.Now - lastDrop).TotalMilliseconds >= 200)
            {
                MovePiece(0, 1);
                lastDrop = DateTime.Now;
            }

            DrawBoard();

            if (pieceDropped)
            {
                PlacePiece();
                pieceDropped = false;
            }

            Thread.Sleep(10); // Small delay to keep the game loop responsive
        }

        inputThread.Join();
        Console.Clear();
        Console.WriteLine("Game Over!");
        Console.WriteLine($"Final Score: {score}");
    }

    /// <summary>
    /// Prompts the user to decide whether to play another game after a "Game Over."
    /// </summary>
    static bool PromptForNewGame()
    {
        Console.WriteLine("Do you want to play again? (Y/N):");
        while (true)
        {
            var input = Console.ReadKey(true).Key;
            if (input == ConsoleKey.Y)
                return true;
            if (input == ConsoleKey.N)
                return false;
        }
    }

    /// <summary>
    /// Initializes the board by clearing all cells.
    /// </summary>
    static void InitializeBoard()
    {
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                board[y, x] = ' ';
    }

    /// <summary>
    /// Spawns a new tetromino piece at the top of the board.
    /// </summary>
    static void SpawnPiece()
    {
        currentPiece = nextPiece;
        nextPiece = Tetrominoes[random.Next(Tetrominoes.Length)];
        piecePosition = (Width / 2, 0);

        if (!CanMove(0, 0))
            gameOver = true;
    }

    /// <summary>
    /// Draws the game board, score, and the next piece preview on the console.
    /// </summary>
    static void DrawBoard()
    {
        Console.SetCursorPosition(0, 0);
        Console.WriteLine($"Score: {score}");

        for (int y = 0; y < Height; y++)
        {
            Console.Write("|");
            for (int x = 0; x < Width; x++)
            {
                if (IsPartOfPiece(x, y))
                    Console.Write("[ ]");
                else if (board[y, x] == ' ')
                    Console.Write("   ");
                else
                    Console.Write("[ ]");
            }
            Console.WriteLine("|");
        }

        Console.WriteLine(new string('-', Width * 3 + 2));

        DrawNextPiece();
    }

    /// <summary>
    /// Draws the preview of the next tetromino piece.
    /// </summary>
    static void DrawNextPiece()
    {
        Console.WriteLine("Next:");
        var preview = new char[4, 4];
        for (int y = 0; y < 4; y++)
            for (int x = 0; x < 4; x++)
                preview[y, x] = ' ';

        foreach (var (x, y) in nextPiece)
        {
            if (x >= 0 && x < 4 && y >= 0 && y < 4)
                preview[y, x] = '#';
        }

        for (int y = 0; y < 4; y++)
        {
            Console.Write(" ");
            for (int x = 0; x < 4; x++)
            {
                if (preview[y, x] == '#')
                    Console.Write("[ ]");
                else
                    Console.Write("   ");
            }
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Handles player input for moving and rotating the active piece.
    /// </summary>
    static void HandleInput()
    {
        while (!gameOver)
        {
            if (!Console.KeyAvailable) continue;

            var key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.LeftArrow:
                    MovePiece(-1, 0);
                    break;
                case ConsoleKey.RightArrow:
                    MovePiece(1, 0);
                    break;
                case ConsoleKey.DownArrow:
                    MovePiece(0, 1);
                    break;
                case ConsoleKey.Spacebar:
                    RotatePiece();
                    break;
            }
        }
    }

    /// <summary>
    /// Checks if the current piece can move to a new position.
    /// </summary>
    static bool CanMove(int dx, int dy)
    {
        foreach (var (x, y) in currentPiece)
        {
            int newX = piecePosition.x + x + dx;
            int newY = piecePosition.y + y + dy;

            if (newX < 0 || newX >= Width || newY >= Height)
                return false;

            if (newY >= 0 && board[newY, newX] != ' ')
                return false;
        }
        return true;
    }

    /// <summary>
    /// Moves the active piece by a specified offset, if possible.
    /// </summary>
    static void MovePiece(int dx, int dy)
    {
        if (CanMove(dx, dy))
        {
            piecePosition = (piecePosition.x + dx, piecePosition.y + dy);
        }
        else if (dy > 0)
        {
            pieceDropped = true;
        }
    }

    /// <summary>
    /// Places the active piece on the board and clears full lines.
    /// </summary>
    static void PlacePiece()
    {
        foreach (var (x, y) in currentPiece)
        {
            int boardX = piecePosition.x + x;
            int boardY = piecePosition.y + y;
            if (boardY >= 0)
                board[boardY, boardX] = '#';
        }

        ClearLines();
        SpawnPiece();
    }

    /// <summary>
    /// Clears any completed lines and shifts the rows above downward.
    /// </summary>
    static void ClearLines()
    {
        for (int y = Height - 1; y >= 0; y--)
        {
            bool fullLine = true;
            for (int x = 0; x < Width; x++)
            {
                if (board[y, x] == ' ')
                {
                    fullLine = false;
                    break;
                }
            }

            if (fullLine)
            {
                score += 100;
                for (int row = y; row > 0; row--)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        board[row, x] = board[row - 1, x];
                    }
                }

                for (int x = 0; x < Width; x++)
                {
                    board[0, x] = ' ';
                }

                y++; // Recheck the current row after clearing
            }
        }
    }

    /// <summary>
    /// Checks if a position is part of the current falling piece.
    /// </summary>
    static bool IsPartOfPiece(int x, int y)
    {
        foreach (var (pieceX, pieceY) in currentPiece)
        {
            if (x == piecePosition.x + pieceX && y == piecePosition.y + pieceY)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Rotates the active piece clockwise, if possible.
    /// </summary>
    static void RotatePiece()
    {
        var rotatedPiece = new (int x, int y)[currentPiece.Length];
        for (int i = 0; i < currentPiece.Length; i++)
        {
            // Rotate 90 degrees clockwise (x, y) -> (-y, x)
            rotatedPiece[i] = (-currentPiece[i].y, currentPiece[i].x);
        }

        var originalPiece = currentPiece;
        currentPiece = rotatedPiece;
        if (!CanMove(0, 0))
            currentPiece = originalPiece; // Revert if rotation is invalid
    }
}
