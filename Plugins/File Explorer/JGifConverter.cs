using System.Collections.Generic;
using System.IO;
using System.Linq;
using MG.GIF;
using UnityEngine;

namespace BerserkPixel.File_Explorer
{
    public class JGifConverter
    {
        // amount of the initial images to show. Keep it low so the device doesn't crash
        private const int AMOUNT_OF_IMAGES = 20;
        
        public static FileAsset GetPreviewGif(byte[] fileData,
            Vector2 pivot,
            string spriteName,
            string filePath,
            FileInfo fileInfo,
            string dateFormat)
        {
            var sprites = new List<Sprite>();
            var delays = new List<float>();

            using (var decoder = new Decoder(fileData.ToArray()))
            {
                var img = decoder.NextImage();
                var currentImages = 0;
                while (img != null && currentImages < AMOUNT_OF_IMAGES)
                {
                    var texture = img.CreateTexture();
                    var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                        pivot, 1);

                    sprites.Add(sprite);

                    delays.Add(img.Delay / 1000.0f);

                    img = decoder.NextImage();
                    currentImages++;
                }
            }

            var asset = new FileAsset(
                sprites[0],
                spriteName,
                fileInfo.CreationTime.ToString(dateFormat),
                filePath,
                fileData
            );

            asset.SetGifImages(sprites, delays);

            return asset;
        }

        public static FileAsset GetFullGif(FileAsset previewAsset)
        {
            var pivot = new Vector2(0.5f, 0.5f);
            var fileData = previewAsset.Bytes;
            var spriteName = previewAsset.Name;
            var filePath = previewAsset.Path;

            var sprites = new List<Sprite>();
            var delays = new List<float>();

            using (var decoder = new Decoder(fileData.ToArray()))
            {
                var img = decoder.NextImage();
                while (img != null)
                {
                    var texture = img.CreateTexture();
                    var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                        pivot, 1);

                    sprites.Add(sprite);

                    delays.Add(img.Delay / 1000.0f);

                    img = decoder.NextImage();
                }
            }

            var asset = new FileAsset(
                previewAsset.Sprite,
                spriteName,
                previewAsset.CreationDate,
                filePath,
                fileData
            );

            asset.SetGifImages(sprites, delays);

            return asset;
        }
    }
}