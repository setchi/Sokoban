using System.Collections.Generic;

namespace Sokoban
{
    class Program
    {
        static void Main(string[] args)
        {
            var operationTable = new Dictionary<CommandTypes, Operation>
            {
                { CommandTypes.MoveUp, new Operation('w', "上") },
                { CommandTypes.MoveDown, new Operation('s', "下") },
                { CommandTypes.MoveLeft, new Operation('a', "左") },
                { CommandTypes.MoveRight, new Operation('d', "右") },
                { CommandTypes.Undo, new Operation('u', "戻る") },
                { CommandTypes.Redo, new Operation('r', "進む") },
                { CommandTypes.Reset, new Operation('@', "リセット") },
            };

            var fieldCharTable = new Dictionary<FieldTypes, char>
            {
                { FieldTypes.Space, ' ' },
                { FieldTypes.Wall, '#' },
                { FieldTypes.Player, 'p' },
                { FieldTypes.Block, 'o' },
                { FieldTypes.Goal, '.' },
                { FieldTypes.BlockOnGoal, 'O' },
                { FieldTypes.PlayerOnGoal, 'P' },
            };

            var fieldString = @"
#########
#       #
# ##o## #
# p   o #
##o...# #
##    # #
####  ###
#########
";

            new Game(operationTable, fieldCharTable)
                .Start(fieldString);
        }
    }
}
