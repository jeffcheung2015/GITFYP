using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Complex
{
    public ushort complexID;
    public BitArray binaryData;//usage depend on complexID
    public byte height;
    public int[] mainPos;//main block's position
    public List<int[]> otherPos; //other blocks's position

}
//|0*12(left branches, 1 means branches, 0 means air)|0*12(right branches)|0*4(height)
public class Tree : Complex//28 bits
{
    public static Vector2 treeBranleftbotUV = new Vector2(260f / 2048, 916f / 2048);
    public static Vector2 treeTopleftbotUV = new Vector2(0f / 2048, 1624f / 2048);
    public static Vector2 treeWoodleftbotUV = new Vector2(0f / 2048, 1198f / 2048);

}
public class CorrTree : Complex
{
    public static Vector2 treeBranleftbotUV = new Vector2(176f / 2048, 916f / 2048);
    public static Vector2 treeTopleftbotUV = new Vector2(0f / 2048, 1542f / 2048);
    public static Vector2 treeWoodleftbotUV = new Vector2(246f / 2048, 1444f / 2048);
}
public class CrimTree : Complex
{
    public static Vector2 treeBranleftbotUV = new Vector2(0f / 2048, 670f / 2048);
    public static Vector2 treeTopleftbotUV = new Vector2(0f / 2048, 1460f / 2048);
    public static Vector2 treeWoodleftbotUV = new Vector2(246f / 2048, 1444f / 2048);
}
public class JunTree : Complex
{
    public static Vector2 treeBranleftbotUV = new Vector2(496f / 2048, 1180f / 2048);
    public static Vector2 treeTopleftbotUV = new Vector2(0f / 2048, 1820f / 2048);
    public static Vector2 treeWoodleftbotUV = new Vector2(833f / 2048, 1934f / 2048);
}
public class SnowTree : Complex
{
    public static Vector2 treeBranleftbotUV = new Vector2(989f / 2048, 1670f / 2048);
    public static Vector2 treeTopleftbotUV = new Vector2(377f / 2048, 1854f / 2048);
    public static Vector2 treeWoodleftbotUV = new Vector2(176f / 2048, 1180f / 2048);
}
public class Vine : Complex
{


}

public class Catus : Complex
{

}
public class CorrCatus : Complex
{

}
public class CrimCatus : Complex
{

}
public class WorkingDesk : Complex
{

}

public class WoodenChest : Complex
{

}
public class GoldChest : Complex
{

}
public class CampFire : Complex
{

}