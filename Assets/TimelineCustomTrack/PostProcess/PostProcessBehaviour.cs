using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using System;

[Serializable]
public class PostProcessBehaviour : PlayableBehaviour
{
    [HideInInspector]
    public Volume volume;
    public VolumeProfile profile;
    public int layer;
    public AnimationCurve weightCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (profile != null)
        {
            QuickVolume(this);
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (volume != null)
        {
            GameObject.DestroyImmediate(volume.gameObject);
        }
    }

    void QuickVolume(PostProcessBehaviour behaviour)
    {
        if (volume == null)
        {
            volume = new GameObject().AddComponent<Volume>();
            volume.gameObject.layer = layer;
            volume.gameObject.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
            volume.gameObject.name = $"PostProcessBehaviourVolume [Profile {behaviour.profile.name}]";
            volume.weight = 0;
            volume.priority = 1;
            volume.isGlobal = true;
            volume.profile = behaviour.profile;
        }
    }

    public void ChangeWeight(float time)
    {
        if (volume == null) { return; }
        volume.weight = weightCurve.Evaluate(time);
    }
}