using System;
using System.Collections.Generic;

namespace Sokoban
{
    class CommandManager
    {
        Stack<Command> _undoStack = new Stack<Command>();
        Stack<Command> _redoStack = new Stack<Command>();

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;

        public void Do(Command command)
        {
            command.Do();
            _undoStack.Push(command);
            _redoStack.Clear();
        }

        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }

        public void Undo()
        {
            if (!CanUndo)
                return;

            var command = _undoStack.Pop();
            command.Undo();
            _redoStack.Push(command);
        }

        public void Redo()
        {
            if (!CanRedo)
                return;

            var command = _redoStack.Pop();
            command.Do();
            _undoStack.Push(command);
        }
    }

    class Command
    {
        Action _doAction;
        Action _undoAction;

        public Command(Action doAction, Action undoAction)
        {
            _doAction = doAction;
            _undoAction = undoAction;
        }

        public void Do() { _doAction(); }
        public void Undo() { _undoAction(); }
    }
}
