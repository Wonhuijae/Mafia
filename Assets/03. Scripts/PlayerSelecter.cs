using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;

public class PlayerSelecter : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private List<CharacterInfo> infoes;
    [SerializeField]
    private GameObject checkedImage;
    [SerializeField]
    private GameObject[] characterButtons;

    public Dictionary<CharacterInfo, GameObject> spawnDict = new();

    public static event Action<CharacterInfo> OnChangeCharacter;

    public Transform defaultSpawnPos;
    public Transform previewPos;

    private Outline outline;
    private CharacterInfo selectInfo;

    private int charIdx;

    private NetworkManager instance;

    private void Awake()
    {
        foreach (CharacterInfo info in infoes)
        {
            spawnDict.Add(info, Instantiate(info.charModel, previewPos.position, Quaternion.identity));
            spawnDict[info].SetActive(false);
        }

        NetworkManager.OnJoinRoom += PlayerInit;
    }

    // 최초 캐릭터 설정
    public void PlayerInit()
    {
        CheckCharacter();
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "charIdx", charIdx } });
        SpawnPlayer(charIdx);
    }

    // 캐릭터가 이미 생성되어 있는지 확인
    // 다른 플레이어에게 선점되어 있다면 사용할 수 없도록 함
    private void CheckCharacter()
    {
        Hashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
        bool[] selectedChars = (bool[])roomProperties["selectedChars"];

        for (int i = 0; i < selectedChars.Length; i++)
        {
            Debug.Log(selectedChars[i]);
            if (!selectedChars[i])
            {
                charIdx = i;
                selectedChars[i] = true;
                roomProperties["selectedChars"] = selectedChars;

                PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
                break;
            }
        }
    }

    void SpawnPlayer(int selectIdx)
    {
        SpawnPlayer(selectIdx, defaultSpawnPos.position);
    }

    void SpawnPlayer(int selectIdx, Vector3 spawnPos)
    {
        var player = PhotonNetwork.Instantiate("Player" + selectIdx, spawnPos, Quaternion.identity);
    }

    // 캐릭터 선택
    public void SelectCharacter(CharacterInfo _characterInfo)
    {
        Hashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
        bool[] selectedChars = (bool[])roomProperties["selectedChars"];

        if (selectInfo != null) spawnDict[selectInfo].SetActive(false);
        if (selectInfo == _characterInfo) return;

        selectInfo = _characterInfo;
        spawnDict[_characterInfo].SetActive(true);

        // 캐릭터 모델 바꾸기
        OnChangeCharacter(selectInfo);
    }

    // 프로퍼티 값이 바뀌면 UI 업데이트
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("selectedChars"))
        {
            UpdateSelectView();
        }
    }

    // 캐릭터 선택 UI 수정
    void UpdateSelectView()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("selectedChars", out object selectedCharsObj))
        {
            bool[] selectedChars = (bool[])selectedCharsObj;

            for (int i = 0; i < characterButtons.Length; i++)
            {
                characterButtons[i].SetActive(selectedChars[i]);
            }
        }
    }

    // 현재 사용자가 선택한 버튼에 Outline
    public void SetButton(Outline _outline)
    {
        if (outline != null) outline.enabled = false;
        if (outline == _outline)
        {
            _outline.enabled = true;
            return;
        }

        outline = _outline;
    }
}
