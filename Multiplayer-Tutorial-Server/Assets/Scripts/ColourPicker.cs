using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColourPicker : MonoBehaviour
{
    public GameObject colourButton;
    public Color pickedColour;
    public GameObject colourPanel;

    public Networking networking;

    public Color[] colours;

    void Start() {
        pickedColour = colourButton.GetComponent<Image>().color;
    }

    public void OnColourPickerClick() {
        colourPanel.SetActive(true);
    }

    public void OnColourPicked(Button colourButtonPicked) {
        pickedColour = colourButtonPicked.image.color;
        colourButton.GetComponent<Image>().color = colourButtonPicked.image.color;
        colourPanel.SetActive(false);

        networking.ChangePlayerColour(pickedColour);
    }
}
