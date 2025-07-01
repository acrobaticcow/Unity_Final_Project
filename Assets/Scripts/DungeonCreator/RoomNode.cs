using UnityEngine;

public class RoomNode : Node
{
    public RoomNode(
        Vector2Int bottomLeftAreaCorner,
        Vector2Int topRightAreaCorner,
        Node parentNode,
        int index
    )
        : base(parentNode)
    {
        this.BottomLeftAreaCorner = bottomLeftAreaCorner;
        this.TopRightAreaCorner = topRightAreaCorner;
        this.BottomRightAreaCorner = new Vector2Int(topRightAreaCorner.x, bottomLeftAreaCorner.y);
        this.TopLeftAreaCorner = new Vector2Int(bottomLeftAreaCorner.x, TopRightAreaCorner.y);
        this.TreeLayerIndex = index;
        this.Size = new(
            topRightAreaCorner.x - bottomLeftAreaCorner.x,
            topRightAreaCorner.y - bottomLeftAreaCorner.y
        );
    }

    public Cell[,] GetGrid()
    {
        Cell[,] cells = new Cell[Width, Length];
        for (int x = BottomLeftAreaCorner.x; x < TopRightAreaCorner.x; x++)
        {
            for (int y = BottomLeftAreaCorner.y; y < TopRightAreaCorner.y; y++)
            {
                Cell.CellSideTag sideTag = Cell.CellSideTag.None;
                Cell.CellTag cellTag = Cell.CellTag.Inner;
                // Around the edge
                if (x == BottomLeftAreaCorner.x)
                {
                    sideTag = Cell.CellSideTag.Left;
                    cellTag = Cell.CellTag.Outer;
                }
                else if (x == TopRightAreaCorner.x)
                {
                    sideTag = Cell.CellSideTag.Left;
                    cellTag = Cell.CellTag.Outer;
                }
                else if (y == BottomLeftAreaCorner.y)
                {
                    sideTag = Cell.CellSideTag.Bottom;
                    cellTag = Cell.CellTag.Outer;
                }
                else if (y == TopRightAreaCorner.y)
                {
                    sideTag = Cell.CellSideTag.Top;
                    cellTag = Cell.CellTag.Outer;
                }

                cells[x, y] = new Cell(new Vector3(x, 0, y), cellTag, sideTag);
            }
        }
        return cells;
    }

    public int Width
    {
        get => (int)(TopRightAreaCorner.x - BottomLeftAreaCorner.x);
    }
    public int Length
    {
        get => (int)(TopRightAreaCorner.y - BottomLeftAreaCorner.y);
    }
}
