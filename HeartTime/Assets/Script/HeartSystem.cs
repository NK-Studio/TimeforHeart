using System;
using UnityEngine;
using UnityEngine.UI;

public class HeartSystem : MonoBehaviour
{
    [Header("하트 갯수")] 
    private int number;

    //하트를 추가하는 타이머가 실행 중인지 체크
    private bool _isAddTimer;
    
    //하트가 소모된 시점의 시간 변수
    private DateTime preTime;

    [Header("시간을 화면에 표시")]
    public Text TimeTx;
    
    [Header("하트 이미지들")]
    public GameObject[] Hearts;

    [Header("하트를 재 충전하는 시간 초")]
    public int HeartSecond = 15;
    
    private void Start()
    {
        //초기에 가지고 있는 하트 갯수를 하트 이미지 갯수 만큼 처리함.
        number = Hearts.Length;
        
        //PT를 정의한 저장이 없다면 아래 코드 구문 실행 X
        if (!PlayerPrefs.HasKey("Heart_PreTime")) return;
        
        //가지고있었던 하트 갯수 만큼 처리함
        number = PlayerPrefs.GetInt("Heart_Number", Hearts.Length);
        
        //하트의 갯수가 풀빵이라면 아래 코드 구문 실행 X
        if (number == Hearts.Length) return;
        
        //하트를 소모한 시간을 가져옴
        var HeartPreTime = DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString("Heart_PreTime")));
        
        //하트를 소모했던 그 시간을 가져온다,
        preTime = HeartPreTime;

        //(게임을 켠 시간) - 하트를 소모한 시간 
        var CheckTime = DateTime.UtcNow - HeartPreTime;

        //추가될 갯수만큼 추가
        number += Mathf.Clamp((int)CheckTime.TotalSeconds / HeartSecond, 0, Hearts.Length);

        //추가를 했음에도 불구하고 채워야되는 하트가 아직 있을 경우
        if (number < Hearts.Length)
            _isAddTimer = true;
        else
        {
            preTime = DateTime.UtcNow;
            PlayerPrefs.SetInt("Heart_Number",5);
            PlayerPrefs.SetString("Heart_PreTime","");
        }
    }

    private void Update()
    {
        //하트 렌더링 시스템
        HeartsSystem();

        //시간에 따라 하트를 채워주는 시스템
        TimeForAddHeartSystem();
    }

    //게임이 일시정지 or 종료 되었을 때
    private void OnApplicationQuit()
    {
        //하트를 추가하는 타이머가 실행되지 않았다면 아래 코드 구문 실행 X
        if (!_isAddTimer) return;

        //하트에 대한 데이터를 저장 함.
        PlayerPrefs.SetInt("Heart_Number",number);
        PlayerPrefs.SetString("Heart_PreTime",preTime.ToBinary().ToString());
    }

    //화면에 하트를 그려내는 시스템
    private void HeartsSystem()
    {
        //숫자가 하트 이미지 갯수보다 넘어가는 것을 방지
        number = Mathf.Clamp(number, 0, Hearts.Length);

        //전체적으로 돌면서 하트를 안보이게 함
        foreach (var val in Hearts)
            val.SetActive(false);

        //넘버 갯수에 따라 하트를 보이게 함
        for (var i = 0; i < Hearts.Length; i++)
            if (number > i)
                Hearts[i].SetActive(true);
    }

    private void TimeForAddHeartSystem()
    {
        //하트 갯수가 하트 이미지 갯수보다 적지 않을 경우 아래 코드 구문 실행 X
        if (!(number < Hearts.Length) || preTime.Second == 0) return;

        var CheckTime = preTime - DateTime.UtcNow;

        var _time = (int) TimeSpan.FromSeconds(HeartSecond).TotalSeconds - Mathf.Abs((int) CheckTime.TotalSeconds);
        
        var minute = _time / 60;

        var second = _time - (minute*60);

        var second1 = second / 10;

        var second2 = second % 10;
        
        //스크린에 지나간 초 표현
        TimeTx.text = $"{minute}:{second1}{second2}";
        
        //second초가 지나지 않았다면 아래 코드 구문 실행 X
        if (!(Mathf.Abs((float) CheckTime.TotalSeconds) > HeartSecond)) return;
        
        //하트를 추가
        number++;

        //갯수가 여전히 안맞으면, 또 생성할 수 있도록 현재 시간을 리플레이스 해준다.
        if (number != Hearts.Length)
            //현재 시간을 기록 함.
            preTime = DateTime.UtcNow;
        else
        {
            _isAddTimer = false;
        }
    }

    //하트를 한개 감소 함.
    public void GameStart()
    {
        //하트를 추가하는 타이머가 실행하는 중이 아니라면, 현재 시간을 기록 함.
        if (!_isAddTimer)
        {
            //현재 시간을 기록 함.
            preTime = DateTime.UtcNow;

            Debug.Log(preTime);
                    
            //하트를 추가하는 타이머를 실행 함.
            _isAddTimer = true;
        }

        //하트 한개 사용
        number--;
    }

    
    

}