using UnityEngine;
using System.Collections;

public class DRectangle {

    public int left;
    public int top;
    public int width;
    public int height;

    public DRectangle(){}

    protected DRectangle(int x, int y, int width, int height)
    {
        left = x;
        top = y;
        this.width = width;
        this.height = height;
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
}
