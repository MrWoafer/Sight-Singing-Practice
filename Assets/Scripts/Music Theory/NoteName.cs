using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NoteLetter
{
    C = 0,
    D = 1,
    E = 2,
    F = 3,
    G = 4,
    A = 5,
    B = 6
}

public enum Accidental
{
    doubleFlat = -2, bb = -2,
    flat = -1, b = -1,
    natural = 0,
    sharp = 1,
    doubleSharp = 2, x = 2
}

public class NoteName
{
    public readonly NoteLetter letter;
    public readonly Accidental accidental;

    public NoteName(NoteLetter letter, Accidental accidental)
    {
        this.letter = letter;
        this.accidental = accidental;
    }

    public static bool operator ==(NoteName note1, NoteName note2)
    {
        return note1.letter == note2.letter && note1.accidental == note2.accidental;
    }
    public static bool operator !=(NoteName note1, NoteName note2)
    {
        return !(note1 == note2);
    }
    public override bool Equals(object obj)
    {
        if (obj.GetType() == typeof(NoteName))
        {
            return (NoteName)obj == this;
        }
        return base.Equals(obj);
    }

    public static bool operator >(NoteName note1, NoteName note2)
    {
        if (note1.letter > note2.letter)
        {
            return true;
        }
        else if (note1.letter < note2.letter)
        {
            return false;
        }
        else
        {
            if (note1.letter == note2.letter)
            {
                return note1.accidental > note2.accidental;
            }
            else
            {
                return note1.accidental < note2.accidental;
            }
        }
    }
    public static bool operator <(NoteName note1, NoteName note2)
    {
        return note2 > note1;
    }
    public static bool operator >=(NoteName note1, NoteName note2)
    {
        return note1 > note2 || note1 == note2;
    }
    public static bool operator <=(NoteName note1, NoteName note2)
    {
        return note2 >= note1;
    }

    public override string ToString()
    {
        switch (accidental)
        {
            case Accidental.doubleFlat: return letter + "bb";
            case Accidental.flat: return letter + "b";
            case Accidental.natural: return letter + "";
            case Accidental.sharp: return letter + "#";
            case Accidental.doubleSharp: return letter + "x";
            default: throw new System.Exception("Unknown accidental: " + accidental);
        }
    }

    public static implicit operator int(NoteName note)
    {
        int value;
        switch (note.letter)
        {
            case NoteLetter.C: value = 0; break;
            case NoteLetter.D: value = 2; break;
            case NoteLetter.E: value = 4; break;
            case NoteLetter.F: value = 5; break;
            case NoteLetter.G: value = 7; break;
            case NoteLetter.A: value = 9; break;
            case NoteLetter.B: value = 11; break;
            default: throw new System.Exception("Unknown note letter: " + note.letter + ". Full note: " + note);
        }

        if (note.accidental == Accidental.doubleFlat)
        {
            value -= 2;
        }
        else if (note.accidental == Accidental.flat)
        {
            value -= 1;
        }
        else if (note.accidental == Accidental.natural)
        {
            value += 0;
        }
        else if (note.accidental == Accidental.sharp)
        {
            value += 1;
        }
        else if (note.accidental == Accidental.doubleSharp)
        {
            value += 2;
        }
        else
        {
            throw new System.Exception("Unknown accidental: " + note.accidental + ". Full note: " + note);
        }

        return value;
    }

    public static NoteLetter AddToNoteLetter(NoteLetter noteLetter, int amount)
    {
        return (NoteLetter)Functions.Mod((int)noteLetter + amount, 7);
    }

    public static int operator +(NoteName note1, NoteName note2)
    {
        return Functions.Mod((int)note1 + (int)note2, 12);
    }

    public static NoteName operator +(NoteName note, Interval interval)
    {
        return (new Note(note, 4) + interval).noteName;
    }

    public static Interval operator -(NoteName note1, NoteName note2)
    {
        if (note1 > note2)
        {
            return new Note(note1, 4) - new Note(note2, 4);
        }
        else
        {
            return Interval.Invert(new Note(note2, 4) - new Note(note1, 4));
        }
    }

