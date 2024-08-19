using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SingleSfx {
    public String name;
    public AudioClip clip;
}

public class MusicSfxManager : MonoBehaviour
{
    [SerializeField] bool logMessages = false;

    public Action MusicUpgradeHappened { get; set; }
    double nextEventTime = Double.PositiveInfinity;

    public AudioSource music_1_start;
    public AudioSource music_1_main;
    public AudioSource music_1_loopback;

    public AudioSource music_2_start;
    public AudioSource music_2_main_a;
    public AudioSource music_2_main_b;
    public AudioSource music_2_main_c;
    public AudioSource music_2_loopback;
    public AudioSource music_2_end_a;
    public AudioSource music_2_end_b;
    public AudioSource music_2_end_c;

    public AudioSource music_3_start;
    public AudioSource music_3_main_a;
    public AudioSource music_3_main_b;
    public AudioSource music_3_main_c;
    public AudioSource music_3_loopback;
    public AudioSource music_3_end_a;
    public AudioSource music_3_end_b;
    public AudioSource music_3_end_c;

    public AudioSource music_4_start;
    public AudioSource music_4_main_a;
    public AudioSource music_4_main_b;
    public AudioSource music_4_main_c;
    public AudioSource music_4_loopback;
    public AudioSource music_4_end_a;
    public AudioSource music_4_end_b;
    public AudioSource music_4_end_c;

    public AudioSource music_5_main;

    public GameObject singleSfxPlayerPrefab;
    public GameObject singleSfxContainer;

    public List<SingleSfx> singleSfxs;
    public AudioClip policeSfx;
    public AudioClip policeSfxWithVoice;

    bool requestingCarChange = false;
    int currentMusic = 0;
    char currentMusicPart = '?';

    float lastPolice = float.NegativeInfinity;
    float lastPoliceWithVoice = float.NegativeInfinity;

    // Update is called once per frame
    void Update()
    {
        //DEBUG
        if(Input.GetKeyDown(KeyCode.Space)){
            PlaySingleSfx(gameObject);
        }/*
        if(Input.GetKeyDown(KeyCode.RightArrow)){
            RequestCarUpgrade();
        }*/

        double time = AudioSettings.dspTime;
        if(time + 0.2d >= nextEventTime){
            ChooseNextAudio();
        }
    }

    public void StartMusic(int startLevel = 1)
    {
        currentMusic = startLevel;
        nextEventTime = AudioSettings.dspTime + 0.2f; //.2 seconds for buffer
        currentMusicPart = 'a';
        switch (startLevel)
        {
            case 1 : PlayNext(music_1_start);break;
            case 2 : PlayNext(music_2_loopback);break;
            case 3 : PlayNext(music_3_loopback);break;
            case 4 : PlayNext(music_4_loopback);break;
            case 5 : PlayNext(music_5_main);break;
            default: Debug.LogError("Invalid start music id : needs to be between 1 and 5");break;
        }
    }

