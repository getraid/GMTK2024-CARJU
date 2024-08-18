using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSfxManager : MonoBehaviour
{
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

    bool requestingCarChange = false;
    int currentMusic = 0;
    char currentMusicPart = '?';

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            StartMusic(1);
        }
        if(Input.GetKeyDown(KeyCode.RightArrow)){
            RequestCarUpgrade();
        }

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
            Debug.LogError("What the fuck is this music ?");
        }
    }

    void PlayNext(AudioSource src){
        Debug.Log("Playing next : " + src.name);
        src.PlayScheduled(nextEventTime);
        nextEventTime += (double)src.clip.samples / src.clip.frequency;
    }

    void TriggerUpgrade(){
        MusicUpgradeHappened?.Invoke();
        Debug.Log("NOW UPGRADE TO LEVEL " + currentMusic.ToString());
    }

    public void RequestCarUpgrade()
    {
        if(requestingCarChange){
            Debug.LogWarning("You've already requested a car upgrade that hasn't happened yet !");
        }else{
            Debug.LogWarning("Car upgrade request registered. Please wait for the music loop to finish");
            requestingCarChange = true;
        }
    }
}
