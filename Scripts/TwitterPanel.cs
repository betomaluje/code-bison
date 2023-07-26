using BerserkPixel.Twitter;
using TMPro;
using Tweetinvi.Models;
using UnityEngine;
using UnityEngine.UI;
using TwitterCredentials = BerserkPixel.Twitter.TwitterCredentials;

public class TwitterPanel : MonoBehaviour
{
    [SerializeField] private TwitterCredentials _twitterCredentials = default;
    [SerializeField] private SocialAccount _twitterAccount;

    [Header("Insert Code UI")] 
    [SerializeField] private TMP_InputField _codeField;
    [SerializeField] private GameObject _codePanel;
    [SerializeField] private GameObject _getCodeButton;

    [Header("Credentials UI")] 
    [SerializeField] private GameObject _userPanel;
    [SerializeField] private Image _profilePicture;
    [SerializeField] private TextMeshProUGUI _profileName;
    [SerializeField] private TextMeshProUGUI _profileStats;

    private void Start()
    {
        _userPanel.SetActive(false);
        _codePanel.SetActive(false);
        _getCodeButton.SetActive(false);
        
        LoadingPanel.LoadingStartEvent.Invoke();
        
        TwitterAPI.Instance.Connect(_twitterCredentials, (authUser) =>
        {
            if (authUser != null)
            {
                LoadUser(authUser);
            }
            else
            {
                // go code
                LoadingPanel.LoadingEndEvent.Invoke();
                _getCodeButton.SetActive(true);
            }
        });
    }

    public async void Click_GetCode()
    {
        Application.OpenURL(await TwitterAPI.Instance.GetCodeUrl(_twitterCredentials));

        _getCodeButton.SetActive(false);
        _codePanel.SetActive(true);
    }

    public async void Click_SubmitCode()
    {
        if (string.IsNullOrEmpty(_codeField.text))
        {
            Debug.LogWarning("Code field is empty");
            return;
        }

        LoadingPanel.LoadingStartEvent.Invoke();

        var authUser = await TwitterAPI.Instance.ConnectWithCode(_codeField.text);

        LoadUser(authUser);
    }

    private void LoadUser(IAuthenticatedUser authUser)
    {
        PopulateUser(authUser);

        ConsolePanel.Instance.WriteConsole("Twitter login success!");

        LoadingPanel.LoadingEndEvent.Invoke();

        _codePanel.SetActive(false);
    }

    private void PopulateUser(IAuthenticatedUser authUser)
    {
        _userPanel.SetActive(true);
        
        SocialPlatformsManager.AccountAdded?.Invoke(_twitterAccount);
        
        _profileName.text = $"{authUser.ScreenName}";
        _profileStats.text = $"Followers: {authUser.FollowersCount} / Following: {authUser.FriendsCount}";

        NetworkManager.GetTextureFromUrl(authUser.ProfileImageUrl, (sprite) =>
        {
            _profilePicture.sprite = sprite;
            _profilePicture.preserveAspect = true;
        });
    }
}