using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BerserkPixel.File_Explorer;
using Mastonet;
using Mastonet.Entities;
using UnityEngine;
using Application = UnityEngine.Application;

namespace BerserkPixel.Mastodon
{
    public class MastodonAPI
    {
        #region Singleton

        private MastodonAPI()
        {
        }

        private static readonly MastodonAPI _instance = null;

        public static MastodonAPI Instance => _instance ?? new MastodonAPI();

        #endregion

        private const string KEY_ACCESS_TOKEN = "mastodon_access_token";

        public bool IsInitialised => _mastodonClient != null;

        private static AuthenticationClient _authenticationClient;

        // static so it preserves data
        private static MastodonClient _mastodonClient;

        private static async Task RegisterApp(MastodonCredentials mastodonCredentials)
        {
            _authenticationClient = new AuthenticationClient(mastodonCredentials.INSTANCE_NAME);

            await _authenticationClient.CreateApp(mastodonCredentials.APP_NAME,
                Scope.Read | Scope.Write | Scope.Follow);
        }

        public async Task<string> GetCodeUrl(MastodonCredentials mastodonCredentials)
        {
            await RegisterApp(mastodonCredentials);

            return _authenticationClient.OAuthUrl();
        }

        public async void Connect(MastodonCredentials mastodonCredentials, Action<Account> autoConnectCallback)
        {
            var token = PlayerPrefs.GetString(KEY_ACCESS_TOKEN, "");
            if (string.IsNullOrEmpty(token))
            {
                // go with code
                autoConnectCallback?.Invoke(null);
            }
            else
            {
                // go automatic
                autoConnectCallback?.Invoke(await ConnectWithToken(mastodonCredentials, token));
            }
        }

        public async Task<Account> ConnectWithCode(MastodonCredentials mastodonCredentials, string code)
        {
            if (_authenticationClient == null)
            {
                await RegisterApp(mastodonCredentials);
            }

            var auth = await _authenticationClient!.ConnectWithCode(code);

            var accessToken = auth.AccessToken;
            
            PlayerPrefs.SetString(KEY_ACCESS_TOKEN, accessToken);

            return await ConnectWithToken(mastodonCredentials, accessToken);
        }

        private async Task<Account> ConnectWithToken(MastodonCredentials mastodonCredentials, string accessToken)
        {
            _mastodonClient = new MastodonClient(mastodonCredentials.INSTANCE_NAME, accessToken);

            var account = await _mastodonClient.GetCurrentUser();

            return account;
        }

        public async Task<Status> PostImage(FileAsset asset, string text)
        {
            var attachments = new List<Attachment>();
            var stream = new FileStream(asset.Path, FileMode.Open);
            var media = new MediaDefinition(stream, asset.Name ?? "img");
            var attachment = await _mastodonClient.UploadMedia(media);
            attachments.Add(attachment);

            var mediaIds = attachments.Select(a => a.Id);

            var visibility = Application.isEditor ? Visibility.Private : Visibility.Public;
            
            var status = await _mastodonClient.PublishStatus(
                text, 
                visibility,
                mediaIds: mediaIds
            );

            attachments.Clear();
            await stream.DisposeAsync();

            return status;
        }
        
        public async Task<Status> SchedulePostImage(FileAsset asset, string text, DateTime scheduleTime)
        {
            var attachments = new List<Attachment>();
            var stream = new FileStream(asset.Path, FileMode.Open);
            var media = new MediaDefinition(stream, asset.Name ?? "img");
            var attachment = await _mastodonClient.UploadMedia(media);
            attachments.Add(attachment);

            var mediaIds = attachments.Select(a => a.Id);

            var visibility = Application.isEditor ? Visibility.Private : Visibility.Public;
            
            var status = await _mastodonClient.PublishStatus(
                text, 
                visibility,
                mediaIds: mediaIds,
                scheduledAt: scheduleTime
            );

            attachments.Clear();
            await stream.DisposeAsync();

            return status;
        }
    }
}