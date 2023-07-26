using System;
using BerserkPixel.File_Explorer;

namespace Posting
{
    public abstract class SocialPoster
    {
        public Action OnPostStart = delegate { };
        public Action<string> OnPostDone = delegate { };
        public Action<string> OnPostError = delegate { };

        protected string _text;
        protected FileAsset _asset;

        protected DateTime _scheduledTime;

        public void SetTime(DateTime scheduledTime)
        {
            _scheduledTime = scheduledTime;
        }

        public virtual void Post(FileAsset asset, string text, bool scheduled = false)
        {
            _asset = asset;
            _text = text;
        }
    }
}