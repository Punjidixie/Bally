using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllRecord
{
    public int totalStars;

    public static AllRecord GetDefault()
    {
        AllRecord result = new AllRecord
        {
            totalStars = 0
        };

        return result;
    }
}
