using System.Collections.Generic;
using UnityEngine;

namespace BerserkPixel.File_Explorer
{
    public class FileAsset
    {
        public Sprite Sprite { get; private set; }
        public string Name { get; private set; }
        public string CreationDate { get; private set; }
        public string Path { get; private set; }
        public byte[] Bytes { get; private set; }

        public List<Sprite> GifImages { get; private set; }
        public List<float> FrameDelay { get; private set; }

        public bool IsGif => GifImages.Count > 0 && FrameDelay.Count > 0;

        public FileAsset(Sprite sprite, string name, string creationDate, string path, byte[] bytes)
        {
            Sprite = sprite;
            Name = name;
            CreationDate = creationDate;
            Path = path;
            Bytes = bytes;

            GifImages = new List<Sprite>();
            FrameDelay = new List<float>();
        }

        public void SetGifImages(List<Sprite> images, List<float> delays)
        {
            GifImages.Clear();
            FrameDelay.Clear();
            
            GifImages = images;
            FrameDelay = delays;
        }
    }
}