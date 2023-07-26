using UnityEngine;

[CreateAssetMenu(fileName = "Account", menuName = "Social Platforms/Account", order = 0)]
public class SocialAccount : ScriptableObject
{
    public SocialPlatforms Platform;
    public Sprite Sprite;
}