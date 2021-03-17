﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using Photon.Pun;
using Photon.Realtime;

public class MainMenuManager : MonoBehaviour
{
    private const string MASTER_VOL_PREF = "settings.mastervolume",
        MUSIC_VOL_PREF = "settings.musicvolume",
        SFX_VOL_PREF = "settings.sfxvolume",
        UI_VOL_PREF = "settings.uivolume";
    
    private const string MASTER_MIX_VOL = "MasterVolume",
        MUSIC_MIX_VOL = "MusicVolume",
        SFX_MIX_VOL = "SFXVolume",
        UI_MIX_VOL = "UIVolume";

    private const string LEADERBOARD_NAME_PREF = "player.username";

    [SerializeField] private string playScene = "Scenes/Game";
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider uiVolumeSlider;
    [SerializeField] private TMPro.TMP_InputField leaderboardNameInput;
    [SerializeField] private TMPro.TMP_InputField gameCodeInput;
    [SerializeField] private Toggle privateGameToggle;
    private YieldInstruction waitForEndOfFrame;

    void Awake()
    {
        waitForEndOfFrame = new WaitForEndOfFrame();
    }

    void Start()
    {
        SetVolumeSliders();
        SetLeaderboardName();
    }

    public void OnPlayButtonClicked()
    {
        SceneManager.LoadScene(playScene);
    }

    public void OnJoinButtonClicked()
    {
        string roomName = gameCodeInput.text;
        if(roomName.Trim().Length == 0) StartCoroutine(CancelJoin());
        else PhotonNetwork.JoinRoom(roomName.ToUpper());
    }

    public void OnJoinRandomButtonClicked()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnCreateButtonClicked()
    {
        bool isPrivate = privateGameToggle.isOn;
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = NetworkManager.MAX_PLAYERS;
        options.IsVisible = !isPrivate;
        PhotonNetwork.CreateRoom(NetworkManager.GenerateRoomCode(), options, TypedLobby.Default);
    }

    public void OnMasterVolumeSliderChanged(float value)
    {
        masterMixer.SetFloat(MASTER_MIX_VOL, value);
        PlayerPrefs.SetFloat(MASTER_VOL_PREF, value);
    }

    public void OnMusicVolumeSliderChanged(float value)
    {
        masterMixer.SetFloat(MUSIC_MIX_VOL, value);
        PlayerPrefs.SetFloat(MUSIC_VOL_PREF, value);
    }

    public void OnSFXVolumeSliderChanged(float value)
    {
        masterMixer.SetFloat(SFX_MIX_VOL, value);
        PlayerPrefs.SetFloat(SFX_VOL_PREF, value);
    }

    public void OnUIVolumeSliderChanged(float value)
    {
        masterMixer.SetFloat(UI_MIX_VOL, value);
        PlayerPrefs.SetFloat(UI_VOL_PREF, value);
    }

    public void OnEditLeaderboardName(string value)
    {
        Leaderboard.Username = value;
        PlayerPrefs.SetString(LEADERBOARD_NAME_PREF, value);
	}

    private void SetLeaderboardName()
    {
        string username = PlayerPrefs.GetString(LEADERBOARD_NAME_PREF);
        if(string.IsNullOrEmpty(username)) 
        {
            Leaderboard.Username = "Guest";
        } else 
        {
            Leaderboard.Username = username;
        }

        leaderboardNameInput.text = Leaderboard.Username;
    }

    private void SetVolumeSliders()
    {
        float masterVolume = PlayerPrefs.GetFloat(MASTER_VOL_PREF);
        masterVolumeSlider.value = masterVolume;
        masterMixer.SetFloat(MASTER_MIX_VOL, masterVolume);

        float musicVolume = PlayerPrefs.GetFloat(MUSIC_VOL_PREF);
        musicVolumeSlider.value = musicVolume;
        masterMixer.SetFloat(MUSIC_MIX_VOL, musicVolume);

        float sfxVolume = PlayerPrefs.GetFloat(SFX_VOL_PREF);
        sfxVolumeSlider.value = sfxVolume;
        masterMixer.SetFloat(SFX_MIX_VOL, sfxVolume);

        float uiVolume = PlayerPrefs.GetFloat(UI_VOL_PREF);
        uiVolumeSlider.value = uiVolume;
        masterMixer.SetFloat(UI_MIX_VOL, uiVolume);
    }

    private IEnumerator CancelJoin()
    {
        yield return waitForEndOfFrame;
        NetworkManager.Singleton.OnJoinRoomFailedEvent.Invoke();
    }
}
