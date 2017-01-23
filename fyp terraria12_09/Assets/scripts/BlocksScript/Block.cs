using UnityEngine;
using System.Collections;
using System;

public struct Block
{
    public bool valid;//substitute of reference type's null feature
    public int[] bWorldPos;//has X and Y get method component
    public bool passable;
    public bool movable;
    public ushort tileID; //Mainly used to determine which type of block is this!!!!
    public byte fluid;
    public int face;
    public byte light;//Set this initially in Chunk.
    public byte wallID;

    public bool IsMovable(Block block)
    {
        return IBlocks.GetBlockType(tileID).IsMovable(this);
    }
    public bool IsPassable(Block block)
    {
        return IBlocks.GetBlockType(tileID).IsPassable(this);
    }
}
/// <summary>
/// An simulation of polymorphism of class
/// </summary>
interface IBlock
{
    bool IsMovable(Block block);
    bool IsPassable(Block block);
}


static class IBlocks
{
    private static IBlock[] types = new IBlock[] {
        new AirBlock(), new DirtBlock(), new GrassBlock(),
        new SandBlock(), new StoneBlock(), new CorrDirtBlock(),
        new JunDirtBlock(), new GrassDirtBlock(),new CopperBlock(),
        new CorrGrassBlock(),  new JungleGrassBlock(),
        new CatusBlock()  };
     
    public static IBlock GetBlockType(int type)
    {
         return types[type];
    }
}

