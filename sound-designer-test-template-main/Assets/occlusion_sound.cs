using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class occlusion_sound : MonoBehaviour
{

    public GameObject PlayerObject; // ссылка на объект плейера для того, чтобы найти дистанции
    public LayerMask OcclusionLayer; // слой, который блокирует лучи
    public float SoundWidth; // расстояние до левого и правого векторов у источника звука. Эмитация на сколько большой размер источника звука
    public float ListenerWidth; // расстояние левого и правого векторов у листенера, компенсация вращения камеры у персонажа
    public Vector3 SoundHeight; // это высота от куда будут кидаться лучи. Компенсация того, что геймобъект располагается в ногах персонажа
    public Vector3 PlayerHeight; // То же самое для листенера

   public float lineCastHitCount = 0f; // переменная для хранения количества лучей, заблокированных поверхностью
    
    private float updateRate = 10f; // переменная для частоты прогона рейкастов. Сейчас 10 раз в секунду
    private float updateStep;
    private float lastUpdateTime;

    void Start()
    {
        updateStep = 1f / updateRate;
        LastUpdateTimeReset();
    }
    private void FixedUpdate()
    {
        
        if(Time.realtimeSinceStartup - lastUpdateTime > updateStep) // запуск счетчика, чтобы работало 10 раз в секнуду
        {
            CheckOcclusion(transform.position + SoundHeight, PlayerObject.transform.position + PlayerHeight); // функция на проверку прохождения лучей
            lineCastHitCount = 0f;
            LastUpdateTimeReset();
        }
        
        
    }

    void LastUpdateTimeReset()
    {
        lastUpdateTime = Time.realtimeSinceStartup;
    }

    void CheckOcclusion(Vector3 sound, Vector3 player) // кидаем рейкасты
    {
        Vector3 SoundLeft = CalculatePoint(sound, player, SoundWidth); // высчитываем вектор и расчитываем точку слева от звука и до личтенера
        Vector3 SoundRight = CalculatePoint(sound, player, -SoundWidth);

        Vector3 PlayerLeft = CalculatePoint(player, sound , ListenerWidth);
        Vector3 PlayerRight = CalculatePoint(player, sound, -ListenerWidth);


        CastLine(SoundLeft, PlayerLeft); // запускаем луч слева от звука до левой точки от листенера
        CastLine(SoundLeft, player);
        CastLine(SoundLeft, PlayerRight);

        CastLine(sound, PlayerLeft);
        CastLine(sound, player);
        CastLine(sound, PlayerRight);

        CastLine(SoundRight, PlayerLeft);
        CastLine(SoundRight, player);
        CastLine(SoundRight, PlayerRight);

        SetParameter(); // функция, которая управляет RTPC в зависимости от количества лучей

    }
    
    private Vector3 CalculatePoint(Vector3 a, Vector3 b, float m) // формула расчета точек для звука и листенера
    {
        float x;
        float z;
        float n = Vector3.Distance(new Vector3(a.x, 0f, a.z), new Vector3(b.x, 0f, b.z));
        float mn = (m / n);
        
            x = a.x + (mn * (a.z - b.z));
            z = a.z - (mn * (a.x - b.x));
       
        return new Vector3(x, a.y, z);
    }

    private void CastLine(Vector3 Start, Vector3 End)
    {
        RaycastHit hit;
        Physics.Linecast(Start, End, out hit, OcclusionLayer);

        if (hit.collider)
        {
           lineCastHitCount++;
            Debug.DrawLine(Start, End, Color.red); // можно удалить строчку, так как это дебаггинг, который рисует красный луч если ударился в поверхность и не прошел до листенера
        }
        else
            Debug.DrawLine(Start, End, Color.green); // рисует луч если прошел
    }

    private void SetParameter()
    {
        AkSoundEngine.SetRTPCValue("RTPC_Occlusion", lineCastHitCount / 9); // устанавливаем RTPC в нужное положение
    }


}


