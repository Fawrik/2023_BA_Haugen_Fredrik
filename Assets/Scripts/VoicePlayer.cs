using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoicePlayer : MonoBehaviour
{
    #region Public vars & functions

    [HideInInspector]
    public CharacterVoice currentCharacterVoice;
    [HideInInspector]
    public VoiceEffect currentVoiceEffect = VoiceEffect.Normal;

    public bool PlayingVoice { get; private set; }
    public bool VoicePaused { get; private set; }

    /// <summary>
    /// Start voice playback for a text box.
    /// </summary>
    public void StartPlayingVoice()
    {
        if (currentCharacterVoice == null) throw new System.Exception("No CharacterVoice has been set!");
        if (PlayingVoice && !VoicePaused) return;

        PlayingVoice = true;
        VoicePaused = false;
        timeToNextSample = 0;
        timePaused = 0;
    }

    /// <summary>
    /// Pause voice playback. Use this for hesitation.
    /// </summary>
    public void PauseVoice()
    {
        if (!PlayingVoice || VoicePaused) return;

        VoicePaused = true;
        timePaused = 0;
    }

    /// <summary>
    /// Resume a voice that was paused.
    /// </summary>
    public void ResumeVoice()
    {
        if (!PlayingVoice || !VoicePaused) return;

        VoicePaused = false;
        if (timePaused < timeToNextSample)
            timeToNextSample -= timePaused;
        else
            timeToNextSample = 0;
        timePaused = 0;
    }

    /// <summary>
    /// Stop voice playback. Use this at the end of a text box.
    /// </summary>
    public void StopPlayingVoice()
    {
        if (!PlayingVoice) return;

        PlayingVoice = false;
        VoicePaused = false;
        timePaused = 0;
    }

    /// <summary>
    /// Set voice pitch directly.
    /// </summary>
    /// <param name="pitchRange">Min and max pitch for randomization. Must be higher than 0, lower than 3.</param>
    /// <param name="resetOtherValues">If true, volume and time between samples will be set to Normal preset values.</param>
    public void SetCustomPitch(Vector2 pitchRange, bool resetOtherValues)
    {
        pitchRange = new(Mathf.Clamp(pitchRange.x, 0.001f, 3), Mathf.Clamp(pitchRange.y, 0.001f, 3));

        currentVoiceEffect = VoiceEffect.Custom;
        currentPitchRange = pitchRange;

        if (resetOtherValues)
        {
            currentVolume = currentCharacterVoice.NormalPreset.Volume;
            currentTimeBetweenSamples = currentCharacterVoice.NormalPreset.TimeBetweenSamples;
        }
    }

    /// <summary>
    /// Set voice volume directly.
    /// </summary>
    /// <param name="volume">New volume value. Must be between 0 and 1.</param>
    /// <param name="resetOtherValues">If true, pitch and time between samples will be set to Normal preset values.</param>
    public void SetCustomVolume(float volume, bool resetOtherValues)
    {
        volume = Mathf.Clamp01(volume);

        currentVoiceEffect = VoiceEffect.Custom;
        currentVolume = volume;

        if (resetOtherValues)
        {
            currentPitchRange = currentCharacterVoice.NormalPreset.PitchRange;
            currentTimeBetweenSamples = currentCharacterVoice.NormalPreset.TimeBetweenSamples;
        }
    }

    /// <summary>
    /// Set time between samples directly.
    /// </summary>
    /// <param name="timeRange">Min and max time for randomization. Must be higher than 0.</param>
    /// <param name="resetOtherValues">If true, pitch and volume will be set to Normal preset values.</param>
    public void SetCustomTimeBetweenSamples(Vector2 timeRange, bool resetOtherValues)
    {
        if (timeRange.x < 0.001f)
            timeRange.x = 0.001f;
        if (timeRange.y < 0.001f)
            timeRange.y = 0.001f;

        currentVoiceEffect = VoiceEffect.Custom;
        currentTimeBetweenSamples = timeRange;

        if (resetOtherValues)
        {
            currentPitchRange = currentCharacterVoice.NormalPreset.PitchRange;
            currentVolume = currentCharacterVoice.NormalPreset.Volume;
        }
    }

    #endregion


    #region Private vars & functions

    readonly int audioSourceCount = 5;
    int lastUsedAudioSource;
    List<AudioSource> AudioSources = new();

    float timePaused = 0;
    float timeToNextSample = 0;

    Vector2 currentPitchRange = Vector2.one;
    float currentVolume = 0.5f;
    Vector2 currentTimeBetweenSamples = new(0.1f, 0.1f);

    private void Start()
    {
        InitializeAudioSources();
    }

    void InitializeAudioSources()
    {
        for (int i = 0; i < audioSourceCount; i++)
        {
            AudioSources.Add(gameObject.AddComponent<AudioSource>());
            AudioSources[i].playOnAwake = false;
        }
        lastUsedAudioSource = audioSourceCount - 1;
    }

    private void Update()
    {
        if (!PlayingVoice) return;

        UpdateVoiceVariables();

        if (VoicePaused)
            timePaused += Time.unscaledDeltaTime;
        else
            VoicePlayback();
    }

    void UpdateVoiceVariables()
    {
        if (currentVoiceEffect == VoiceEffect.Custom) return;
        VoiceEffectPreset voiceEffect = currentCharacterVoice.GetPresetValues(currentVoiceEffect);
        currentPitchRange = voiceEffect.PitchRange;
        currentVolume = voiceEffect.Volume;
        currentTimeBetweenSamples = voiceEffect.TimeBetweenSamples;
    }

    void VoicePlayback()
    {
        timeToNextSample -= Time.unscaledDeltaTime;

        if (timeToNextSample <= 0)
        {
            PlayVoiceSample();
            timeToNextSample = Random.Range(currentTimeBetweenSamples.x, currentTimeBetweenSamples.y);
        }
    }

    void PlayVoiceSample()
    {
        // Alternate between Audio Sources to prevent voice samples cutting each other off (which can lead to ugly audio snapping)
        if (lastUsedAudioSource == AudioSources.Count - 1)
            lastUsedAudioSource = 0;
        else
            lastUsedAudioSource++;
        AudioSource audioSource = AudioSources[lastUsedAudioSource];

        audioSource.clip = currentCharacterVoice.VoiceSamples[Random.Range(0, currentCharacterVoice.VoiceSamples.Length)];
        audioSource.volume = currentVolume;
        audioSource.pitch = Random.Range(currentPitchRange.x, currentPitchRange.y);

        audioSource.Play();
    }

    #endregion
}