public class ShapeZeroUV{
    public static Vector2[,] uvList = new Vector2[,] {
        //top
         {new Vector2(0f/1024, 72f/2048),new Vector2(16f/1024, 72f/2048),new Vector2(0f/1024, 88f/2048),new Vector2(16f/1024, 88f/2048),},
         //{new Vector2(126f/2048, 144f/2048),new Vector2(142f/2048, 144f/2048),new Vector2(126f/2048, 2048),new Vector2(142f/2048, 160f/2048),},
         //right
         {new Vector2(36f/1024, 18f/2048),new Vector2(52f/1024, 18f/2048),new Vector2(36f/1024, 34f/2048),new Vector2(52f/1024, 34f/2048),},
         //{new Vector2(90f/2048, 162f/2048),new Vector2(106f/2048, 162f/2048),new Vector2(90f/2048, 178f/2048),new Vector2(106f/2048, 178f/2048),},
         //bot
         {new Vector2(0f/1024, 0f/2048),new Vector2(16f/1024, 0f/2048),new Vector2(0f/1024, 16f/2048),new Vector2(16f/1024, 16f/2048),},
         //{new Vector2(126f/2048, 162f/2048),new Vector2(142f/2048, 162f/2048),new Vector2(126f/2048, 178f/2048),new Vector2(142f/2048, 178f/2048),},
         //left
         {new Vector2(0f/1024, 18f/2048),new Vector2(16f/1024, 18f/2048),new Vector2(0f/1024, 34f/2048),new Vector2(16f/1024, 34f/2048),},
         //{new Vector2(108f/2048, 144f/2048),new Vector2(124f/2048, 144f/2048),new Vector2(108f/2048, 160f/2048),new Vector2(124f/2048, 160f/2048),},

         //right top
         {new Vector2(72f/1024, 18f/2048),new Vector2(88f/1024, 18f/2048),new Vector2(72f/1024, 34f/2048),new Vector2(88f/1024, 34f/2048),},
         //{new Vector2(108f/2048, 54f/2048),new Vector2(124f/2048, 54f/2048),new Vector2(108f/2048, 70f/2048),new Vector2(124f/2048, 70f/2048),},

         //{new Vector2(216f/2048, 90f/2048),new Vector2(232f/2048, 90f/2048),new Vector2(216f/2048, 106f/2048),new Vector2(232f/2048, 106f/2048),},
         //{new Vector2(108f/2048, 90f/2048),new Vector2(124f/2048, 90f/2048),new Vector2(108f/2048, 106f/2048),new Vector2(124f/2048, 106f/2048),},

         //right bot
         {new Vector2(72f/1024, 0f/2048),new Vector2(88f/1024, 0f/2048),new Vector2(72f/1024, 16f/2048),new Vector2(88f/1024, 16f/2048),},
         //{new Vector2(108f/2048, 36f/2048),new Vector2(124f/2048, 36f/2048),new Vector2(108f/2048, 52f/2048),new Vector2(124f/2048, 52f/2048),},

         //{new Vector2(216f/2048, 36f/2048),new Vector2(232f/2048, 36f/2048),new Vector2(216f/2048, 52f/2048),new Vector2(232f/2048, 52f/2048),},
         //{new Vector2(108f/2048, 72f/2048),new Vector2(124f/2048, 72f/2048),new Vector2(108f/2048, 88f/2048),new Vector2(124f/2048, 88f/2048),},

         //left bot
         {new Vector2(54f/1024, 0f/2048),new Vector2(70f/1024, 0f/2048),new Vector2(54f/1024, 16f/2048),new Vector2(70f/1024, 16f/2048),},
         //{new Vector2(90f/2048, 36f/2048),new Vector2(106f/2048, 36f/2048),new Vector2(90f/2048, 52f/2048),new Vector2(106f/2048, 52f/2048),},

         //{new Vector2(198f/2048, 36f/2048),new Vector2(214f/2048, 36f/2048),new Vector2(198f/2048, 52f/2048),new Vector2(214f/2048, 52f/2048),},
         //{new Vector2(144f/2048, 72f/2048),new Vector2(160f/2048, 72f/2048),new Vector2(144f/2048, 88f/2048),new Vector2(160f/2048, 88f/2048),},

         //left top
         {new Vector2(54f/1024, 18f/2048),new Vector2(70f/1024, 18f/2048),new Vector2(54f/1024, 34f/2048),new Vector2(70f/1024, 34f/2048),},
         //{new Vector2(90f/2048, 54f/2048),new Vector2(106f/2048, 54f/2048),new Vector2(90f/2048, 70f/2048),new Vector2(106f/2048, 70f/2048),},

         //{new Vector2(198f/2048, 54f/2048),new Vector2(214f/2048, 54f/2048),new Vector2(198f/2048, 70f/2048),new Vector2(214f/2048, 70f/2048),},
         //{new Vector2(144f/2048, 90f/2048),new Vector2(160f/2048, 90f/2048),new Vector2(144f/2048, 106f/2048),new Vector2(160f/2048, 106f/2048),},

         //left right
         {new Vector2(72f/1024, 108f/2048),new Vector2(88f/1024, 108f/2048),new Vector2(72f/1024, 124f/2048),new Vector2(88f/1024, 124f/2048),},
         //{new Vector2(180f/2048, 162f/2048),new Vector2(196f/2048, 162f/2048),new Vector2(180f/2048, 178f/2048),new Vector2(196f/2048, 178f/2048),},

         //{new Vector2(0f/2048, 252f/2048),new Vector2(16f/2048, 252f/2048),new Vector2(0f/2048, 268f/2048),new Vector2(16f/2048, 268f/2048),},
         //{new Vector2(18f/2048, 252f/2048),new Vector2(34f/2048, 252f/2048),new Vector2(18f/2048, 268f/2048),new Vector2(34f/2048, 268f/2048),},

         //top bot
         {new Vector2(0f/1024, 90f/2048),new Vector2(16f/1024, 90f/2048),new Vector2(0f/1024, 106f/2048),new Vector2(16f/1024, 106f/2048),},
         //{new Vector2(126f/2048, 180f/2048),new Vector2(142f/2048, 180f/2048),new Vector2(126f/2048, 196f/2048),new Vector2(142f/2048, 196f/2048),},

         //{new Vector2(36f/2048, 252f/2048),new Vector2(52f/2048, 252f/2048),new Vector2(36f/2048, 268f/2048),new Vector2(52f/2048, 268f/2048),},
         //{new Vector2(54f/2048, 252f/2048),new Vector2(70f/2048, 252f/2048),new Vector2(54f/2048, 268f/2048),new Vector2(70f/2048, 268f/2048),},

         //top right bot
         {new Vector2(54f/1024, 108f/2048),new Vector2(70f/1024, 108f/2048),new Vector2(54f/1024, 124f/2048),new Vector2(70f/1024, 124f/2048),},
         //{new Vector2(144f/2048, 198f/2048),new Vector2(160f/2048, 198f/2048),new Vector2(144f/2048, 214f/2048),new Vector2(160f/2048, 214f/2048),},

         //{new Vector2(108f/2048, 126f/2048),new Vector2(124f/2048, 126f/2048),new Vector2(108f/2048, 142f/2048),new Vector2(124f/2048, 142f/2048),},
         

         //left bot right
         {new Vector2(0f/1024, 162f/2048),new Vector2(16f/1024, 162f/2048),new Vector2(0f/1024, 178f/2048),new Vector2(16f/1024, 178f/2048),},
         //{new Vector2(108f/2048, 216f/2048),new Vector2(124f/2048, 216f/2048),new Vector2(108f/2048, 232f/2048),new Vector2(124f/2048, 232f/2048),},

         //{new Vector2(252f/2048, 0f/2048),new Vector2(268f/2048, 0f/2048),new Vector2(252f/2048, 16f/2048),new Vector2(268f/2048, 16f/2048),},

         //top left bot
         {new Vector2(0f/1024, 108f/2048),new Vector2(16f/1024, 108f/2048),new Vector2(0f/1024, 124f/2048),new Vector2(16f/1024, 124f/2048),},
         //{new Vector2(126f/2048, 198f/2048),new Vector2(142f/2048, 198f/2048),new Vector2(126f/2048, 214f/2048),new Vector2(142f/2048, 214f/2048),},

         //{new Vector2(144f/2048, 126f/2048),new Vector2(160f/2048, 126f/2048),new Vector2(144f/2048, 142f/2048),new Vector2(160f/2048, 142f/2048),},

         //left top right
         {new Vector2(0f/1024, 216f/2048),new Vector2(16f/1024, 216f/2048),new Vector2(0f/1024, 232f/2048),new Vector2(16f/1024, 232f/2048),},
         //{new Vector2(144f/2048, 216f/2048),new Vector2(160f/2048, 216f/2048),new Vector2(144f/2048, 232f/2048),new Vector2(160f/2048, 232f/2048),},

         //{new Vector2(252f/2048, 54f/2048),new Vector2(268f/2048, 54f/2048),new Vector2(252f/2048, 70f/2048),new Vector2(268f/2048, 70f/2048),},

         //4 sides same block
         {new Vector2(18f/1024, 18f/2048),new Vector2(34f/1024, 18f/2048),new Vector2(18f/1024, 34f/2048),new Vector2(34f/1024, 34f/2048),},
         //{new Vector2(18f/2048, 36f/2048),new Vector2(34f/2048, 36f/2048),new Vector2(18f/2048, 52f/2048),new Vector2(34f/2048, 52f/2048),},

         {new Vector2(54f/1024, 198f/2048),new Vector2(70f/1024, 198f/2048),new Vector2(54f/1024, 214f/2048),new Vector2(70f/1024, 214f/2048),},
         //{new Vector2(54f/2048, 180f/2048),new Vector2(70f/2048, 180f/2048),new Vector2(54f/2048, 196f/2048),new Vector2(70f/2048, 196f/2048),},

         //!4 sides same block
         //{new Vector2(234f/2048, 0f/2048),new Vector2(250f/2048, 0f/2048),new Vector2(234f/2048, 16f/2048),new Vector2(250f/2048, 16f/2048),},//49
         //{new Vector2(234f/2048, 54f/2048),new Vector2(250f/2048, 54f/2048),new Vector2(234f/2048, 70f/2048),new Vector2(250f/2048, 70f/2048),},

         //{new Vector2(162f/2048, 108f/2048),new Vector2(178f/2048, 108f/2048),new Vector2(162f/2048, 124f/2048),new Vector2(178f/2048, 124f/2048),},    
         //{new Vector2(126f/2048, 108f/2048),new Vector2(142f/2048, 108f/2048),new Vector2(126f/2048, 124f/2048),new Vector2(142f/2048, 124f/2048),},

         //{new Vector2(252f/2048, 108f/2048),new Vector2(268f/2048, 108f/2048),new Vector2(252f/2048, 124f/2048),new Vector2(268f/2048, 124f/2048),},
         //{new Vector2(198f/2048, 162f/2048),new Vector2(214f/2048, 162f/2048),new Vector2(198f/2048, 178f/2048),new Vector2(214f/2048, 178f/2048),},
    };
}

