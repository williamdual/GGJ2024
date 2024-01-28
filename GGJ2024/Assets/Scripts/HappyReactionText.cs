using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HappyReactionText : ReactionText
{

    public override IEnumerator FadeInAndOut(float t, TextMeshPro i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        Vector3 pos = transform.position;
        TextMeshPro tmp = GetComponent<TextMeshPro>();
        while (i.color.a < 1.0f)
        {
            pos.y += Time.deltaTime * 1f;
            pos.x += Random.Range(-3f, 3f) * Time.deltaTime;
            transform.position = pos;
            tmp.paragraphSpacing += Time.deltaTime * 20f;
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / (t * 0.15f)));
            pos.y += Time.deltaTime * 0.5f;
            transform.position = pos;
            yield return null;
        }
        Destroy(gameObject);
    }
}