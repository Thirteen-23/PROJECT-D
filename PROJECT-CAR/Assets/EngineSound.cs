
using UnityEngine;

[System.Serializable]
public class EngineNote
{
    public AudioSource source;
    public float minRPM;
    public float peakRPM;
    public float maxRPM;
    public float pitchReferenceRPM;

    public float SetPitchAndGetVolumeForRPM(float rpm)
    {
        source.pitch = rpm / pitchReferenceRPM;

        if (rpm < minRPM || rpm > maxRPM)
        {
            return 0f;
        }

        if (rpm < peakRPM)
        {
            return Mathf.InverseLerp(minRPM, peakRPM, rpm);
        }
        else
        {
            return Mathf.InverseLerp(maxRPM, peakRPM, rpm);
        }
    }

    public void SetVolume(float volume)
    {
        source.mute = (source.volume = volume) == 0;
    }

}

public class EngineSound : MonoBehaviour
{
    public EngineNote[] engineNotes;
    public Car_Movement carValues;
    public AI_Controls m_AICarValues;
    public float rpm;
    public float masterVolume;

    static float[] workingVolumes = new float[3]; // or maximum number of engine notes you need

    private void Start()
    {
        if (gameObject.tag == "AI")
        {
            m_AICarValues = GetComponent<AI_Controls>();
        }
        else
        {
            carValues = GetComponent<Car_Movement>();
        }
    }
    private void Update()
    {
        float maxVol = 5;
        float progress = Mathf.Sin(Time.deltaTime * 0.02f);
        if (gameObject.tag == "AI")
        {

            rpm = Mathf.Lerp((int)m_AICarValues.engineRPM, maxVol, progress);
            // The total volume calculated for all engine notes won't generally sum to 1.
            // Calculate what they do sum to and then scale the individual volumes to ensure
            // consistent volume across the RPM range.
            float totalVolume = 0f;
            for (int i = 0; i < engineNotes.Length; ++i)
            {
                totalVolume += workingVolumes[i] = engineNotes[i].SetPitchAndGetVolumeForRPM(rpm);
            }

            if (totalVolume > 0f)
            {
                for (int i = 0; i < engineNotes.Length; ++i)
                {
                    engineNotes[i].SetVolume(masterVolume * workingVolumes[i] / totalVolume);
                }
            }
        }
        else
        {
            rpm = Mathf.Lerp((int)carValues.engineRPM, maxVol, progress);
            // The total volume calculated for all engine notes won't generally sum to 1.
            // Calculate what they do sum to and then scale the individual volumes to ensure
            // consistent volume across the RPM range.
            float totalVolume = 0f;
            for (int i = 0; i < engineNotes.Length; ++i)
            {
                totalVolume += workingVolumes[i] = engineNotes[i].SetPitchAndGetVolumeForRPM(rpm);
            }

            if (totalVolume > 0f)
            {
                for (int i = 0; i < engineNotes.Length; ++i)
                {
                    engineNotes[i].SetVolume(masterVolume * workingVolumes[i] / totalVolume);
                }
            }
        }
    }

}