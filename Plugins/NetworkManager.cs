using System;
using UnityEngine;
using UnityEngine.Networking;

namespace BerserkPixel.Twitter
{
    public static class NetworkManager
    {
        private static UnityWebRequest _request;
        private static Vector2 _pivotCenter = new Vector2(0.5f, 0.5f);

        public static void GetTextureFromUrl(string url, Action<Texture2D> callback)
        {
            GetTexture(url, request =>
            {
#if UNITY_2020_3_OR_NEWER
                if (request.result == UnityWebRequest.Result.Success)
                {
#else
            if (!request.isNetworkError && !request.isHttpError) {
#endif
                    DownloadHandlerTexture downloadHandlerTexture = request.downloadHandler as DownloadHandlerTexture;
                    var texture = downloadHandlerTexture.texture;
                    callback(texture);
                }
                else
                {
                    Debug.Log($"[Network Manager] {request.error}: {request.downloadHandler.text}.");
                    callback(null);
                }
            });
        }
        
        public static void GetTextureFromUrl(string url, Action<Sprite> callback)
        {
            GetTexture(url, request =>
            {
#if UNITY_2020_3_OR_NEWER
                if (request.result == UnityWebRequest.Result.Success)
                {
#else
            if (!request.isNetworkError && !request.isHttpError) {
#endif
                    DownloadHandlerTexture downloadHandlerTexture = request.downloadHandler as DownloadHandlerTexture;
                    var texture = downloadHandlerTexture.texture;
                    var sprite = ConvertTextureToSprite(texture);
                    callback(sprite);
                }
                else
                {
                    Debug.Log($"[Network Manager] {request.error}: {request.downloadHandler.text}.");
                    callback(null);
                }
            });
        }

        private static Sprite ConvertTextureToSprite(Texture2D texture, Vector2 pivot)
        {
            return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), pivot, 50f,
                0, SpriteMeshType.FullRect);
        }
        
        private static Sprite ConvertTextureToSprite(Texture2D texture)
        {
            return ConvertTextureToSprite(texture, _pivotCenter);
        }

        public static void PostTextFromUrl(string url, string postData, Action<string> callback)
        {
            PostRequest(url, postData, request =>
            {
#if UNITY_2020_3_OR_NEWER
                if (request.result == UnityWebRequest.Result.Success)
                {
#else
            if (!request.isNetworkError && !request.isHttpError) {
#endif
                    var text = request.downloadHandler.text;
                    callback(JsonUtility.ToJson(text));
                }
                else
                {
                    Debug.Log($"[Network Manager] {request.error}: {request.downloadHandler.text}.");
                    callback(null);
                }
            });
        }

        public static void GetTextFromUrl(string url, Action<string> callback)
        {
            GetTextFromUrl(url, null, callback);
        }

        public static void GetTextFromUrl(string url, string apiKey, Action<string> callback)
        {
            GetRequest(url, apiKey, request =>
            {
#if UNITY_2020_3_OR_NEWER
                if (request.result == UnityWebRequest.Result.Success)
                {
#else
            if (!request.isNetworkError && !request.isHttpError) {
#endif
                    var text = request.downloadHandler.text;
                    callback(text);
                }
                else
                {
                    Debug.Log($"[Network Manager] {request.error}: {request.downloadHandler.text}.");
                    callback(null);
                }
            });
        }

        private static void GetRequest(string url, string apiKey, Action<UnityWebRequest> callback)
        {
            if (_request != null)
            {
                return;
            }

            _request = UnityWebRequest.Get(url);
            if (!string.IsNullOrEmpty(apiKey))
                _request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

            _request.SetRequestHeader("Content-Type", "application/json");
            var op = _request.SendWebRequest();
            op.completed += operation =>
            {
                callback(_request);
                _request.Dispose();
                _request = null;
            };
        }

        private static void PostRequest(string url, string postData, Action<UnityWebRequest> callback)
        {
            if (_request != null)
            {
                return;
            }

            _request = UnityWebRequest.PostWwwForm(url, postData);
            var op = _request.SendWebRequest();
            op.completed += operation =>
            {
                callback(_request);
                _request.Dispose();
                _request = null;
            };
        }

        private static void GetTexture(string url, Action<UnityWebRequest> callback)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);

            var op = request.SendWebRequest();
            op.completed += operation =>
            {
                callback(request);
                request.Dispose();
                request = null;
            };
        }
    }
}