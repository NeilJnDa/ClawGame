public enum Direction
{
    //Do not Change the order
    Up = 0,
    Down = 1,
    Left = 2,
    Right = 3,
    Above = 4,
    Below = 5
}
public static class DirectionUtility
{
    public static Direction RevertDirection(Direction direction)
    {
        if (direction == Direction.Up) return Direction.Down;
        if (direction == Direction.Down) return Direction.Up;
        if (direction == Direction.Left) return Direction.Right;
        if (direction == Direction.Right) return Direction.Left;
        if (direction == Direction.Above) return Direction.Below;
        if (direction == Direction.Below) return Direction.Above;

        //Default
        return Direction.Up;
    }
}
