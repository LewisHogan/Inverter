using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverterPlayerController : MonoBehaviour
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

    private void FixedUpdate()
    {
        if (InverterController.InverterStatus == EInverterStatus.FORWARD)
        {
            var forward = Input.GetAxis("Vertical") * 3;
            var side = Input.GetAxis("Horizontal") * 3;
            rb.AddForce(Vector3.forward * forward, ForceMode.Acceleration);
            rb.AddForce(Vector3.right * side, ForceMode.Acceleration);
        }
    }

    private void OnDestroy()
    {
        InverterController.DeregisterInvertable(this.gameObject);
    }
}
