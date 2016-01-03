using System;
using System.Collections.Generic;

namespace Sokoban
{
    class CommandManager
    {
        readonly Stack<Command> _undoStack = new Stack<Command>();
        readonly Stack<Command> _redoStack = new Stack<Command>();

        /// <summary>
        /// Undo 可能な状態かどうかを示します
        /// </summary>
        public bool CanUndo => _undoStack.Count > 0;

        /// <summary>
        /// Redo 可能な状態かどうかを示します
        /// </summary>
        public bool CanRedo => _redoStack.Count > 0;

        /// <summary>
        /// コマンドを実行します
        /// </summary>
        /// <param name="command"></param>
        public void Do(Command command)
        {
            command.Do();
            _undoStack.Push(command);
            _redoStack.Clear();
        }

        /// <summary>
        /// 履歴をクリアします
        /// </summary>
        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }

        /// <summary>
        /// Undo を実行します
        /// </summary>
        public void Undo()
        {
            if (!CanUndo)
                return;

            var command = _undoStack.Pop();
            command.Undo();
            _redoStack.Push(command);
        }

        /// <summary>
        /// Redo を実行します
        /// </summary>
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
