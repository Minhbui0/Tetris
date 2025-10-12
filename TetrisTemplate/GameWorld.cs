using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;


/// <summary>
/// A class for representing the game world.
/// This contains the grid, the falling block, and everything else that the player can see/do.
/// </summary>
class GameWorld
{
    /// <summary>
    /// An enum for the different game states that the game can have.
    /// </summary>
    enum GameState
    {
        Playing,
        GameOver
    }

    /// <summary>
    /// The random-number generator of the game.
    /// </summary>
    //public static Random Random { get { return random; } }
   // static Random random;
    

    /// <summary>
    /// The main font of the game.
    /// </summary>
    SpriteFont font;

    /// <summary>
    /// The current game state.
    /// </summary>
    GameState gameState;

    /// <summary>
    /// The main grid of the game.
    /// </summary>
    TetrisGrid grid;
    TetrisGrid previewGrid;
    TetrisBlock currentBlock;
    TetrisBlock nextBlock;

    Point blockPosition;

    public TetrisBlock[] blocks;
    public static Random random = new Random();
    public int blockNumber;   
    private int Score;
    private int Counter;
    private Vector2 previewGridPosition = new Vector2(620, 100);



    public GameWorld()
    {
        random = new Random();
        gameState = GameState.Playing;
        font = TetrisGame.ContentManager.Load<SpriteFont>("SpelFont");
        grid = new TetrisGrid(new Vector2(200, 50));
        //previewGrid = new TetrisGrid(new Vector2(620, 100));
        previewGrid = new TetrisGrid(previewGridPosition);
        //grid.AddTestBlocks();

        blocks = new TetrisBlock[] { new OBlock(), new IBlock(), new LBlock(), new JBlock(), new SBlock(), new ZBlock(), new TBlock() };
        nextBlock = blocks[blockNumber];
        SpawnNewBlock();
    }

    // Generates a new random block
    private TetrisBlock GetRandomBlock()
    {
        blockNumber = random.Next(blocks.Length);
        return blocks[blockNumber];
    }

    // Spawns in the new random block
    public void SpawnNewBlock()
    {
        currentBlock = nextBlock;
        blockPosition = new Point(3, 0);
        nextBlock = GetRandomBlock();
    }

    // Helper function that returns the random next block for the drawing function
    public TetrisBlock GetNextBlock()
    {
        return nextBlock;
    }

    // Places the current block in the grid
    private void PlaceBlock()
    {
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                if (currentBlock.IsCellOccupied(x, y))
                {
                    int gridX = blockPosition.X + x;
                    int gridY = blockPosition.Y + y;

                    if (gridX >= 0 && gridX < grid.Width &&
                        gridY >= 0 && gridY < grid.Height)
                    {
                        grid.SetCell(gridX, gridY, Color.Green);
                    }
                }
            }
        }
        RemoveFullRows(); // After placing, checks if there are any rows completely filled
        SpawnNewBlock(); // After placing, runs SpawNewBlock method to spawn in a new block
    }
    
    // Method that clears any completely filled rows
    private void RemoveFullRows()
    {
        Counter = 0;

        for (int y = grid.Height - 1; y >= 0; y--)
        {
            bool rowIsFull = true;

            for (int x = 0; x < grid.Width; x++)
            {
                if (!grid.IsCellOccupied(x, y))
                {
                    rowIsFull = false;
                    break;
                }
            }

            if (rowIsFull)
            {
                Counter++;

                // Clears rows
                for (int x = 0; x < grid.Width; x++)
                {
                    grid.SetCell(x, y, Color.White);
                }

                // Shift rows down
                for (int row = y; row > 0; row--)
                {
                    for (int x = 0; x < grid.Width; x++)
                    {
                        Color above = grid.GetCell(x, row - 1);
                        grid.SetCell(x, row, above);
                    }
                }

                // Sets top row to empty (since there is no row above to copy from)
                for (int x = 0; x < grid.Width; x++)
                {
                    grid.SetCell(x, 0, Color.White);
                }

                y++; // Check the row again if it is completely filled again after first clear
            }
        }

        // Adds score if there cleared rows. The more rows cleared consecutively, the higher the awarded score will be.
        if (Counter > 0)
        {
            Score += 1000 * ((int)Math.Pow(2, Counter) - 1);
        }
    }




    public void HandleInput(GameTime gameTime, InputHelper inputHelper)
    {
        // Press D rotates right
        if(inputHelper.KeyPressed(Keys.D))
        {
            currentBlock.rotateRight();
            if(!IsValidPosition())
                currentBlock.rotateLeft(); // turn left if invalid

        }

        // Press A rotates left
        if (inputHelper.KeyPressed(Keys.A))
        {
            currentBlock.rotateLeft();
            if (!IsValidPosition())
                currentBlock.rotateRight(); // turn right if invalid
        }

        // Press right arrow to move right
        if (inputHelper.KeyPressed(Keys.Right)) 
        {
            blockPosition.X++;
            if (!IsValidPosition())
                blockPosition.X--; // move left if invalid
        }

        // Press left arrow to move right
        if (inputHelper.KeyPressed(Keys.Left))
        {
            blockPosition.X--;
            if (!IsValidPosition())
                blockPosition.X++; // move right if invalid
        }

        // Press down arrow to move down
        if (inputHelper.KeyPressed(Keys.Down))
        {
            blockPosition.Y++;

            if (!IsValidPosition())
            {
                blockPosition.Y--; // move up if invalid
                PlaceBlock();
            }
        }

        // Press spacebar to immediately move down.
        if (inputHelper.KeyPressed(Keys.Space))
        {
            for (int i = 0; i < grid.Height; i++)
            {
                if (!IsValidPosition())
                {
                    blockPosition.Y--;
                    PlaceBlock();
                    break;
            }
                blockPosition.Y++;
            }
        }
    }




    
    // Check if the current block position is valid, meaning its within the bounds of the grid and there is no overlap.
    private bool IsValidPosition()
    {
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                if (currentBlock.IsCellOccupied(x, y))
                {
                    int gridX = blockPosition.X + x;
                    int gridY = blockPosition.Y + y;

                    // Check if outside grid bounds
                    if (gridX < 0 || gridX >= grid.Width ||
                        gridY < 0 || gridY >= grid.Height)
                    {
                        return false;
                    }

                    // Check if overlapping with existing blocks
                    if (grid.IsCellOccupied(gridX, gridY))
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    public void Update(GameTime gameTime)
    {
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        grid.Draw(gameTime, spriteBatch);
        if(currentBlock != null)
        {
            currentBlock.Draw(gameTime, spriteBatch, grid, blockPosition);
        }

        spriteBatch.DrawString(font, "Next", new Vector2(600, 60), Color.White);

        if (nextBlock != null)
        {
            nextBlock.Draw(gameTime, spriteBatch, previewGrid, new Point(0, 0));
        }

        Vector2 scorePosition = new Vector2(previewGridPosition.X + 20, previewGridPosition.Y - 30);   
        spriteBatch.DrawString(font, $"Score: {Score}", scorePosition, Color.Black);   // Shows the score counter on the screen.

        spriteBatch.End();
    }

    public void Reset()
    {
    }
}
