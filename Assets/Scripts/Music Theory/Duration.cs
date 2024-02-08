using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Duration
{
    public float beatsIn4_4 { get; private set; }

    public bool isDotted => IsDotted();

    public Duration(float beatsIn4_4)
    {
        if (beatsIn4_4 <= 0f)
        {
            throw new System.Exception("Cannot have a non-positive beat number: " + beatsIn4_4);
        }

        this.beatsIn4_4 = beatsIn4_4;
    }
    public Duration(float beats, TimeSignature timeSignature)
    {
        if (beats <= 0f)
        {
            throw new System.Exception("Cannot have a non-positive beat number: " + beatsIn4_4);
        }

        beatsIn4_4 = beats * timeSignature.tempoUnit.beatsIn4_4;
    }
    public Duration(string duration)
    {
        switch (duration.ToLower())
        {
            case "semiquaver": beatsIn4_4 = 0.25f; break;
            case "sixteenth": beatsIn4_4 = 0.25f; break;

            case "quaver": beatsIn4_4 = 0.5f; break;
            case "eighth": beatsIn4_4 = 0.5f; break;

            case "dotted quaver": beatsIn4_4 = 0.75f; break;
            case "dotted eighth": beatsIn4_4 = 0.75f; break;

            case "crotchet": beatsIn4_4 = 1f; break;
            case "quarter": beatsIn4_4 = 1f; break;

            case "dotted crotchet": beatsIn4_4 = 1.5f; break;
            case "dotted quarter": beatsIn4_4 = 1.5f; break;

            case "minim": beatsIn4_4 = 2f; break;
            case "half": beatsIn4_4 = 2f; break;

            case "dotted minim": beatsIn4_4 = 3f; break;
            case "dotted half": beatsIn4_4 = 3f; break;

            case "semibreve": beatsIn4_4 = 4f; break;
            case "whole": beatsIn4_4 = 4f; break;

            case "breve": beatsIn4_4 = 8f; break;
            case "double whole": beatsIn4_4 = 8f; break;

            default: throw new System.Exception("Invalid duration: " + duration);
        }
    }

    public static bool operator ==(Duration duration1, Duration duration2)
    {
        return duration1.beatsIn4_4 == duration2.beatsIn4_4;
    }
    public static bool operator !=(Duration duration1, Duration duration2)
    {
        return !(duration1 == duration2);
    }
    public override bool Equals(object obj)
    {
        if (obj.GetType() == typeof(Duration))
        {
            return (Duration)obj == this;
        }
        else if (obj.GetType() == typeof(float))
        {
            return (float)obj == this;
        }
        return base.Equals(obj);
    }

    public static bool operator ==(Duration duration1, float duration2)
    {
        return duration1.beatsIn4_4 == duration2;
    }
    public static bool operator !=(Duration duration1, float duration2)
    {
        return !(duration1 == duration2);
    }
    public static bool operator ==(float duration1, Duration duration2)
    {
        return duration1 == duration2.beatsIn4_4;
    }
    public static bool operator !=(float duration1, Duration duration2)
    {
        return !(duration1 == duration2);
    }

    public static bool operator >(Duration duration1, Duration duration2)
    {
        return duration1.beatsIn4_4 > duration2.beatsIn4_4;
    }
    public static bool operator <(Duration duration1, Duration duration2)
    {
        return duration2 > duration1;
    }
    public static bool operator >=(Duration duration1, Duration duration2)
    {
        return duration1 == duration2 || duration1 > duration2;
    }
    public static bool operator <=(Duration duration1, Duration duration2)
    {
        return duration1 == duration2 || duration1 < duration2;
    }

    public static Duration operator +(Duration duration1, Duration duration2)
    {
        return new Duration(duration1.beatsIn4_4 + duration2.beatsIn4_4);
    }
    public static Duration operator -(Duration duration1, Duration duration2)
    {
        if (duration1 > duration2)
        {
            return new Duration(duration1.beatsIn4_4 - duration2.beatsIn4_4);
        }
        else
        {
            throw new System.Exception("duration1 must be > duration 2. duration1: " + duration1 + ", duration2: " + duration2);
        }
    }
    public static float operator /(Duration duration1, Duration duration2)
    {
        return duration1.beatsIn4_4 / duration2.beatsIn4_4;
    }

    public static Duration operator *(Duration duration, float scalar)
    {
        if (scalar > 0)
        {
            return new Duration(duration.beatsIn4_4 * scalar);
        }
        else
        {
            throw new System.Exception("Cannot scale durations by non-positive amounts: " + scalar);
        }
    }
    public static Duration operator *(float scalar, Duration duration)
    {
        return duration * scalar;
    }
    public static Duration operator /(Duration duration, float scalar)
    {
        if (scalar > 0)
        {
            return new Duration(duration.beatsIn4_4 / scalar);
        }
        else
        {
            throw new System.Exception("Cannot scale durations by non-positive amounts: " + scalar);
        }
    }

    public override string ToString()
    {
        return beatsIn4_4.ToString();
    }

    public float GetBeats(TimeSignature timeSignature)
    {
        return beatsIn4_4 / timeSignature.tempoUnit.beatsIn4_4;
    }

    private bool IsDotted()
    {
        return new float[] { 0.75f, 1.5f, 3f }.Contains(beatsIn4_4);
    }

    public Duration MakeDotted()
    {
        if (!isDotted)
        {
            return new Duration(beatsIn4_4 * 1.5f);
        }
        else
        {
            return this;
        }
    }
    public Duration MakeUndotted()
    {
        if (isDotted)
        {
            return new Duration(beatsIn4_4 / 1.5f);
        }
        else
        {
            return this;
        }
    }

    ///

    public static readonly Duration semiquaver = new Duration("semiquaver");
    public static readonly Duration sixteenth = new Duration("sixteenth");

    public static readonly Duration quaver = new Duration("quaver");
    public static readonly Duration eighth = new Duration("eighth");

    public static readonly Duration dottedQuaver = new Duration("dotted quaver");
    public static readonly Duration dottedEighth = new Duration("dotted eighth");

    public static readonly Duration crotchet = new Duration("crotchet");
    public static readonly Duration quarter = new Duration("quarter");

    public static readonly Duration dottedCrotchet = new Duration("dotted crotchet");
    public static readonly Duration dottedQuarter = new Duration("dotted quarter");

    public static readonly Duration minim = new Duration("minim");
    public static readonly Duration half = new Duration("half");

    public static readonly Duration dottedMinim = new Duration("dotted minim");
    public static readonly Duration dottedHalf = new Duration("dotted half");

    public static readonly Duration semibreve = new Duration("semibreve");
    public static readonly Duration whole = new Duration("whole");

    public static readonly Duration breve = new Duration("breve");
    public static readonly Duration doubleWhole = new Duration("double whole");
}
