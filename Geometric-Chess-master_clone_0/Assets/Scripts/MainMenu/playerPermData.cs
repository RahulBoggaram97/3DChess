using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class playerPermData 
{
    //id made by firebase during registration
    public const string LOCAL_ID_PREF_KEY = "firebaseAuthenticatedLocalID";
    public const string EMAIL_ID_PREF_KEY = "firebaseGoolgeAuthenticatedID";
    public const string USERNAME_PREF_KEY = "userName";
    public static string PHONE_NO_PREF_KEY = "phoneNumber";
    public static string PROFILE_PIC_URL_PREF_KEY = "profilePicUrl";

    //buyable
    public static string MONEY_PREF_KEY = "money";
   

    //tourney stats
    public const string WON_MATCHES_PREF_KEY = "won";
    public const string LOSE_MATCHES_PREF_KEY = "lose";
    public const string DRAWN_MATCHES_PREF_KEY = "drawn";
    public const string TOTAL_MATCHES_PREF_KEY = "total";
    public const string SCORE_PREF_KEY = "score";

    //refer
    public const string REFER_CODE_PREF_KEY = "referCode";
    public const string REFERED_BY_CODE_PREF_KEY = "referedBy";


    //tournaments
    public const string CURRENT_TOURNAMENT_PREF_KEY = "currentTournament";

    //EMAIL
    public static void setEmail(string value)
    {
        PlayerPrefs.SetString(EMAIL_ID_PREF_KEY, value);
    }

    public static string getEmail()
    {
        return PlayerPrefs.GetString(EMAIL_ID_PREF_KEY);
    }

    //LOCAL ID
    public static void setLocalId(string value)
    {
        PlayerPrefs.SetString(LOCAL_ID_PREF_KEY, value);
    }

    public static string getLocalId()
    {
        return PlayerPrefs.GetString(LOCAL_ID_PREF_KEY);
    }

    //USER NAME
    public static void setUserName(string value)
    {
        PlayerPrefs.SetString(USERNAME_PREF_KEY, value);
    }

    public static string getUserName()
    {
        return PlayerPrefs.GetString(USERNAME_PREF_KEY);
    }

    //PROFILE PIC
    public static void setProfilePicUrl(string value)
    {
        PlayerPrefs.SetString(PROFILE_PIC_URL_PREF_KEY, value);
    }

    public static string getProfilePicUrl()
    {
        return PlayerPrefs.GetString(PROFILE_PIC_URL_PREF_KEY);
    }





    //MONEY
    public static void setMoney(int value)
    {
        PlayerPrefs.SetInt(MONEY_PREF_KEY, value);
    }

    public static int getMoney()
    {
        return PlayerPrefs.GetInt(MONEY_PREF_KEY);
    }

   



    //PHONE NUM
    public static void setPhoneNumber(string value)
    {
        PlayerPrefs.SetString(PHONE_NO_PREF_KEY, value);
    }

    public static string getPhoneNumber()
    {
        return PlayerPrefs.GetString(PHONE_NO_PREF_KEY);
    }


    //WON MATCHES
    public static void setWonMatches(string value)
    {
        PlayerPrefs.SetString(WON_MATCHES_PREF_KEY, value);
    }

    public static string getWonMatches()
    {
        return PlayerPrefs.GetString(WON_MATCHES_PREF_KEY);
    }

    //LOSE MATCHES
    public static void setLoseMatches(string value)
    {
        PlayerPrefs.SetString(LOSE_MATCHES_PREF_KEY, value);
    }

    public static string getLoseMatchesr()
    {
        return PlayerPrefs.GetString(LOSE_MATCHES_PREF_KEY);
    }

    //DRAWN MATCHES
    public static void setDrawnMatches(string value)
    {
        PlayerPrefs.SetString(DRAWN_MATCHES_PREF_KEY, value);
    }

    public static string getDrawnMatches()
    {
        return PlayerPrefs.GetString(DRAWN_MATCHES_PREF_KEY);
    }

    //TOTAL MATCHES
    public static void setTotalMatches(string value)
    {
        PlayerPrefs.SetString(TOTAL_MATCHES_PREF_KEY, value);
    }

    public static string getTotalMatches()
    {
        return PlayerPrefs.GetString(TOTAL_MATCHES_PREF_KEY);
    }

    //SCORE
    public static void setScore(string score)
    {
        PlayerPrefs.SetString(SCORE_PREF_KEY, score);
    }

    public static string getScore()
    {
        return PlayerPrefs.GetString(SCORE_PREF_KEY);
    }
 
   

    //REFER CODE
    public static void setReferCode(string value)
    {
        PlayerPrefs.SetString(REFER_CODE_PREF_KEY, value);
    }

    public static string getReferCode()
    {
        return PlayerPrefs.GetString(REFER_CODE_PREF_KEY);
    }

    //REFERED BY
    public static void setReferdBy(string value)
    {
        PlayerPrefs.SetString(REFERED_BY_CODE_PREF_KEY, value);
    }

    public static string getReferedBy()
    {
        return PlayerPrefs.GetString(REFERED_BY_CODE_PREF_KEY);
    }



    //TOURNAMENTS
    public static void setCurrentTournamentId(string value)
    {
        PlayerPrefs.SetString(CURRENT_TOURNAMENT_PREF_KEY, value);
    }

    public static string getCurrentTournamentId()
    {
       return PlayerPrefs.GetString(CURRENT_TOURNAMENT_PREF_KEY);
    }

    public static void nullTheCurrentTournamentId()
    {
        PlayerPrefs.SetString(CURRENT_TOURNAMENT_PREF_KEY, string.Empty);
    }

   
}