public class ShapeOneUV{
    public static Vector2[,] uvList = new Vector2[,] {
        //top
         {new Vector2(18f/1024, 380f/1024),new Vector2(34f/1024, 380f/1024),new Vector2(18f/1024, 396f/1024),new Vector2(34f/1024, 396f/1024),},
         
         //right
         {new Vector2(72f/1024, 362f/1024),new Vector2(88f/1024, 362f/1024),new Vector2(72f/1024, 378f/1024),new Vector2(88f/1024, 378f/1024),},
         
         //bot
         {new Vector2(18f/1024, 344f/1024),new Vector2(34f/1024, 344f/1024),new Vector2(18f/1024, 360f/1024),new Vector2(34f/1024, 360f/1024),},
         
         //left
         {new Vector2(0f/1024, 362f/1024),new Vector2(16f/1024, 362f/1024),new Vector2(0f/1024, 378f/1024),new Vector2(16f/1024, 378f/1024),},

         //right top
         {new Vector2(18f/1024, 326f/1024),new Vector2(34f/1024, 326f/1024),new Vector2(18f/1024, 342f/1024),new Vector2(34f/1024, 342f/1024),},
         
         //right bot
         {new Vector2(18f/1024, 308f/1024),new Vector2(34f/1024, 308f/1024),new Vector2(18f/1024, 324f/1024),new Vector2(34f/1024, 324f/1024),},
         
         //left bot
         {new Vector2(0f/1024, 308f/1024),new Vector2(16f/1024, 308f/1024),new Vector2(0f/1024, 324f/1024),new Vector2(16f/1024, 324f/1024),},
         
         //left top
         {new Vector2(0f/1024, 326f/1024),new Vector2(16f/1024, 326f/1024),new Vector2(0f/1024, 342f/1024),new Vector2(16f/1024, 342f/1024),},
         
         //left right
         {new Vector2(90f/1024, 380f/1024),new Vector2(106f/1024, 380f/1024),new Vector2(90f/1024, 396f/1024),new Vector2(106f/1024, 396f/1024),},
         
         //top bot
         {new Vector2(108f/1024, 308f/1024),new Vector2(124f/1024, 308f/1024),new Vector2(108f/1024, 324f/1024),new Vector2(124f/1024, 324f/1024),},
         
         //top right bot
         {new Vector2(216f/1024, 380f/1024),new Vector2(232f/1024, 380f/1024),new Vector2(216f/1024, 396f/1024),new Vector2(232f/1024, 396f/1024),},
         
         //left bot right 
         {new Vector2(126f/1024, 326f/1024),new Vector2(142f/1024, 326f/1024),new Vector2(126f/1024, 342f/1024),new Vector2(142f/1024, 342f/1024),},
         
         //top left bot
         {new Vector2(162f/1024, 380f/1024),new Vector2(178f/1024, 380f/1024),new Vector2(162f/1024, 396f/1024),new Vector2(178f/1024, 396f/1024),},
         
         //left top right
         {new Vector2(126f/1024, 380f/1024),new Vector2(142f/1024, 380f/1024),new Vector2(126f/1024, 396f/1024),new Vector2(142f/1024, 396f/1024),},
         
         //outward
         {new Vector2(18f/1024, 362f/1024),new Vector2(34f/1024, 362f/1024),new Vector2(18f/1024, 378f/1024),new Vector2(34f/1024, 378f/1024),},
         
         //inward
         {new Vector2(162f/1024, 326f/1024),new Vector2(178f/1024, 326f/1024),new Vector2(162f/1024, 342f/1024),new Vector2(178f/1024, 342f/1024),},
         
        
    };
}

