using UnityEngine;
using System.Collections;
using System.IO;

using UnityEngine.EventSystems;

public class ButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{
	Animator animator;//button has an animation rotating about its origin 

	public GameObject gObjActivate1;//pass by reference , only assign it in game window if necessary
	public GameObject gObjActivate2;//pass by reference , only assign it in game window if necessary
	public GameObject gObjActivate3;//pass by reference , only assign it in game window if necessary
	public GameObject gObjActivate4;//pass by reference , only assign it in game window if necessary

	public GameObject gObjDeactivate1;//pass by reference , only assign it in game window if necessary
	public GameObject gObjDeactivate2;//pass by reference , only assign it in game window if necessary
	public GameObject gObjDeactivate3;//pass by reference , only assign it in game window if necessary
	public GameObject gObjDeactivate4;//pass by reference , only assign it in game window if necessary

    public GameObject tempWorldPrefab;

	void Start(){
		animator = transform.GetChild(0).GetComponent<Animator>();
	}
	public void OnPointerEnter(PointerEventData eventData)
	{//this is the System Defined function like Start() Awake() Update(), you add it manually if needed.
		animator.SetBool ("mouseIsOver", true);
	}

	public void OnPointerExit (PointerEventData eventData) 
	{//this is the System Defined function like Start() Awake() Update(), you add it manually if needed.
		animator.SetBool ("mouseIsOver", false);		
	}

	//function to be invoked when world button clicked -> go to next scene
	public void WButtonClicked(){
		LoadWorldScript.LoadTheWorld = true;
		LoadWorldScript.worldName = gameObject.name;
		GORefScript.gos7.SetActive (true);
		GORefScript.gos2.SetActive (false);
		GORefScript.gos4.SetActive (false);

	}

	//function to be invoked when "Single Player" Or "Back" Button Clicked
	public void SPOBButtonClicked(){

        ObjActDeact();

	}

    public void ObjActDeact()
    {
        if (gObjActivate1 != null)
        {
            gObjActivate1.SetActive(true);
        }
        if (gObjActivate2 != null)
        {
            gObjActivate2.SetActive(true);
        }
        if (gObjActivate3 != null)
        {
            gObjActivate3.SetActive(true);
        }
        if (gObjActivate4 != null)
        {
            gObjActivate4.SetActive(true);
        }
        if (gObjDeactivate1 != null)
        {
            gObjDeactivate1.SetActive(false);
        }
        if (gObjDeactivate2 != null)
        {
            gObjDeactivate2.SetActive(false);
        }
        if (gObjDeactivate3 != null)
        {
            gObjDeactivate3.SetActive(false);
        }
        if (gObjDeactivate4 != null)
        {
            gObjDeactivate4.SetActive(false);
        }
    }
	public void CNWButtonClicked(){
        ObjActDeact();
		
	}

    public void Submit()
    {
        if (InputScript.endEditWN  && InputScript.wName.Length > 0)
        {
            string saveLocation = SaveWorldScript.saveFolderName + "/" + InputScript.wName + "/";
            if (!Directory.Exists(saveLocation))
            {
                if (gObjActivate1 != null) { gObjActivate1.SetActive(true); }
                if (gObjActivate2 != null) { gObjActivate2.SetActive(true); }
                if (gObjDeactivate1 != null) { gObjDeactivate1.SetActive(false); }
                if (gObjDeactivate2 != null) { gObjDeactivate2.SetActive(false); }
                CreateAndSaveNewWorld(InputScript.wName);

                //reset variable region                
                InputScript.wName = "";
                InputScript.endEditWN = false;
                ObjActDeact();
            }
            else
            {
                Debug.Log("world exists");
            }
        }
        else
        {
            Debug.Log("some input field are not filled");            
        }
    }
    // called in the inputField's End edit and generate a temporary world obj 
    public void CreateAndSaveNewWorld(string worldName)
    {
        string saveLocation = SaveWorldScript.saveFolderName + "/" + worldName + "/";

        if (!Directory.Exists(saveLocation))
        {
            GameObject newWorldGO = Instantiate(tempWorldPrefab) as GameObject;
            World world = newWorldGO.GetComponent<World>();
            world.worldName = worldName;
        }
    }
}
