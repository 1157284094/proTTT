using UnityEngine;

public class ToolFunctions
{
    const float maxRayDistance = 1000;

    public static bool GetTextureSize(SpriteRenderer sr, out float width, out float height)
    {
        if (sr == null || sr.sprite == null)
        {
            Debug.LogError("SpriteRenderer is null", sr.gameObject);
            width = 0;
            height = 0;
            return false;
        }
        width = sr.sprite.bounds.size.x;
        height = sr.sprite.bounds.size.y;
        return true;
    }

    public static void DebugLog(string strf, params object[] args)
    {
        //可以在这里做log优先级的设置，或者直接在发布版本return
        Debug.Log(string.Format(strf, args));
    }

    public static bool IsClickUILayer(Vector3 inputPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(inputPosition);
        RaycastHit hit;

        int layerMask = 1 << LayerMask.NameToLayer("UI");

        if (Physics.Raycast(ray, out hit, maxRayDistance, layerMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
