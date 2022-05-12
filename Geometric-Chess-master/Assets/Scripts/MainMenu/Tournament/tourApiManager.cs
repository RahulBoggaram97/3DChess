using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using SimpleJSON;

public class tourApiManager : MonoBehaviour
{


    private void Start()
    {
        loadingScreen.SetActive(true);
        getAlltournamnets();

    }

    [Header("prefabs")]
    public GameObject tournamentCard;
    public GameObject placeToInstantateCards;
    public GameObject playerCard;
    public GameObject playerCardSpawnTransform;

    public GameObject loadingScreen;


    [Header("other scripts")]
    public tourUiHangler uiManager;


    [Header("TextFields")]
    public TextMeshProUGUI tournamentId;
    public InputField joinTournamentId;
    public Text debugText;


    public void createTournament() => StartCoroutine(createTournamen_Coroutine());
    public void joinTournament()
    {
        if(joinTournamentId.text != null)
        StartCoroutine(joinTournament_Coroutine(joinTournamentId.text));
    }

    public void getAlltournamnets() => StartCoroutine(getAllTournament_Coroutine());

    public void getPlayersinTournament(string t_id) => StartCoroutine(getAllTournamentPlayer_Coroutine(t_id));

    IEnumerator createTournamen_Coroutine()
    {
        Debug.Log("creatingTournament");



        string uri = "https://chessgame-backend.herokuapp.com/api/createTournament";

        Debug.Log(uri);

        WWWForm form = new WWWForm();

        using (UnityWebRequest request = UnityWebRequest.Post(uri, form))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(request.error);

            }
            else
            {
                Debug.Log(request.downloadHandler.text);

                tournamentId.text = request.downloadHandler.text;

                Debug.Log(tournamentId.text.Substring(7, 13));

               

                StartCoroutine(joinTournament_Coroutine(tournamentId.text.Substring(7, 13)));

            }
        }



    }


    IEnumerator joinTournament_Coroutine(string tournamentId)
    {
        Debug.Log("joinTournament");



        string uri = "https://chessgame-backend.herokuapp.com/api/createTournamentPlayers";

        Debug.Log(uri);

        WWWForm form = new WWWForm();
        form.AddField("UserId", playerPermData.getLocalId());
        form.AddField("T_Id", tournamentId);
        form.AddField("Creator", "user");

        using (UnityWebRequest request = UnityWebRequest.Post(uri, form))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(request.error);

            }
            else
            {
                Debug.Log(request.downloadHandler.text);

                if (request.downloadHandler.text == "Joined")
                {
                    uiManager.joinTournamentOnPhoton(tournamentId);
                    tourUiHangler.setCurrentTournamentId(tournamentId);
                }
                else
                {
                    debugText.text = request.downloadHandler.text;
                }

            }
        }


    }

    IEnumerator getAllTournament_Coroutine()
    {
        Debug.Log("getting all the Tournaments");



        string uri = "https://chessgame-backend.herokuapp.com/api/getAllTournament";

        Debug.Log(uri);

       

        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(request.error);
                loadingScreen.GetComponentInChildren<Text>().text = "Sorry couldn't load. the error is " + request.error; 

            }
            else
            {
                //Debug.Log(request.downloadHandler.text);

                //{ "T_Id":"1650877725643",
                //        "T_Name":"",
                //        "TotalPoints":0,
                //        "TotalGames":0,
                //        "Description":"",
                //        "T_Img":"",
                //        "Status":"",
                //        "T_Fee":0,
                //        "Max_Players":0,
                //        "GameTime":null},


                JSONNode node = JSON.Parse(request.downloadHandler.text);


                Debug.Log(node[0].ToString());

                int i = 0;

                while (node[i] != null) 
                {
                    
                    
                    GameObject spawnedCard = Instantiate(tournamentCard, placeToInstantateCards.transform);
                    tournamentCard cardInfo = spawnedCard.GetComponent<tournamentCard>();

                    cardInfo.t_id = node[i]["T_Id"].ToString().Substring(1, node[i]["T_Id"].ToString().Length - 2);
                    cardInfo.t_name = node[i]["T_Name"].ToString().Substring(1, node[i]["T_Name"].ToString().Length - 2);
                    cardInfo.description = node[i]["Description"].ToString().Substring(1, node[i]["Description"].ToString().Length - 2);
                    cardInfo.t_fee = int.Parse(node[i]["T_Fee"].ToString());

                    cardInfo.setAllTheText();
                        i++;
                    
                }

                loadingScreen.SetActive(false);


            }
        }
    }

    IEnumerator getAllTournamentPlayer_Coroutine(string t_id)
    {
        Debug.Log("getting all the Tournaments");



        string uri = "https://chessgame-backend.herokuapp.com/api/getTournamentPlayers/1650868168446";

        Debug.Log(uri);
        WWWForm  form = new WWWForm();
       


        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(request.error);

            }
            else
            {

                Debug.Log(request.downloadHandler.text);


                //{ "UserId":"1641563424450",
                //        "T_Id":"1650868168446",
                //        "TimeStamp":"2022-04-25T08:30:30.000Z",
                //        "T_Points":0,
                //        "MatchesWon":1,
                //        "MatchesLoss":0,
                //        "MatchesDrawn":0,
                //        "TotalMatches":0,
                //        "Rank":0,
                //        "inMatch":0}

                JSONNode node = JSON.Parse(request.downloadHandler.text);

                Debug.Log("the node can be : ");

                Debug.Log(node[0]["UserId"].ToString());


                int i = 0;

                while (node[i] != null)
                {


                    GameObject spawnedCard = Instantiate(playerCard, playerCardSpawnTransform.transform);
                    
                    playerCard cardInfo = spawnedCard.GetComponent<playerCard>();

                    cardInfo.playerName = node[i]["UserId"].ToString().Substring(1, node[i]);
                    cardInfo.playerWins = node[i]["MatchesWon"].ToString();

                    cardInfo.setAllText();



                    i++;

                }

            }
        }
    }

}