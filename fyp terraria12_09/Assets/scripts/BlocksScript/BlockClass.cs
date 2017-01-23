using UnityEngine;
using System.Collections;

public class AirBlock : IBlock
{
    public bool IsMovable(Block block)
    {
        return false;
    }
    public bool IsPassable(Block block)
    {
        return true;
    }
}
public class DirtBlock : IBlock
{
    public static Vector2 leftBotUV = new Vector2(0/1024,288f/2048);
   
    public bool IsMovable(Block block)
    {
        return false;
    }
    public bool IsPassable(Block block)
    {
        return false;
    }

}
public class GrassBlock : IBlock
{
    
    public bool IsMovable(Block block)
    {
        return false;
    }
    public bool IsPassable(Block block)
    {
        return true;
    }
}
public class SandBlock : IBlock
{
    public static Vector2 leftBotUV = new Vector2(540f/1024,576f/2048);

    public bool IsMovable(Block block)
    {
        return true;
    } 
    public bool IsPassable(Block block)
    {
        return false;
    }
}
public class StoneBlock : IBlock
{
    public static Vector2 leftBotUV = new Vector2(0f/1024,576f/2048);

    public bool IsMovable(Block block)
    {
        return false;
    }
    public bool IsPassable(Block block)
    {
        return false;
    }
}
public class CorrDirtBlock : IBlock
{
    public static Vector2 leftBotUV = new Vector2(576f/1024,628f/1024);
    
    public bool IsMovable(Block block)
    {
        return false;
    }
    public bool IsPassable(Block block)
    {
        return false;
    }
}
public class CorrGrassBlock : IBlock
{
   
    public bool IsMovable(Block block)
    {
        return false;
    }
    public bool IsPassable(Block block)
    {
        return true;
    }

}
public class JunDirtBlock : IBlock
{
    public static Vector2 leftBotUV = new Vector2(0/1024,232f/1024);
    
    public bool IsMovable(Block block)
    {
        return false;
    }
    public bool IsPassable(Block block)
    {
        return false;
    }
}
public class CopperBlock : IBlock
{
    public static Vector2 leftBotUV = new Vector2(270f/1024,864f/2048);

    public bool IsMovable(Block block)
    {
        return false;
    }
    public bool IsPassable(Block block)
    {
        return false;
    }
}
public class SilverBlock : IBlock
{
    public static Vector2 leftBotUV = new Vector2(540f/1024,864f/2048);

    public bool IsMovable(Block block)
    {
        return false;
    }
    public bool IsPassable(Block block)
    {
        return false;
    }
}
public class GoldBlock : IBlock
{
    public static Vector2 leftBotUV = new Vector2(0f/1024,1152f/2048);

    public bool IsMovable(Block block)
    {
        return false;
    }
    public bool IsPassable(Block block)
    {
        return false;
    }
}
public class SnowBlock : IBlock
{
    public static Vector2 leftBotUV = new Vector2(0f/1024,0f/2048);

    public bool IsMovable(Block block)
    {
        return true;
    }
    public bool IsPassable(Block block)
    {
        return false;
    }
}
public class CatusBlock : IBlock
{
    public bool IsMovable(Block block)
    {
        return false;
    }
    public bool IsPassable(Block block)
    {
        return true;
    }
}
public class GrassDirtBlock : IBlock
{
    public static Vector2 leftBotUV = new Vector2(0f/1024,628f/1024); //add this with shape1uv
    
    public bool IsMovable(Block block)
    {
        return false;
    }
    public bool IsPassable(Block block)
    {
        return false;
    }
}
public class JungleGrassBlock : IBlock
{

    public bool IsMovable(Block block)
    {
        return false;
    }
    public bool IsPassable(Block block)
    {
        return true;
    }
}


public class TreeWoodBlock : IBlock
{
    public bool IsMovable(Block block)
    {
        return false;
    }
    public bool IsPassable(Block block)
    {
        return true;
    }
}
public class TreeLeaveBlock : IBlock
{
    public bool IsMovable(Block block)
    {
        return false;
    }
    public bool IsPassable(Block block)
    {
        return true;
    }
}
public class TreeTopBlock : IBlock
{
    public bool IsMovable(Block block)
    {
        return false;
    }
    public bool IsPassable(Block block)
    {
        return true;
    }
}