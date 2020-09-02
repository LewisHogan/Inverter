using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * The many states of Time
 */
public enum EInverterStatus
{
    FORWARD,
    PAUSED,
    BACKWARD
}

/**
 * Controls the flow of time for all invertable game objects.
 * 
 * Note: There is no limit on how big the stack can grow, be very careful with memory consumption!
 */
public class InverterController : MonoBehaviour
{
    public static InverterController Controller { get; private set; }
    public EInverterStatus InverterStatus;

    private int _currentTimeFrame;
    private Stack<TimeFrame> _frames;

    private List<GameObject> _invertables;
    private bool initialFramePushed = false;

    void Awake()
    {
        _currentTimeFrame = 0;
        _frames = new Stack<TimeFrame>();
        _invertables = new List<GameObject>();
        InverterStatus = EInverterStatus.FORWARD;
        if (Controller == null) Controller = this;
    }

    // Update is called once per frame
    void Update()
    {
        // Can't push the first frame in start because this script might run before the
        // other components register themselves with it.
        if (!initialFramePushed)
        {
            _frames.Push(CreateTimeFrame(_currentTimeFrame++));
            initialFramePushed = true;
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (InverterStatus == EInverterStatus.FORWARD)
                InverterStatus = EInverterStatus.PAUSED;
            else if (InverterStatus == EInverterStatus.PAUSED)
                InverterStatus = EInverterStatus.BACKWARD;
            else if (InverterStatus == EInverterStatus.BACKWARD)
                InverterStatus = EInverterStatus.FORWARD;
        };
    }

    void FixedUpdate()
    {
        switch(InverterStatus)
        {
            case EInverterStatus.FORWARD:
                FixedTime();
                break;
            case EInverterStatus.PAUSED:
                PausedTime();
                break;
            case EInverterStatus.BACKWARD:
                InvertedFixedTime();
                break;
        }
    }

    public void RegisterInvertable(GameObject invertable)
    {
        _invertables.Add(invertable);
    }

    public void DeregisterInvertable(GameObject invertable)
    {
        _invertables.Remove(invertable);
    }

    void FixedTime()
    {
        _frames.Push(CreateTimeFrame(_currentTimeFrame++));
    }

    void PausedTime()
    {
        ConsumeTimeFrame(_frames.Peek());
    }

    void InvertedFixedTime()
    {
        // TODO: Load state from frame stack
        // TODO: Special case, we never pop the first timeframe off the stack, since that's our epoch
            
        if (_frames.Count > 1)
        {
            ConsumeTimeFrame(_frames.Pop());
            _currentTimeFrame--;
            return;
        }

        ConsumeTimeFrame(_frames.Peek());
    }

    TimeFrame CreateTimeFrame(int frameNumber)
    {
        TimeFrame frame = new TimeFrame(frameNumber);

        // If we have a frameNumber > 0 we can do delta

        // TODO: Special case for the first frame, since there's no past frame we just store everything
        if (frameNumber == 0)
        {
            
        }

        //  For now we just do a full frame each physics tick
        // TODO: Only store the things that changed since last tick.
        foreach (GameObject gameObject in _invertables)
        {
            frame.ChangedProperties.Set<Vector3>(gameObject, "position", gameObject.transform.position);
            frame.ChangedProperties.Set<Quaternion>(gameObject, "rotation", gameObject.transform.rotation);
            frame.ChangedProperties.Set<Vector3>(gameObject, "scale", gameObject.transform.localScale);

            // If we have a rigid body we want to store the velocity and angular velocity as well as position information
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                frame.ChangedProperties.Set<Vector3>(rb, "position", rb.position);
                frame.ChangedProperties.Set<Vector3>(rb, "velocity", rb.velocity);
                frame.ChangedProperties.Set<Vector3>(rb, "angularVelocity", rb.angularVelocity);
                frame.ChangedProperties.Set<bool>(rb, "isKinematic", rb.isKinematic);
            }
        }

        return frame;
    }

    void ConsumeTimeFrame(TimeFrame frame)
    {
        foreach (GameObject gameObject in _invertables)
        {
            if (frame.ChangedProperties.Has(gameObject, "position"))
            {
                gameObject.transform.position = frame.ChangedProperties.Get<Vector3>(gameObject, "position");
            }

            if (frame.ChangedProperties.Has(gameObject, "rotation"))
            {
                gameObject.transform.rotation = frame.ChangedProperties.Get<Quaternion>(gameObject, "rotation");
            }

            if (frame.ChangedProperties.Has(gameObject, "scale"))
            {
                gameObject.transform.localScale = frame.ChangedProperties.Get<Vector3>(gameObject, "scale");
            }
            
            
            // If we have a rigid body we want to grab the velocity and angular velocity as well as position information
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                if (frame.ChangedProperties.Has(rb, "position"))
                {
                    rb.position = frame.ChangedProperties.Get<Vector3>(rb, "position");
                }

                if (frame.ChangedProperties.Has(rb, "velocity"))
                {
                    rb.velocity = frame.ChangedProperties.Get<Vector3>(rb, "velocity");
                }

                if (frame.ChangedProperties.Has(rb, "angularVelocity"))
                {
                    rb.angularVelocity = frame.ChangedProperties.Get<Vector3>(rb, "angularVelocity");
                }

                if (frame.ChangedProperties.Has(rb, "isKinematic"))
                {
                    rb.isKinematic = frame.ChangedProperties.Get<bool>(rb, "isKinematic");
                }                
            }
        }
    }
}


