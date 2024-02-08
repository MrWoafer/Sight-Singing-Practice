using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note
{
    public NoteName noteName { get; protected set; }
    public int octave { get; protected set; }

    public NoteLetter noteLetter => noteName.letter;
    public Accidental accidental => noteName.accidental;
    public float frequency => GetFrequency();

    public Note(NoteName noteName, int octave)
    {
        this.noteName = noteName;
        this.octave = octave;
    }

    public static bool operator ==(Note note1, Note note2)
    {
        return note1.noteName == note2.noteName && note1.octave == note2.octave;
    }
    public static bool operator !=(Note note1, Note note2)
    {
        return !(note1 == note2);
    }
    public override bool Equals(object obj)
    {
        if (obj.GetType() == typeof(Note))
        {
            return (Note)obj == this;
        }
        return base.Equals(obj);
    }

    public static bool operator >(Note note1, Note note2)
    {
        if (note1.octave > note2.octave)
        {
            return true;
        }
        else if (note1.octave < note2.octave)
        {
            return false;
        }
        else
        {
            return note1.noteName > note2.noteName;
        }
    }
    public static bool operator <(Note note1, Note note2)
    {
        return note2 > note1;
    }

    public static bool operator >=(Note note1, Note note2)
    {
        return note1 > note2 || note1 == note2;
    }
    public static bool operator <=(Note note1, Note note2)
    {
        return note2 >= note1;
    }

    public static int SemitoneDifference(Note note1, Note note2)
    {
        if (note2 > note1)
        {
            return -SemitoneDifference(note2, note1);
        }

        int diff = 0;
        while (note1.octave > note2.octave)
        {
            diff += 12;
            note1 = note1.NewOctave(note1.octave - 1);
        }

        if (note1 > note2)
        {
            return diff + NoteName.SemitoneDifference(note1.noteName, note2.noteName);
        }
        else
        {
            return diff - NoteName.SemitoneDifference(note2.noteName, note1.noteName);
        }
    }

    public static int LetterDifference(Note note1, Note note2)
    {
        if (note2 > note1)
        {
            return -LetterDifference(note2, note1);
        }

        /*int diff = 0;
        while (note1.octave > note2.octave)
        {
            diff += 7;
            note1 = note1.NewOctave(note1.octave - 1);
        }

        if (note1 > note2)
        {
            return diff + (note1.noteLetter - note2.noteLetter);
        }
        else
        {
            return diff - (note2.noteLetter - note1.noteLetter);
        }*/

        return (note1 - note2).number - 1;
    }

    public static Interval operator -(Note note1, Note note2)
    {
        if (note2 > note1)
        {
            return -(note2 - note1);
        }

        Note tempNote = note1;
        int intervalNumber = 1;
        while (tempNote.octave > note2.octave)
        {
            intervalNumber += 7;
            tempNote = tempNote.NewOctave(tempNote.octave - 1);
        }

        if (tempNote < note2)
        {
            intervalNumber -= 7;
            tempNote = tempNote.NewOctave(tempNote.octave + 1);
        }

        /// e.g. for the case Db5 - D#4, we put the interval up an octave so we get the correct answer of double diminished octave.
        if (tempNote.noteLetter == note2.noteLetter && tempNote.octave != note2.octave)
        {
            intervalNumber += 7;
        }
        else
        {
            intervalNumber += Functions.Mod(note1.noteLetter - note2.noteLetter, 7);
        }

        Interval interval;
        foreach (IntervalQuality quality in Interval.intervalQualities)
        {
            interval = new Interval(quality, intervalNumber);
            if (interval.IsValidInterval() && interval.semitones == SemitoneDifference(note1, note2))
            {
                return interval;
            }
        }

        throw new System.Exception("Could not find the interval between: " + note1 + " and " + note2);
    }

    public static Note operator -(Note note, Interval interval)
    {
        Note result = note;
        for (int i = 1; i < (int)interval.number; i++)
        {
            if (result.noteLetter == NoteLetter.C)
            {
                result = result.NewOctave(result.octave - 1);
            }
            result = result.NewNoteName(new NoteName((NoteLetter)Functions.Mod((int)result.noteLetter - 1, 7), Accidental.natural));
        }

        foreach (Accidental accidental in NoteName.accidentals)
        {
            result = result.NewAccidental(accidental);
            if (SemitoneDifference(note, result) == interval.semitones)
            {
                return result;
            }
        }

        throw new System.Exception("Could not subtract interval " + interval + " from " + note);
    }

    public static Note operator +(Note note, Interval interval)
    {
        Note result = note;
        for (int i = 1; i < (int)interval.number; i++)
        {
            if (result.noteLetter == NoteLetter.B)
            {
                result = result.NewOctave(result.octave + 1);
            }
            result = result.NewNoteName(new NoteName((NoteLetter)Functions.Mod((int)result.noteLetter + 1, 7), Accidental.natural));
        }

        foreach (Accidental accidental in NoteName.accidentals)
        {
            result = result.NewAccidental(accidental);
            if (SemitoneDifference(result, note) == interval.semitones)
            {
                return result;
            }
        }

        throw new System.Exception("Could not add interval " + interval + " to " + note);
    }

    public static implicit operator int(Note note)
    {
        return SemitoneDifference(note, Note.C4);
    }

    public override string ToString()
    {
        return noteName + octave.ToString();
    }

    public bool IsEnharmonicTo(Note note)
    {
        return this - note == 0;
    }
    public static bool AreEnharmonic(Note note1, Note note2)
    {
        return note1.IsEnharmonicTo(note2);
    }

    public bool In(Key key)
    {
        return key.Contains(this);
    }

    private float GetFrequency()
    {
        return 440f * Mathf.Pow(2, SemitoneDifference(this, Note.A4) / 12f);
    }

    public Note NewOctave(int octave)
    {
        return new Note(noteName, octave);
    }
    public Note NewNoteName(NoteName noteName)
    {
        return new Note(noteName, octave);
    }
    public Note NewAccidental(Accidental accidental)
    {
        return NewNoteName(noteName.NewAccidental(accidental));
    }

    public Note Flatten()
    {
        return NewNoteName(noteName.Flatten());
    }
    public Note Sharpen()
    {
        return NewNoteName(noteName.Sharpen());
    }

    public static Note Max(Note note1, Note note2)
    {
        return note1 > note2 ? note1 : note2;
    }
    public static Note Min(Note note1, Note note2)
    {
        return note1 < note2 ? note1 : note2;
    }

    public NoteWithDuration SetDuration(Duration duration)
    {
        return new NoteWithDuration(this, duration);
    }

    ///

    public static readonly Note C2 = new Note(NoteName.C, 2);
    public static readonly Note Csharp2 = new Note(NoteName.Csharp, 2);
    public static readonly Note Db2 = new Note(NoteName.Db, 2);
    public static readonly Note D2 = new Note(NoteName.D, 2);
    public static readonly Note Dsharp2 = new Note(NoteName.Dsharp, 2);
    public static readonly Note Eb2 = new Note(NoteName.Eb, 2);
    public static readonly Note E2 = new Note(NoteName.E, 2);
    public static readonly Note F2 = new Note(NoteName.F, 2);
    public static readonly Note Fsharp2 = new Note(NoteName.Fsharp, 2);
    public static readonly Note Gb2 = new Note(NoteName.Gb, 2);
    public static readonly Note G2 = new Note(NoteName.G, 2);
    public static readonly Note Gsharp2 = new Note(NoteName.Gsharp, 2);
    public static readonly Note Ab2 = new Note(NoteName.Ab, 2);
    public static readonly Note A2 = new Note(NoteName.A, 2);
    public static readonly Note Asharp2 = new Note(NoteName.Asharp, 2);
    public static readonly Note Bb2 = new Note(NoteName.Bb, 2);
    public static readonly Note B2 = new Note(NoteName.B, 2);

    public static readonly Note C3 = new Note(NoteName.C, 3);
    public static readonly Note Csharp3 = new Note(NoteName.Csharp, 3);
    public static readonly Note Db3 = new Note(NoteName.Db, 3);
    public static readonly Note D3 = new Note(NoteName.D, 3);
    public static readonly Note Dsharp3 = new Note(NoteName.Dsharp, 3);
    public static readonly Note Eb3 = new Note(NoteName.Eb, 3);
    public static readonly Note E3 = new Note(NoteName.E, 3);
    public static readonly Note F3 = new Note(NoteName.F, 3);
    public static readonly Note Fsharp3 = new Note(NoteName.Fsharp, 3);
    public static readonly Note Gb3 = new Note(NoteName.Gb, 3);
    public static readonly Note G3 = new Note(NoteName.G, 3);
    public static readonly Note Gsharp3 = new Note(NoteName.Gsharp, 3);
    public static readonly Note Ab3 = new Note(NoteName.Ab, 3);
    public static readonly Note A3 = new Note(NoteName.A, 3);
    public static readonly Note Asharp3 = new Note(NoteName.Asharp, 3);
    public static readonly Note Bb3 = new Note(NoteName.Bb, 3);
    public static readonly Note B3 = new Note(NoteName.B, 3);

    public static readonly Note C4 = new Note(NoteName.C, 4);
    public static readonly Note Csharp4 = new Note(NoteName.Csharp, 4);
    public static readonly Note Db4 = new Note(NoteName.Db, 4);
    public static readonly Note D4 = new Note(NoteName.D, 4);
    public static readonly Note Dsharp4 = new Note(NoteName.Dsharp, 4);
    public static readonly Note Eb4 = new Note(NoteName.Eb, 4);
    public static readonly Note E4 = new Note(NoteName.E, 4);
    public static readonly Note F4 = new Note(NoteName.F, 4);
    public static readonly Note Fsharp4 = new Note(NoteName.Fsharp, 4);
    public static readonly Note Gb4 = new Note(NoteName.Gb, 4);
    public static readonly Note G4 = new Note(NoteName.G, 4);
    public static readonly Note Gsharp4 = new Note(NoteName.Gsharp, 4);
    public static readonly Note Ab4 = new Note(NoteName.Ab, 4);
    public static readonly Note A4 = new Note(NoteName.A, 4);
    public static readonly Note Asharp4 = new Note(NoteName.Asharp, 4);
    public static readonly Note Bb4 = new Note(NoteName.Bb, 4);
    public static readonly Note B4 = new Note(NoteName.B, 4);

    public static readonly Note C5 = new Note(NoteName.C, 5);
    public static readonly Note Csharp5 = new Note(NoteName.Csharp, 5);
    public static readonly Note Db5 = new Note(NoteName.Db, 5);
    public static readonly Note D5 = new Note(NoteName.D, 5);
    public static readonly Note Dsharp5 = new Note(NoteName.Dsharp, 5);
    public static readonly Note Eb5 = new Note(NoteName.Eb, 5);
    public static readonly Note E5 = new Note(NoteName.E, 5);
    public static readonly Note F5 = new Note(NoteName.F, 5);
    public static readonly Note Fsharp5 = new Note(NoteName.Fsharp, 5);
    public static readonly Note Gb5 = new Note(NoteName.Gb, 5);
    public static readonly Note G5 = new Note(NoteName.G, 5);
    public static readonly Note Gsharp5 = new Note(NoteName.Gsharp, 5);
    public static readonly Note Ab5 = new Note(NoteName.Ab, 5);
    public static readonly Note A5 = new Note(NoteName.A, 5);
    public static readonly Note Asharp5 = new Note(NoteName.Asharp, 5);
    public static readonly Note Bb5 = new Note(NoteName.Bb, 5);
    public static readonly Note B5 = new Note(NoteName.B, 5);
}
