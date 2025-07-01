using UnityEngine;

public class Cell
{
    public Vector3 position;
    public CellTag zone;
    public CellSideTag side;
    public bool isBlocked;

    public Cell(Vector3 position, CellTag zone, CellSideTag side)
    {
        this.position = position;
        this.zone = zone;
        this.side = side;
    }

    public override string ToString()
    {
        return position + " " + zone + " " + side;
    }

    public enum CellTag
    {
        Inner,
        Outer
    }

    public enum CellSideTag
    {
        Top,
        Bottom,
        Right,
        Left,
        Center,
        None
    }
}
