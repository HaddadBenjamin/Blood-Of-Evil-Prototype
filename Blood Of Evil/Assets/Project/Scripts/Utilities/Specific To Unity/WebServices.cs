using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BloodOfEvil.Utilities
{
    public class Webservices : MonoBehaviour
    {

        const string BaseURL = @"http://localhost/ISART-WS";

        private string token = null;
        private int uid;

        public delegate void RequestDone<T>(Response<T> resp);

        private RequestDone<LoginData> backup;

        #region Call Coroutines
        public void Login(string login, string password, RequestDone<LoginData> callback)
        {
            StartCoroutine(DoLogin(login, password, callback));
        }

        public void CreateAccount(string login, string password, RequestDone<object> callback)
        {
            StartCoroutine(DoCreateAccount(login, password, callback));
        }

        public void SavePicture(string title, byte[] picture, RequestDone<object> callback)
        {
            StartCoroutine(DoSavePicture(title, picture, callback));
        }
        #endregion

        #region Coroutines
        IEnumerator DoLogin(string login, string password, RequestDone<LoginData> callback)
        {
            string service = "login";

            WWWForm data = new WWWForm();
            data.AddField("login", login);
            data.AddField("pass", password);

            backup = callback;
            yield return GenericRequest<LoginData>(service, data, this.LoginDone);
        }

        IEnumerator DoCreateAccount(string login, string password, RequestDone<object> callback)
        {
            string service = "createAccount";

            WWWForm data = new WWWForm();
            data.AddField("login", login);
            data.AddField("pass", password);

            Debug.Log("create account");

            yield return GenericRequest<object>(service, data, callback);
        }

        IEnumerator DoSavePicture(string title, byte[] picture, RequestDone<object> callback)
        {
            string service = "savePicture";

            WWWForm data = new WWWForm();
            data.AddField("title", title);
            data.AddField("uid", this.uid);
            data.AddField("token", this.token);
            data.AddBinaryData("picture", picture);

            yield return GenericRequest<object>(service, data, callback);
        }
        #endregion

        #region Done Callbacks
        public void LoginDone(Response<LoginData> resp)
        {
            if (resp.code == 0)
            {
                this.token = resp.data.token;
                this.uid = resp.data.uid;
            }
            backup(resp);
        }
        #endregion

        IEnumerator GenericRequest<T>(string service, WWWForm data, RequestDone<T> callback)
        {
            string endpoint = String.Format("{0}/{1}", Webservices.BaseURL, service);

            WWW www = new WWW(endpoint, data);
            yield return www;

            if (String.IsNullOrEmpty(www.error))
            {
                Debug.Log(www.text);
                Response<T> resp = JsonUtility.FromJson<Response<T>>(www.text);

                if (resp != null)
                {
                    if (null == callback)
                        Debug.Log("call back is empty");
                    else
                        callback(resp);
                }
                else
                    Debug.LogError("JSON Parsing ERROR");
            }
            else
                Debug.LogError("HTTP ERROR : " + www.error);
        }

    }

    [Serializable]
    public class Response<T>
    {
        public int code;
        public string message;
        public T data;
    }

    [Serializable]
    public class LoginData
    {
        public string token;
        public int uid;
    }
}