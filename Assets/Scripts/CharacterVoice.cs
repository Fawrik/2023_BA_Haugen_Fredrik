using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum VoiceEffect { Normal, Emphasis, Cheerful, Unhappy, Agitated, Whispering, Slow, Stuttering, Custom };

[CreateAssetMenu(fileName = "New Character Voice", menuName = "Create New Character Voice")]
public class CharacterVoice : ScriptableObject
{
    [SerializeField]
    string voiceName = "New Voice";
    [SerializeField, Tooltip("If more than one sample is added, they will play randomly.")]
    AudioClip[] voiceSamples;
    [SerializeField]
    VoiceEffectPreset normalPreset, emphasisPreset, cheerfulPreset, unhappyPreset, agitatedPreset, whisperingPreset, slowPreset, stutteringPreset;

    public string VoiceName => voiceName;
    public AudioClip[] VoiceSamples => voiceSamples;
    public VoiceEffectPreset NormalPreset => normalPreset;
    public VoiceEffectPreset EmphasisPreset => emphasisPreset;
    public VoiceEffectPreset CheerfulPreset => cheerfulPreset;
    public VoiceEffectPreset UnhappyPreset => unhappyPreset;
    public VoiceEffectPreset AgitatedPreset => agitatedPreset;
    public VoiceEffectPreset WhisperingPreset => whisperingPreset;
    public VoiceEffectPreset SlowPreset => slowPreset;
    public VoiceEffectPreset StutteringPreset => stutteringPreset;

    public VoiceEffectPreset GetPresetValues(VoiceEffect effectPreset)
    {
        switch (effectPreset)
        {
            case VoiceEffect.Normal:
                return normalPreset;
            case VoiceEffect.Emphasis:
                return emphasisPreset;
            case VoiceEffect.Cheerful:
                return cheerfulPreset;
            case VoiceEffect.Unhappy:
                return unhappyPreset;
            case VoiceEffect.Agitated:
                return agitatedPreset;
            case VoiceEffect.Whispering:
                return whisperingPreset;
            case VoiceEffect.Slow:
                return slowPreset;
            case VoiceEffect.Stuttering:
                return stutteringPreset;
            case VoiceEffect.Custom:
                Debug.LogWarning("Can't get values for Custom voice effect");
                return normalPreset;
            default:
                return normalPreset;
        }
    }

    [ContextMenu("Apply Normal values to all presets")]
    void ApplyNormalToAllPresets()
    {
        foreach (var voice in new VoiceEffectPreset[] { emphasisPreset, cheerfulPreset, unhappyPreset, agitatedPreset, whisperingPreset, slowPreset, stutteringPreset })
        {
            voice.SetPitchRange(normalPreset.PitchRange);
            voice.SetVolume(normalPreset.Volume);
            voice.SetTimeBetweenSamples(normalPreset.TimeBetweenSamples);
        }
    }
}

[System.Serializable]
public class VoiceEffectPreset
{
    [SerializeField, Tooltip("Pitch bending applied to voice samples.")]
    Vector2 pitchRange = Vector2.one;
    [SerializeField, Tooltip("")]
    float volume = 1;
    [SerializeField, Tooltip("Seconds between voice samples starting. Must be more than 0!")]
    Vector2 timeBetweenSamples = new(0.1f, 0.1f);

    public Vector2 PitchRange => pitchRange;
    public float Volume => volume;
    public Vector2 TimeBetweenSamples => timeBetweenSamples;

    public void SetPitchRange(Vector2 newRange) => pitchRange = newRange;
    public void SetVolume(float newVolume) => volume = newVolume;
    public void SetTimeBetweenSamples(Vector2 newRange) => timeBetweenSamples = newRange;
}
