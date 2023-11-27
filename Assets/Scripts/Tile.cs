using UnityEngine;
public class Tile : MonoBehaviour
{
    public int value;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateColor();
    }
    public void UpdateColor()
    {
        float h = 0.7f;
        float s = Mathf.Lerp(0.05f,1,value/10f);
        float v = 1;
        spriteRenderer.color = Color.HSVToRGB(h,s,v);
    }
}
