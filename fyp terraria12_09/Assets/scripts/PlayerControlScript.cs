using UnityEngine;
using System.Collections;

public class PlayerControlScript : MonoBehaviour {
	
	Rigidbody2D rb;
	SpriteRenderer sr;
	Animator animator;

    public World world;

	int key = 1;//used to set the tile type the mouse can generate, default key = 1

    Camera cam;
    GameObject camGO;

    public GameObject background;

	void Awake(){		
		rb = GetComponent<Rigidbody2D> ();
		sr = GetComponent<SpriteRenderer> ();
		animator = GetComponent<Animator> ();
        cam = GetComponentInChildren<Camera>();
        camGO = cam.gameObject;
	}


    bool isWalkingOrRunning = false;//to ensure even rb.velocity.y == 0,the isWalking or isRunning in animator not set to false   


    float x;
    public void HandleMouse()
	{
		frame++;

		if (Input.GetMouseButton(0) && frame > mouseResponseRate)
        {
			frame = 0;
            RaycastHit2D rch2d = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),Vector2.zero);   

            if (rch2d.collider != null && rch2d.collider.gameObject.tag == "Chunk")
            {   
                
                Chunk c = rch2d.collider.gameObject.GetComponent<Chunk>();
                int chunkx = c.pos[0];
                int chunky = c.pos[1];
                int x = Mathf.FloorToInt((Camera.main.ScreenToWorldPoint(Input.mousePosition).x - (float)chunkx)/Chunk.tileSize) + Chunk.chunkSize / 2;
                int y = Mathf.FloorToInt((Camera.main.ScreenToWorldPoint(Input.mousePosition).y - (float)chunky)/Chunk.tileSize) + Chunk.chunkSize / 2 ;
                //Ensure not out of range (0,Chunk.chunkSize), problem happens when not setting these constraints
                //It is because raycast float calculation may return a slightly unexpected value due to the inaccurate mouse pos 
                x = (x <= 0) ? 0 : (x >= Chunk.chunkSize - 1) ? Chunk.chunkSize - 1 : x;
                y = (y <= 0) ? 0 : (y >= Chunk.chunkSize - 1) ? Chunk.chunkSize - 1 : y;

                if (rch2d.collider.isTrigger == true)
                {
                    Block b = c.blocks[x, y];
                    if (b.tileID == BlockIndex.air)
                    {
						if (key == 2) {
                            c.SetBlock (b.bWorldPos[0], b.bWorldPos[1], BlockIndex.dirt, b.fluid, b.light, b.wallID);                                              
							c.SetBlockImage (x, y);
                            c.AddPathsRemovePaths1(x, y);
                            c.hasChanged = true;
						} else if (key == 3) {
							c.SetBlock (b.bWorldPos[0], b.bWorldPos[1], BlockIndex.stone, b.fluid, b.light, b.wallID);
                            c.SetBlockImage (x, y);
                            c.AddPathsRemovePaths1(x, y);
                            c.hasChanged = true;
						} else if (key == 4) {
							c.SetBlock (b.bWorldPos[0], b.bWorldPos[1], BlockIndex.sand, b.fluid, b.light, b.wallID);
                            //c.AddBlock (x, y);//#movable block added to chunk's movable hash set!!
                            c.SetBlockImage (x, y);
                            c.AddPathsRemovePaths1(x, y);
                            c.hasChanged = true;
						} else if (key == 5) {
							c.SetBlock (b.bWorldPos[0], b.bWorldPos[1], BlockIndex.copper, b.fluid, b.light, b.wallID);
                            c.SetBlockImage (x, y);
                            c.AddPathsRemovePaths1(x, y);
                            c.hasChanged = true;
						} else if (key == 6) {
							c.SetBlock (b.bWorldPos[0], b.bWorldPos[1], BlockIndex.corrDirt, b.fluid, b.light, b.wallID);
                            c.SetBlockImage (x, y);
                            c.AddPathsRemovePaths1(x, y);
                            c.hasChanged = true;
						} else if (key == 7) {
							c.SetBlock (b.bWorldPos[0], b.bWorldPos[1], BlockIndex.junDirt, b.fluid, b.light, b.wallID);
                            c.SetBlockImage (x, y);
                            c.AddPathsRemovePaths1(x, y);
                            c.hasChanged = true;
						} else {
							return; 
						}
                        //update light amount by decreasing it by -1
                        
                        
                        if(b.wallID == WallIndex.noWall)
                        {
                            c.Update5x5NeigLight(b.bWorldPos[0], b.bWorldPos[1], -1);//update the tile's light first
                            c.UpdateCurrAndCallUpdateNeig4x4Color(x, y, false);//update the color
                        }
                        else
                        {
                            c.UpdateCurrAndCallUpdateNeig4x4Color(x, y, false);//update the color
                        }
                        
                        

                    }
                    else if(c.blocks[x,y].passable == true && c.blocks[x,y].tileID != BlockIndex.catus) // grass block
                    {
                        c.SetBlock(b.bWorldPos[0], b.bWorldPos[1], BlockIndex.air, b.fluid, b.light, b.wallID);
                        c.RemoveBlock (x, y);

                        c.hasChanged = true;

                    }
                    
					//after setblockimage need to update filter
					c.UpdateFilter ();                   
                    //Update mesh's polygon collider
                    c.ClearCollTriList ();
					c.RefreshChunkCollTri ();
                    

                }
                else//not a trigger, it is a collider -> unpassable  
				{   
					if (c.blocks [x, y].tileID != BlockIndex.air && c.blocks[x,y].tileID != BlockIndex.catus && key == 1) {
                        
                        if (c.blocks[x,y].tileID == BlockIndex.sand)
                        {
							
							Block upperBlock = c.GetBlock (c.blocks [x, y].bWorldPos[0], c.blocks [x, y].bWorldPos[1] + Chunk.tileSize);
							if (upperBlock.passable == true && upperBlock.tileID != BlockIndex.air) {
                                //should be done using while loop loop upperblock and check if it is catus block
                                /* remove this comment later
                                if (upperBlock.no == 0) {
									c.SetBlock (c.blocks [x, y + 1].bWorldPos[0], c.blocks [x, y + 1].bWorldPos[1], BlockIndex.air);
									c.RemoveBlock (x, y + 1);
									c.SetBlock (c.blocks [x, y + 2].bWorldPos[0], c.blocks [x, y + 2].bWorldPos[1], BlockIndex.air);
									c.RemoveBlock (x, y + 2);
									c.SetBlock (c.blocks [x, y + 3].bWorldPos[0], c.blocks [x, y + 3].bWorldPos[1], BlockIndex.air);
									c.RemoveBlock (x, y + 3);
									c.SetBlock (c.blocks [x + 1, y + 1].bWorldPos[0], c.blocks [x + 1, y + 1].bWorldPos[1], BlockIndex.air);
									c.RemoveBlock (x + 1, y + 1);
									c.SetBlock (c.blocks [x + 1, y + 2].bWorldPos[0], c.blocks [x + 1, y + 2].bWorldPos[1], BlockIndex.air);
									c.RemoveBlock (x + 1, y + 2);
									c.SetBlock (c.blocks [x + 1, y + 3].bWorldPos[0], c.blocks [x + 1, y + 3].bWorldPos[1], BlockIndex.air);
									c.RemoveBlock (x + 1, y + 3);
								} else if (upperBlock.no == 1 || upperBlock.no == 2) {
									c.SetBlock (c.blocks [x, y + 1].bWorldPos[0], c.blocks [x, y + 1].bWorldPos[1], BlockIndex.air);
									c.RemoveBlock (x, y + 1);
									c.SetBlock (c.blocks [x, y + 2].bWorldPos[0], c.blocks [x, y + 2].bWorldPos[1], BlockIndex.air);
									c.RemoveBlock (x, y + 2);
								} else if (upperBlock.no == 3 || upperBlock.no == 5) {
									c.SetBlock (c.blocks [x, y + 1].bWorldPos[0], c.blocks [x, y + 1].bWorldPos[1], BlockIndex.air);
									c.RemoveBlock (x, y + 1);
									c.SetBlock (c.blocks [x, y + 2].bWorldPos[0], c.blocks [x, y + 2].bWorldPos[1], BlockIndex.air);
									c.RemoveBlock (x, y + 2);
									c.SetBlock (c.blocks [x, y + 3].bWorldPos[0], c.blocks [x, y + 3].bWorldPos[1], BlockIndex.air);
									c.RemoveBlock (x, y + 3);
								}else if(upperBlock.no == 4){
									c.SetBlock (c.blocks [x, y + 1].bWorldPos[0], c.blocks [x, y + 1].bWorldPos[1], BlockIndex.air);
									c.RemoveBlock (x, y + 1);
								}
                                */
                            }
                        }

						c.SetBlock (c.blocks [x, y].bWorldPos[0], c.blocks [x, y].bWorldPos[1], BlockIndex.air, c.blocks[x, y].fluid, 
                            c.blocks[x, y].light, c.blocks[x, y].wallID);

                        //Add light by increasing the neig tile by 1
                        if (c.blocks[x, y].wallID == WallIndex.noWall)
                        {
                            c.Update5x5NeigLight(c.blocks[x, y].bWorldPos[0], c.blocks[x, y].bWorldPos[1], 1);
                            c.UpdateCurrAndCallUpdateNeig4x4Color(x, y, true);
                        }
                        else
                        {
                            c.UpdateCurrAndCallUpdateNeig4x4Color(x, y, true);
                        }

                        c.AddPaths1RemovePaths(x, y);

                        c.RemoveBlock (x, y);

						c.hasChanged = true;

						c.UpdateFilter ();
						c.ClearCollTriList ();
						c.RefreshChunkCollTri ();
					}		
                }





				  
            }
        }
    }
	int frame = 0;
	public int mouseResponseRate = 0;

    void Update()
    {
        SetKeyCode();      
		HandleMouse ();

        //set camera and background fixed within a range when the player reached the max world size
		if(gameObject.transform.position.x <= -(World.worldLeftChunksNo-2) * Chunk.chunkSize * Chunk.tileSize)
        {
            cam.transform.localPosition = new Vector3((-(World.worldLeftChunksNo - 2) * Chunk.chunkSize * Chunk.tileSize - transform.position.x)/100, 
                cam.transform.localPosition.y,cam.transform.localPosition.z);
            background.transform.localPosition = new Vector3((-(World.worldLeftChunksNo - 2) * Chunk.chunkSize * Chunk.tileSize - transform.position.x) / 100,
                cam.transform.localPosition.y, cam.transform.localPosition.z);
        }
        if (gameObject.transform.position.x >= (World.worldSizeInChunkx-World.worldLeftChunksNo-2) * Chunk.chunkSize * Chunk.tileSize)
        {
            cam.transform.localPosition = new Vector3( ((World.worldSizeInChunkx - World.worldLeftChunksNo - 2) * Chunk.chunkSize * Chunk.tileSize - transform.position.x) / 100,
                cam.transform.localPosition.y, cam.transform.localPosition.z);
            background.transform.localPosition = new Vector3( ((World.worldSizeInChunkx - World.worldLeftChunksNo - 2) * Chunk.chunkSize * Chunk.tileSize - transform.position.x) / 100,
                cam.transform.localPosition.y, cam.transform.localPosition.z);
        }

        //set the x movement range of the player in the world
        if (gameObject.transform.position.x <= -World.worldLeftChunksNo * Chunk.chunkSize * Chunk.tileSize + Chunk.tileSize/2)
        {
            gameObject.transform.position = new Vector3(-World.worldLeftChunksNo * Chunk.chunkSize * Chunk.tileSize + Chunk.tileSize, gameObject.transform.position.y, gameObject.transform.position.z);
        }
        if (gameObject.transform.position.x >= (World.worldSizeInChunkx-World.worldLeftChunksNo) * Chunk.chunkSize * Chunk.tileSize - Chunk.tileSize / 2)
        {
            gameObject.transform.position = new Vector3((World.worldSizeInChunkx - World.worldLeftChunksNo) * Chunk.chunkSize * Chunk.tileSize - Chunk.tileSize / 2,
                gameObject.transform.position.y, gameObject.transform.position.z);
        }
    }

    void SetKeyCode()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))//airlock
        {
            key = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))//dirtblock
        {
            key = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))//stoneblock
        {
            key = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))//sandblock
        {
            key = 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))//Copperblock
        {
            key = 5;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))//Corruption dirt block
        {
            key = 6;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))//jungle dirt block
        {
            key = 7;
        }
		else if (Input.GetKeyDown(KeyCode.Alpha8))//Grass block
		{
			key = 8;
		}


    }

    void FixedUpdate()
    {
        x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            sr.flipX = true;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            sr.flipX = false;
        }
        if (Input.GetKeyDown(KeyCode.Space) && animator.GetBool("inFreeFall") == false)
        {
            rb.AddForce(new Vector3(0, 200, 0), ForceMode2D.Impulse);
            animator.SetBool("inFreeFall", true);
        }
        if ((Input.GetAxis("Horizontal") > 0.0000002f) ||
            (Input.GetAxis("Horizontal") < -0.0000002f) &&
            animator.GetBool("inFreeFall") == false)
        {
            isWalkingOrRunning = true;
            animator.SetBool("isWalking", true);
        }
        else
        {
            isWalkingOrRunning = false;
            animator.SetBool("isWalking", false);
        }	

        if((rb.velocity.y > 0.00002f || rb.velocity.y < -0.00002f) )
        {
            animator.SetBool("inFreeFall", true);
            animator.SetBool("isWalking", false);
        }
        else 
        {
            animator.SetBool("inFreeFall", false);
        }	

		transform.Translate(x, 0, 0);
	}
    
}
