using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteWithDuration : Note
{
    public Duration duration { get; protected set; }

    public NoteWithDuration(NoteName noteName, int octave, Duration duration) : base(noteName, octave)
    {
        this.duration = duration;
    }
    public NoteWithDuration(Note note, Duration duration) : this(note.noteName, note.octave, duration) { }

    public static bool operator ==(NoteWithDuration note1, NoteWithDuration note2)
    {
        return (Note)note1 == (Note)note2 && note1.duration == note2.duration;
    }
    public static bool operator !=(NoteWithDuration note1, NoteWithDuration note2)
    {
        return !(note1 == note2);
    }
    public override bool Equals(object obj)
    {
        if (obj.GetType() == typeof(NoteWithDuration))
        {
            return (NoteWithDuration)obj == this;
        }
        return base.Equals(obj);
    }

    public static NoteWithDuration operator +(NoteWithDuration note, Interval interval)
    {
        return new NoteWithDuration((Note)note + interval, note.duration);
    }

    public float GetSeconds(float bpm, TimeSignature timeSignature)
    {
        return 60f / bpm * duration.GetBeats(timeSignature);
    }
}
