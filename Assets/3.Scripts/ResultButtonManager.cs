using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultButtonManager : MonoBehaviour
{
    public GameObject meteoriteCanvas;
    public GameObject moleCavas;

    void Start()
    {
        if (meteoriteCanvas != null) {
            meteoriteCanvas.SetActive(true);
        }
        if (moleCavas != null) {
            moleCavas.SetActive(false);
        }
    }

    public void DisPlayMeteoriteCanvs()
    {
        if (meteoriteCanvas != null) {
            meteoriteCanvas.SetActive(true);
        }
        if (moleCavas != null) {
            moleCavas.SetActive(false);
        }
    }

    public void DisPlayMoleCanvs()
    {
        if (meteoriteCanvas != null) {
            meteoriteCanvas.SetActive(false);
        }
        if (moleCavas != null) {
            moleCavas.SetActive(true);
        }
    }
}
