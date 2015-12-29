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

        /// <summary>
        /// Undo 可能な状態かどうかを示します
        /// </summary>
        public bool CanUndo => _commandManager.CanUndo;

        /// <summary>
        /// Redo 可能な状態かどうかを示します
        /// </summary>
        public bool CanRedo => _commandManager.CanRedo;

        /// <summary>
        /// クリア状態かどうかを示します
        /// </summary>
        public bool IsClear => _map.GoalPositions
            .Select(_map.GetField)
            .All(type => type == FieldTypes.Block);

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="map"></param>
        public Sokoban(Map map)
        {
            _map = map;
        }

        /// <summary>
        /// 操作を Undo します
        /// </summary>
        public void Undo()
        {
            _commandManager.Undo();
        }

        /// <summary>
        /// Undo を取り消します
        /// </summary>
        public void Redo()
        {
            _commandManager.Redo();
        }

        /// <summary>
        /// フィールド内の移動を試みます
        /// </summary>
        /// <param name="command"></param>
        /// <returns>移動に成功したら true</returns>
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
