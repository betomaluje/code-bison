using System;
using BerserkPixel.File_Explorer;
using DG.Tweening;
using TMPro;
using UI.Dates;
using UnityEngine;
using UnityEngine.UI;

public class ImageDetails : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _imageName;

    [Header("Post Manager")] 
    [SerializeField] private SocialPlatformsManager _socialPlatforms;

    [Header("Input")] 
    [SerializeField] private bool _checkForEmptyPrompt;
    [SerializeField] private TMP_InputField _promptField;
    [SerializeField] private Slider _characterCount;
    [SerializeField] private DatePicker _datePicker;

    [Header("Full Screen Image")] 
    [SerializeField] private Image _fullScreenImage;
    
    [Header("Open Post Panel")]
    [SerializeField] private Button _openPostContainer;
    [SerializeField] private TextMeshProUGUI _openPostText;

    [Header("Animations")] 
    [SerializeField] private RectTransform _writePostContainer;
    [SerializeField] private CanvasGroup _writePostCanvasGroup;
    [SerializeField] private float _animDuration = .35f;
    [SerializeField] private Ease _animEase = Ease.Linear;

    private FileAsset _asset;
    private DateTime _scheduledTime = DateTime.Now;

    // for gif purposes
    private int _curFrame = 0;
    private float _gifTime = 0.0f;
    private bool _playingGif;

    private void Start()
    {
        // we update the UI to today's date
        var now = new SerializableDate(_scheduledTime);
        _datePicker.SelectedDate = now;
        
        // reset some values
        _openPostText.DOFade(0, 0);
        _fullScreenImage.DOFade(0, 0);
        _openPostContainer.image.DOFade(0, 0);
    }

    public void Show()
    {
        Click_CloseWritePost();
        ChangeFullScreenImage();
    }

    private void ChangeFullScreenImage()
    {
        _fullScreenImage.preserveAspect = true;
        _fullScreenImage.gameObject.SetActive(true);
        _fullScreenImage.transform.localScale = Vector3.one;
        _fullScreenImage.DOFade(1, _animDuration).SetEase(_animEase).OnComplete(() =>
        {
            if (_asset.IsGif)
            {
                _curFrame = 0;
                _playingGif = true;
            }
            else
            {
                _fullScreenImage.sprite = _asset.Sprite;
            } 
        });
    }

    public void SetupAsset(FileAsset asset)
    {
        _playingGif = false;
        _asset = asset;

        _image.sprite = asset.Sprite;
        _image.preserveAspect = true;
        _imageName.text = asset.Name;
    }

    public void SetScheduleTime(DateTime dateTime)
    {
        // we save the current chosen time to set it afterwards
        var timeSpan = _scheduledTime.TimeOfDay;
        _scheduledTime = dateTime;
        _scheduledTime = _scheduledTime.Date + timeSpan;
    }

    public void SetTime(string time)
    {
        var split = time.Split(":");
        var hour = int.Parse(split[0]);
        var minutes = int.Parse(split[1]);
        var timeSpan = new TimeSpan(hour, minutes, 0);
        _scheduledTime = _scheduledTime.Date + timeSpan;
    }

    public void CheckCharacterCount(string text)
    {
        var count = text.Length;
        _characterCount.DOValue(count, _animDuration, true).SetEase(_animEase);
    }

    public void Publish()
    {
        if (_asset == null)
        {
            ConsolePanel.Instance.WriteConsole("You need to get the Code from your social platforms first", -1);
            return;
        }

        if (_checkForEmptyPrompt)
        {
            if (string.IsNullOrEmpty(_promptField.text))
            {
                ConsolePanel.Instance.WriteConsole("Please add a prompt to post");
                return;
            }
        }
        
        LoadingPanel.LoadingStartEvent.Invoke();

        if (_asset.IsGif)
        {
            var completeAsset = JGifConverter.GetFullGif(_asset);
            _socialPlatforms.Post(completeAsset, _promptField.text, null, false);
        }
        else
        {
            _socialPlatforms.Post(_asset, _promptField.text, null, false);   
        }
    }

    public void SchedulePost()
    {
        if (_asset == null)
        {
            ConsolePanel.Instance.WriteConsole("You need to get the Code from your social platforms first", -1);
            return;
        }

        if (_checkForEmptyPrompt)
        {
            if (string.IsNullOrEmpty(_promptField.text))
            {
                ConsolePanel.Instance.WriteConsole("Please add a prompt to post");
                return;
            }
        }
        
        LoadingPanel.LoadingStartEvent.Invoke();
        
        if (_asset.IsGif)
        {
            var completeAsset = JGifConverter.GetFullGif(_asset);
            _socialPlatforms.Post(completeAsset, _promptField.text, _scheduledTime, true);
        }
        else
        {
            _socialPlatforms.Post(_asset, _promptField.text, _scheduledTime, true);   
        }
    }

    #region Full Screen Image

    public void OnPointerEnter()
    {
        if (_asset == null) return;

        FadeInPostButton();
    }

    public void OnPointerExit()
    {
        _playingGif = false;

        FadeOutPostButton();
    }

    public void Click_OpenWritePost()
    {
        _writePostContainer.gameObject.SetActive(true);
        
        _writePostContainer.DOAnchorPos(new Vector2(0, 0), _animDuration).SetEase(_animEase);
        _writePostCanvasGroup.DOFade(1, _animDuration * 2).SetEase(_animEase);
    }
    
    public void Click_CloseWritePost()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(_writePostCanvasGroup.DOFade(0, _animDuration).SetEase(_animEase));
        sequence.Append(_writePostContainer.DOAnchorPos(new Vector2(0, -1000f), _animDuration)
            .SetEase(_animEase)
            .OnComplete(() => _writePostContainer.gameObject.SetActive(false)));
    }

    private void FadeInPostButton()
    {
        _openPostContainer.image.DOFade(1, _animDuration);
        _openPostText.DOFade(1, _animDuration);
    }

    private void FadeOutPostButton()
    {
        _openPostText.DOFade(0, _animDuration);
        _openPostContainer.image.DOFade(0, _animDuration);
    }
    
    private void Update()
    {
        if (!_playingGif)
        {
            return;
        }

        _gifTime += Time.deltaTime;

        if (_gifTime >= _asset.FrameDelay[_curFrame])
        {
            _curFrame = (_curFrame + 1) % _asset.GifImages.Count;
            _gifTime = 0.0f;

            _fullScreenImage.sprite = _asset.GifImages[_curFrame];
        }
    }

    #endregion
}