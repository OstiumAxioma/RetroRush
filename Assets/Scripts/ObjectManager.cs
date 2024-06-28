using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectManager : MonoBehaviour
{
    public Image playerHealthBar;
    void Start()
    {
        
    }

    public bool IsLowerThanGround() 
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        return player.transform.position.y < -4;
    }

    public bool IsHealthLowerThanZero()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        return player.GetComponent<Health>().CurrentHealth <= 0;

    }

    public void CleanGround() {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy")) { 
            Destroy(enemy);
        }
    }

    public void StopMovement() 
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        //player.GetComponent<Rigidbody>().isKinematic = false;
        //player.GetComponent<Rigidbody>().useGravity = false;
        player.GetComponent<CharacterHandleWeapon>().ShootStop();
        player.GetComponent<CharacterHandleWeapon>().AbilityPermitted = false;
        player.GetComponent<CharacterMovement>().InputAuthorized = false;
        player.GetComponent<CharacterHandleWeapon>().ShootStop();
    }

    public void StartMovement() 
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        //player.GetComponent<Rigidbody>().isKinematic = true;
        //player.GetComponent<Rigidbody>().useGravity = true;
        //player.GetComponent<CharacterPause>().UnPauseCharacter();
        player.GetComponent<CharacterHandleWeapon>().AbilityPermitted = true;
        player.GetComponent<CharacterMovement>().InputAuthorized = true;
    }

    private void UpdateHealth() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) {
            Debug.Log("Player is null");
        }
        float newWidth = player.GetComponent<Health>().CurrentHealth;
        playerHealthBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealth();
    }
}
