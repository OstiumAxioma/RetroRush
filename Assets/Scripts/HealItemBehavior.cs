using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.InventoryEngine;
using Unity.VisualScripting;

public class HealItemBehavior : MonoBehaviour
{
    public float healAmount = 30.0f;
    public AudioSource healthPickupSource;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            healthPickupSource.Play();
            GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().ReceiveHealth(healAmount,gameObject);
            Destroy(gameObject, 0.5f);
        }
    }
}
