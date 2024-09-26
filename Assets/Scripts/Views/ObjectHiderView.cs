using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHiderView : MonoBehaviour
{
    [SerializeField] private string hidedObjectTag;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == hidedObjectTag)
            collision.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == hidedObjectTag)
            collision.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
    }

    public void ChangeZoneSize()
    {

    }
}
