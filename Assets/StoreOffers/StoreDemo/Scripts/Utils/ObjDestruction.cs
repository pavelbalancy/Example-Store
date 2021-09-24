using UnityEngine;
using System.Collections;

public class ObjDestruction : MonoBehaviour
{
    public float timeOut = 5;
    private bool _started = false;
    private bool _destroying = false;
    
    public void OnEnable()
    {
        if (_started)
            Launch();
    }

    private void Launch()
    {
        _destroying = true;
        StartCoroutine(DestroyObject());
    }

    private void Start()
    {
        _started = true;
        Launch();
    }

    private IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(timeOut);
        if (_destroying)
        {
            _destroying = false;
            Destroy(gameObject);
        }
    }
}