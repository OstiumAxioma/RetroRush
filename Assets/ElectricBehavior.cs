using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.TopDownEngine;

public class ElectricBehavior : MonoBehaviour
{
    public float damageAmount = -0.05f;
    public AudioSource electricSource;
    // Start is called before the first frame update
    void Start()
    {
        electricSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            collider.gameObject.GetComponent<Health>().ReceiveHealth(damageAmount, gameObject);
        }
    }

    void OnDestroy()
    {
        if (electricSource != null)
        {
            electricSource.Stop();
        }
    }
}
