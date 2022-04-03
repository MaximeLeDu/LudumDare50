using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WalkCountdown : MonoBehaviour
{

    private Slider slider;
    private readonly float totalTime = 15;
    private float currentTime;

    private bool hasLoadedMain = false;
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        currentTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        slider.value = currentTime / totalTime;
        if (currentTime >= totalTime && !hasLoadedMain)
        {
            GameManager.Instance.LoadMain();
            hasLoadedMain = true;
        }
    }
}
