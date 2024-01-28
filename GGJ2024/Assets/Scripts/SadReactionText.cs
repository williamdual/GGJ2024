using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SadReactionText : ReactionText
{

    public override IEnumerator FadeInAndOut(float t, TextMeshPro i)
    {
        float originalXScale = transform.localScale.x;
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        Vector3 pos = transform.position;
        Vector3 scale = transform.localScale;
        TextMeshPro tmp = GetComponent<TextMeshPro>();
        while (i.color.a < 1.0f)
        {
            pos.y += Time.deltaTime * 1f;
            transform.position = pos;
            scale.y -= Time.deltaTime * 0.01f;
            transform.localScale = scale;
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
        while (transform.localScale.x < originalXScale * 1.5 || i.color.a > 0.0f || transform.localScale.y > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / (t * 2)));
            if (transform.localScale.y <= 0)
            {
                scale.y = 0;
            }
            else 
            {
                scale.y -= Time.deltaTime;
            }
            scale.x += Time.deltaTime * 0.4f;
            pos.x += Time.deltaTime * 3;
            transform.localScale = scale;
            transform.position = pos;
            yield return null;
        }
        Destroy(gameObject);
    }
}