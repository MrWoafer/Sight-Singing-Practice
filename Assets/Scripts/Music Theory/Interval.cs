using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IntervalNumber
{
    unison = 0,
    second = 1,
    third = 2,
    fourth = 3,
    fifth = 4,
    sixth = 5,
    seventh = 6,
    octave = 7,
    ninth = 8,
    tenth = 9,
    eleventh = 10,
    twelfth = 11,
    thirteenth = 12,
    fourteenth = 13,
    doubleOctave = 14
}

public enum IntervalQuality
{
    doubleDiminished = -3,
    diminished = -2,
    minor = -1,
    perfect = 0,
    major = 1,
    augmented = 2,
    doubleAugmented = 3,
}

public class Interval
{
    public IntervalQuality quality { get; private set; }
    public int number { get; private set; }

    public int semitones => GetSemitones();
    public bool isAscending => number > 0;
    public bool isDescending => number < 0;

    public Interval(IntervalQuality quality, IntervalNumber number)
    {
        this.quality = quality;
        this.number = (int)number + 1;
    }
    public Interval(IntervalQuality quality, int number)
    {
        this.quality = quality;
        this.number = number;
    }

    public bool IsValidInterval()
    {
        if (number == 0)
        {
            return false;
        }

        bool perfectIntervalNumber = false;

        int n = Mathf.Abs(number);
        if (Functions.ModCount1(n, 7) == 1 || Functions.ModCount1(n, 7) == 4 || Functions.ModCount1(n, 7) == 5)
        {
            perfectIntervalNumber = true;
        }

        if (quality == IntervalQuality.diminished)
        {
            if (n == 1)
            {
                return false;
            }
        }
        else if (quality == IntervalQuality.minor)
        {
            if (perfectIntervalNumber)
            {
                return false;
            }
        }
        else if (quality == IntervalQuality.major)
        {
            if (perfectIntervalNumber)
            {
                return false;
            }
        }
        else if (quality == IntervalQuality.perfect)
        {
            if (!perfectIntervalNumber)
            {
                return false;
            }
        }
        else if (quality == IntervalQuality.augmented)
        {

        }
        else if (quality == IntervalQuality.doubleDiminished)
        {
            if (n == 1)
            {
                return false;
            }
        }
        else if (quality == IntervalQuality.doubleAugmented)
        {

        }
        else
        {
            throw new System.Exception("Unknown interval quality: " + quality);
        }

        return true;
    }
    public static bool IsValidInterval(Interval interval)
    {
        return interval.IsValidInterval();
    }

    public static bool operator ==(Interval interval1, Interval interval2)
    {
        return interval1.quality == interval2.quality && interval1.number == interval2.number;
    }
    public static bool operator !=(Interval interval1, Interval interval2)
    {
        return !(interval1 == interval2);
    }
    public override bool Equals(object obj)
    {
        if (obj.GetType() == typeof(Interval))
        {
            return (Interval)obj == this;
        }
        return base.Equals(obj);
    }

    public static Interval operator -(Interval interval)
    {
        return new Interval(interval.quality, -interval.number);
    }

    public static Interval operator +(Interval interval, int shift)
    {
        return new Interval(interval.quality, interval.number + shift);
    }
    public static Interval operator +(int shift, Interval interval)
    {
        return interval + shift;
    }
    public static Interval operator -(Interval interval, int shift)
    {
        return interval + (-shift);
    }

    public static Interval operator *(Interval interval, int scalar)
    {
        return new Interval(interval.quality, interval.number * scalar - scalar + 1);
    }
    public static Interval operator *(int scalar, Interval interval)
    {
        return interval * scalar;
    }

    public override string ToString()
    {
        string str = "";

        if (number < 0)
        {
            str = "-";
        }

        switch (quality)
        {
            case IntervalQuality.perfect: return str + "P" + Mathf.Abs(number);
            case IntervalQuality.major: return str + "M" + Mathf.Abs(number);
            case IntervalQuality.minor: return str + "m" + Mathf.Abs(number);
            case IntervalQuality.diminished: return str + "d" + Mathf.Abs(number);
            case IntervalQuality.augmented: return str + "A" + Mathf.Abs(number);
            case IntervalQuality.doubleDiminished: return str + "dd" + Mathf.Abs(number);
            case IntervalQuality.doubleAugmented: return str + "AA" + Mathf.Abs(number);
            default: throw new System.Exception("Unknown interval quality: " + quality);
        }
    }

