using System.Threading.Tasks;
using BerserkPixel.File_Explorer;
using BerserkPixel.Mastodon;

namespace Posting
{
    public class MastodonPoster : SocialPoster
    {
        public override async void Post(FileAsset asset, string text, bool scheduled = false)
        {
            base.Post(asset, text, scheduled);

            if (MastodonAPI.Instance.IsInitialised)
            {
                var status = await Toot(scheduled);

                if (status.Success)
                {
                    OnPostDone?.Invoke(status.Message);
                }
                else
                {
                    OnPostError?.Invoke(status.Message);
                }
            }
            else
            {
                OnPostError?.Invoke("You need to get the Code from Mastodon first");
            }
        }

        private async Task<PostStatus> Toot(bool scheduled = false)
        {
            OnPostStart?.Invoke();

            if (scheduled)
            {
                var status = await MastodonAPI.Instance.SchedulePostImage(_asset, _text, _scheduledTime);
                var postStatus = new PostStatus(true)
                {
                    Message =
                        $"Scheduled post {_asset.Name} at {_scheduledTime:yyyy-MM-dd HH:mm:ss} / <a href=\"{status.Url}\" />"
                };

                return postStatus;
            }
            else
            {
                var status = await MastodonAPI.Instance.PostImage(_asset, _text);
                OnPostDone?.Invoke($"Done posting {_asset.Name} / <a href=\"{status.Url}\" />");

                var postStatus = new PostStatus(true)
                {
                    Message = $"Done posting {_asset.Name} / <a href=\"{status.Url}\" />"
                };

                return postStatus;
            }
        }
    }
}