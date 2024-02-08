using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Mode
{
    unknown = -1,

    major = 0, ionian = 0,
    dorian = 1,
    phrygian = 2,
    lydian = 3,
    mixolydian = 4,
    minor = 5, aeolian = 5,
    locrian = 6,

    harmonicMinor = 7,
    melodicMinor = 8,

    majorPentatonic = 9,
    minorPentatonic = 10
}

public class Key : IEnumerable
{
    public NoteName tonic { get; private set; }
    public Mode mode { get; private set; }

    public NoteName[] notes { get; private set; }
    public int size => notes.Length;

    public Key relativeMajor => Relative(Mode.major);
    public Key relativeMinor => Relative(Mode.minor);
    public Key parallelMajor => Parallel(Mode.major);
    public Key parallelMinor => Parallel(Mode.minor);

    public Key(NoteName tonic, Mode mode)
    {
        this.tonic = tonic;
        this.mode = mode;
        SetUpNotes();
    }

    private void SetUpNotes()
    {
        notes = new NoteName[7];
        notes[0] = tonic;

        for (int i = 1; i < size; i++)
        {
            NoteLetter noteLetter = NoteName.AddToNoteLetter(tonic.letter, i);

            if (new NoteName(noteLetter, Accidental.natural) - tonic == GetSemitonesFromTonic()[i - 1])
            {
                notes[i] = new NoteName(noteLetter, Accidental.natural);
            }
            else if (new NoteName(noteLetter, Accidental.flat) - tonic == GetSemitonesFromTonic()[i - 1])
            {
                notes[i] = new NoteName(noteLetter, Accidental.flat);
            }
            else if (new NoteName(noteLetter, Accidental.sharp) - tonic == GetSemitonesFromTonic()[i - 1])
            {
                notes[i] = new NoteName(noteLetter, Accidental.sharp);
            }
            else if (new NoteName(noteLetter, Accidental.doubleFlat) - tonic == GetSemitonesFromTonic()[i - 1])
            {
                notes[i] = new NoteName(noteLetter, Accidental.doubleFlat);
            }
            else if (new NoteName(noteLetter, Accidental.doubleSharp) - tonic == GetSemitonesFromTonic()[i - 1])
            {
                notes[i] = new NoteName(noteLetter, Accidental.doubleSharp);
            }
            else
            {
                throw new System.Exception("Something went wrong setting up " + this);
            }
        }
    }

    private int[] GetSemitonesFromTonic()
    {
        switch (mode)
        {
            case Mode.major: return new int[] { 2, 4, 5, 7, 9, 11 };
            case Mode.minor: return new int[] { 2, 3, 5, 7, 8, 10 };
            default: throw new System.Exception("Unimplemented mode: " + mode);
        }
    }

    public NoteName this[int i]
    {
        get
        {
            i = Functions.Mod(i, size);
            return notes[i];
        }
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return (IEnumerator)GetEnumerator();
    }
    public KeyIEnumerator GetEnumerator()
    {
        return new KeyIEnumerator(this);
    }
    public bool Contains(NoteName note)
    {
        foreach (NoteName n in notes)
        {
            if (n == note)
            {
                return true;
            }
        }
        return false;
    }
    public bool Contains(Note note)
    {
        return Contains(note.noteName);
    }

    public NoteName this[ScaleDegree scaleDegree]
    {
        get
        {
            NoteName note = this[Functions.Mod(scaleDegree.number - 1, size)];
            if (scaleDegree.accidental == Accidental.natural)
            {

            }
            else if (scaleDegree.accidental == Accidental.flat)
            {
                note = note.Flatten();
            }
            else if (scaleDegree.accidental == Accidental.sharp)
            {
                note = note.Sharpen();
            }
            else
            {
                throw new System.Exception("Invalid accidental for scale degree: " + scaleDegree.accidental + ". Scale degree: " + scaleDegree);
            }
            return note;
        }
    }

    public NoteName this[NoteLetter noteLetter]
    {
        get
        {
            return GetDiatonicNote(noteLetter);
        }
    }
    public NoteName GetDiatonicNote(NoteLetter noteLetter)
    {
        foreach (Accidental accidental in NoteName.accidentals)
        {
            NoteName note = new NoteName(noteLetter, accidental);
            if (Contains(note))
            {
                return note;
            }
        }

        throw new System.Exception("Couldn't find accidental for " + noteLetter + " in key of " + this);
    }

    public ScaleDegree this[NoteName noteName]
    {
        get
        {
            return ScaleDegreeOf(noteName);
        }
    }

    public Key this[Mode mode]
    {
        get
        {
            return Relative(mode);
        }
    }

    public override string ToString()
    {
        return tonic + " " + Functions.ToUpperFirstLetter(mode.ToString());
    }

