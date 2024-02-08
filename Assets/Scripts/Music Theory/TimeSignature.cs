using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSignature
{
    public int topNumber { get; private set; }
    public int bottomNumber { get; private set; }

    public Duration tempoUnit => GetTempoUnit();
    public int weakBeat => GetWeakBeat();

    public TimeSignature(int topNumber, int bottomNumber)
    {
        this.topNumber = topNumber;
        this.bottomNumber = bottomNumber;

        if (!IsValid())
        {
            throw new System.Exception("Invalid time signature: " + this);
        }
    }

    public static bool operator ==(TimeSignature time1, TimeSignature time2)
    {
        return time1.topNumber == time2.topNumber && time1.bottomNumber == time2.bottomNumber;
    }
    public static bool operator !=(TimeSignature time1, TimeSignature time2)
    {
        return !(time1 == time2);
    }

    public override string ToString()
    {
        return topNumber + "/" + bottomNumber;
    }

    private bool IsValid()
    {
        return topNumber > 0 && bottomNumber > 0;
    }

    private Duration GetTempoUnit()
    {
        switch (bottomNumber)
        {
            case 2: return Duration.half;
            case 4: return Duration.quarter;
            case 8: return Duration.eighth;
            case 16: return Duration.sixteenth;
            default: throw new System.Exception("Unimplemented time signature bottom number: " + bottomNumber);
        }
    }

    private int GetWeakBeat()
    {
        if (this == TimeSignature._4_4)
        {
            return 2;
        }
        if (this == TimeSignature._6_8)
        {
            return 3;
        }
        else if (this == TimeSignature._12_8)
        {
            return 6;
        }
        else
        {
            return topNumber;
        }
    }

    ///

    public static readonly TimeSignature common = new TimeSignature(4, 4);
    public static readonly TimeSignature cut = new TimeSignature(2, 2);

    public static readonly TimeSignature _2_4 = new TimeSignature(2, 4);
    public static readonly TimeSignature _3_4 = new TimeSignature(3, 4);
    public static readonly TimeSignature _4_4 = new TimeSignature(4, 4);
    public static readonly TimeSignature _5_4 = new TimeSignature(5, 4);

    public static readonly TimeSignature _6_8 = new TimeSignature(6, 8);
    public static readonly TimeSignature _9_8 = new TimeSignature(9, 8);
    public static readonly TimeSignature _12_8 = new TimeSignature(12, 8);
}
