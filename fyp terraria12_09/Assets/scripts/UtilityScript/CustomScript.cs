using UnityEngine;
using System.Collections;

public class CustomScript
{

    //custom parsing the string to int is the fastest proof :http://cc.davelozinski.com/c-sharp/fastest-way-to-convert-a-string-to-an-int
    public static int StrToInt(string str)
    {
        int res = -1;

        int y = 0;

        for (int i = 0; i < str.Length; i++)
        {
            if(str[0] == '-' && i == 0)
            {
                continue;
            }
            y = y * 10 + (str[i] - '0');
        }
        res += y + 1;
        
        if(str[0] == '-')
        {
            res = -res;
        }

        return res;
    }
}
