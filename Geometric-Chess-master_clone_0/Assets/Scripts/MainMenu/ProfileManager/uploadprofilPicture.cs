using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


   public class uploadprofilPicture : MonoBehaviour
    {
        public int maxSize;

        public Image profileImage;

        public Texture2D texture;

        public Text debugText;

        public void imagePicker()
        {
            texture = null;
            NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
            {
                Debug.Log("Image path: " + path);
                if (path != null)
                {

                    // Create Texture from selected image
                    Texture2D tex = NativeGallery.LoadImageAtPath(path, maxSize);
                    texture = duplicateTexture(tex);




                    if (texture == null)
                    {
                        Debug.Log("Couldn't load texture from " + path);

                        StartCoroutine(setDebugText("Couldn't load texture from "));
                        
                        return;
                    }
                    profileImage.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
                    Debug.Log(texture.isReadable);

                    uploadProfileImage();
                    

                }
            });
        }

        //make the texture readable
        Texture2D duplicateTexture(Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }



        public void uploadProfileImage() => StartCoroutine(UploadAFile_coroutine());

        public IEnumerator UploadAFile_coroutine()
        {
            StartCoroutine(setDebugText("Uploading profile picture... "));
            string url = "https://chessgame-backend.herokuapp.com/userDPUpload";

            yield return new WaitForEndOfFrame();

            texture = profileImage.sprite.texture;

            Debug.Log(texture.isReadable);

            byte[] bytes = texture.EncodeToJPG(); //Can also encode to jpg, just make sure to change the file extensions down below


            Debug.Log(bytes);

            // Create a Web Form, this will be our POST method's data
            WWWForm form = new WWWForm();
            form.AddField("UserId", playerPermData.getLocalId());
            form.AddBinaryData("uploadimage", bytes, "profileImage.jpg", "image/jpg");

            //POST the screenshot to GameSparks
            using (UnityWebRequest request = UnityWebRequest.Post(url, form))
            {
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(request.error);
                }
                else
                {
                    Debug.Log(request.downloadHandler.text);
                    StartCoroutine(setDebugText("Profile picture uploaded. "));

                }
            }

        }


        IEnumerator setDebugText(string message)
        {
            debugText.text = message;
            yield return new WaitForSeconds(5);

            debugText.text = "";
        }
    }

