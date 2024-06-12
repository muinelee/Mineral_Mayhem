using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EarthSlamShowExample
{
    public class CrackControl : MonoBehaviour
    {
        [SerializeField] Crack _CrackPrefab;
        [SerializeField] float _OpenValue;
        [SerializeField] float _Speed;
        [SerializeField] float _Range;


        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                StopAllCoroutines();
                StartCoroutine(Coroutine_CrackOpen());
            }
        }



        IEnumerator Coroutine_CrackOpen()
        {
            int range = Mathf.RoundToInt(_Range);
            Vector3 startPoint = transform.position;

            for(int i = 0; i < range; i+=_CrackPrefab.Length)
            {
                Crack crack = Instantiate(_CrackPrefab, transform);
                crack.transform.position = startPoint;
                crack.transform.forward = transform.forward;
                startPoint += transform.forward * _CrackPrefab.Length;
                //for (int j=0; j<crack.BlendShapeCount; j++)
                //{
                //   crack.SetBlendShape(j, 0);
                //}
                }
            yield return null;




        }



    }

}