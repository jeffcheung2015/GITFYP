using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using System.IO;

//this class is used to create a game obj and trigger the script of worldgeneration

public class InputScript : MonoBehaviour {
	InputField inputComp;

	public GameObject gObjActivate1;//pass by reference , only assign it in game window if necessary
	public GameObject gObjActivate2;//pass by reference , only assign it in game window if necessary
	
	public GameObject gObjDeactivate1;//pass by reference , only assign it in game window if necessary
	public GameObject gObjDeactivate2;//pass by reference , only assign it in game window if necessary

    public GameObject placeHolder;

    Text inputText;
	void Start ()
	{
        
		inputComp = gameObject.GetComponent<InputField> ();
        inputText = inputComp.textComponent;
        
	}

    void Update()
    {
        if (placeHolder != null)
        {
            if (inputText.text.Length > 0 && placeHolder.activeInHierarchy == true)
            {
                placeHolder.SetActive(false);

            }
            else if (inputText.text.Length == 0 && placeHolder.activeInHierarchy == false)
            {
                placeHolder.SetActive(true);
            }
        }
    }
   
    public static string wName = "";//worldName

    public static bool endEditWN = false;
    

	// called in the inputField's End edit 
	public void EndEditingWName(string worldName){

        wName = worldName;
        endEditWN = true;
	}

}
