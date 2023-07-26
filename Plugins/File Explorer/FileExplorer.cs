using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace BerserkPixel.File_Explorer
{
    public class FileExplorer
    {
        #region Singleton

        private FileExplorer()
        {
        }

        private static readonly FileExplorer _instance = null;

        public static FileExplorer Instance => _instance ?? new FileExplorer();

        #endregion

        public const string KEY_LAST_FOLDER = "explorer_last_folder";

        // all allowed file extensions we would like to load
        private readonly string[] _allowedExtensions = {".png", ".jpg", ".gif"};

        public async Task<Dictionary<string, FileAsset>> LoadImages(string path)
        {
            var loadedFiles = new Dictionary<string, FileAsset>();

            var dir = new DirectoryInfo(path);
            var allFilePaths = dir.EnumerateFiles("*.*", SearchOption.AllDirectories)
                .Where(file => _allowedExtensions.Any(file.Extension.ToLower().EndsWith))
                .OrderByDescending(file => file.CreationTime)
                .ToArray();

            var pivot = new Vector2(0.5f, 0.5f);
            const string dateFormat = "dd-MM-yyyy HH:mm";

            var duplicatedCounter = 0;

            foreach (var fileInfo in allFilePaths)
            {
                var filePath = $"{fileInfo.DirectoryName}/{fileInfo.Name}";

                if (!File.Exists(filePath)) continue;

                var extension = Path.GetExtension(filePath);

                var spriteName = Path.GetFileNameWithoutExtension(filePath);

                if (loadedFiles.ContainsKey(spriteName))
                {
                    spriteName += $" ({duplicatedCounter})";
                    duplicatedCounter++;
                }

                var fileData = await File.ReadAllBytesAsync(filePath);

                if (extension.Equals(".gif"))
                {
                    ProcessGifImage(
                        fileData,
                        pivot,
                        spriteName,
                        filePath,
                        fileInfo,
                        dateFormat,
                        ref loadedFiles
                    );
                }
                else
                {
                    ProcessStaticImage(
                        fileData,
                        pivot,
                        spriteName,
                        filePath,
                        fileInfo,
                        dateFormat,
                        ref loadedFiles
                    );
                }
            }

            return loadedFiles;
        }

        private static void ProcessGifImage(
            byte[] fileData,
            Vector2 pivot,
            string spriteName,
            string filePath,
            FileInfo fileInfo,
            string dateFormat,
            ref Dictionary<string, FileAsset> loadedFiles
        )
        {
            var asset = JGifConverter.GetPreviewGif(
                fileData,
                pivot,
                spriteName,
                filePath,
                fileInfo,
                dateFormat);

            loadedFiles.Add(spriteName, asset);
        }

        private static void ProcessStaticImage(
            byte[] fileData,
            Vector2 pivot,
            string spriteName,
            string filePath,
            FileInfo fileInfo,
            string dateFormat,
            ref Dictionary<string, FileAsset> loadedFiles
        )
        {
            var newTexture2D = new Texture2D(1, 1);

            newTexture2D.LoadImage(fileData);

            var newSprite = Sprite.Create(newTexture2D, new Rect(0, 0, newTexture2D.width, newTexture2D.height),
                pivot, 1);

            var asset = new FileAsset(
                newSprite,
                spriteName,
                fileInfo.CreationTime.ToString(dateFormat),
                filePath,
                fileData
            );

            loadedFiles.Add(spriteName, asset);
        }
    }
}