using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager> { 

    public AudioClip[] m_musicClips;
    public AudioClip[] m_winClips;
    public AudioClip[] m_loseClips;
    public AudioClip[] m_bonusClips;

    [Range(0, 1)]
    public float m_musincVolume = 0.5f;

    [Range(0, 1)]
    public float m_fxVolume = 1.0f;

    public float _lowPitch = 0.95f;
    public float _highPitch = 1.15f;

    void Start() {
        PlayRandomMusic();
    }

    public AudioSource PlayClipAtPoint(AudioClip _clip, Vector3 _position, float _volume = 1f, bool _randomizePitch = true) {

        if (_clip != null) {
            GameObject go = new GameObject("SoundFX" + _clip.name);
            go.transform.position = _position;

            AudioSource source = go.AddComponent<AudioSource>();
            source.clip = _clip;
            
            //
            if (_randomizePitch) { 
                float randomPitch = Random.Range(_lowPitch, _highPitch);
                source.pitch = randomPitch;
            }

            source.Play();
            Destroy(go, _clip.length);
            return source;
        }
        return null;
    }

    public AudioSource PlayRandom(AudioClip[] _clips, Vector3 _position, float _volume = 1f) {
        if(_clips != null) {
            if(_clips.Length != 0) {
                int randomIndex = Random.Range(0, _clips.Length);

                if(_clips[randomIndex] != null) {
                    AudioSource source = PlayClipAtPoint(_clips[randomIndex], _position, _volume);
                    return source;
                }// if3
            }// if2
        }// if1

        return null;
    }

    public void PlayRandomMusic() {
        PlayRandom(m_musicClips, Vector3.zero, m_musincVolume);
    }
    public void PlayWinSound() {
        PlayRandom(m_winClips, Vector3.zero, m_musincVolume);
    }
    public void PlayLoseSound() {
        PlayRandom(m_loseClips, Vector3.zero, m_musincVolume);
    }
    public void PlayBonusSound() {
        PlayRandom(m_bonusClips, Vector3.zero, m_musincVolume);
    }
}
