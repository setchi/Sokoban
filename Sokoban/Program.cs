using System;
using System.Collections.Generic;
using System.Linq;

namespace Sokoban
{
    class Program
    {
        static void Main(string[] args)
        {
            var fieldString =
@"
#########
#       #
# ##o## #
# p   o #
##o...# #
##    # #
####  ###
#########
";

            var mapSerializer = new MapSerializer(new Dictionary<FieldTypes, char>
            {
                { FieldTypes.Space, ' ' },
                { FieldTypes.Wall, '#' },
                { FieldTypes.Player, 'p' },
                { FieldTypes.Block, 'o' },
                { FieldTypes.Goal, '.' },
                { FieldTypes.BlockOnGoal, 'O' },
                { FieldTypes.PlayerOnGoal, 'P' },
            });

            var map = mapSerializer.Deserialize(fieldString);
            var sokoban = new Sokoban(map);
            var countMove = 0;

            Console.WriteLine("倉庫番");
            Console.WriteLine("Please Enter Key... Game Start");
            Console.ReadLine();

            while (!sokoban.IsClear)
            {
                Console.WriteLine(mapSerializer.Serialize(map));
                Console.WriteLine("移動回数: " + countMove);
                Console.WriteLine("移動: (上->w, 左->a, 下->s, 右->d) + Enter");
                Console.WriteLine("戻る->u, 進む->r, リセット->@, 入力キャンセル->!を含める");

                var input = Console.ReadLine();

                if (input.Length == 0)
                {
                    continue;
                }

                if (input.Contains('!'))
                {
                    // Cancel
                    continue;
                }

                var command = input[0];

                if (command == 'u' && sokoban.CanUndo)
                {
                    sokoban.Undo();
                    countMove--;
                    continue;
                }

                if (command == 'r' && sokoban.CanRedo)
                {
                    sokoban.Redo();
                    countMove++;
                    continue;
                }

                if (command == '@')
                {
                    while (sokoban.CanUndo)
                    {
                        sokoban.Undo();
                    }

                    countMove = 0;
                    continue;
                }

                if (sokoban.TryMove(command))
                {
                    countMove++;
                }
            }

            Console.WriteLine(mapSerializer.Serialize(map));
            Console.WriteLine("移動回数: " + countMove);
            Console.WriteLine("ゲームクリア！おめでとう！！");
        }
    }
}
