using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoopScript : MonoBehaviour
{
    public AudioClip destroySound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Broom"))
        {
            SoundManager.Instance.RandomizeSfx(destroySound);
            Destroy(gameObject);
            Destroy(collision.gameObject);
            GameManager.Instance.hasSpawnedObject = false;
        }
    }
}
