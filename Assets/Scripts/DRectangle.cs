using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class DRectangle : ScriptableObject{ 

    public int left;
    public int top;
    public int width;
    public int height;

    public virtual DRectangle Create(int x, int y, int width, int height)
    {
        left = x;
        top = y;
        this.width = width;
        this.height = height;
        return this;
    }

    public int right
    {
        get { return left + width - 1; }
    }

    public int bottom
    {
        get { return top + height - 1; }
    }

    public Vector3 position
    {
        get { return new Vector3(left, 0, top); }
    }

    public bool collidesWith(DRectangle other)
    {
        if (left > other.right ) return false;
        if (top > other.bottom ) return false;
        if (right < other.left ) return false;
        if (bottom < other.top ) return false;
        return true;
    }

    public bool contains(Vector3 other)
    {
        if (left > other.x   ) return false; //Debug.Log("LEFT");
        if (top > other.z    ) return false; //Debug.Log("TOP " + right + "," + other.x);
        if (right < other.x  ) return false; //Debug.Log("Right");
        if (bottom < other.z ) return false; //Debug.Log("Bottom");
        return true;
    }

    public bool collidesWith(int top, int left, int right, int bottom)
    {
        if (this.left > right - 1) return false;
        if (this.top > bottom - 1) return false;
        if (this.right < left + 1) return false;
        if (this.bottom < top + 1) return false;
        return true;
    }

    public bool contains(DRectangle other)
    {
        if (left   > other.left)   return false;
        if (right  < other.right)  return false;
        if (top    > other.top)    return false;
        if (bottom < other.bottom) return false;
        return true;
    }


    // Returns a new DRectangle object with the dimensions reduced by (x,y)*2
    public DRectangle collapse(int shrinkWidthBy, int shrinkHeightBy, int minWidth, int minHeight)
    {
        DRectangle rect = ScriptableObject.CreateInstance<DRectangle>();
        rect.left   = left + shrinkWidthBy;
        rect.top    = top + shrinkHeightBy;
        rect.width  = Mathf.Max(minWidth, width - (shrinkWidthBy * 2));
        rect.height = Mathf.Max(minHeight,height - (shrinkHeightBy * 2));
        //Debug.Log("Old " + this.ToString());
        //Debug.Log("New " + rect.ToString());

        return rect;
    }

    public override string ToString()
    {
        return "DRectangle(" + left + "," + top + "," + width + "," + height + ")";
    }
}
