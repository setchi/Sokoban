using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sokoban
{
    class Game
    {
        readonly IReadOnlyDictionary<char, CommandTypes> _commandTypeTable;
        readonly IReadOnlyDictionary<CommandTypes, Operation> _operationTable;
        readonly IReadOnlyDictionary<FieldTypes, char> _fieldCharTable;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="operationTable"></param>
        /// <param name="fieldCharTable"></param>
        public Game(
            IReadOnlyDictionary<CommandTypes, Operation> operationTable,
            IReadOnlyDictionary<FieldTypes, char> fieldCharTable)
        {
            _fieldCharTable = fieldCharTable;
            _operationTable = operationTable;
            _commandTypeTable = operationTable.ToDictionary(
                kv => kv.Value.Command,
                kv => kv.Key);
        }

        /// <summary>
        /// 与えられたフィールド文字列でゲームを開始します
        /// </summary>
        /// <param name="fieldString"></param>
        public void Start(string fieldString)
        {
            var mapSerializer = new MapSerializer(_fieldCharTable);
            var player = new Player(mapSerializer.Deserialize(fieldString));
            var countMove = 0;

            Console.WriteLine("倉庫番");
            Console.WriteLine("Please Enter Key... Game Start");
            Console.ReadLine();

            var commandDescription = CreateOperationDescription();

            while (!player.IsClear)
            {
                Console.WriteLine(mapSerializer.Serialize(player.Map));
                Console.WriteLine($"移動回数: {countMove}");
                Console.WriteLine(commandDescription);

                var input = Console.ReadLine();

                if (input.Length == 0 || input.Contains('!'))
                {
                    continue;
                }

                if (!_commandTypeTable.ContainsKey(input[0]))
                {
                    continue;
                }

                var commandType = _commandTypeTable[input[0]];

                switch (commandType)
                {
                    case CommandTypes.Undo:
                        if (!player.CanUndo)
                        {
                            break;
                        }

                        player.Undo();
                        countMove--;
                        break;

                    case CommandTypes.Redo:
                        if (!player.CanRedo)
                        {
                            break;
                        }

                        player.Redo();
                        countMove++;
                        break;

                    case CommandTypes.Reset:
                        while (player.CanUndo)
                        {
                            player.Undo();
                        }

                        countMove = 0;
                        break;

                    case CommandTypes.MoveUp:
                    case CommandTypes.MoveDown:
                    case CommandTypes.MoveLeft:
                    case CommandTypes.MoveRight:
                        if (player.TryMove(commandType))
                        {
                            countMove++;
                        }

                        break;
                }
            }

            Console.WriteLine(mapSerializer.Serialize(player.Map));
            Console.WriteLine($"移動回数: {countMove}");
            Console.WriteLine("ゲームクリア！おめでとう！！");
        }

        /// <summary>
        /// 操作説明用の文字列を作ります
        /// </summary>
        /// <returns></returns>
        string CreateOperationDescription()
        {
            var sb = new StringBuilder();

            sb.Append("移動: (");
            sb.Append(CreateCommandDescriptions(
                CommandTypes.MoveUp,
                CommandTypes.MoveLeft,
                CommandTypes.MoveDown,
                CommandTypes.MoveRight));
            sb.Append(") + Enter");

            sb.Append(Environment.NewLine);
            sb.Append(CreateCommandDescriptions(
                CommandTypes.Undo,
                CommandTypes.Redo,
                CommandTypes.Reset));
            sb.Append(", 入力キャンセル->!を含める");

            return sb.ToString();
        }

        /// <summary>
        /// コマンド説明用の文字列を作ります
        /// </summary>
        /// <param name="commandTypes"></param>
        /// <returns></returns>
        string CreateCommandDescriptions(params CommandTypes[] commandTypes)
        {
            return string.Join(", ", commandTypes
                .Where(_operationTable.ContainsKey)
                .Select(type => _operationTable[type])
                .Select(operation => $"{operation.Name}->{operation.Command}"));
        }
    }
}
