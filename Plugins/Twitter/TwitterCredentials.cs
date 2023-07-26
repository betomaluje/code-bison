using UnityEngine;

namespace BerserkPixel.Twitter
{
    [CreateAssetMenu(fileName = "Credentials", menuName = "Twitter/Credentials", order = 0)]
    public class TwitterCredentials : ScriptableObject
    {
        public string CONSUMER_KEY = "YOUR_CONSUME_KEY";
        public string CONSUMER_SECRET = "YOUR_CONSUME_SECRET";
    }
}