    void ChooseNextAudio()
    {
        if(currentMusic == 1){
            if(music_1_loopback.isPlaying){
                PlayNext(music_1_main);
            }else{
                if(requestingCarChange){
                    Invoke("TriggerUpgrade", (float)(nextEventTime + 0.8d - AudioSettings.dspTime));
                    PlayNext(music_2_start);
                    PlayNext(music_2_main_a);
                    currentMusic = 2;
                    currentMusicPart = 'a';
                    requestingCarChange = false;
                    
                }else{
                    PlayNext(music_1_loopback);
                }
            }
        }else if(currentMusic == 2){
            if(music_2_loopback.isPlaying){
                PlayNext(music_2_main_a);
                currentMusicPart = 'a';
            }else{
                if(requestingCarChange){
                    Invoke("TriggerUpgrade", (float)(nextEventTime + 0.75d - AudioSettings.dspTime));
                    if(currentMusicPart == 'a'){
                        PlayNext(music_2_end_a);
                    }else if(currentMusicPart == 'b'){
                        PlayNext(music_2_end_b);
                    }else{
                        PlayNext(music_2_end_c);
                    }
                    PlayNext(music_3_start);
                    PlayNext(music_3_main_a);
                    currentMusic = 3;
                    currentMusicPart = 'a';
                    requestingCarChange = false;
                }else{
                    if(currentMusicPart == 'a'){
                        PlayNext(music_2_main_b);
                        currentMusicPart = 'b';
                    }else if(currentMusicPart == 'b'){
                        PlayNext(music_2_main_c);
                        currentMusicPart = 'c';
                    }else{
                        PlayNext(music_2_loopback);
                        currentMusicPart = 'a';
                    }
                }
            }
        }else if(currentMusic == 3){
            if(music_3_loopback.isPlaying){
                PlayNext(music_3_main_a);
                currentMusicPart = 'a';
            }else{
                if(requestingCarChange){
                    Invoke("TriggerUpgrade", (float)(nextEventTime + 0.66666666d - AudioSettings.dspTime));
                    if(currentMusicPart == 'a'){
                        PlayNext(music_3_end_a);
                    }else if(currentMusicPart == 'b'){
                        PlayNext(music_3_end_b);
                    }else{
                        PlayNext(music_3_end_c);
                    }
                    PlayNext(music_4_start);
                    PlayNext(music_4_main_a);
                    currentMusic = 4;
                    currentMusicPart = 'a';
                    requestingCarChange = false;
                }else{
                    if(currentMusicPart == 'a'){
                        PlayNext(music_3_main_b);
                        currentMusicPart = 'b';
                    }else if(currentMusicPart == 'b'){
                        PlayNext(music_3_main_c);
                        currentMusicPart = 'c';
                    }else{
                        PlayNext(music_3_loopback);
                        currentMusicPart = 'a';
                    }
                }
            }
        }else if(currentMusic == 4){
            if(music_4_loopback.isPlaying){
                PlayNext(music_4_main_a);
                currentMusicPart = 'a';
            }else{
                if(requestingCarChange){
                    Invoke("TriggerUpgrade", (float)(nextEventTime + 0.66666666d - AudioSettings.dspTime));
                    if(currentMusicPart == 'a'){
                        PlayNext(music_4_end_a);
                    }else if(currentMusicPart == 'b'){
                        PlayNext(music_4_end_b);
                    }else{
                        PlayNext(music_4_end_c);
                    }
                    PlayNext(music_5_main);
                    currentMusic = 5;
                    currentMusicPart = '?';
                    requestingCarChange = false;
                }else{
                    if(currentMusicPart == 'a'){
                        PlayNext(music_4_main_b);
                        currentMusicPart = 'b';
                    }else if(currentMusicPart == 'b'){
                        PlayNext(music_4_main_c);
                        currentMusicPart = 'c';
                    }else{
                        PlayNext(music_4_loopback);
                        currentMusicPart = 'a';
                    }
                }
            }
        }else if(currentMusic != 5){
            if (logMessages)
            {
                Debug.LogError("What the fuck is this music ?");
            }
        }
    }

    void PlayNext(AudioSource src){
        if (logMessages)
        {
            Debug.Log("Playing next : " + src.name);
        }

        src.PlayScheduled(nextEventTime);
        nextEventTime += (double)src.clip.samples / src.clip.frequency;
    }

    void TriggerUpgrade(){
        MusicUpgradeHappened?.Invoke();

        if (logMessages)
        {
            Debug.Log("NOW UPGRADE TO LEVEL " + currentMusic.ToString());
        }
    }

    public void RequestCarUpgrade()
    {
        if(requestingCarChange){
            if (logMessages)
            {
                Debug.LogWarning("You've already requested a car upgrade that hasn't happened yet !");
            }
        }else{
            if (logMessages)
            {
                Debug.LogWarning("Car upgrade request registered. Please wait for the music loop to finish");
            }
            requestingCarChange = true;
        }
    }

