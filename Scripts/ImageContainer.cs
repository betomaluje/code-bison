using BerserkPixel.File_Explorer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImageContainer : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _imageName;
    [SerializeField] private TextMeshProUGUI _imageDate;

    private FileAsset _asset;
    private ImageDetails _imageDetails;

    private void Awake()
    {
        _imageDetails = FindObjectOfType<ImageDetails>();
    }

    public void SetupAsset(FileAsset asset)
    {
        _asset = asset;

        _image.color = Color.white;
        _image.sprite = asset.Sprite;
        _image.preserveAspect = true;
        _imageName.text = asset.Name;
        _imageDate.text = asset.CreationDate;
    }

    public void OnMouseDown()
    {
        _imageDetails.SetupAsset(_asset);
        _imageDetails.Show();
    }
}