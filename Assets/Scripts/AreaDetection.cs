using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AreaDetection : MonoBehaviour
{
    public string tag;
    public UnityEvent onArea;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == tag)
        {
            onArea.Invoke();
        }
    }
}
