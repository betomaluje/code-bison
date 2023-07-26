using BerserkPixel.Mastodon;
using BerserkPixel.Twitter;
using Mastonet.Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Application = UnityEngine.Application;

public class MastodonPanel : MonoBehaviour
{
    [SerializeField] private MastodonCredentials _mastodonCredentials = default;
    [SerializeField] private SocialAccount _mastodonAccount;

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
        
        MastodonAPI.Instance.Connect(_mastodonCredentials, (authUser) =>
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
        Application.OpenURL(await MastodonAPI.Instance.GetCodeUrl(_mastodonCredentials));
        
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
        
        var authUser = await MastodonAPI.Instance.ConnectWithCode(_mastodonCredentials, _codeField.text);
        
        LoadUser(authUser);
    }

    private void LoadUser(Account authUser)
    {
        PopulateUser(authUser);
        
        ConsolePanel.Instance.WriteConsole("Mastodon login success!");

        LoadingPanel.LoadingEndEvent.Invoke();

        _codePanel.SetActive(false);
    }

    private void PopulateUser(Account authUser)
    {
        _userPanel.SetActive(true);
        
        SocialPlatformsManager.AccountAdded?.Invoke(_mastodonAccount);

        _profileName.text = $"{authUser.AccountName}";
        _profileStats.text = $"Followers: {authUser.FollowersCount} / Following: {authUser.FollowingCount}";

        NetworkManager.GetTextureFromUrl(authUser.AvatarUrl, (sprite) =>
        {
            if (sprite == null) return;
            
            _profilePicture.color = Color.white;
            _profilePicture.sprite = sprite;
            _profilePicture.preserveAspect = true;
        });
    }
}