using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class botChessApiCaller : MonoBehaviour
{




    string uri = "http://43.204.29.59:8000/bot";

    string jsonStr;

    string move;

    string fenString;

    int depth;

   [Header("Other Scripts")]
   public botChessBoard board;

    public void GetMoves(string currentPosFenString, int currentdepth )
    {
        fenString = currentPosFenString;
        depth = currentdepth;
        makeJosn();
        POST();

    }

    
    public void makeJosn()
    {
        javaObject obj = new javaObject();
        obj.fen = fenString;
        obj.depth = depth;

         jsonStr = JsonUtility.ToJson(obj);
        Debug.Log(jsonStr);
    }



    public WWW POST()
    {
        Debug.Log("making the header");
        WWW www;
        Hashtable postHeader = new Hashtable();
        postHeader.Add("Content-Type", "application/json");

        // convert json string to byte
        var formData = System.Text.Encoding.UTF8.GetBytes(jsonStr);

        www = new WWW(uri, formData, postHeader);
        StartCoroutine(WaitForRequest(www));
        return www;
    }

    IEnumerator WaitForRequest(WWW data)
    {
        Debug.Log("getting the move");
        yield return data; // Wait until the download is done
        if (data.error != null)
        {
           Debug.Log(data.error);
            Debug.Log(data.text);

        }
        else
        {
           Debug.Log(data.text);

            JSONNode node = JSON.Parse(data.text);

            Debug.Log(node["move"].ToString().Substring(1, node["move"].ToString().Length -2));
            move = node["move"].ToString().Substring(1, node["move"].ToString().Length - 2);
            board.makeComeMove(move);
        }
    }

}

public class javaObject
{
    public string fen;
    public int depth;
}
