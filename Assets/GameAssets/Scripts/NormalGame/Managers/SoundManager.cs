using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public List<AudioSource> PossibleAudioSource = new List<AudioSource>();
    public AudioSource AmbientSound;
    public float minSound, maxSound;

    private void Start ()
    {
        NormalAmbientSound();
    }

    [ContextMenu("Rename Sources & Add to list")]
    public void RenamePossibleSources ()
    {
        Transform child = transform.GetChild(1);
        int index = 1;
        foreach (Transform t in child)
        {
            t.gameObject.name = "Source_" + index.ToString();
            PossibleAudioSource.Add(t.GetComponent<AudioSource>());
            index++;
        }
    }

    public void LowerAmbientSound ()
    {
        if (AmbientSound != null)
        {
            AmbientSound.volume = minSound;
        }
    }

    public void NormalAmbientSound ()
    {
        if (AmbientSound != null)
        {
            AmbientSound.volume = maxSound;
        }
    }

    public void PlaySound ( string AudioFile , bool loopState = false , float Volume = 1f )
    {
        AudioClip _Clip = (AudioClip)Resources.Load("Sounds/" + AudioFile);
        AudioSource AS = ReturnOpenSource();
        AS.clip = _Clip;
        AS.loop = loopState;
        AS.volume = Volume;
        AS.Play();
    }

    public void pauseAmbientSound ()
    {
        if (AmbientSound != null)
        {
            AmbientSound.Pause();
        }
    }

    public void ShutDownAllAudio ()
    {
        for (int i = 0 ; i < PossibleAudioSource.Count ; i++)
        {
            PossibleAudioSource [i].Stop();
            PossibleAudioSource [i].clip = null;
        }
    }


    public void PlayAmbientSound ( string AudioFile )
    {
        AudioClip _Clip = (AudioClip)Resources.Load("Sounds/" + AudioFile);
        if (AmbientSound != null)
        {
            AmbientSound.clip = _Clip;
            AmbientSound.Play();
        }
    }

    public void StopAudio ( string ClipName )
    {
        for (int i = 0 ; i < PossibleAudioSource.Count ; i++)
        {
            if (PossibleAudioSource [i].clip)
            {
                if (PossibleAudioSource [i].clip.name == ClipName)
                {
                    PossibleAudioSource [i].Stop();
                    PossibleAudioSource [i].loop = false;
                    PossibleAudioSource [i].volume = 1f;
                }
            }
        }
    }

    AudioSource ReturnOpenSource ()
    {
        for (int i = 0 ; i < PossibleAudioSource.Count ; i++)
        {
            if (!PossibleAudioSource [i].isPlaying)
            {
                return PossibleAudioSource [i];
            }
        }

        return PossibleAudioSource [0];
    }

    public void PauseAllSounds ()
    {
        for (int i = 0 ; i < PossibleAudioSource.Count ; i++)
        {
            if (PossibleAudioSource [i].clip)
            {
                PossibleAudioSource [i].Pause();
            }
        }

        AmbientSound.Pause();
    }


    public void UnPauseAllSounds ()
    {
        for (int i = 0 ; i < PossibleAudioSource.Count ; i++)
        {
            if (PossibleAudioSource [i].clip)
            {
                PossibleAudioSource [i].UnPause();
            }
        }

        AmbientSound.UnPause();
    }
}