    public void TryPoliceSfx(){
        if(Time.time - lastPoliceWithVoice > 200f && Time.time - lastPolice > 30f){
            GameObject newSingleSfxPlayer = Instantiate(singleSfxPlayerPrefab, singleSfxContainer.transform);
            newSingleSfxPlayer.GetComponent<AudioSource>().clip = policeSfxWithVoice;
            lastPoliceWithVoice = Time.time;
            lastPolice = Time.time;
        }else if(Time.time - lastPolice > 30f){
            GameObject newSingleSfxPlayer = Instantiate(singleSfxPlayerPrefab, singleSfxContainer.transform);
            newSingleSfxPlayer.GetComponent<AudioSource>().clip = policeSfx;
            lastPolice = Time.time;
        }
    }

    public void PlaySingleSfx(GameObject gameObject){
        GameObject newSingleSfxPlayer = Instantiate(singleSfxPlayerPrefab, singleSfxContainer.transform);
        if(gameObject.name == "music_sfx_manager"){ // this is an exemple called using PlaySingleSfx(gameObject);
            newSingleSfxPlayer.GetComponent<AudioSource>().clip = singleSfxs.Find(singleSfx => singleSfx.name == "barril").clip;
        }/*else if(gameObject.name == "..."){
            //here are all the one shot sound effects that exist, not too sure what the destroyed gameobjects' names are tho
            newSingleSfxPlayer.GetComponent<AudioSource>().clip = singleSfxs.Find(singleSfx => singleSfx.name == "barril").clip;
            newSingleSfxPlayer.GetComponent<AudioSource>().clip = singleSfxs.Find(singleSfx => singleSfx.name == "big_building_break").clip;
            newSingleSfxPlayer.GetComponent<AudioSource>().clip = singleSfxs.Find(singleSfx => singleSfx.name == "bush").clip;
            newSingleSfxPlayer.GetComponent<AudioSource>().clip = singleSfxs.Find(singleSfx => singleSfx.name == "car_crash").clip;
            newSingleSfxPlayer.GetComponent<AudioSource>().clip = singleSfxs.Find(singleSfx => singleSfx.name == "cone_hit").clip;
            newSingleSfxPlayer.GetComponent<AudioSource>().clip = singleSfxs.Find(singleSfx => singleSfx.name == "fence_break").clip;
            newSingleSfxPlayer.GetComponent<AudioSource>().clip = singleSfxs.Find(singleSfx => singleSfx.name == "fuel").clip;
            newSingleSfxPlayer.GetComponent<AudioSource>().clip = singleSfxs.Find(singleSfx => singleSfx.name == "metal_bin").clip;
            newSingleSfxPlayer.GetComponent<AudioSource>().clip = singleSfxs.Find(singleSfx => singleSfx.name == "metal_post_big").clip;
            newSingleSfxPlayer.GetComponent<AudioSource>().clip = singleSfxs.Find(singleSfx => singleSfx.name == "metal_post_small").clip;
            newSingleSfxPlayer.GetComponent<AudioSource>().clip = singleSfxs.Find(singleSfx => singleSfx.name == "small_building_break").clip;
            newSingleSfxPlayer.GetComponent<AudioSource>().clip = singleSfxs.Find(singleSfx => singleSfx.name == "stone_wall_break").clip;
            newSingleSfxPlayer.GetComponent<AudioSource>().clip = singleSfxs.Find(singleSfx => singleSfx.name == "tree").clip;
            newSingleSfxPlayer.GetComponent<AudioSource>().clip = singleSfxs.Find(singleSfx => singleSfx.name == "ui_click").clip;
        }*/else{
            Debug.LogError("Trying to PlaySingleSfx on a unknown gameobject name");
            Destroy(newSingleSfxPlayer);
        }
    }
}
