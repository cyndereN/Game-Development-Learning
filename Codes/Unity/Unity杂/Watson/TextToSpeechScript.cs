using IBM.Watson.TextToSpeech.V1;
using IBM.Watson.TextToSpeech.V1.Model;
using IBM.Cloud.SDK.Utilities;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Authentication.Iam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IBM.Cloud.SDK;

public class TextToSpeechScript : MonoBehaviour
{
    #region
    [Space(10)]
    [Tooltip("The IAM apikey")]
    [SerializeField]
    private string iamApiKey;
    [SerializeField]
    private string serviceUrl;
    #endregion

    private TextToSpeechService service;

    private string responseVoice = "en-US_AllisonV3Voice";
    private string defaultText = "Hello, welcome to the Watson Unity SDK!";
    private string synthesizeMimeType = "audio/wav";

    private List<string> voiceName = new List<string>();
    bool namesReady = false;

    // Start is called before the first frame update
    void Start()
    {
        LogSystem.InstallDefaultReactors();
        Runnable.Run(CreateService());
    }

    private IEnumerator CreateService()
    {
        if(string.IsNullOrEmpty(iamApiKey))
        {
            throw new IBMException("Please add IAM ApiKey");
        }

        IamAuthenticator authenticator = new IamAuthenticator(apikey: iamApiKey);

        while (!authenticator.CanAuthenticate())
            yield return null;

        service = new TextToSpeechService(authenticator);
        if(!string.IsNullOrEmpty(serviceUrl))
        {
            service.SetServiceUrl(serviceUrl);
        }
        GetVoicesList();
    }

    private void PlayClip(AudioClip clip)
    {
        if(Application.isPlaying && clip != null)
        {
            GameObject audioObject = new GameObject("AudioObject");
            AudioSource source = audioObject.AddComponent<AudioSource>();
            source.spatialBlend = 0.0f;
            source.loop = false;
            source.clip = clip;
            source.Play();

            GameObject.Destroy(audioObject, clip.length);
        }
    }

    private IEnumerator SynthesizeText(string text)
    {
        if(string.IsNullOrEmpty(text))
        {
            text = defaultText;
        }

        byte[] synthesizeResponse = null;
        AudioClip clip = null;
        service.Synthesize(
            callback: (DetailedResponse<byte[]> response, IBMError error) =>
            {
                synthesizeResponse = response.Result;
                clip = WaveFile.ParseWAV("myClip", synthesizeResponse);
                PlayClip(clip);
            },
            text: text,
            voice: responseVoice,
            accept: synthesizeMimeType
        );

        while (synthesizeResponse == null)
            yield return null;

        yield return new WaitForSeconds(clip.length);
    }

    public void SynthesizeThis(string theWords)
    {
        Runnable.Run(SynthesizeText(theWords));
    }

    private void GetVoicesList()
    {
        Voices theVoiceList = null;
        List<Voice> AvailableVoices = new List<Voice>();

        service.ListVoices(
            callback: (DetailedResponse<Voices> response, IBMError error) =>
            {
                theVoiceList = response.Result;
                AvailableVoices = theVoiceList._Voices;

                foreach (var res in AvailableVoices)
                {
                    voiceName.Add(res.Name);
                }

                namesReady = true;
            }
            );
    }

    public List<string> GetName()
    {
        return voiceName;
    }

    public bool NameListReady()
    {
        if (namesReady)
        {
            namesReady = false;
            return true;
        }
        else
            return false;
    }

    public void SetVoice(string voiceSet)
    {
        responseVoice = voiceSet;
    }
}
