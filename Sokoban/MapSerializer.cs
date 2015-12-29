using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Sokoban
{
    class MapSerializer
    {
        public Map Deserialize(string fieldString, IDictionary<char, FieldTypes> fieldTable)
        {
            var fieldRows = fieldString
                .Split(Environment.NewLine.ToCharArray())
                .Where(row => !string.IsNullOrWhiteSpace(row));

            var fieldSize = new Size(
                fieldRows.First().Count(),
                fieldRows.Count());

            var field = fieldRows
                .SelectMany(row => row)
                .Select(c => fieldTable[c])
                .ToList();

            var goalPositions = new HashSet<Point>(field
                .Select((type, index) => new { type, index })
                .Where(x => x.type == FieldTypes.Goal)
                .Select(x => new Point(
                    x.index % fieldSize.Width,
                    x.index / fieldSize.Width)));

            var playerIndex = field.FindIndex(type => type == FieldTypes.Player);
            var playerPosition = new Point(
                playerIndex % fieldSize.Width,
                playerIndex / fieldSize.Width);

            foreach (var pos in goalPositions.Concat(new[] { playerPosition }))
            {
                field[pos.Y * fieldSize.Width + pos.X] = FieldTypes.Space;
            }

            return new Map(field, fieldSize, playerPosition, goalPositions);
        }

        public string Serialize(Map map, IDictionary<FieldTypes, char> charTable)
        {
            return new string(Enumerable.Range(0, map.FieldSize.Height)
                .SelectMany(y => Enumerable.Range(0, map.FieldSize.Width)
                    .Select(x => charTable[GetViewFieldType(map, new Point(x, y))])
                    .Concat(Environment.NewLine.ToCharArray()))
                .ToArray());
        }

        FieldTypes GetViewFieldType(Map map, Point position)
        {
            var fieldType = position == map.PlayerPosition
                ? FieldTypes.Player
                : map.Field[position.Y * map.FieldSize.Width + position.X];

            if (map.GoalPositions.Contains(position))
            {
                fieldType = GetOnGoalFieldType(fieldType);
            }

            return fieldType;
        }

        FieldTypes GetOnGoalFieldType(FieldTypes type)
        {
            switch (type)
            {
                case FieldTypes.Player:
                    return FieldTypes.PlayerOnGoal;
                case FieldTypes.Block:
                    return FieldTypes.BlockOnGoal;
                default:
                    return FieldTypes.Goal;
            }
        }
    }
}
