using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class REUIManager : MonoBehaviour
{
    // SINGLETON PART
    private static REUIManager instance = null;
    public static REUIManager Instance => instance;
    //

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject); 
    }

    [SerializeField]
    private Transform headTransform;

    [SerializeField]
    private Slider slider;
    [SerializeField]
    private TextMeshProUGUI timer;
    [SerializeField]
    private TextMeshProUGUI segmentSequence;
    [SerializeField]
    private GameObject uiMenuGO;
    [SerializeField]
    private Image pauseButtonImage;
    [SerializeField]
    private Sprite pauseSprite;
    [SerializeField]
    private Sprite playSprite;
    [SerializeField]
    private TMP_Dropdown segmentDropdown;
    [SerializeField]
    private Button minusButton;
    [SerializeField]
    private Button plusButton;

    private float firstTime;
    private float lastTime;
    private float recordDuration;
    private bool justChanged;

    void Start()
    {
       
        if (slider == null)
        {
            Debug.LogError("Pas de slider assigné");
        }
        else
        {
            
            slider.onValueChanged.AddListener(OnSliderValueChanged);
            slider.gameObject.SetActive(false);

        }
    }

    void OnSliderValueChanged(float sliderValue)
    {
        float time = Mathf.Lerp(firstTime,lastTime,sliderValue);
        if (!justChanged)
        {
            REManager.Instance.TryGoToInstant(time);
        }
        justChanged = false;
        UpdateTimeDisplayer(time);
        
    }

    public void UpdateSliderDuringReplay(float time)
    {
        justChanged = true;
        slider.value = 1 - (lastTime - time)/recordDuration;
    }

    public void UpdateReviewUI()
    {
        Storer.Instance.UpdateFirstLastTimeValue();
        firstTime = Storer.Instance.firstTime;
        lastTime = Storer.Instance.lastTime;
        recordDuration = lastTime - firstTime;
        slider.gameObject.SetActive(true);
        slider.value = 0;
        UpdateTimeDisplayer(firstTime);
        ShowSegmentSequence();
        UpdateDropdownValues();
        ShowUI();
    }

    private string ConvertToMMSS(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        string formattedTime = string.Format("{0:00}:{1:00}", minutes, seconds);
        return formattedTime;
    }

    public void UpdateTimeDisplayer(float time)
    {
        timer.text = ConvertToMMSS(time - firstTime) + " / " + ConvertToMMSS(recordDuration);

    }

    public void UpdatePauseImage(bool isPaused)
    {
        if (isPaused)
        {
            pauseButtonImage.sprite = playSprite;
        }
        else
        {
            pauseButtonImage.sprite = pauseSprite;
        }
    }

    public void UpdateUIPosition()
    {
        if (!REManager.Instance.isPlaying)
        {
            ShowUI();
            uiMenuGO.SetActive(true);
            uiMenuGO.transform.position = headTransform.position + new Vector3(headTransform.forward.x, -0.3f, 1.3f * headTransform.forward.z); //rajouter un facteur spawnDist au besoin
            uiMenuGO.transform.LookAt(headTransform);
            uiMenuGO.transform.Rotate(0, 180, 0);
        } 
    }

    public void HideUI()
    {
        uiMenuGO.SetActive(false);
    }

    public void ShowUI()
    {
        uiMenuGO.SetActive(true);

    }

    public void ShowSegmentSequence()
    {
        string segmentSeq = "";
        foreach (RecordSegment recSeg in Storer.Instance.currentReviewSegments)
        {
            segmentSeq += recSeg.segmentNumber + " > ";
        }
        segmentSequence.text = segmentSeq;
    }

    public void RemoveLastSegment()
    {
        Storer.Instance.RemoveLastSegmentInReview();
        UpdateReviewUI();
    }

    public void AddSegment()
    {
        int selectedSegmentNumber = int.Parse(segmentDropdown.options[segmentDropdown.value].text);
        Storer.Instance.currentReviewSegments.Add(Storer.recSegDict[selectedSegmentNumber]);
        UpdateReviewUI();

    }

    private void UpdateDropdownValues()
    {
    
        segmentDropdown.ClearOptions(); // Efface les options existantes

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        foreach (RecordSegment recSeg in Storer.Instance.currentReviewSegments[Storer.Instance.currentReviewSegments.Count -1].nextSegments)
        {

            options.Add(new TMP_Dropdown.OptionData(recSeg.segmentNumber.ToString())); // Ajoute chaque valeur à la liste d'options
        }

        segmentDropdown.AddOptions(options); // Ajoute les options au dropdown

        if (segmentDropdown.options.Count > 0)
        {
            segmentDropdown.value = 0; // Sélectionne la première option par défaut
            plusButton.interactable = true;
        }
        else
        {
            plusButton.interactable = false;
        }
        if (Storer.Instance.currentReviewSegments.Count <= 1)
        {
            minusButton.interactable = false;
        }
        else
        {
            minusButton.interactable = true;
        }

    }

}
