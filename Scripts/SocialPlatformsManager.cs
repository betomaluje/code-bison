using System;
using System.Collections.Generic;
using BerserkPixel.File_Explorer;
using DG.Tweening;
using Posting;
using UnityEngine;
using UnityEngine.UI;

public class SocialPlatformsManager : MonoBehaviour
{
    [SerializeField] private Transform _container;
    [SerializeField] private GameObject _accountPrefab;

    public static Action<SocialAccount> AccountAdded;

    private HashSet<SocialPoster> _socialPosters;

    private void Awake()
    {
        _socialPosters = new HashSet<SocialPoster>();
    }

    private void OnEnable()
    {
        AccountAdded += OnSocialAdded;
    }

    private void OnDisable()
    {
        AccountAdded -= OnSocialAdded;
    }

    private void OnSocialAdded(SocialAccount account)
    {
        var accountPrefab = Instantiate(_accountPrefab, _container);
        if (accountPrefab.TryGetComponent(out Image image))
        {
            image.sprite = account.Sprite;
        }

        accountPrefab.transform.DOShakeScale(.5f, Vector3.one, 5);

        switch (account.Platform)
        {
            case SocialPlatforms.Mastodon:
                _socialPosters.Add(new MastodonPoster());
                break;
            case SocialPlatforms.Twitter:
                _socialPosters.Add(new TwitterPoster());
                break;
        }

        UnRegisterPosters();
        RegisterPosters();
    }

    private void OnDestroy()
    {
        UnRegisterPosters();
    }

    private void RegisterPosters()
    {
        foreach (var socialPoster in _socialPosters)
        {
            socialPoster.OnPostStart += HandlePostStart;
            socialPoster.OnPostDone += HandlePostSuccess;
            socialPoster.OnPostError += HandlePostError;
        }
    }

    private void UnRegisterPosters()
    {
        foreach (var socialPoster in _socialPosters)
        {
            socialPoster.OnPostStart -= HandlePostStart;
            socialPoster.OnPostDone -= HandlePostSuccess;
            socialPoster.OnPostError -= HandlePostError;
        }
    }

    private void HandlePostStart()
    {
        LoadingPanel.LoadingStartEvent.Invoke();
    }

    private void HandlePostSuccess(string message)
    {
        ConsolePanel.Instance.WriteConsole(message);
        LoadingPanel.LoadingEndEvent.Invoke();
    }

    private void HandlePostError(string message)
    {
        ConsolePanel.Instance.WriteConsole(message, -1);
        LoadingPanel.LoadingEndEvent.Invoke();
    }

    public void Post(FileAsset asset, string text, DateTime? scheduledTime, bool scheduled = false)
    {
        if (_socialPosters.Count <= 0)
        {
            ConsolePanel.Instance.WriteConsole(
                "No linked accounts. Make sure to go to Accounts and link your social platforms",
                3
            );
            return;
        }

        foreach (var poster in _socialPosters)
        {
            if (scheduledTime.HasValue)
            {
                poster.SetTime(scheduledTime.Value);
            }

            poster.Post(asset, text, scheduled);
        }
    }
}