    public static bool operator ==(Key key1, Key key2)
    {
        if (key1.size != key2.size)
        {
            return false;
        }
        for (int i = 0; i < key1.size; i++)
        {
            if (key1[i] != key2[i])
            {
                return false;
            }
        }
        return true;
    }
    public static bool operator !=(Key key1, Key key2)
    {
        return !(key1 == key2);
    }
    public override bool Equals(object obj)
    {
        if (obj.GetType() == typeof(Key))
        {
            return (Key)obj == this;
        }
        return base.Equals(obj);
    }

    public bool IsModeOf(Key key)
    {
        if (size != key.size)
        {
            return false;
        }
        foreach (NoteName note in this)
        {
            if (!key.Contains(note))
            {
                return false;
            }
        }
        return true;
    }
    public static bool AreModes(Key key1, Key key2)
    {
        return key1.IsModeOf(key2);
    }

    public bool IsEnharmonicTo(Key key)
    {
        if (size != key.size)
        {
            return false;
        }
        for (int i = 0; i < size; i++)
        {
            if (!NoteName.AreEnharmonic(this[i], key[i]))
            {
                return false;
            }
        }
        return true;
    }
    public static bool AreEnharmonic(Key key1, Key key2)
    {
        return key1.IsEnharmonicTo(key2);
    }

    public ScaleDegree ScaleDegreeOf(Note note)
    {
        return ScaleDegreeOf(note.noteName);
    }
    public ScaleDegree ScaleDegreeOf(NoteName note)
    {
        if (mode == Mode.major)
        {
            for (int i = 1; i <= size; i++)
            {
                if (this[i - 1] == note)
                {
                    return new ScaleDegree(i, Accidental.natural);
                }
            }
            for (int i = 1; i <= size; i++)
            {
                if (this[i - 1].Flatten() == note)
                {
                    return new ScaleDegree(i, Accidental.flat);
                }
                if (this[i - 1].Sharpen() == note)
                {
                    return new ScaleDegree(i, Accidental.sharp);
                }
            }

            throw new System.Exception("Could not find a scale degree for " + note + " in " + this);
        }
        else
        {
            return parallelMajor.ScaleDegreeOf(note);
        }
    }

    public ScaleDegree[] ScaleDegreesOf(Note[] notes)
    {
        NoteName[] NoteName = new NoteName[notes.Length];
        for (int i = 0; i < notes.Length; i++)
        {
            NoteName[i] = notes[i].noteName;
        }

        return ScaleDegreesOf(NoteName);
    }
    public ScaleDegree[] ScaleDegreesOf(List<Note> notes)
    {
        NoteName[] NoteName = new NoteName[notes.Count];
        for (int i = 0; i < notes.Count; i++)
        {
            NoteName[i] = notes[i].noteName;
        }

        return ScaleDegreesOf(NoteName);
    }
    public ScaleDegree[] ScaleDegreesOf(NoteWithDuration[] notes)
    {
        NoteName[] NoteName = new NoteName[notes.Length];
        for (int i = 0; i < notes.Length; i++)
        {
            NoteName[i] = notes[i].noteName;
        }

        return ScaleDegreesOf(NoteName);
    }
    public ScaleDegree[] ScaleDegreesOf(List<NoteWithDuration> notes)
    {
        NoteName[] NoteName = new NoteName[notes.Count];
        for (int i = 0; i < notes.Count; i++)
        {
            NoteName[i] = notes[i].noteName;
        }

        return ScaleDegreesOf(NoteName);
    }
    public ScaleDegree[] ScaleDegreesOf(NoteName[] notes)
    {
        ScaleDegree[] scaleDegrees = new ScaleDegree[notes.Length];
        for (int i = 0; i < notes.Length; i++)
        {
            scaleDegrees[i] = ScaleDegreeOf(notes[i]);
        }

        return scaleDegrees;
    }

    public NoteName AddIntervalNumber(NoteName note, int intervalNumber)
    {
        //ScaleDegree scaleDegree = ScaleDegreeOf(note);
        if (intervalNumber == 0)
        {
            throw new System.Exception("Cannot have an interval number 0");
        }
        else if (intervalNumber > 0)
        {
            //return this[new ScaleDegree(Functions.Mod(scaleDegree.number - 1 + intervalNumber - 1, size) + 1, scaleDegree.accidental)];
            return this[ScaleDegreeOf(this[note.letter]).number - 1 + intervalNumber - 1] - (this[note.letter] - note);
        }
        else
        {
            //return this[new ScaleDegree(Functions.Mod(scaleDegree.number - 1 + intervalNumber + 1, size) + 1, scaleDegree.accidental)];
            return this[ScaleDegreeOf(this[note.letter]).number - 1 + intervalNumber + 1] - (this[note.letter] - note);
        }
    }
    public Note AddIntervalNumber(Note note, int intervalNumber)
    {
        if (intervalNumber == 0)
        {
            throw new System.Exception("Cannot have an interval number 0");
        }
        if (intervalNumber > 0)
        {
            return new Note(AddIntervalNumber(note.noteName, intervalNumber), note.octave + Mathf.FloorToInt(((int)note.noteLetter + intervalNumber - 1) / 7f));
        }
        else
        {
            return new Note(AddIntervalNumber(note.noteName, intervalNumber), note.octave + Mathf.FloorToInt(((int)note.noteLetter + intervalNumber + 1) / 7f));
        }
    }

