using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColourPicker : MonoBehaviour
{
    public GameObject colourButton;
    public GameObject colourPanel;

    public Color[] colours;

    public void OnColourPickerClick() {
        colourPanel.SetActive(true);
    }

    public void OnColourPicked(Button colourButtonPicked) {
        colourButton.GetComponent<Image>().color = colourButtonPicked.image.color;
        colourPanel.SetActive(false);
    }

}
