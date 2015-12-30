using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Sokoban
{
    class MapSerializer
    {
        readonly IReadOnlyDictionary<FieldTypes, char> _charTable;
        readonly IReadOnlyDictionary<char, FieldTypes> _fieldTypeTable;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="charTable">フィールド種類 -> 文字 のテーブル</param>
        public MapSerializer(IReadOnlyDictionary<FieldTypes, char> charTable)
        {
            _charTable = charTable;
            _fieldTypeTable = charTable.ToDictionary(
                kv => kv.Value,
                kv => kv.Key);
        }

        /// <summary>
        /// 文字列からマップのインスタンスを生成します
        /// </summary>
        /// <param name="fieldString"></param>
        /// <returns></returns>
        public Map Deserialize(string fieldString)
        {
            return InstantiateMap(ParseFieldString(fieldString));
        }

        /// <summary>
        /// マップのインスタンスを文字列化します
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public string Serialize(Map map)
        {
            return string.Join(string.Empty, Enumerable.Range(0, map.Height)
                .SelectMany(y => Enumerable.Range(0, map.Width)
                    .Select(x => GetDisplayFieldType(map, new Point(x, y)))
                    .Select(fieldType => _charTable[fieldType])
                    .Concat(Environment.NewLine)));
        }

        /// <summary>
        /// 文字列からフィールドの2次元配列を生成します
        /// </summary>
        /// <param name="fieldString"></param>
        /// <returns></returns>
        FieldTypes[,] ParseFieldString(string fieldString)
        {
            // 文字列を改行コードで区切ります
            // 空白文字のみの行は無視します
            var fieldRows = fieldString
                .Split(Environment.NewLine.ToCharArray())
                .Where(row => !string.IsNullOrWhiteSpace(row));

            if (fieldRows.Count() == 0)
            {
                throw new ArgumentException("フィールドが空です");
            }

            // フィールドサイズを測ります
            // 行数を Height, 1行目の文字数を Width とします
            var height = fieldRows.Count();
            var width = fieldRows.First().Count();

            // すべての行が同じ文字数でなかったらエラー
            if (!fieldRows.All(row => row.Count() == width))
            {
                throw new ArgumentException(
                    "フィールドの横幅が一律ではありません: "
                    + Environment.NewLine
                    + string.Join(Environment.NewLine, fieldRows));
            }

            // フィールドの2次元配列を作ります
            var fieldArray = new FieldTypes[width, height];
            foreach (var y in Enumerable.Range(0, height))
            {
                foreach (var x in Enumerable.Range(0, width))
                {
                    var character = fieldRows.ElementAt(y).ElementAt(x);
                    fieldArray[x, y] = _fieldTypeTable[character];
                }
            }

            return fieldArray;
        }

        /// <summary>
        /// フィールドの2次元配列からマップのインスタンスを生成します
        /// </summary>
        /// <param name="fieldArray"></param>
        /// <returns></returns>
        Map InstantiateMap(FieldTypes[,] fieldArray)
        {
            var toPosition = new Func<int, Point>(i => new Point(
                i % fieldArray.GetLength(0),
                i / fieldArray.GetLength(0)));

            // ゴール地点を抽出します
            var goalPositions = new HashSet<Point>(
                Enumerable.Range(0, fieldArray.Length).Select(toPosition)
                    .Where(pos => fieldArray[pos.X, pos.Y] == FieldTypes.Goal));

            // プレイヤー地点を抽出します
            var playerPosition = Enumerable.Range(0, fieldArray.Length).Select(toPosition)
                .Where(pos => fieldArray[pos.X, pos.Y] == FieldTypes.Player)
                .First();

            // マップのフィールド配列では、移動の障害となり得るものだけ管理します
            // フィールド配列のプレイヤー地点とゴール地点をスペースに置き換えます
            foreach (var pos in goalPositions.Concat(new[] { playerPosition }))
            {
                fieldArray[pos.X, pos.Y] = FieldTypes.Space;
            }

            return new Map(fieldArray, playerPosition, goalPositions);
        }

        /// <summary>
        /// 表示用のフィールド種類を取得します
        /// </summary>
        /// <param name="map"></param>
        /// <param name="position"></param>
        /// <returns>表示用のフィールド種類</returns>
        FieldTypes GetDisplayFieldType(Map map, Point position)
        {
            var fieldType = position == map.PlayerPosition
                ? FieldTypes.Player
                : map.GetField(position);

            if (map.GoalPositions.Contains(position))
            {
                fieldType = GetOnGoalFieldType(fieldType);
            }

            return fieldType;
        }

        /// <summary>
        /// ゴール地点に乗っているときの表示用フィールド種類を取得します
        /// </summary>
        /// <param name="type"></param>
        /// <returns>ゴール地点に乗っているときの表示用フィールド種類</returns>
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
