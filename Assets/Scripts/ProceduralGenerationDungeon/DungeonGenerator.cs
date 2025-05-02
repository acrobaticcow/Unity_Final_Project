using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator
{
    RoomNode rootNode;
    List<RoomNode> allSpaceNodes = new();

    private int dungeonWidth,
        dungeonLength;

    public DungeonGenerator(int dungeonWidth, int dungeonLength)
    {
        this.dungeonLength = dungeonLength;
        this.dungeonWidth = dungeonWidth;
    }

    public List<Node> CalculateRooms(
        int maxIterations,
        int roomWidthMin,
        int roomLengthMin,
        float roomBottomCornerModifier,
        float roomTopCornerModifier,
        int roomOffset
    )
    {
        BinarySpacePartitioner bsp = new(dungeonWidth, dungeonLength);
        allSpaceNodes = bsp.PrepareNodesCollection(maxIterations, roomWidthMin, roomLengthMin);
        List<Node> roomSpaces = StructureHelper.TraverseGraphToExtractLowestLeafs(bsp.RootNode);

        RoomGenerator roomGenerator = new(maxIterations, roomWidthMin, roomLengthMin);
        List<RoomNode> rooms = roomGenerator.GenerateRoomsInGivenSpaces(
            roomSpaces,
            roomBottomCornerModifier,
            roomTopCornerModifier,
            roomOffset
        );

        return new(rooms);
    }
}
