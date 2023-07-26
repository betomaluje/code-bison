using System.Threading.Tasks;
using BerserkPixel.File_Explorer;
using BerserkPixel.Twitter;

namespace Posting
{
    public class TwitterPoster : SocialPoster
    {
        public override async void Post(FileAsset asset, string text, bool scheduled = false)
        {
            base.Post(asset, text, scheduled);

            if (TwitterAPI.IsInitialised)
            {
                var status = await Tweet(scheduled);

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
                OnPostError?.Invoke("You need to get the Code from Twitter first");
            }
        }

        private async Task<PostStatus> Tweet(bool scheduled = false)
        {
            OnPostStart?.Invoke();

            var status = await TwitterAPI.Instance.PostImage(_asset, _text);

            var postStatus = new PostStatus(status.Response.IsSuccessStatusCode)
            {
                Message = $"Done posting {_asset.Name} / <a href=\"{status.Response.URL}\" />"
            };

            return postStatus;
        }
    }
}