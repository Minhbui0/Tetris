using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
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

    
    // Background music
    Song backgroundMusic;

    // Sound effects
    SoundEffect blockPlaceSound;
    SoundEffect lineClearSound;
    SoundEffect rotateSound;
    SoundEffect levelUpSound;
    SoundEffect gameOverSound;
    SoundEffect MoveSound;
    
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
    private int Level;
    private float fallTimer = 0f;

    private string notificationText = "";
    private float notificationTimer = 0f;
    private float notificationDuration = 1.0f; // Show for 1 second
    private Color notificationColor = Color.GreenYellow;





    public GameWorld()
    {
        random = new Random();
        gameState = GameState.Playing;
        font = TetrisGame.ContentManager.Load<SpriteFont>("SpelFont");
        grid = new TetrisGrid(new Vector2(200, 50));
        previewGrid = new TetrisGrid(previewGridPosition);
       


        blocks = new TetrisBlock[] { new OBlock(), new IBlock(), new LBlock(), new JBlock(), new SBlock(), new ZBlock(), new TBlock() };
        nextBlock = blocks[blockNumber];
        SpawnNewBlock();

        // Load background music
        
        backgroundMusic = TetrisGame.ContentManager.Load<Song>("Audio/tetris_music"); //tetris music owned by "Copyleft Music" https://www.youtube.com/watch?v=VRo2e7aK1lg
        MediaPlayer.IsRepeating = true;
        MediaPlayer.Volume = 0.1f; // 10% volume
        MediaPlayer.Play(backgroundMusic);
        
        // Load sound effects
        
        blockPlaceSound = TetrisGame.ContentManager.Load<SoundEffect>("Audio/blockplace_soundeffect"); // blockplace soundeffect owned by "Gaming sound FX"
        lineClearSound = TetrisGame.ContentManager.Load<SoundEffect>("Audio/lineclear_soundeffect"); //lineclear soundeffect owned by Nintendo
        rotateSound = TetrisGame.ContentManager.Load<SoundEffect>("Audio/rotate_sound"); // rotate soundeffect owned by Nintendo
        levelUpSound = TetrisGame.ContentManager.Load<SoundEffect>("Audio/levelup_soundeffect"); // level up soundeffect owned by "CPhT Fluke"
        gameOverSound = TetrisGame.ContentManager.Load<SoundEffect>("Audio/gameover_soundeffect"); // game over soundeffect owned by Nintendo
        MoveSound = TetrisGame.ContentManager.Load<SoundEffect>("Audio/move_soundeffect"); // move soundeffect owned by "Sound Effect Database"

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
    public void PlaceBlock()
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
                        grid.SetCell(gridX, gridY, currentBlock.color);
                    }
                }
            }
        }
        blockPlaceSound.Play(0.15f, 0.0f, 0.0f);
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
            lineClearSound.Play(0.2f, 0.0f, 0.0f);
        }

        int oldLevel = Level;

        Level = Score / 3000;

        // Show level up notification
        if (Level > oldLevel)
        {
            ShowNotification($"Level {Level}!", 1.5f);
            levelUpSound.Play(1f, 0.0f, 0.0f);
        }

    }




    public void HandleInput(GameTime gameTime, InputHelper inputHelper)
    {
        // if game over, only check for restart

        if (gameState == GameState.GameOver)
        {

            if (inputHelper.KeyPressed(Keys.R))
            {
                Reset();
            }
            return;

        }
        // Press D rotates right
        if (inputHelper.KeyPressed(Keys.D))
        {
            
            currentBlock.rotateRight();
            if (!IsValidPosition())
            {
                currentBlock.rotateLeft(); // turn left if invalid
            }
            else
            {
               rotateSound.Play(0.04f, 0.0f, 0.0f);
            }
        }

        // Press A rotates left
        if (inputHelper.KeyPressed(Keys.A))
        {
            
            currentBlock.rotateLeft();
            if (!IsValidPosition())
            {
                currentBlock.rotateRight(); // turn right if invalid
            }
            else
            {
                rotateSound.Play(0.04f, 0.0f, 0.0f);
            }
        }

        // Press right arrow to move right
        if (inputHelper.KeyPressed(Keys.Right))
        {
            blockPosition.X++;
            if (!IsValidPosition())
            {
                blockPosition.X--; // move left if invalid
            }
            else
            {
                MoveSound.Play(0.5f, 0.0f, 0.0f);
            }
                
        }

        // Press left arrow to move right
        if (inputHelper.KeyPressed(Keys.Left))
        {
            blockPosition.X--;
            if (!IsValidPosition())
            {
                blockPosition.X++; // move right if invalid
            }
            else
            {
                MoveSound.Play(0.5f, 0.0f, 0.0f);
            }
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
            else
            {
                MoveSound.Play(0.5f, 0.0f, 0.0f);
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

    private float CalculateFallInterval()
    {
        float baseSpeed = 2.0f; // Starting speed at level 0
        float minSpeed = 0.1f;  // Minimum fall interval (fastest speed)
        float calculatedSpeed = baseSpeed * (float)Math.Pow(0.85f, Level);
        return Math.Max(calculatedSpeed, minSpeed);
    }

    public void Update(GameTime gameTime)
    {
        if (gameState != GameState.Playing)
            return;

        // Update notification timer
        if (notificationTimer > 0)
        {
            notificationTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        fallTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        float currentFallInterval = CalculateFallInterval(); // Dynamic based on level

        if (fallTimer >= currentFallInterval)
        {
            fallTimer = 0f;
            blockPosition.Y++;

            if (!IsValidPosition())
            {
                blockPosition.Y--;
                PlaceBlock();

                if (!IsValidPosition())
                {
                    gameState = GameState.GameOver;
                    gameOverSound.Play(0.6f, 0.0f, 0.0f);
                }
            }
        }
    }


    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        grid.Draw(gameTime, spriteBatch);
        if (currentBlock != null)
        {
            currentBlock.Draw(gameTime, spriteBatch, grid, blockPosition);
        }

        // Draw notification if active
        if (notificationTimer > 0)
        {
            float scale = 2.5f;

            Vector2 textSize = font.MeasureString(notificationText);
            Vector2 screenCenter = new Vector2(400, 450);
            Vector2 textOrigin = textSize / 2f;

            // fade effect
            float fadeTime = 0.5f;
            float alpha = notificationTimer < fadeTime ? notificationTimer / fadeTime : 1.0f;
            Color fadedColor = notificationColor * alpha;

            // draw the notification on screen
            spriteBatch.DrawString(font, notificationText, screenCenter, fadedColor, 0f, textOrigin, scale, SpriteEffects.None, 0f);
        }
        // Draw Game Over screen
        if (gameState == GameState.GameOver)
        {
            
            // Semi-transparent overlay
            Vector2 gameOverPos = new Vector2(400, 450);
            Vector2 gameOverSize = font.MeasureString("GAME OVER");
            Vector2 gameOverOrigin = gameOverSize / 2f;

            spriteBatch.DrawString(font, "GAME OVER", gameOverPos, Color.Red, 0f, gameOverOrigin, 3.0f, SpriteEffects.None, 0f);

            // Restart instruction
            Vector2 restartPos = new Vector2(400, 380);
            Vector2 restartSize = font.MeasureString("Press R to Restart");
            Vector2 restartOrigin = restartSize / 2f;

            spriteBatch.DrawString(font, "Press R to Restart", restartPos, Color.White, 0f, restartOrigin, 1.5f, SpriteEffects.None, 0f);
        }


        spriteBatch.DrawString(font, "Next", new Vector2(600, 60), Color.White);

        if (nextBlock != null)
        {
            nextBlock.Draw(gameTime, spriteBatch, previewGrid, new Point(0, 0));
        }

        Vector2 scorePosition = new Vector2(previewGridPosition.X + -550, previewGridPosition.Y - 30);
        spriteBatch.DrawString(font, $"Score:{Score}", scorePosition, Color.Black);   // Shows the score counter on the screen.

        Vector2 levelPosition = new Vector2(previewGridPosition.X + -250, previewGridPosition.Y - 100);
        spriteBatch.DrawString(font, $"Level:{Level}", levelPosition, Color.Black);   // Shows the score counter on the screen.

        spriteBatch.End();
    }

    public void ShowNotification(string message, float duration = 0.5f)
    {
        notificationText = message;
        notificationTimer = duration;
    }


    public void Reset()
    {

        // Reset game state
        gameState = GameState.Playing;

        // Clear the grid
        grid = new TetrisGrid(new Vector2(200, 50));

        // Reset score and level
        Score = 0;
        Level = 0;
        Counter = 0;

        // Reset timers
        fallTimer = 0f;
        notificationTimer = 0f;

        // Spawn new blocks
        blockNumber = random.Next(blocks.Length);
        nextBlock = blocks[blockNumber];
        SpawnNewBlock();
    }
}
