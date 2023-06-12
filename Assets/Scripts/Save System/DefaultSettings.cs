using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DefaultSettings
{

    public static int GetRequiredStars(string setName)
    {
        switch (setName)
        {
            case "Level":
                return 0;

            case "AfterLVS":
                return 0;

            case "FinalBeta":
                return 40;

            default:
                return 0;
        }
        
    }

    public static int GetNumberOfLevels(string setName)
    {
        switch (setName)
        {
            case "Level":
                return 20;

            case "AfterLVS":
                return 20;

            case "FinalBeta":
                return 20;

            default:
                return 20;
        }

    }
}
