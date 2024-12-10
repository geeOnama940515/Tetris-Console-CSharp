
# Tetris Game in C# Console

This is a simple implementation of the classic Tetris game written in C#. It uses a console-based UI with basic gameplay mechanics. The game includes features such as:

- Falling tetrominoes that the player can move and rotate.
- A game board that updates with each piece placed.
- Scoring and line-clearing mechanics.
- The option to play again after the game ends.

## How to Play

- Use the **Arrow keys** to move the current piece:
  - **Left Arrow**: Move the piece left.
  - **Right Arrow**: Move the piece right.
  - **Down Arrow**: Move the piece down faster.
  - **Spacebar**: Rotate the piece clockwise.
- The game ends when a new piece can't be placed on the board.

## Features

- **Tetrominoes**: The game uses seven different tetromino shapes (Square, T-shape, L-shape, J-shape, S-shape, Z-shape, and Line).
- **Next Piece Preview**: Shows the next piece that will fall after the current one.
- **Line Clearing**: When a row is completely filled with blocks, it gets cleared, and the player gets points.
- **Game Over and Restart**: After the game ends, you can choose to play again or exit.


## Explanation of Code

### 1. **Data Structures**

- **`board`**: A 2D array that represents the game board. Each cell can hold a character that represents either an empty space or a filled block.
- **`currentPiece` and `nextPiece`**: Arrays representing the current tetromino and the next piece that will fall. Each piece consists of an array of coordinates relative to the piece's "center."
- **`piecePosition`**: The current position of the tetromino on the board.
- **`Tetrominoes`**: A collection of predefined tetromino shapes.

### 2. **Game Flow**

The main loop of the game follows this flow:
1. **Spawn a new piece**: When the current piece lands, a new one spawns from the `nextPiece`.
2. **Move pieces**: The player can move pieces using the arrow keys, or rotate them using the spacebar.
3. **Gravity**: The pieces fall automatically at regular intervals.
4. **Line Clearing**: After each piece is placed, the program checks if any rows are completely filled. If so, those rows are cleared, and the rows above shift down.
5. **Game Over**: If a piece cannot be placed because of the current board state, the game ends.
6. **Restart Option**: After the game ends, the user is prompted whether they want to play again.

### 3. **Input Handling**

- **Arrow Keys**: Handle the movement of the active piece. If the user presses the left or right arrow key, the piece moves horizontally. If the down arrow is pressed, the piece accelerates downward.
- **Spacebar**: Rotates the piece clockwise. The rotation is checked to ensure it doesn't cause the piece to overlap with existing blocks.

### 4. **Piece Collision Detection**

Before moving or rotating a piece, the game checks whether the new position or rotation is valid. It ensures the piece doesn’t move outside the board boundaries or overlap with existing blocks.

### 5. **Drawing the Board**

The board is drawn in the console, with each block represented by a "[ ]" character. The next piece is also previewed in the UI for the player to plan their moves.

### 6. **Clearing Lines**

Once a full line is detected, it is cleared by shifting all the rows above it down. The player earns points for each line cleared.

### 7. **Scoring**

The player receives 100 points for every line cleared.

### 8. **Game Over and Restart**

When a piece cannot be placed on the board, the game ends. The user is prompted to choose whether to play a new game or quit.

## Conclusion

This project was created to showcase how to implement a basic Tetris game in a console environment using C#. It covers the core mechanics of the game, such as handling pieces, detecting collisions, rotating shapes, clearing lines, and managing game flow. This is a fun and educational project for learning C# and understanding the logic behind creating simple 2D games.

