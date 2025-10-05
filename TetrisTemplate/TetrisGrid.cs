using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection.Metadata;

/// <summary>
/// A class for representing the Tetris playing grid.
/// </summary>
class TetrisGrid
{
    /// The sprite of a single empty cell in the grid.
    Texture2D emptyCell;

    /// The position at which this TetrisGrid should be drawn.
    Vector2 position;

    /// The number of grid elements in the x-direction.
    public int Width { get { return 10; } }
   
    /// The number of grid elements in the y-direction.
    public int Height { get { return 20; } }
    /// <summary>

    /// </summary>
    /// Declares the field
    Color[,] grid;

    /// <summary>
    /// Creates a new TetrisGrid.
    /// 
    /// </summary>
    /// <param name="b"></param>
    public TetrisGrid(Vector2 gridPosition)
    {
        emptyCell = TetrisGame.ContentManager.Load<Texture2D>("block");
        position = gridPosition;
        grid = new Color[Width, Height];
        Clear();
    }

    /// <summary>
    /// Draws the grid on the screen.
    /// </summary>
    /// <param name="gameTime">An object with information about the time that has passed in the game.</param>
    /// <param name="spriteBatch">The SpriteBatch used for drawing sprites and text.</param>
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    { 
        for (int i = 0; i < Width; i++)
        {
            for(int j = 0;j < Height; j++)
            {
                // draws the empty cells of the grid
                spriteBatch.Draw(emptyCell, new Rectangle((int)position.X + i * 40, (int)position.Y + j * 40, 40, 40), Color.White);
                // checks if the empty cell doesn't have a color, if it does have a color, draw a colored block
                if (grid[i, j] != Color.White)
                {
                    spriteBatch.Draw(emptyCell, new Rectangle((int)position.X + i * 40, (int)position.Y + j * 40, 40, 40), grid[i, j]);
                }
            }
        }
    }

    //method to set a cell in the grid to a color
    public void SetCell(int x, int y, Color color)
    {
        if(x>=0 && x < Width && y>=0 && y < Height)
        {
            grid[x, y] = color;
        }

    }
    /// <summary>
    /// Clears the grid.
    /// </summary>
    public void Clear()
    {
        for(int i =0; i < Width; i++)
        {
            for(int j = 0; j < Height; j++)
            {
                grid[i, j] = Color.White;
            }
        }
        
    }


    /// <summary>
    /// Test method to populate some colored blocks
    /// </summary>
    /// 

    // method to test if the array works
    public void AddTestBlocks()
    {
        /*
        // Add a red horizontal line at the bottom
        grid[0, 19] = Color.Red;
        grid[1, 19] = Color.Red;
        grid[2, 19] = Color.Red;
        grid[3, 19] = Color.Red;

        // Add a blue vertical line
        grid[5, 16] = Color.Blue;
        grid[5, 17] = Color.Blue;
        grid[5, 18] = Color.Blue;
        grid[5, 19] = Color.Blue;

        // Add scattered test blocks
        grid[8, 18] = Color.Yellow;
        grid[9, 18] = Color.Green;
        grid[8, 19] = Color.Purple;
        grid[9, 19] = Color.Orange;
        */


        IBlock testBlock = new IBlock();
        testBlock.DrawOnGrid(this, 0, 0);

        JBlock jblock = new JBlock();
        jblock.DrawOnGrid(this, 5, 5);

        LBlock lblock = new LBlock();
        lblock.DrawOnGrid(this, 4, 8);

    }
    

    
}