    public static implicit operator int(Interval interval)
    {
        return interval.semitones;
    }

    public bool IsEnharmonicTo(Interval interval)
    {
        return semitones == interval.semitones;
    }
    public static bool AreEnharmonic(Interval interval1, Interval interval2)
    {
        return interval1.IsEnharmonicTo(interval2);
    }

    public int GetSemitones()
    {
        if (!IsValidInterval(this))
        {
            throw new System.Exception("Invalid interval: " + this.ToString());
        }

        int semitoneValue = 12 * Mathf.FloorToInt((number - 1) / 7f);
        bool perfectIntervalNumber = false;

        if (Functions.ModCount1(number, 7) == 1)
        {
            semitoneValue += 0;
            perfectIntervalNumber = true;
        }
        else if (Functions.ModCount1(number, 7) == 2)
        {
            semitoneValue += 2;
        }
        else if (Functions.ModCount1(number, 7) == 3)
        {
            semitoneValue += 4;
        }
        else if (Functions.ModCount1(number, 7) == 4)
        {
            semitoneValue += 5;
            perfectIntervalNumber = true;
        }
        else if (Functions.ModCount1(number, 7) == 5)
        {
            semitoneValue += 7;
            perfectIntervalNumber = true;
        }
        else if (Functions.ModCount1(number, 7) == 6)
        {
            semitoneValue += 9;
        }
        else if (Functions.ModCount1(number, 7) == 7)
        {
            semitoneValue += 11;
        }
        else
        {
            throw new System.Exception("Unimplemented interval number: " + number);
        }

        if (quality == IntervalQuality.diminished)
        {
            semitoneValue -= perfectIntervalNumber ? 1 : 2;
            if (number == 1)
            {
                throw new System.Exception("Something's gone wrong. We shouldn't have a diminished unison");
            }
        }
        else if (quality == IntervalQuality.minor)
        {
            if (perfectIntervalNumber)
            {
                throw new System.Exception("Something's gone wrong. We shouldn't have a minor " + number);
            }
            semitoneValue -= 1;
        }
        else if (quality == IntervalQuality.major)
        {
            if (perfectIntervalNumber)
            {
                throw new System.Exception("Something's gone wrong. We shouldn't have a major " + number);
            }
        }
        else if (quality == IntervalQuality.augmented)
        {
            semitoneValue += 1;
        }
        else if (quality == IntervalQuality.doubleDiminished)
        {
            semitoneValue -= perfectIntervalNumber ? 2 : 3;
            if (number == 1)
            {
                throw new System.Exception("Something's gone wrong. We shouldn't have a double diminished unison");
            }
        }
        else if (quality == IntervalQuality.doubleAugmented)
        {
            semitoneValue += 2;
        }
        else if (quality == IntervalQuality.perfect)
        {
            if (!perfectIntervalNumber)
            {
                throw new System.Exception("Something's gone wrong. We shouldn't have a " + quality + " " + number);
            }
        }
        else
        {
            throw new System.Exception("Unimplemented interval quality: " + quality);
        }

        return semitoneValue;
    }

    public static Interval Invert(Interval interval)
    {
        int invertedNumber = 9 - interval.number;

        foreach (Interval i in Interval.intervals)
        {
            if (i.number == invertedNumber && i.semitones + interval.semitones == 12)
            {
                return i;
            }
        }

        throw new System.Exception("Could not invert interval: " + interval);
    }
    public Interval Invert()
    {
        return Interval.Invert(this);
    }

    public int Sign()
    {
        return (int)Mathf.Sign((float)number);
    }

    ///

    public static Interval[] intervals => GetIntervals();
    public static IntervalQuality[] intervalQualities => GetIntervalQualities();

    public static readonly Interval unison = new Interval(IntervalQuality.perfect, IntervalNumber.unison);
    public static readonly Interval diminishedSecond = new Interval(IntervalQuality.diminished, IntervalNumber.second);

    public static readonly Interval augmentedUnison = new Interval(IntervalQuality.augmented, IntervalNumber.unison);
    public static readonly Interval minorSecond = new Interval(IntervalQuality.minor, IntervalNumber.second);
    public static readonly Interval semitone = new Interval(IntervalQuality.minor, IntervalNumber.second);
    public static readonly Interval halfStep = new Interval(IntervalQuality.minor, IntervalNumber.second);

