using System.Collections.Generic;

namespace TheGreenMemoir.Core.Application.Commands
{
    /// <summary>
    /// Thực thi commands (có thể thêm undo sau)
    /// </summary>
    public class CommandInvoker
    {
        private readonly Stack<ICommand> _commandHistory = new Stack<ICommand>();

        /// <summary>
        /// Thực thi command
        /// </summary>
        public CommandResult ExecuteCommand(ICommand command)
        {
            if (command == null)
                return CommandResult.Failed("Command is null");

            if (!command.CanExecute())
                return CommandResult.Failed("Command cannot be executed");

            var result = command.Execute();

            if (result.IsSuccess)
            {
                _commandHistory.Push(command);
            }

            return result;
        }

        /// <summary>
        /// Kiểm tra xem có thể undo không
        /// </summary>
        public bool CanUndo => _commandHistory.Count > 0;

        /// <summary>
        /// Lấy số lượng commands trong history
        /// </summary>
        public int HistoryCount => _commandHistory.Count;
    }
}

