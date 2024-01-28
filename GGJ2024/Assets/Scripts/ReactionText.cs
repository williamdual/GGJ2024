using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReactionText : MonoBehaviour
{
    public float lifeTime = 2f;

    void Start()
    {
        //set alpha of text to 0
        Color c = GetComponent<TextMeshPro>().color;
        c.a = 0;
        GetComponent<TextMeshPro>().color = c;
        StartCoroutine(FadeInAndOut(lifeTime, GetComponent<TextMeshPro>()));
    }

    public virtual IEnumerator FadeInAndOut(float t, TextMeshPro i)
    {
        yield return null;
    }


    public void SetText(string text)
    {
        GetComponent<TextMeshPro>().text = text;
    }
    
    // public IEnumerator FadeInAndOut(float t, TextMeshPro i)
    // {
    //     i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
    //     Vector3 pos = transform.position;
    //     while (i.color.a < 1.0f)
    //     {
    //         pos.y += Time.deltaTime * 1f;
    //         pos.x += Random.Range(-2f, 2f) * Time.deltaTime;
    //         transform.position = pos;
    //         i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
    //         yield return null;
    //     }
    //     yield return new WaitForSeconds(1f);
    //     while (i.color.a > 0.0f)
    //     {
    //         i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / (t * 0.15f)));
    //         pos.y += Time.deltaTime * 0.5f;
    //         transform.position = pos;
    //         yield return null;
    //     }
    //     Destroy(gameObject);
    // }
}