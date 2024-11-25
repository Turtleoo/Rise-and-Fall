using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Mixer Settings")]
    public AudioMixer audioMixer; // Reference to the AudioMixer
    public string[] exposedParameters; // List of exposed parameter names
    public float[] startValues; // Corresponding start values for the parameters

    private void Awake()
    {
        if (exposedParameters.Length != startValues.Length)
        {
            Debug.LogError("Mismatch between exposed parameters and start values. Ensure they have the same length.");
            return;
        }

        ResetAudioMixerParameters();
    }

    /// <summary>
    /// Resets all exposed parameters to their predefined start values.
    /// </summary>
    private void ResetAudioMixerParameters()
    {
        for (int i = 0; i < exposedParameters.Length; i++)
        {
            if (!string.IsNullOrEmpty(exposedParameters[i]))
            {
                audioMixer.SetFloat(exposedParameters[i], startValues[i]);
            }
        }

        Debug.Log("Audio mixer parameters reset to start values.");
    }
}
