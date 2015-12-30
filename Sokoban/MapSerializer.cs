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
            // 文字列を改行コードで区切ります
            // 空白文字のみの行は無視します
            var fieldRows = fieldString
                .Split(Environment.NewLine.ToCharArray())
                .Where(row => !string.IsNullOrWhiteSpace(row));

            // フィールドサイズを数えます
            // 1行目の文字数を Width, 行数を Height とします
            var fieldSize = new Size(
                fieldRows.First().Count(),
                fieldRows.Count());

            // フィールドを表す一次元リストを生成します
            var field = fieldRows
                .SelectMany(row => row)
                .Select(c => _fieldTypeTable[c])
                .ToList();

            // ゴール地点を抽出します
            var goalPositions = new HashSet<Point>(field
                .Select((type, index) => new { type, index })
                .Where(x => x.type == FieldTypes.Goal)
                .Select(x => new Point(
                    x.index % fieldSize.Width,
                    x.index / fieldSize.Width)));

            // プレイヤー地点を抽出します
            var playerIndex = field.FindIndex(type => type == FieldTypes.Player);
            var playerPosition = new Point(
                playerIndex % fieldSize.Width,
                playerIndex / fieldSize.Width);

            // フィールドを表すリストでは移動の障害となり得るものだけ管理します
            // そのためプレイヤー地点とゴール地点はスペースに置き換えます
            foreach (var pos in goalPositions.Concat(new[] { playerPosition }))
            {
                field[pos.Y * fieldSize.Width + pos.X] = FieldTypes.Space;
            }

            return new Map(field, fieldSize, playerPosition, goalPositions);
        }

        /// <summary>
        /// マップのインスタンスを文字列化します
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public string Serialize(Map map)
        {
            return new string(Enumerable.Range(0, map.FieldSize.Height)
                .SelectMany(y => Enumerable.Range(0, map.FieldSize.Width)
                    .Select(x => _charTable[GetViewFieldType(map, new Point(x, y))])
                    .Concat(Environment.NewLine.ToCharArray()))
                .ToArray());
        }

        /// <summary>
        /// 表示用のフィールド種類を取得します
        /// </summary>
        /// <param name="map"></param>
        /// <param name="position"></param>
        /// <returns>表示用のフィールド種類</returns>
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
