using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FlashingTextScript : MonoBehaviour {
    Text textComponent;
    public string itemName;
    public string richText;
    public float blinkInterval = 0.3f;
    public float minimalAlpha = 0.1f;
    bool isFading = true;
    float currentTimeElapsed = 0;

	// Use this for initialization
	void Start () {
        textComponent = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        currentTimeElapsed += Time.deltaTime;

        if (currentTimeElapsed >= blinkInterval)
        {
            currentTimeElapsed -= blinkInterval;
            isFading = !isFading;
        }

        if (isFading)
            textComponent.CrossFadeAlpha(minimalAlpha, blinkInterval, false);
        else
            textComponent.CrossFadeAlpha(1f, blinkInterval, false);


    }
}
