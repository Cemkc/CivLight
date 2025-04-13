using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct AStarNode
{
    public int tileId;
    public int originId;
    
    public int GCost;
    public int HCost;
    public int FCost;
}

public class AStarPathfinder
{
    public static List<HexTile> FindPath(HexTile startTile, HexTile targetTile, HexGrid grid)
    {
        List<AStarNode> openList = new List<AStarNode>();
        HashSet<int> closedSet = new HashSet<int>();
        Dictionary<int, AStarNode> allNodes = new Dictionary<int, AStarNode>();
        
        AStarNode startNode = new AStarNode
        {
            tileId = startTile.TileId,
            originId = -1,
            GCost = 0,
            HCost = grid.HexGridDistance(startTile, targetTile),
            FCost = grid.HexGridDistance(startTile, targetTile)
        };

        openList.Add(startNode);
        allNodes[startNode.tileId] = startNode;
        
        while (openList.Count > 0)
        {
            openList = openList.OrderBy(n => n.FCost).ToList();
            AStarNode currentNode = openList[0];
            openList.RemoveAt(0);
            closedSet.Add(currentNode.tileId);

            if (currentNode.tileId == targetTile.TileId)
            {
                return ReconstructPath(allNodes, currentNode.tileId, grid);
            }

            foreach (Vector2Int neighborCoord in grid.NeighborTileCoords(grid.GetTile(currentNode.tileId).OffsetCoordinate))
            {
                HexTile neighborTile = grid.GetTile(neighborCoord);
                if (neighborTile == null || !neighborTile.IsWalkable() || closedSet.Contains(neighborTile.TileId))
                    continue;

                int gCost = currentNode.GCost + 1;
                AStarNode neighborNode;

                if (allNodes.TryGetValue(neighborTile.TileId, out neighborNode))
                {
                    if (gCost < neighborNode.GCost)
                    {
                        neighborNode.GCost = gCost;
                        neighborNode.FCost = neighborNode.GCost + neighborNode.HCost;
                        neighborNode.originId = currentNode.tileId;
                        allNodes[neighborTile.TileId] = neighborNode;
                    }
                }
                else
                {
                    neighborNode = new AStarNode
                    {
                        tileId = neighborTile.TileId,
                        originId = currentNode.tileId,
                        GCost = gCost,
                        HCost = grid.HexGridDistance(neighborTile, targetTile),
                        FCost = gCost + grid.HexGridDistance(neighborTile, targetTile)
                    };
                    allNodes[neighborTile.TileId] = neighborNode;
                    openList.Add(neighborNode);
                }
            }
        }

        return new List<HexTile>(); // No path found
    }

    private static List<HexTile> ReconstructPath(Dictionary<int, AStarNode> allNodes, int targetId, HexGrid grid)
    {
        List<HexTile> path = new List<HexTile>();
        int currentId = targetId;

        while (currentId != -1)
        {
            path.Add(grid.GetTile(currentId));
            currentId = allNodes[currentId].originId;
        }

        path.Reverse();
        return path;
    }
}
