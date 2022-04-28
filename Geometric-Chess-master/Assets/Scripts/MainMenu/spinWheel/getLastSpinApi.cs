using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


    public class getLastSpinApi : MonoBehaviour
    {
        public spinWheelManager spinManager;

        public bool canSpin;

        public void getLastSpin() 
        {
            this.gameObject.SetActive(true);
            StartCoroutine(getLastSpinApi_Coroutine());
        }


        public void addCoinsFromWheel(string spinCoins) => StartCoroutine(addCoinsFromWheelCoroutine(spinCoins));

        IEnumerator getLastSpinApi_Coroutine()
        {
           

           
            string url = "https://chessgame-backend.herokuapp.com/api/getLastSpin";

            Debug.Log("getting last spin data...");

            WWWForm form = new WWWForm();

            
        


            form.AddField("userid", playerPermData.getLocalId());

         

            

         

            using (UnityWebRequest request = UnityWebRequest.Post(url, form))

            {

                
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(request.error + "this is from lastSpinApi");

                    if (request.downloadHandler.text == "Failure")
                    {
                        canSpin = false;
                    }
                        
                    else  if(request.downloadHandler.text == "Success")
                    {
                        canSpin = true;
                    }
                    Debug.Log(canSpin);
                   
                }
                else
                {
                    if (request.downloadHandler.text == "Success")
                        canSpin = true;
                    else canSpin = false;
                    Debug.Log(request.downloadHandler.text);
                    Debug.Log(canSpin);
                    
                    

                }

             
            }
        }
        

        IEnumerator addCoinsFromWheelCoroutine(string spinCoins)
        {
            string url = "https://chessgame-backend.herokuapp.com/api/addCoins";

            WWWForm form = new WWWForm();
            Debug.Log(playerPermData.getPhoneNumber());
            form.AddField("userid", playerPermData.getLocalId());
            form.AddField("coins", spinCoins);

            using (UnityWebRequest request = UnityWebRequest.Post(url, form))
            {
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(request.error);
                    Debug.Log(request.downloadHandler.text);
                }
                else
                {
                    Debug.Log(request.downloadHandler.text);

                }
            }
        }
    }


