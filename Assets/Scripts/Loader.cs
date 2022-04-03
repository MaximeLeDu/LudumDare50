using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject soundManager;

    private void Awake()
    {
        if(GameManager.Instance == null)
            Instantiate(gameManager);

        if (SoundManager.Instance == null)
            Instantiate(soundManager);
    }
}
