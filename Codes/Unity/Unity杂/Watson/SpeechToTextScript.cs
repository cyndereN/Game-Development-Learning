using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IBM.Watson.SpeechToText.V1; 
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Authentication.Iam;
using IBM.Cloud.SDK.Utilities;
using IBM.Cloud.SDK.DataTypes;


public class SpeechToTextScript : MonoBehaviour
{
    #region SET THESE VARIABLES IN THE INSPECTOR
    [Tooltip("The services URL (optional)")]
    [SerializeField]
    private string _serviceUrl;
    [Header("IAM Authentication")]
    [Tooltip("The IAM apikey")]
    [SerializeField]
    private string _iamApikey;
   
    [Header("Parameters")]
    [Tooltip("The Model to use. This deafaults to en-US_BroadbandModel")]
    [SerializeField]
    private string _recognizeModel;
    //For the Recognize models list go to: https://cloud.ibm.com/docs/speech-to-text?topic=speech-to-text-models 
    #endregion

    private int _recordingRoutine = 0;
    private string _microphoneID = null;
    private AudioClip _recording = null;
    private int _recordingBufferSize = 1;
    private int _recordingHZ = 22050;
    
    private SpeechToTextService _service;

    private string[] _keywordsSetHere;
    private string _currentKeyword;

    private string _finalTranscript;

    void Start()
    {
        LogSystem.InstallDefaultReactors();
        Runnable.Run(CreateService());
    }
    
    private IEnumerator CreateService()
    {
        if (string.IsNullOrEmpty(_iamApikey))
        {
            throw new IBMException("Please provide IAM apikey for this service");
        }

        IamAuthenticator authenticator = new IamAuthenticator(apikey: _iamApikey);

        while (!authenticator.CanAuthenticate())
            yield return null;

        _service = new SpeechToTextService(authenticator);

        if (!string.IsNullOrEmpty(_serviceUrl))
        {
            _service.SetServiceUrl(_serviceUrl);
        }

        Active = true;
        StartRecording();
    }

    public bool Active
    {
        get { return _service.IsListening; }
        set
        {
            if (value && !_service.IsListening)
            {
                _service.RecognizeModel = (string.IsNullOrEmpty(_recognizeModel) ? "en-US_BroadbandModel" : _recognizeModel);
                _service.DetectSilence = true;
                _service.EnableWordConfidence = true; 
                _service.SilenceThreshold = 0.01f;
                _service.MaxAlternatives = 1;
                _service.EnableInterimResults = true;   
                _service.OnError = OnError;
                _service.InactivityTimeout = -1;
                _service.WordAlternativesThreshold = null;
                _service.EndOfPhraseSilenceTime = null;
                _service.Keywords = _keywordsSetHere;
                _service.KeywordsThreshold = 0.01f;
                _service.StartListening(OnRecognize, OnRecognizeSpeaker);
            }
            else if (!value && _service.IsListening)
            {
                _service.StopListening();
            }
         
        }
    }

    private void StartRecording()
    {
        if (_recordingRoutine == 0)
        {
            UnityObjectUtil.StartDestroyQueue();
            _recordingRoutine = Runnable.Run(RecordingHandler());
        }
    }

    private void StopRecording()
    {
        if (_recordingRoutine != 0)
        {
            Microphone.End(_microphoneID);
            Runnable.Stop(_recordingRoutine);
            _recordingRoutine = 0;
        }
    }

    private void OnError(string error)
    {
        Active = false;
        Log.Debug("SpeechToTextScript.OnError()", "Error!{0}", error);
    }