    public static readonly Interval majorSecond = new Interval(IntervalQuality.major, IntervalNumber.second);
    public static readonly Interval tone = new Interval(IntervalQuality.major, IntervalNumber.second);
    public static readonly Interval wholeStep = new Interval(IntervalQuality.major, IntervalNumber.second);
    public static readonly Interval diminishedThird = new Interval(IntervalQuality.diminished, IntervalNumber.third);

    public static readonly Interval augmentedSecond = new Interval(IntervalQuality.augmented, IntervalNumber.second);
    public static readonly Interval minorThird = new Interval(IntervalQuality.minor, IntervalNumber.third);

    public static readonly Interval majorThird = new Interval(IntervalQuality.major, IntervalNumber.third);
    public static readonly Interval diminishedFourth = new Interval(IntervalQuality.diminished, IntervalNumber.fourth);

    public static readonly Interval augmentedThird = new Interval(IntervalQuality.augmented, IntervalNumber.third);
    public static readonly Interval perfectFourth = new Interval(IntervalQuality.perfect, IntervalNumber.fourth);

    public static readonly Interval augmentedFourth = new Interval(IntervalQuality.augmented, IntervalNumber.fourth);
    public static readonly Interval diminishedFifth = new Interval(IntervalQuality.diminished, IntervalNumber.fifth);
    public static readonly Interval tritone = new Interval(IntervalQuality.diminished, IntervalNumber.fifth);

    public static readonly Interval perfectFifth = new Interval(IntervalQuality.perfect, IntervalNumber.fifth);
    public static readonly Interval diminishedSixth = new Interval(IntervalQuality.diminished, IntervalNumber.sixth);

    public static readonly Interval augmentedFifth = new Interval(IntervalQuality.augmented, IntervalNumber.fifth);
    public static readonly Interval minorSixth = new Interval(IntervalQuality.minor, IntervalNumber.sixth);

    public static readonly Interval majorSixth = new Interval(IntervalQuality.major, IntervalNumber.sixth);
    public static readonly Interval diminishedSeventh = new Interval(IntervalQuality.diminished, IntervalNumber.seventh);

    public static readonly Interval augmentedSixth = new Interval(IntervalQuality.augmented, IntervalNumber.sixth);
    public static readonly Interval minorSeventh = new Interval(IntervalQuality.minor, IntervalNumber.seventh);

    public static readonly Interval majorSeventh = new Interval(IntervalQuality.major, IntervalNumber.seventh);
    public static readonly Interval diminishedOctave = new Interval(IntervalQuality.diminished, IntervalNumber.octave);

    public static readonly Interval augmentedSeventh = new Interval(IntervalQuality.augmented, IntervalNumber.seventh);
    public static readonly Interval octave = new Interval(IntervalQuality.perfect, IntervalNumber.octave);

    public static readonly Interval minorNinth = new Interval(IntervalQuality.minor, IntervalNumber.ninth);

    public static readonly Interval majorNinth = new Interval(IntervalQuality.major, IntervalNumber.ninth);

    public static readonly Interval perfectEleventh = new Interval(IntervalQuality.perfect, IntervalNumber.eleventh);

    public static readonly Interval minorThirteenth = new Interval(IntervalQuality.minor, IntervalNumber.thirteenth);

    public static readonly Interval majorThirteenth = new Interval(IntervalQuality.major, IntervalNumber.thirteenth);

    private static Interval[] GetIntervals()
    {
        List<Interval> intervals = new List<Interval>();

        Interval interval;
        for (int i = 1; i <= 15; i++)
        {
            foreach (IntervalQuality quality in intervalQualities)
            {
                interval = new Interval(quality, i);

                if (interval.IsValidInterval())
                {
                    intervals.Add(interval);
                }
            }
        }

        return intervals.ToArray();
    }

    private static IntervalQuality[] GetIntervalQualities()
    {
        return new IntervalQuality[] { IntervalQuality.perfect, IntervalQuality.diminished, IntervalQuality.minor, IntervalQuality.major, IntervalQuality.augmented, IntervalQuality.doubleDiminished,
                                       IntervalQuality.doubleAugmented };
    }
}
