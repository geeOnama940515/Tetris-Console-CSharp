using System;
using System.Threading;

class Tetris
{
    const int Width = 10;
    const int Height = 20;
    static char[,] board = new char[Height, Width];
    static (int x, int y)[] currentPiece;
    static (int x, int y)[] nextPiece;
    static (int x, int y) piecePosition;
    static Random random = new Random();
    static bool gameOver = false;
    static int score = 0;
    static bool pieceDropped = false;

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

    static void InitializeBoard()
    {
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                board[y, x] = ' ';
    }

    static void SpawnPiece()
    {
        currentPiece = nextPiece;
        nextPiece = Tetrominoes[random.Next(Tetrominoes.Length)];
        piecePosition = (Width / 2, 0);

        if (!CanMove(0, 0))
            gameOver = true;
    }

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

                y++; // Recheck the same line after shifting
            }
        }
    }

    static bool IsPartOfPiece(int x, int y)
    {
        foreach (var (px, py) in currentPiece)
        {
            if (x == piecePosition.x + px && y == piecePosition.y + py)
                return true;
        }
        return false;
    }

    static void RotatePiece()
    {
        var rotatedPiece = new (int x, int y)[currentPiece.Length];
        for (int i = 0; i < currentPiece.Length; i++)
        {
            rotatedPiece[i] = (-currentPiece[i].y, currentPiece[i].x);
        }

        var originalPiece = currentPiece;
        currentPiece = rotatedPiece;

        if (!CanMove(0, 0))
            currentPiece = originalPiece;
    }
}
