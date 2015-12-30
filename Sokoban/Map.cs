using System.Collections.Generic;
using System.Drawing;

namespace Sokoban
{
    struct Map
    {
        /// <summary>
        /// フィールド上の障害物を表す二次元配列
        /// </summary>
        public FieldTypes[,] Field { get; }

        /// <summary>
        /// フィールドの幅
        /// </summary>
        public int Width => Field.GetLength(0);

        /// <summary>
        /// フィールドの高さ
        /// </summary>
        public int Height => Field.GetLength(1);

        /// <summary>
        /// ゴール地点の集合
        /// </summary>
        public ISet<Point> GoalPositions { get; }

        /// <summary>
        /// プレイヤー地点
        /// </summary>
        public Point PlayerPosition { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="field"></param>
        /// <param name="playerPosition"></param>
        /// <param name="goalPositions"></param>
        public Map(FieldTypes[,] field, Point playerPosition, ISet<Point> goalPositions)
        {
            Field = field;
            PlayerPosition = playerPosition;
            GoalPositions = goalPositions;
        }

        /// <summary>
        /// 指定した位置のフィールド種類を取得します
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public FieldTypes GetField(Point position)
        {
            return Field[position.X, position.Y];
        }

        /// <summary>
        /// 指定した位置にフィールド種類を設定します
        /// </summary>
        /// <param name="position"></param>
        /// <param name="type"></param>
        public void SetField(Point position, FieldTypes type)
        {
            Field[position.X, position.Y] = type;
        }
    }
}
