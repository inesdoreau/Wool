using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Velcro : MonoBehaviour
{
    public float forceToDescratch;
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
        if (other.gameObject.tag.Equals("Yarn"))
        {
            other.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }

        if (other.gameObject.tag.Equals("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().velcroNormal = this.transform.forward;
            other.gameObject.GetComponent<PlayerController>().Climbing = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().Climbing = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag.Equals("Yarn"))
        {
            //Debug.Log(other.gameObject.GetComponent<CharacterJoint>().currentForce.magnitude);
            if (other.gameObject.GetComponent<CharacterJoint>().currentForce.magnitude > forceToDescratch)
            {
                other.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            }
        }
    }
}
