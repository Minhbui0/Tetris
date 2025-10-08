using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;



class TetrisBlock
{
    //declares block array with 4x4 dimensions
    protected bool[,] blockArray = new bool[4, 4];
    protected Color color = Color.Green;


    
    public void rotateRight()
    {
        //declare the tranposed array.
        bool[,] transposedArray  = new bool[4, 4];

        //tranpose the array by swapping the rows with the columns
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                transposedArray[j, i] = blockArray[i, j];

            }
        }

        //reverse each row horizontally 
        
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                bool temp = transposedArray[i, j];
                transposedArray[i, j] = transposedArray[i, 3 - j];
                transposedArray[i, 3 - j] = temp;
            }
        }
        
        blockArray = transposedArray;
    }

    public void rotateLeft()
    {
        //declare the tranposed array
        bool[,] transposedArray = new bool[4, 4];

        //transpose the array by swapping the rows with the columns
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                transposedArray[j, i] = blockArray[i, j];

            }
        }

        //reverse each column vertically
        for (int j = 0; j < 4; j++)
        {
            for (int i = 0; i < 2; i++)
            {
                bool temp = transposedArray[i, j];
                transposedArray[i, j] = transposedArray[3 - i, j];
                transposedArray[3 - i, j] = temp;
            }
        }

        blockArray = transposedArray;
    }

    //method to draw the blocks onto the grid
    /*
    public void DrawOnGrid(TetrisGrid grid, int gridX, int gridY)
    {
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                if (blockArray[y, x])  
                {
                    grid.SetCell(gridX + x, gridY + y, color);
                }
            }
        }
    }
    */

    // Draws the block at a specific position without modifying the grid.
    
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, TetrisGrid grid, Point position)
    {
        Texture2D blockSprite = TetrisGame.ContentManager.Load<Texture2D>("block");
        Vector2 gridPosition = grid.position; 

        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                if (blockArray[y, x])
                {
                    int drawX = (int)gridPosition.X + (position.X + x) * 40;
                    int drawY = (int)gridPosition.Y + (position.Y + y) * 40;
                    spriteBatch.Draw(blockSprite,
                        new Rectangle(drawX, drawY, 40, 40), color);
                }
            }
        }
    }

   
    //Check if a specific cell in the block array is occupied.
    
    public bool IsCellOccupied(int x, int y)
    {
        return blockArray[y, x];
    }


}

//classes of 7 different types of blocks
class OBlock : TetrisBlock
{
    public OBlock()
    {
        blockArray = new bool[4, 4]
        {
            {false, false, false, false },
            {false, true, true, false },
            {false, true, true, false },
            {false, false, false, false }
        };     
    }
}

class IBlock : TetrisBlock
{

    public IBlock()
    {
        blockArray = new bool[4, 4]
        {
            {false, true, false, false },
            {false, true, false, false },
            {false, true, false, false },
            {false, true, false, false }
        };
    }
}

class LBlock : TetrisBlock
{

    public LBlock()
    {
        blockArray = new bool[4, 4]
        {
            {false, true, false, false },
            {false, true, false, false },
            {false, true, true, false },
            {false, false, false, false }
        };
    }
}

class JBlock : TetrisBlock
{

    public JBlock()
    {
        blockArray = new bool[4, 4]
        {
            {false, false, true, false },
            {false, false, true, false },
            {false, true, true, false },
            {false, false, false, false }
        };
    }
}

class SBlock : TetrisBlock
{

    public SBlock()
    {
        blockArray = new bool[4, 4]
        {
            {false, false, false, false },
            {false, false, true, true },
            {false, true, true, false },
            {false, false, false, false }
        };
    }
}

class ZBlock : TetrisBlock
{

    public ZBlock()
    {
        blockArray = new bool[4, 4]
        {
            {false, false, false, false },
            {false, true, true, false },
            {false, false, true, true },
            {false, false, false, false }
        };
    }
}

class TBlock : TetrisBlock
{

    public TBlock()
    {
        blockArray = new bool[4, 4]
        {
            {false, false, false, false },
            {true, true, true, false },
            {false, true, false, false },
            {false, false, false, false }
        };
    }
}





