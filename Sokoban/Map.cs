using System.Collections.Generic;
using System.Drawing;

namespace Sokoban
{
    class Map
    {
        public IList<FieldTypes> Field { get; }
        public Size FieldSize { get; }
        public HashSet<Point> GoalPositions { get; }
        public Point PlayerPosition { get; set; }

        public Map(IList<FieldTypes> field, Size fieldSize, Point playerPositions, HashSet<Point> goalPositions)
        {
            Field = field;
            FieldSize = fieldSize;
            PlayerPosition = playerPositions;
            GoalPositions = goalPositions;
        }

        public FieldTypes GetField(Point position)
        {
            return Field[position.Y * FieldSize.Width + position.X];
        }

        public void SetField(Point position, FieldTypes type)
        {
            Field[position.Y * FieldSize.Width + position.X] = type;
        }
    }
}
