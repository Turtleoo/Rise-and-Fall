using UnityEngine;
using UnityEngine.Audio;

public class ButtonAudioReset : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioMixer audioMixer; // Reference to the AudioMixer
    public string[] exposedParameters; // List of exposed parameters to reset

    private float[] initialValues; // Stores the initial values of the parameters

    private void Start()
    {
        if (audioMixer == null || exposedParameters == null || exposedParameters.Length == 0)
        {
            Debug.LogError("AudioMixer or exposed parameters not set up properly.");
            return;
        }

        // Save the initial values of the exposed parameters
        initialValues = new float[exposedParameters.Length];

        for (int i = 0; i < exposedParameters.Length; i++)
        {
            if (!audioMixer.GetFloat(exposedParameters[i], out initialValues[i]))
            {
                Debug.LogWarning($"Failed to get the initial value for parameter: {exposedParameters[i]}.");
            }
        }
    }

    public void ResetAudioParameters()
    {
        if (audioMixer == null || exposedParameters == null || exposedParameters.Length == 0)
        {
            Debug.LogError("AudioMixer or exposed parameters not set up properly.");
            return;
        }

        // Restore the initial values of the exposed parameters
        for (int i = 0; i < exposedParameters.Length; i++)
        {
            audioMixer.SetFloat(exposedParameters[i], initialValues[i]);
        }

        Debug.Log("Audio parameters reset to their initial states.");
    }
}