public class WallUV{
    public static Vector2[,] uvList = new Vector2[,] {
         //top         
         {new Vector2(72f/1024, 148f/1024),new Vector2(104f/1024, 148f/1024),new Vector2(72f/1024, 180f/1024),new Vector2(104f/1024, 180f/1024),},//right bot left
         //right
         {new Vector2(144f/1024, 112f/1024),new Vector2(176f/1024, 112f/1024),new Vector2(144f/1024, 144f/1024),new Vector2(176f/1024, 144f/1024),},//top left bot  
         //bot
         {new Vector2(72f/1024, 76f/1024),new Vector2(104f/1024, 76f/1024),new Vector2(72f/1024, 108f/1024),new Vector2(104f/1024, 108f/1024),},//left top right                      
         //left
         {new Vector2(0f/1024, 112f/1024),new Vector2(32f/1024, 112f/1024),new Vector2(0f/1024, 144f/1024),new Vector2(32f/1024, 144f/1024),},//bot right top      
         //right top
         {new Vector2(36f/1024, 40f/1024),new Vector2(68f/1024, 40f/1024),new Vector2(36f/1024, 72f/1024),new Vector2(68f/1024, 72f/1024),},//left bot         
         //right bot
         {new Vector2(36f/1024, 4f/1024),new Vector2(68f/1024, 4f/1024),new Vector2(36f/1024, 36f/1024),new Vector2(68f/1024, 36f/1024),},//left top         
         //left bot
         {new Vector2(0f/1024, 4f/1024),new Vector2(32f/1024, 4f/1024),new Vector2(0f/1024, 36f/1024),new Vector2(32f/1024, 36f/1024),},//right top          
         //left top
         {new Vector2(0f/1024, 40f/1024),new Vector2(32f/1024, 40f/1024),new Vector2(0f/1024, 72f/1024),new Vector2(32f/1024, 72f/1024),},//right bot         
         //left right
         {new Vector2(180f/1024, 112f/1024),new Vector2(212f/1024, 112f/1024),new Vector2(180f/1024, 144f/1024),new Vector2(212f/1024, 144f/1024),},//top bot         
         //top bot         
         {new Vector2(252f/1024, 4f/1024),new Vector2(284f/1024, 4f/1024),new Vector2(252f/1024, 36f/1024),new Vector2(284f/1024, 36f/1024),},//left right         
         //top right bot
         {new Vector2(432f/1024, 148f/1024),new Vector2(464f/1024, 148f/1024),new Vector2(432f/1024, 180f/1024),new Vector2(464f/1024, 180f/1024),},//left         
         //left bot right 
         {new Vector2(216f/1024, 40f/1024),new Vector2(248f/1024, 40f/1024),new Vector2(216f/1024, 72f/1024),new Vector2(248f/1024, 72f/1024),},//top
         //top left bot
         {new Vector2(324f/1024, 148f/1024),new Vector2(356f/1024, 148f/1024),new Vector2(324f/1024, 180f/1024),new Vector2(356f/1024, 180f/1024),},//right
         //left top right
         {new Vector2(216f/1024, 148f/1024),new Vector2(248f/1024, 148f/1024),new Vector2(216f/1024, 180f/1024),new Vector2(248f/1024, 180f/1024),},//bot
         //outward
         {new Vector2(288f/1024, 76f/1024),new Vector2(320f/1024, 76f/1024),new Vector2(288f/1024, 108f/1024),new Vector2(320f/1024, 108f/1024),},//outward
         //inward
         {new Vector2(396f/1024, 40f/1024),new Vector2(428f/1024, 40f/1024),new Vector2(396f/1024, 72f/1024),new Vector2(428f/1024, 72f/1024),},//inward             
    };
}
//material 3
public class WoodUV //20*20
{
    //a means middle part, b means left or right part of base
    public static Vector2[,] uvList = new Vector2[,] {
        //Base 1
        {new Vector2(0f/2048, 244f/2048),new Vector2(20f/2048, 244f/2048),new Vector2(0f/2048, 264f/2048),new Vector2(20f/2048, 264f/2048),},
        //Base 2 left a
        {new Vector2(0f/2048, 112f/2048),new Vector2(20f/2048, 112f/2048),new Vector2(0f/2048, 132f/2048),new Vector2(20f/2048, 132f/2048),},
        //Base 2 left b
        {new Vector2(22f/2048, 112f/2048),new Vector2(42f/2048, 112f/2048),new Vector2(22f/2048, 132f/2048),new Vector2(42f/2048, 132f/2048),},
        //Base 2 right a
        {new Vector2(66f/2048, 112f/2048),new Vector2(86f/2048, 112f/2048),new Vector2(66f/2048, 132f/2048),new Vector2(86f/2048, 132f/2048),},
        //Base 2 right b
        {new Vector2(44f/2048, 112f/2048),new Vector2(64f/2048, 112f/2048),new Vector2(44f/2048, 132f/2048),new Vector2(64f/2048, 132f/2048),},
        //middle 1
        {new Vector2(44f/2048, 134f/2048),new Vector2(64f/2048, 134f/2048),new Vector2(44f/2048, 154f/2048),new Vector2(64f/2048, 154f/2048),},
        //middle 2
        {new Vector2(22f/2048, 134f/2048),new Vector2(42f/2048, 134f/2048),new Vector2(22f/2048, 154f/2048),new Vector2(42f/2048, 154f/2048),},
        //middle 3 (must connect with branches right or branches with 6 pixel unit)
        {new Vector2(66f/2048, 134f/2048),new Vector2(86f/2048, 134f/2048),new Vector2(66f/2048, 154f/2048),new Vector2(86f/2048, 154f/2048),},
        //right (must connect with branches middle 3)
        {new Vector2(88f/2048, 134f/2048),new Vector2(108f/2048, 134f/2048),new Vector2(88f/2048, 154f/2048),new Vector2(108f/2048, 154f/2048),},
        //middle 4 (must connect with branches left or branches with 4 pixel unity)
        {new Vector2(88f/2048, 200f/2048),new Vector2(108f/2048, 200f/2048),new Vector2(88f/2048, 220f/2048),new Vector2(108f/2048, 220f/2048),},
        //left (must connect with branches middle 4)
        {new Vector2(66f/2048, 200f/2048),new Vector2(86f/2048, 200f/2048),new Vector2(66f/2048, 220f/2048),new Vector2(86f/2048, 220f/2048),},
        //null top
        {new Vector2(0f/2048, 2f/2048),new Vector2(20f/2048, 2f/2048),new Vector2(0f/2048, 22f/2048),new Vector2(20f/2048, 22f/2048),},
    };
}
//material 3
public class TreeTopUV //80*80
{
    public static Vector2[,] uvList = new Vector2[,] {
        //top1
        {new Vector2(0f/2048,2f/2048), new Vector2(80f/2048,2f/2048), new Vector2(0f/2048,82f/2048), new Vector2(80f/2048,82f/2048) },
        //top2
        {new Vector2(82f/2048,2f/2048), new Vector2(162f/2048,2f/2048), new Vector2(82f/2048,82f/2048), new Vector2(162f/2048,82f/2048) },
        //top3
        {new Vector2(164f/2048,2f/2048), new Vector2(244f/2048,2f/2048), new Vector2(164f/2048,82f/2048), new Vector2(244f/2048,82f/2048) },
    };
}
//material 3
public class TreeBranchesUV //40*40
{
    public static Vector2[,] uvList = new Vector2[,] {
        //left top
        {new Vector2(0f/2048,86f/2048), new Vector2(40f/2048,86f/2048), new Vector2(0f/2048,126f/2048), new Vector2(40f/2048,126f/2048) },
        //right top
        {new Vector2(42f/2048,86f/2048), new Vector2(82f/2048,86f/2048), new Vector2(42f/2048,126f/2048), new Vector2(82f/2048,126f/2048) },
        //left mid
        {new Vector2(0f/2048,44f/2048), new Vector2(40f/2048,44f/2048), new Vector2(0f/2048,84f/2048), new Vector2(40f/2048,84f/2048) },
        //right mid
        {new Vector2(42f/2048,44f/2048), new Vector2(82f/2048,44f/2048), new Vector2(42f/2048,84f/2048), new Vector2(82f/2048,84f/2048) },
        //left bot
        {new Vector2(0f/2048,2f/2048), new Vector2(40f/2048,2f/2048), new Vector2(0f/2048,42f/2048), new Vector2(40f/2048,42f/2048) },
        //right bot
        {new Vector2(42f/2048,2f/2048), new Vector2(82f/2048,2f/2048), new Vector2(42f/2048,42f/2048), new Vector2(82f/2048,42f/2048) },
    };
}

public class TorchUV //20*20
{

}
