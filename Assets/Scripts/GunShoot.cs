using GameState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShoot : MonoBehaviour
{
    private bool isGamePause = false;
    public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !MoreMountains.InventoryEngine.InventoryInputManager.InventoryIsOpen
            && !MoreMountains.TopDownEngine.GUIManager.isPause
            && !MoreMountains.TopDownEngine.GUIManager.isDeath
            && !GlobalGameState.isGameOver)
        {
            //Debug.Log("Play sound");
            PlaySound();
        }
    }

    void PlaySound()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        } 
    }
}
