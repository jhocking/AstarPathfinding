namespace Astar
{
    public class PathNode
    {
        public readonly int x;
        public readonly int y;
        public readonly float g;
        public readonly float h;
        public readonly PathNode parentNode;

        public PathNode(int xPos, int yPos, float gVal, float hVal, PathNode link)
        {
            x = xPos;
            y = yPos;
            g = gVal;
            h = hVal;
            parentNode = link;
        }
    }
}
