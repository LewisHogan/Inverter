using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvertableToyController : MonoBehaviour
{
    public InverterController InverterController;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        if (InverterController == null)
            InverterController = InverterController.Controller;

        InverterController.RegisterInvertable(this.gameObject);
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        InverterController.DeregisterInvertable(this.gameObject);
    }
}
