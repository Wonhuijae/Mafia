using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;
using Photon.Realtime;

public class PlayerSelecter : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private List<CharacterInfo> infoes;
    [SerializeField]
    private GameObject[] characterButtons;

    public Dictionary<CharacterInfo, GameObject> spawnDict = new();
    
    public static event Action<GameObject> OnCharacterInit;
    public static event Action<string, int, int> OnChangeCharacter;

    public Transform defaultSpawnPos;
    public Transform previewPos;

    private Outline outline;
    private CharacterInfo selectInfo;

    private int charIdx;
    private int curIdx;

    private NetworkManager instance;

    private void Awake()
    {
        foreach (CharacterInfo info in infoes)
        {
            spawnDict.Add(info, Instantiate(info.charModel, previewPos.position, Quaternion.identity));
            Destroy(spawnDict[info].GetComponentInChildren<PhotonAnimatorView>());
            spawnDict[info].SetActive(false);
        }

        NetworkManager.OnJoinRoom += PlayerInit;
    }

    private void OnDisable()
    {
        NetworkManager.OnJoinRoom -= PlayerInit;
    }

    // 최초 캐릭터 설정
    public void PlayerInit()
    {
        // 중복 체크
        CheckCharacter();
        // 캐릭터 선점
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { PropertyKeyName.keyCharIdx, charIdx } });
        // 스폰
        SpawnPlayer(charIdx);
    }

    // 캐릭터가 이미 생성되어 있는지 확인
    // 다른 플레이어에게 선점되어 있다면 사용할 수 없도록 함
    private void CheckCharacter()
    {
        Hashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
        string keyName = PropertyKeyName.keySelectedChars;
        bool[] selectedChars = (bool[])roomProperties[keyName];

        for (int i = 0; i < selectedChars.Length; i++)
        {
            if (!selectedChars[i])
            {
                charIdx = i;
                curIdx = i;
                selectedChars[i] = true;
                roomProperties[keyName] = selectedChars;

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

        // 닉네임 색깔
        float[] nickColor = ColorToFloat(infoes[selectIdx].charColor);
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { PropertyKeyName.keyNickNameColor, nickColor } });
        
        if (OnCharacterInit != null) OnCharacterInit(player);
    }

    float[] ColorToFloat(Color color)
    {
        return new float[]{ color.r, color.g, color.b, color.a};
    }

    // 캐릭터 선택
    public void SelectCharacter(CharacterInfo _characterInfo)
    {
        Hashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
        bool[] selectedChars = (bool[])roomProperties[PropertyKeyName.keySelectedChars];
        if (selectedChars == null) return;

        int newIdx = infoes.IndexOf(_characterInfo);
        // 이미 선택되어 있을 경우 종료
        if (selectedChars[newIdx] == true) return;
        // 이전 선택과 같은 경우 종료
        if (selectInfo == _characterInfo) return;

        // 이전 프리뷰 오브젝트 있을 경우 비활성화
        if (selectInfo != null) spawnDict[selectInfo].SetActive(false);
        selectInfo = _characterInfo;
        spawnDict[_characterInfo].SetActive(true);

        // 캐릭터 모델 바꾸기
        OnChangeCharacter(selectInfo.charName, curIdx, newIdx);
        curIdx = newIdx;

        PhotonNetwork.LocalPlayer.SetCustomProperties
            (new Hashtable { { PropertyKeyName.keyNickNameColor, ColorToFloat(selectInfo.charColor) } });
    }

    // 프로퍼티 값이 바뀌면 UI 업데이트
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(PropertyKeyName.keySelectedChars))
        {
            UpdateSelectView();
        }
    }

    // 캐릭터 선택 UI 수정
    void UpdateSelectView()
    {
        Hashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;

        if (roomProperties.TryGetValue(PropertyKeyName.keySelectedChars, out object selectedCharsObj))
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
