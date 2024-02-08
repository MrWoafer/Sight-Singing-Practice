using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MIDIInstrument
{
    sine,
    piano
}

public class AudioManager : MonoBehaviour
{
    [Header("Settings")]
    [Range(0f, 1f)]
    public float volume = 1f;
    public MIDIInstrument instrument = MIDIInstrument.sine;
    [Min(1000)]
    public int sampleRate = 44100;

    [Header("References")]
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private Slider volumeSlider;

    private int sinePosition = 0;

    private Coroutine playPitchCoroutine;
    private Coroutine playPitchesCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0;
        audioSource.Stop();

        volumeSlider.value = volume;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayPitch(Notes.A4, 1f);
        }*/
    }

    private AudioClip CreateSinePitch(Note note, float duration)
    {
        sinePosition = 0;
        int numOfSamples = Mathf.RoundToInt(duration * sampleRate);
        AudioClip audioClip = AudioClip.Create("sine pitch", numOfSamples, 1, sampleRate, true, (data) => OnSineAudioRead(data, note.frequency), OnSineAudioSetPosition);

        return audioClip;
    }

    void OnSineAudioRead(float[] data, float frequency)
    {
        int count = 0;
        while (count < data.Length)
        {
            data[count] = Mathf.Sin(2f * Mathf.PI * frequency * sinePosition / sampleRate);
            sinePosition++;
            count++;
        }
    }
    void OnSineAudioSetPosition(int newPosition)
    {
        sinePosition = newPosition;
    }

    public void PlayPitch(NoteWithDuration note, float bpm, TimeSignature timeSignature)
    {
        PlayPitchForSeconds((Note)note, note.GetSeconds(bpm, timeSignature));
    }
    public void PlayPitchForSeconds(Note note, float duration)
    {
        audioSource.Stop();
        if (playPitchCoroutine != null)
        {
            StopCoroutine(playPitchCoroutine);
        }

        if (instrument == MIDIInstrument.sine)
        {
            AudioClip audioClip = CreateSinePitch(note, 10f);
            audioSource.clip = audioClip;
        }
        else
        {
            throw new System.Exception("Unimplemented instrument: " + instrument);
        }

        audioSource.volume = volume * volume * volume;

        audioSource.Play();
        playPitchCoroutine = StartCoroutine(PlayPitchCoroutine(duration));
    }

    IEnumerator PlayPitchCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        audioSource.Stop();
        yield return null;
    }

    public void PlayPitches(List<NoteWithDuration> notes, float bpm, TimeSignature timeSignature)
    {
        PlayPitches(notes.ToArray(), bpm, timeSignature);
    }
    public void PlayPitches(NoteWithDuration[] notes, float bpm, TimeSignature timeSignature)
    {
        if (playPitchesCoroutine != null)
        {
            StopCoroutine(playPitchesCoroutine);
        }
        
        playPitchesCoroutine = StartCoroutine(PlayPitchesCoroutine(notes, bpm, timeSignature));
    }

    IEnumerator PlayPitchesCoroutine(NoteWithDuration[] notes, float bpm, TimeSignature timeSignature)
    {
        for (int i = 0; i < notes.Length; i++)
        {
            while (audioSource.isPlaying && i > 0)
            {
                yield return null;
            }
            PlayPitch(notes[i], bpm, timeSignature);
        }
        yield return null;
    }

    public void SetVolume(float volume)
    {
        this.volume = volume;
    }

    public void StopAll()
    {
        if (playPitchCoroutine != null)
        {
            StopCoroutine(playPitchCoroutine);
        }
        if (playPitchesCoroutine != null)
        {
            StopCoroutine(playPitchesCoroutine);
        }

        audioSource.Stop();
    }
}
