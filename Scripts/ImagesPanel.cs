using System.IO;
using BerserkPixel.File_Explorer;
using SFB;
using TMPro;
using UnityEngine;

public class ImagesPanel : MonoBehaviour
{
    [SerializeField] private Transform _imagesContainer;
    [SerializeField] private ImageContainer _imagePrefab;

    [Header("Folder Selection")] 
    [SerializeField] private TMP_InputField _folderInput;

    private void Start()
    {
        var savedPath = PlayerPrefs.GetString(FileExplorer.KEY_LAST_FOLDER, "");

        if (!string.IsNullOrEmpty(savedPath))
        {
            LoadImages(savedPath);
        }
        else
        {
            // Display some sort of empty view
        }
    }

    private bool IsValidFolder(string path) => Directory.Exists(path);

    public void OpenFolder()
    {
        var loadedPath = StandaloneFileBrowser.OpenFolderPanel("Select Directory", "", false);
        
        LoadImages(loadedPath.Length > 0 ? loadedPath[0] : null);
    }

    public void Refresh()
    {
        LoadImages(_folderInput.text);
    }

    public async void LoadImages(string path)
    {
        LoadingPanel.LoadingStartEvent.Invoke();
        
        if (string.IsNullOrEmpty(path))
        {
            ConsolePanel.Instance.WriteConsole($"Invalid folder {path}");
            LoadingPanel.LoadingEndEvent.Invoke();
            return;
        }

        if (!IsValidFolder(path))
        {
            ConsolePanel.Instance.WriteConsole($"Invalid folder {path}");
            LoadingPanel.LoadingEndEvent.Invoke();
            return;
        }

        PlayerPrefs.SetString(FileExplorer.KEY_LAST_FOLDER, path);

        _folderInput.text = path;

        var allImages = await FileExplorer.Instance.LoadImages(path);

        _imagesContainer.DestroyAllChildren();

        foreach (var asset in allImages.Values)
        {
            var image = Instantiate(_imagePrefab, _imagesContainer);
            image.SetupAsset(asset);
        }
        
        LoadingPanel.LoadingEndEvent.Invoke();
    }

    private void OnDestroy()
    {
        if (_imagesContainer != null && _imagesContainer.childCount > 0)
            _imagesContainer.DestroyAllChildrenImmediate();
    }
}