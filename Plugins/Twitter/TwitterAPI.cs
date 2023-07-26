using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BerserkPixel.File_Explorer;
using Tweetinvi;
using Tweetinvi.Core.Web;
using Tweetinvi.Models;
using UnityEngine;

namespace BerserkPixel.Twitter
{
    public class TwitterAPI
    {
        #region Singleton

        private TwitterAPI()
        {
        }

        private static readonly TwitterAPI _instance = null;

        public static TwitterAPI Instance =>
            _instance ?? new TwitterAPI();

        #endregion

        private const string KEY_ACCESS_TOKEN = "twitter_access_token";
        private const string KEY_ACCESS_TOKEN_SECRET = "twitter_access_token_secret";
        private const string KEY_USER_ID = "twitter_user_id";

        public static bool IsInitialised => UserClient != null;

        public static TwitterClient UserClient { get; private set; }
        public static TwitterClient AppClient { get; private set; }

        private static IAuthenticationRequest _authenticationRequest;

        private void RegisterApp(TwitterCredentials twitterCredentials)
        {
            if (AppClient != null) return;

            AppClient = new TwitterClient(
                twitterCredentials.CONSUMER_KEY,
                twitterCredentials.CONSUMER_SECRET
            );
        }

        public async void Connect(TwitterCredentials twitterCredentials, Action<IAuthenticatedUser> autoConnectCallback)
        {
            var token = PlayerPrefs.GetString(KEY_ACCESS_TOKEN, "");
            var tokenSecret = PlayerPrefs.GetString(KEY_ACCESS_TOKEN_SECRET, "");
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(tokenSecret))
            {
                // go with code
                autoConnectCallback?.Invoke(null);
            }
            else
            {
                // go automatic
                autoConnectCallback?.Invoke(await ConnectWithToken(twitterCredentials, token, tokenSecret));
            }
        }

        private async Task<IAuthenticatedUser> ConnectWithToken(
            TwitterCredentials twitterCredentials,
            string token,
            string secret
        )
        {
            UserClient = new TwitterClient(twitterCredentials.CONSUMER_KEY,
                twitterCredentials.CONSUMER_SECRET,
                token,
                secret
            );

            var user = await UserClient.Users.GetAuthenticatedUserAsync();
            PlayerPrefs.SetString(KEY_USER_ID, user.IdStr);
            return user;
        }

        public async Task<string> GetCodeUrl(TwitterCredentials twitterCredentials)
        {
            RegisterApp(twitterCredentials);

            // Start the authentication process
            _authenticationRequest = await AppClient.Auth.RequestAuthenticationUrlAsync();

            return _authenticationRequest.AuthorizationURL;
        }

        public async Task<IAuthenticatedUser> ConnectWithCode(string code)
        {
            if (_authenticationRequest == null)
            {
                return null;
            }

            // With this pin code it is now possible to get the credentials back from Twitter
            var userCredentials =
                await AppClient.Auth.RequestCredentialsFromVerifierCodeAsync(code, _authenticationRequest);

            PlayerPrefs.SetString(KEY_ACCESS_TOKEN, userCredentials.AccessToken);
            PlayerPrefs.SetString(KEY_ACCESS_TOKEN_SECRET, userCredentials.AccessTokenSecret);

            // You can now save those credentials or use them as followed
            UserClient = new TwitterClient(userCredentials);

            var user = await UserClient.Users.GetAuthenticatedUserAsync();
            PlayerPrefs.SetString(KEY_USER_ID, user.IdStr);
            return user;
        }

        public async Task<ITwitterResult> PostImage(FileAsset asset, string text)
        {
            // taken from https://github.com/linvi/tweetinvi/issues/1212

            var imageBinary = asset.Bytes;

            // we upload image using v1.1
            var uploadedImage = await UserClient.Upload.UploadTweetImageAsync(imageBinary);

            // we post the tweet using v2
            var poster = new TweetsV2Poster(UserClient);

            var result = await poster.PostTweet(
                new TweetV2PostRequest(text, uploadedImage)
            );

            if (!result.Response.IsSuccessStatusCode)
            {
                throw new Exception(
                    "Error when posting tweet: " + Environment.NewLine + result.Content
                );
            }

            return result;
        }

        [Obsolete("This API form Twitter is not working. Please don't use this")]
        public async Task<ITwitterResult> SchedulePostImage(FileAsset asset, string text, DateTime scheduleTime)
        {
            // more info: https://developer.twitter.com/en/docs/twitter-ads-api/creatives/api-reference/scheduled-tweets#post-accounts-account-id-scheduled-tweets
            var userId = PlayerPrefs.GetString(KEY_USER_ID, null);

            var imageBinary = asset.Bytes;

            // we upload image using v1.1
            var uploadedImage = await UserClient.Upload.UploadTweetImageAsync(imageBinary);

            // we post the tweet using v2
            var poster = new TweetsV2Poster(UserClient);

            var tweetParams = new TweetV2PostRequest(text, uploadedImage);
            var result = await poster.ScheduleTweet(tweetParams, userId);

            if (!result.Response.IsSuccessStatusCode)
            {
                throw new Exception(
                    "Error when posting tweet: " + Environment.NewLine + result.Content
                );
            }

            return result;
        }
    }
}