    private IEnumerator RecordingHandler()
    {
        Log.Debug("SpeechToTextScript.RecordingHandler()", "devices: {0}", Microphone.devices);
        _recording = Microphone.Start(_microphoneID, true, _recordingBufferSize, _recordingHZ);
        yield return null;

        if (_recording == null)
        {
            StopRecording();
            yield break;
        }

        bool bFirstBlock = true;
        int midPoint = _recording.samples / 2;
        float[] samples = null;

        while (_recordingRoutine != 0 && _recording != null)
        {
            int writePos = Microphone.GetPosition(_microphoneID);
            if (writePos > _recording.samples || !Microphone.IsRecording(_microphoneID))
            {
                Log.Error("SpeechToTextScript.RecordingHandler()", "Microphone Disconnected");

                StopRecording();
                yield break;
            }

            if ((bFirstBlock && writePos >= midPoint) || (!bFirstBlock && writePos < midPoint))
            {
                samples = new float[midPoint];
                _recording.GetData(samples, bFirstBlock ? 0 : midPoint);

                AudioData record = new AudioData();
                record.MaxLevel = Mathf.Max(Mathf.Abs(Mathf.Min(samples)), Mathf.Max(samples));
                record.Clip = AudioClip.Create("Recording", midPoint, _recording.channels, _recordingHZ, false);
                record.Clip.SetData(samples, 0);

                _service.OnListen(record);

                bFirstBlock = !bFirstBlock;
            }
            else
            {
                int remaining = bFirstBlock ? (midPoint - writePos) : (_recording.samples - writePos);
                float timeRemaining = (float)remaining / (float)_recordingHZ;

                yield return new WaitForSeconds(timeRemaining);
            }
        }
        yield break;
    }
    
    private void OnRecognize(SpeechRecognitionEvent result)
    {
        if (result != null && result.results.Length > 0)
        {
            foreach (var res in result.results)
            {
                foreach (var alt in res.alternatives)
                {
                    string text = string.Format("{0}", alt.transcript);
                    Log.Debug("SpeechtoTextScript.OnRecognize()", text);
                    _finalTranscript = text;
                }

                if (res.keywords_result != null && res.keywords_result.keyword != null)
                {
                    foreach (var keyword in res.keywords_result.keyword)
                    {
                        string text = string.Format("{0}", keyword.normalized_text);
                        Log.Debug("SpeechtoTextScript.OnRecognize()", text + " Keyword Detected");
                        _currentKeyword = text;
                    }
                }
            }
        }
    }

    private void OnRecognizeSpeaker(SpeakerRecognitionEvent result)
    {
        //if (result != null)
        //{
        //    foreach (SpeakerLabelsResult labelResult in result.speaker_labels)
        //    {
        //        Log.Debug("SpeechtoTextScript.OnRecognizeSpeaker()", string.Format("speaker result: {0} | confidence: {3} | from: {1} | to: {2}", labelResult.speaker, labelResult.from, labelResult.to, labelResult.confidence));
        //    }
        //}
    }


   


    public void SetKeywords(string[] theKeywords)
    {
        _keywordsSetHere = theKeywords;
    }

    public string GetCurrentKeyword()
    {
        return _currentKeyword;
    }

    public string GetFinalTranscript()
    {
        return _finalTranscript;
    }
}  


//Was in OnRecognize
//if (result != null && result.results.Length > 0)
        //{
        //    foreach (var res in result.results)
        //    {
        //foreach (var alt in res.alternatives)
        //{
        //    string text = string.Format("{0}, {1}\n", alt.transcript, res.final/* ? "Final" : "Interim", alt.confidence*/);
        //    Log.Debug("SpeechtoTextScript.OnRecognize()", text);

        //}

        //if (res.keywords_result != null && res.keywords_result.keyword != null)
        //{
        //    foreach (var keyword in res.keywords_result.keyword)
        //    {
        //        Log.Debug("SpeechtoTextScript.OnRecognize()", "keyword: {0}, confidence: {1}, start time: {2}, end time: {3}", keyword.normalized_text, keyword.confidence, keyword.start_time, keyword.end_time);
        //    }
        //}

        //if (res.word_alternatives != null)
        //{
        //    foreach (var wordAlternative in res.word_alternatives)
        //    {
        //        Log.Debug("SpeechtoTextScript.OnRecognize()", "Word alternatives found. Start time: {0} | EndTime: {1}", wordAlternative.start_time, wordAlternative.end_time);
        //        foreach (var alternative in wordAlternative.alternatives)
        //            Log.Debug("SpeechtoTextScript.OnRecognize()", "\t word: {0} | confidence: {1}", alternative.word, alternative.confidence);
        //    }
        //}
        //}
        // }