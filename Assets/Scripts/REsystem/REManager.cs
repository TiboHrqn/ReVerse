using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


// le voice record ne fonctionne plus avec l'arrivée du redo : il faut retirer l'enregistrement des voix d'ici et l'ajouter aux recordSegment

[RequireComponent(typeof(VoiceRecord))]
public class REManager : MonoBehaviour  
{
    // SINGLETON PART
    private static REManager instance = null;
    public static REManager Instance => instance;
    //

    public bool isPlaying { get; private set; }
    public bool isRecording { get; private set; }
    public bool isReplaying { get; private set; }
    public bool isSimulationPaused { get; private set; }

    public float simulationTime { get; private set; }

    [SerializeField] 
    private float recordDelay;
    private float timer;
    private float timeDiff;

    [SerializeField] private RecSetupConfig recSetupConfig;

    [SerializeField] private GameObject avatar;

    private VoiceRecord voiceRecord; //
    

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
        

    private void Start()
    {
        voiceRecord = GetComponent<VoiceRecord>();
        RecSetup.LoadConfig(recSetupConfig);
        RecSetup.SetupAllGO();                 /// a bouger plus tard peut etre
        isPlaying = true;
        isRecording = false;
        isReplaying = false;
        isSimulationPaused = false;
        timer = 0;
        simulationTime = 0;
        timeDiff = 0;
        ScriptDisabler.Instance.FindScriptsToDisable();
        ScriptEnabler.Instance.FindScriptsToEnable();
        if (avatar != null)
        {
            avatar.SetActive(false);
        }
        else
        {
            Debug.Log("pas d'avatar assigné");
        }
        REUIManager.Instance.HideUI();
    }
    

    private void Update()
    {
        timer += Time.deltaTime;
        float time = Time.time;       
        if (isRecording && timer >= recordDelay)
        {
            simulationTime = time - timeDiff;
            Recorder.Instance.Record(simulationTime);
            timer = 0;
        }
        if (!isSimulationPaused)
        {
            simulationTime = time - timeDiff;
            if(isReplaying)
            {
                if (simulationTime < Storer.Instance.lastTime)
                {
                    ReplayInstant(simulationTime);
                    REUIManager.Instance.UpdateSliderDuringReplay(simulationTime);
                }
                else
                {
                    simulationTime = Storer.Instance.lastTime;
                    Pause();
                }
            }
            
        }
        
        
    }

    public void StartRecord()
    {
        isRecording = true;
        voiceRecord.StartRecord();
        REUIManager.Instance.HideUI();
    }

    public void EndRecord()
    {   
        Storer.Instance.UpdateFirstLastTimeValue();
        isRecording = false;
        voiceRecord.EndRecord();
    }

    public void TryGoToInstant(float time) //appelé par le slider
    {
        if(!isRecording && !isPlaying && isSimulationPaused)
        {
            ReplayInstant(time);
            simulationTime = time;
            timeDiff = Time.time - simulationTime;
            isReplaying = true;
            Pause();
            //avatar.SetActive(true);

        }
    }

    public void ReplayInstant(float time) //seulement un replay visuel
    {
        Storer.Instance.RestoreUnactivesStates(time);
    }

    private void EndPlay()
    {
        ScriptDisabler.Instance.DisableScripts();
        Storer.Instance.DisableActiveComponents();
        isPlaying = false;
    }

    private void StartPlay()
    {
        ScriptDisabler.Instance.EnableScripts();
        ScriptEnabler.Instance.DisableScripts();
        Storer.Instance.RestoreAllStates(simulationTime);

    }


    public void StartReplay()
    {
        if(isRecording)
        {
            EndRecord();
        }
        EndPlay();
        Pause();
        Storer.Instance.WriteStorerTrace();
        REUIManager.Instance.UpdateReviewUI();
        ScriptEnabler.Instance.EnableScripts();
        float startTime = Storer.Instance.firstTime;
        isReplaying = true;
        simulationTime = startTime;
        timeDiff = Time.time - simulationTime;
        //avatar.SetActive(true);
        ReplayInstant(startTime);
    }

    private void EndReplay()
    {
        isReplaying = false;
        Pause();
        //avatar.SetActive(false);
    }

    private void Pause()
    {
        simulationTime = Time.time - timeDiff;
        isSimulationPaused = true;
        REUIManager.Instance.UpdatePauseImage(true);
        Storer.Instance.PausePausableComponents();
        voiceRecord.PauseReplay();
    }

    private void Unpause()
    {
        timeDiff = Time.time - simulationTime;
        isSimulationPaused = false;
        REUIManager.Instance.UpdatePauseImage(false);
        voiceRecord.StartReplay(simulationTime - Storer.Instance.firstTime);
    }

    

    public void PauseUnpause()
    {
        if (isSimulationPaused)
        {
            Unpause();
        }
        else
        {
            Pause();
        }
    }

    public void StartRedo()
    {
        if(simulationTime <= Storer.Instance.firstTime  || simulationTime >= Storer.Instance.lastTime)
        {
            return;
        }
        Storer.Instance.Split(simulationTime);
        timeDiff = Time.time - simulationTime;
        StartPlay();
        StartRecord();
        
    }

}
