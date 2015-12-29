using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Sokoban
{
    class Sokoban
    {
        readonly Map _map;
        readonly CommandManager _commandManager = new CommandManager();
        readonly IDictionary<char, Size> _directionTable = new Dictionary<char, Size>()
        {
            { 'w', new Size(0, -1) },
            { 's', new Size(0, 1) },
            { 'a', new Size(-1, 0) },
            { 'd', new Size(1, 0) },
        };

        public bool CanUndo => _commandManager.CanUndo;
        public bool CanRedo => _commandManager.CanRedo;
        public bool IsClear => _map.GoalPositions
            .Select(_map.GetField)
            .All(type => type == FieldTypes.Block);

        public Sokoban(Map map)
        {
            _map = map;
        }

        public void Undo()
        {
            _commandManager.Undo();
        }

        public void Redo()
        {
            _commandManager.Redo();
        }

        public bool TryMove(char command)
        {
            if (!_directionTable.ContainsKey(command))
            {
                return false;
            }

            var direction = _directionTable[command];
            var nextPosition = _map.PlayerPosition + direction;
            var nextField = _map.GetField(nextPosition);

            if (nextField == FieldTypes.Wall)
            {
                return false;
            }

            if (nextField == FieldTypes.Space)
            {
                _commandManager.Do(new Command(
                    () => _map.PlayerPosition += direction,
                    () => _map.PlayerPosition -= direction));
                return true;
            }

            if (nextField == FieldTypes.Block)
            {
                var nextNextPosition = nextPosition + direction;

                if (_map.GetField(nextNextPosition) != FieldTypes.Space)
                {
                    return false;
                }

                _commandManager.Do(new Command(
                    () => {
                        _map.PlayerPosition += direction;
                        _map.SetField(nextPosition, FieldTypes.Space);
                        _map.SetField(nextNextPosition, FieldTypes.Block);
                    },
                    () => {
                        _map.PlayerPosition -= direction;
                        _map.SetField(nextPosition, FieldTypes.Block);
                        _map.SetField(nextNextPosition, FieldTypes.Space);
                    }));

                return true;
            }

            return false;
        }
    }
}