    public bool IsModeOfMajor()
    {
        return (int)mode >= 0 && (int)mode <= 6;
    }

    public Key Relative(Mode mode)
    {
        if (IsModeOfMajor())
        {
            return new Key(this[Functions.Mod(mode - this.mode, 7)], mode);
        }
        else
        {
            throw new System.Exception("Cannot get the relative " + mode + " of " + this.mode);
        }
    }

    public Key Parallel(Mode mode)
    {
        return new Key(tonic, mode);
    }

    public Note[] Scale(int startingOctave, bool includeTonicOctaveUp = true)
    {
        Note[] scale = new Note[size + (includeTonicOctaveUp ? 1 : 0)];
        for (int i = 0; i < size + (includeTonicOctaveUp ? 1 : 0); i++)
        {
            scale[i] = AddIntervalNumber(new Note(tonic, startingOctave), i + 1);
        }
        return scale;
    }

    public NoteWithDuration[] Scale(int startingOctave, Duration duration, bool includeTonicOctaveUp = true)
    {
        Note[] scale = Scale(startingOctave, includeTonicOctaveUp);
        NoteWithDuration[] scaleWithDuration = new NoteWithDuration[scale.Length];
        for (int i = 0; i < scale.Length; i++)
        {
            scaleWithDuration[i] = scale[i].SetDuration(duration);
        }
        return scaleWithDuration;
    }

    ///

    public static readonly Key Cbmajor = new Key(NoteName.Cb, Mode.major);
    public static readonly Key Cbminor = new Key(NoteName.Cb, Mode.minor);

    public static readonly Key Cmajor = new Key(NoteName.C, Mode.major);
    public static readonly Key Cminor = new Key(NoteName.C, Mode.minor);

    public static readonly Key CsharpMajor = new Key(NoteName.Csharp, Mode.major);
    public static readonly Key CsharpMinor = new Key(NoteName.Csharp, Mode.minor);

    public static readonly Key Dbmajor = new Key(NoteName.Db, Mode.major);
    public static readonly Key Dbminor = new Key(NoteName.Db, Mode.minor);

    public static readonly Key Dmajor = new Key(NoteName.D, Mode.major);
    public static readonly Key Dminor = new Key(NoteName.D, Mode.minor);

    public static readonly Key Ebmajor = new Key(NoteName.Eb, Mode.major);
    public static readonly Key Ebminor = new Key(NoteName.Eb, Mode.minor);

    public static readonly Key Emajor = new Key(NoteName.E, Mode.major);
    public static readonly Key Eminor = new Key(NoteName.E, Mode.minor);

    public static readonly Key Fmajor = new Key(NoteName.F, Mode.major);
    public static readonly Key Fminor = new Key(NoteName.F, Mode.minor);

    public static readonly Key FsharpMajor = new Key(NoteName.Fsharp, Mode.major);
    public static readonly Key FsharpMinor = new Key(NoteName.Fsharp, Mode.minor);

    public static readonly Key Gbmajor = new Key(NoteName.Gb, Mode.major);
    public static readonly Key Gbminor = new Key(NoteName.Gb, Mode.minor);

    public static readonly Key Gmajor = new Key(NoteName.G, Mode.major);
    public static readonly Key Gminor = new Key(NoteName.G, Mode.minor);

    public static readonly Key Abmajor = new Key(NoteName.Ab, Mode.major);
    public static readonly Key Abminor = new Key(NoteName.Ab, Mode.minor);

    public static readonly Key Amajor = new Key(NoteName.A, Mode.major);
    public static readonly Key Aminor = new Key(NoteName.A, Mode.minor);

    public static readonly Key Bbmajor = new Key(NoteName.Bb, Mode.major);
    public static readonly Key Bbminor = new Key(NoteName.Bb, Mode.minor);

    public static readonly Key Bmajor = new Key(NoteName.B, Mode.major);
    public static readonly Key Bminor = new Key(NoteName.B, Mode.minor);
}

public class KeyIEnumerator : IEnumerator
{
    public NoteName[] notes;

    int position = -1;

    public KeyIEnumerator(Key key)
    {
        notes = key.notes;
    }

    public bool MoveNext()
    {
        position++;
        return (position < notes.Length);
    }

    public void Reset()
    {
        position = -1;
    }

    object IEnumerator.Current
    {
        get
        {
            return Current;
        }
    }

    public NoteName Current
    {
        get
        {
            try
            {
                return notes[position];
            }
            catch (System.IndexOutOfRangeException)
            {
                throw new System.InvalidOperationException();
            }
        }
    }
}