    public static NoteName operator -(NoteName note, Interval interval)
    {
        return (new Note(note, 4) - interval).noteName;
    }

    public static int SemitoneDifference(NoteName note1, NoteName note2)
    {
        return (int)note1 - (int)note2;
    }

    public bool IsEnharmonicTo(NoteName note)
    {
        return (int)this == (int)note;
    }
    public static bool AreEnharmonic(NoteName note1, NoteName note2)
    {
        return note1.IsEnharmonicTo(note2);
    }

    public bool In(Key key)
    {
        return key.Contains(this);
    }

    public NoteName Natural()
    {
        return new NoteName(letter, Accidental.natural);
    }
    public NoteName Flatten()
    {
        switch (accidental)
        {
            case Accidental.doubleSharp: return NewAccidental(Accidental.sharp);
            case Accidental.sharp: return NewAccidental(Accidental.natural);
            case Accidental.natural: return NewAccidental(Accidental.flat);
            case Accidental.flat: return NewAccidental(Accidental.doubleSharp);
            case Accidental.doubleFlat: throw new System.Exception("Cannot flatten a double flat note. Note: " + this);
            default: throw new System.Exception("Unknown accidental: " + accidental);
        }
    }
    public NoteName Sharpen()
    {
        switch (accidental)
        {
            case Accidental.doubleSharp: throw new System.Exception("Cannot sharpen a double sharp note. Note: " + this);
            case Accidental.sharp: return NewAccidental(Accidental.doubleSharp);
            case Accidental.natural: return NewAccidental(Accidental.sharp);
            case Accidental.flat: return NewAccidental(Accidental.natural);
            case Accidental.doubleFlat: return NewAccidental(Accidental.flat);
            default: throw new System.Exception("Unknown accidental: " + accidental);
        }
    }

    public NoteName NewAccidental(Accidental accidental)
    {
        return new NoteName(letter, accidental);
    }

    ///

    public static readonly NoteName Bsharp = new NoteName(NoteLetter.B, Accidental.sharp);
    public static readonly NoteName C = new NoteName(NoteLetter.C, Accidental.natural);

    public static readonly NoteName Csharp = new NoteName(NoteLetter.C, Accidental.sharp);
    public static readonly NoteName Db = new NoteName(NoteLetter.D, Accidental.flat);

    public static readonly NoteName D = new NoteName(NoteLetter.D, Accidental.natural);

    public static readonly NoteName Dsharp = new NoteName(NoteLetter.D, Accidental.sharp);
    public static readonly NoteName Eb = new NoteName(NoteLetter.E, Accidental.flat);

    public static readonly NoteName E = new NoteName(NoteLetter.E, Accidental.natural);
    public static readonly NoteName Fb = new NoteName(NoteLetter.F, Accidental.flat);

    public static readonly NoteName Esharp = new NoteName(NoteLetter.E, Accidental.sharp);
    public static readonly NoteName F = new NoteName(NoteLetter.F, Accidental.natural);

    public static readonly NoteName Fsharp = new NoteName(NoteLetter.F, Accidental.sharp);
    public static readonly NoteName Gb = new NoteName(NoteLetter.G, Accidental.flat);

    public static readonly NoteName G = new NoteName(NoteLetter.G, Accidental.natural);

    public static readonly NoteName Gsharp = new NoteName(NoteLetter.G, Accidental.sharp);
    public static readonly NoteName Ab = new NoteName(NoteLetter.A, Accidental.flat);

    public static readonly NoteName A = new NoteName(NoteLetter.A, Accidental.natural);

    public static readonly NoteName Asharp = new NoteName(NoteLetter.A, Accidental.sharp);
    public static readonly NoteName Bb = new NoteName(NoteLetter.B, Accidental.flat);

    public static readonly NoteName B = new NoteName(NoteLetter.B, Accidental.natural);
    public static readonly NoteName Cb = new NoteName(NoteLetter.C, Accidental.flat);

    public static readonly Accidental[] accidentals = new Accidental[] { Accidental.doubleFlat, Accidental.flat, Accidental.natural, Accidental.sharp, Accidental.doubleSharp };
}
