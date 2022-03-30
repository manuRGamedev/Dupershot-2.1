using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    #region VARIABLES

    public static AudioManager instance;
    [Header("Music")]
    [SerializeField] SoundtrackCollection[] SoundtrackCollections;
    [SerializeField] Dictionary<string, SoundtrackCollection> musicDic = new Dictionary<string, SoundtrackCollection>();
    Queue<Soundtrack> songQueue;
    [SerializeField] float collectionTransitionTime;
    [SerializeField] float songTransitionTime;

    Soundtrack newSong;
    //Para controllar lo que está sonando ahora mismo.
    Soundtrack currentSoundtrack;
    string currentCollection;
    public bool isMusicPlaying = true;

    private AudioSource soundtrackSource;
    private AudioSource auxAudioSource;

    [Header("FX")]
    [SerializeField] SoundCollection[] soundCollections;
    [SerializeField] Dictionary<string, SoundCollection> soundDic = new Dictionary<string, SoundCollection>();

    #endregion

    #region MONOBEHAVIOUR METHODS
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        songQueue = new Queue<Soundtrack>();
        soundtrackSource = gameObject.AddComponent<AudioSource>();
        auxAudioSource = gameObject.AddComponent<AudioSource>();

        isMusicPlaying = false;

        //Inicializa un diccionario con las colecciones de soundtracks
        foreach (SoundtrackCollection soundtrackCollection in SoundtrackCollections)
        {            
            musicDic.Add(soundtrackCollection.colectionName, soundtrackCollection);
        }

        //Inicializa un diccionario con las colecciones de sonidos
        foreach (SoundCollection soundCollection in soundCollections)
        {
            soundCollection.Initialize();

            foreach (Sound sound in soundCollection.sounds)
            {
                //Initialise Sounds
                sound.source = gameObject.AddComponent<AudioSource>();
                sound.source.clip = sound.clip;

                sound.source.volume = sound.volume * PlayerPrefs.GetFloat("MasterVolume") * PlayerPrefs.GetFloat("FxVolume");
                sound.source.pitch = sound.pitch;
                sound.source.loop = sound.loop;
            }
            //Se añade el sonido al diccionario
            soundDic.Add(soundCollection.colectionName, soundCollection);
        }

        currentCollection = string.Empty;
    }

    
    private void Update()
    {
        if (isMusicPlaying && !soundtrackSource.isPlaying)
        {
            if (songQueue.Count > 0)
            {
                Soundtrack nextSoundtrack = songQueue.Dequeue();

                SetSong(nextSoundtrack);
            }
            else
            {
                if (currentCollection != null)
                {
                    SetCurrentTheme(currentCollection, currentSoundtrack);
                }
            }
        }
    }    

    #endregion

    #region MUSIC METHODS
    public void SetMusicPlaying(bool value)
    {
        isMusicPlaying = value;
    }
    /// <summary>
    /// Esablece la nueva SountrackCollection que estará sonando.
    /// </summary>
    /// <param name="themeName"></param>
    /// 
    public void SetCurrentTheme(string themeName)
    {
        songQueue.Clear();

        foreach (Soundtrack st in musicDic[themeName].ReturnPlaylist())
        {
            songQueue.Enqueue(st);
        }

        currentCollection = themeName;

        if (isMusicPlaying)
        {
            isMusicPlaying = false;
            StartCoroutine(SongSwapCorroutine(songQueue.Dequeue()));
        }
        else
        {
            isMusicPlaying = false;
            SetSong(songQueue.Dequeue());
        }        
    }

    /// <summary>
    /// Esablece la nueva SountrackCollection que estará sonando, quitando de la playlist el último track que ha sonado (para evitar repetir canciones).
    /// </summary>
    /// <param name="themeName"></param>
    public void SetCurrentTheme(string themeName, Soundtrack lastSong)
    {
        songQueue.Clear();

        foreach (Soundtrack st in musicDic[themeName].ReturnPlaylist(lastSong))
        {
            songQueue.Enqueue(st);
        }

        currentCollection = themeName;

        if (isMusicPlaying)
        {
            isMusicPlaying = false;
            StartCoroutine(SongSwapCorroutine(songQueue.Dequeue()));
        }
        else
        {
            isMusicPlaying = false;
            SetSong(songQueue.Dequeue());
        }
    }

    public void SetSong(Soundtrack song)
    {
        currentSoundtrack = song;

        soundtrackSource.clip = song.clip;
        soundtrackSource.volume = song.volume * PlayerPrefs.GetFloat("MusicVolume") * PlayerPrefs.GetFloat("MasterVolume");

        soundtrackSource.Play();

        isMusicPlaying = true;
    }

    IEnumerator SongSwapCorroutine(Soundtrack newSong)
    {
        auxAudioSource.clip = newSong.clip;
        auxAudioSource.volume = 0f;
        auxAudioSource.Play();

        float time = 0f;
        float startVolume = soundtrackSource.volume;

        while (time < songTransitionTime)
        {
            soundtrackSource.volume = Mathf.Lerp(startVolume, 0, time / songTransitionTime);
            auxAudioSource.volume = Mathf.Lerp(0f, newSong.volume * PlayerPrefs.GetFloat("MusicVolume") * PlayerPrefs.GetFloat("MasterVolume"), time / songTransitionTime);
            time += Time.deltaTime;
            yield return null;
        }
        soundtrackSource.clip = newSong.clip;
        soundtrackSource.volume = newSong.volume * PlayerPrefs.GetFloat("MusicVolume") * PlayerPrefs.GetFloat("MasterVolume");
        soundtrackSource.Play();
        soundtrackSource.time = songTransitionTime;
        auxAudioSource.Stop();

        isMusicPlaying = true;
    }    

    #endregion

    #region FX METHODS
    public void Play (string name)
    {
        SoundCollection soundCollection = soundDic[name];

        Sound choosenSound = soundCollection.GetSound();

        choosenSound.source.pitch = choosenSound.pitch + Random.Range(-choosenSound.randomPitchVariation, choosenSound.randomPitchVariation);
        choosenSound.source.Play();
    }

    public void PlayAtPitch(string name, float pitch)
    {
        SoundCollection soundCollection = soundDic[name];

        Sound choosenSound = soundCollection.GetSound();

        //Ajusta el volumen y pitch del sonido
        choosenSound.source.pitch = pitch;
        
        //Reproduce el sonido
        choosenSound.source.Play();
    }

    #endregion

    #region CONFIG METHODS

    /// <summary>
    /// Ajusta el volumen de todos los sonidos del juego acorde a las opciones escogidas por el jugador.
    /// </summary>
    public void ModifyAudioVolumes()
    {
        //Se estblecen los ajustes que se refieren a los sonidos
        foreach (SoundCollection soundCollection in soundCollections)
        {
            foreach (Sound sound in soundCollection.sounds)
            {
                sound.source.volume = sound.volume * PlayerPrefs.GetFloat("MasterVolume") * PlayerPrefs.GetFloat("FxVolume");
            }
        }
        //ATENCION: ES NECESARIO IMPLEMENTAR EL AJUSTE DE AUDIO REFERENTE AL SOUNDTRACK
        if (currentSoundtrack != null)
        {
            soundtrackSource.volume = currentSoundtrack.volume * PlayerPrefs.GetFloat("MasterVolume") * PlayerPrefs.GetFloat("MusicVolume");
        }
    }

    #endregion
}

