using UnityEngine;

namespace BerserkPixel.Mastodon
{
    [CreateAssetMenu(fileName = "Credentials", menuName = "Mastodon/Credentials", order = 0)]
    public class MastodonCredentials : ScriptableObject
    {
        public string APP_NAME = "Bison for Mastodon";
        public string INSTANCE_NAME = "mastodon.example.place";
    }
}