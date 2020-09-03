using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * A frame that represents all of the changed properties since the previous keyframe.
 */
public class TimeFrame
{
    public int FrameNumber { get; private set; } = 0;
    public bool IsKeyFrame { get; private set; }
    public DeltaProperties ChangedProperties = new DeltaProperties();

    public TimeFrame(int frameNumber, bool isKeyFrame = false)
    {
        FrameNumber = frameNumber;
        IsKeyFrame = isKeyFrame;
    }
}

/**
 * A record of all the changes in values for this frame (showing the new values)
 */
public class DeltaProperties
{
    public Dictionary<string, object> Properties { get; } = new Dictionary<string, object>();

    public bool Has(string key) => Properties.ContainsKey(key);
    public bool Has(Object o, string key) => Properties.ContainsKey($"{o.GetInstanceID()}-{key}");

    public T Get<T>(string key) => (T)Properties[key];
    public T Get<T>(Object o, string key) => (T)Properties[$"{o.GetInstanceID()}-{key}"];

    public void Set<T>(string key, T property) => Properties[key] = property;
    public void Set<T>(Object o, string key, T property) => Properties[$"{o.GetInstanceID()}-{key}"] = property;

}