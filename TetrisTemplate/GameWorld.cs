using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
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

    TetrisBlock currentBlock;

    Point blockPosition;

    public TetrisBlock[] blocks;
    public int blockNumber;
    public static Random random = new Random();



    public GameWorld()
    {
        random = new Random();
        gameState = GameState.Playing;
        font = TetrisGame.ContentManager.Load<SpriteFont>("SpelFont");
        grid = new TetrisGrid(new Vector2(200, 50));
        //grid.AddTestBlocks();

        SpawnNewBlock();
    }

    // Spawns in a new random block
    public void SpawnNewBlock()
    {
        blocks = new TetrisBlock[] { new OBlock(), new IBlock(), new LBlock(), new JBlock(), new SBlock(), new ZBlock(), new TBlock() };
        blockNumber = random.Next(blocks.Length);
        currentBlock = blocks[blockNumber];
        blockPosition = new Point(3, 0);
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
        SpawnNewBlock(); // After placing, runs SpawNewBlock method to spawn in a new block
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




    
    //Check if the current block position is valid, meaning its within the bounds of the grid and there is no overlap.
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
        

        spriteBatch.End();

        
        
    }

    public void Reset()
    {
    }




}
