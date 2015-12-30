using System.Collections.Generic;
using System.Drawing;

namespace Sokoban
{
    struct Map
    {
        /// <summary>
        /// フィールド上の障害物を表す一次元リスト
        /// </summary>
        public IList<FieldTypes> Field { get; }

        /// <summary>
        /// フィールドのサイズ
        /// </summary>
        public Size FieldSize { get; }

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
        /// <param name="fieldSize"></param>
        /// <param name="playerPositions"></param>
        /// <param name="goalPositions"></param>
        public Map(IList<FieldTypes> field, Size fieldSize, Point playerPositions, ISet<Point> goalPositions)
        {
            Field = field;
            FieldSize = fieldSize;
            PlayerPosition = playerPositions;
            GoalPositions = goalPositions;
        }

        /// <summary>
        /// 指定した位置のフィールド種類を取得します
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public FieldTypes GetField(Point position)
        {
            return Field[position.Y * FieldSize.Width + position.X];
        }

        /// <summary>
        /// 指定した位置にフィールド種類を設定します
        /// </summary>
        /// <param name="position"></param>
        /// <param name="type"></param>
        public void SetField(Point position, FieldTypes type)
        {
            Field[position.Y * FieldSize.Width + position.X] = type;
        }
    }
}
