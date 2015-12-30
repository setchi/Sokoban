namespace Sokoban
{
    struct Operation
    {
        public char Command { get; }
        public string Name { get; }

        public Operation(char command, string name)
        {
            Command = command;
            Name = name;
        }
    }
}