#region AUDIO CLASSES

[System.Serializable]
public class Sound
{
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.1f, 3f)] public float pitch = 1f;
    [Range(0f, 1f)] public float randomPitchVariation = 0f;
    public bool loop;
    [Range(0.01f, 1f)] public float probability = 1f;

    [HideInInspector] public AudioSource source;
}

[System.Serializable]
public class SoundCollection
{
    public string colectionName = "New Sound Collection";
    public Sound[] sounds;

    private float selectionRange = 0f;

    public void Initialize()
    {
        selectionRange = 0f;

        for (int i = 0; i < sounds.Length; i++)
        {
            selectionRange += sounds[i].probability;
        }
    }

    /// <summary>
    /// Choose a random sound of the collection
    /// </summary>
    /// <returns></returns>
    public Sound GetSound()
    {
        if (sounds.Length == 1)
        {
            return sounds[0];
        }
        else
        {
            if (selectionRange == 0f)
            {
                int randomIndex = Random.Range(0, sounds.Length);
                return sounds[randomIndex];
            }

            else
            {
                float randomSelection = Random.Range(0, selectionRange);
                float probCount = 0f;

                foreach (Sound s in sounds)
                {
                    probCount += s.probability;

                    if (randomSelection <= probCount)
                    {
                        return s;
                    }
                }

                return sounds[0];
            }
        }
    }
}

[System.Serializable]
public class Soundtrack
{
    public AudioClip clip;
    [Range(0f,1f)]public float volume;
    public int loopAmount = 1;
}

[System.Serializable]
public class SoundtrackCollection
{
    public string colectionName = "New Soundtrack Collection";
    public Soundtrack[] soundtracks;

    public Soundtrack[] ReturnPlaylist(Soundtrack lastSoundtrack)
    {
        //Crea una lista de soundtracks para realizar la selección
        List<Soundtrack> storedSoundtacks = new List<Soundtrack>();

        if (storedSoundtacks.Count > 1 && storedSoundtacks.Contains(lastSoundtrack))
        {
            storedSoundtacks.Remove(lastSoundtrack);
        }

        //Crea la lista en la que se definirá la playlist que devolverá en forma de Array
        List<Soundtrack> playlist = new List<Soundtrack>();

        foreach (Soundtrack st in soundtracks)
        {
            storedSoundtacks.Add(st);
        }

        int forloopIterations = storedSoundtacks.Count;

        //Archiva en orden aleatorio los temas storeados.
        for (int i = 0; i< forloopIterations; i++)
        {
            int randomIndex = Random.Range(0, storedSoundtacks.Count);
            Soundtrack choosenSoundtrack = storedSoundtacks.ToArray()[randomIndex];

            for (int j = 0; j < choosenSoundtrack.loopAmount; j++)
            {
                playlist.Add(choosenSoundtrack);
            }

            storedSoundtacks.Remove(choosenSoundtrack);
        }

        //Devuelve la playlistCreada
        return playlist.ToArray();
    }

    public Soundtrack[] ReturnPlaylist()
    {
        //Crea una lista de soundtracks para realizar la selección
        List<Soundtrack> storedSoundtacks = new List<Soundtrack>();

        //Crea la lista en la que se definirá la playlist que devolverá en forma de Array
        List<Soundtrack> playlist = new List<Soundtrack>();

        foreach (Soundtrack st in soundtracks)
        {
            storedSoundtacks.Add(st);
        }

        int forloopIterations = storedSoundtacks.Count;

        //Archiva en orden aleatorio los temas storeados.
        for (int i = 0; i < forloopIterations; i++)
        {
            int randomIndex = Random.Range(0, storedSoundtacks.Count);
            Soundtrack choosenSoundtrack = storedSoundtacks.ToArray()[randomIndex];

            for (int j = 0; j < choosenSoundtrack.loopAmount; j++)
            {
                playlist.Add(choosenSoundtrack);
            }

            storedSoundtacks.Remove(choosenSoundtrack);
        }

        //Devuelve la playlistCreada
        return playlist.ToArray();
    }
}

#endregion