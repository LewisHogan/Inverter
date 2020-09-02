using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvertableToyController : MonoBehaviour
{
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        InverterController.Controller.RegisterInvertable(this.gameObject);
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        InverterController.Controller.DeregisterInvertable(this.gameObject);
    }
}
