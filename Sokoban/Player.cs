using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Sokoban
{
    class Player
    {
        readonly CommandManager _commandManager = new CommandManager();
        readonly IReadOnlyDictionary<CommandTypes, Size> _directionTable = new Dictionary<CommandTypes, Size>()
        {
            { CommandTypes.MoveUp, new Size(0, -1) },
            { CommandTypes.MoveDown, new Size(0, 1) },
            { CommandTypes.MoveLeft, new Size(-1, 0) },
            { CommandTypes.MoveRight, new Size(1, 0) },
        };

        Map _map;

        /// <summary>
        /// マップのインスタンスを返します
        /// </summary>
        public Map Map => _map;

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
        public Player(Map map)
        {
            _map = map;
        }

        /// <summary>
        /// 操作を Undo します
        /// </summary>
        public void Undo() { _commandManager.Undo(); }

        /// <summary>
        /// Undo を取り消します
        /// </summary>
        public void Redo() { _commandManager.Redo(); }

        /// <summary>
        /// フィールド内の移動を試みます
        /// </summary>
        /// <param name="commandType"></param>
        /// <returns>移動に成功したら true</returns>
        public bool TryMove(CommandTypes commandType)
        {
            if (!_directionTable.ContainsKey(commandType))
            {
                return false;
            }

            var direction = _directionTable[commandType];
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
