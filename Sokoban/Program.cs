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

            var charTable = new Dictionary<FieldTypes, char>
            {
                { FieldTypes.Space, ' ' },
                { FieldTypes.Wall, '#' },
                { FieldTypes.Player, 'p' },
                { FieldTypes.Block, 'o' },
                { FieldTypes.Goal, '.' },
                { FieldTypes.BlockOnGoal, 'O' },
                { FieldTypes.PlayerOnGoal, 'P' },
            };

            var mapSerializer = new MapSerializer();
            var map = mapSerializer.Deserialize(
                fieldString,
                charTable.ToDictionary(
                    kv => kv.Value,
                    kv => kv.Key));

            var sokoban = new Sokoban(map);
            var countMove = 0;

            while (!sokoban.IsClear)
            {
                Console.WriteLine(mapSerializer.Serialize(map, charTable));
                Console.WriteLine("移動回数: " + countMove);

                var input = Console.ReadLine();

                if (input.Length == 0)
                {
                    continue;
                }

                if (input.IndexOf('!') > -1)
                {
                    // Cancel
                    continue;
                }

                if (input[0] == 'u' && sokoban.CanUndo)
                {
                    sokoban.Undo();
                    countMove--;
                    continue;
                }

                if (sokoban.TryMove(input[0]))
                {
                    countMove++;
                }
            }

            Console.WriteLine(mapSerializer.Serialize(map, charTable));
            Console.WriteLine("移動回数: " + countMove);
            Console.WriteLine("ゲームクリア！おめでとう！！");
        }
    }
}
