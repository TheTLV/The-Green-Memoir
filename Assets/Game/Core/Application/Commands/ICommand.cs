namespace TheGreenMemoir.Core.Application.Commands
{
    /// <summary>
    /// Interface cho Command Pattern
    /// </summary>
    public interface ICommand
    {
        bool CanExecute();
        CommandResult Execute();
    }

    /// <summary>
    /// Kết quả của command
    /// </summary>
    public class CommandResult
    {
        public bool IsSuccess { get; }
        public string ErrorMessage { get; }

        private CommandResult(bool isSuccess, string errorMessage)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        public static CommandResult Success()
        {
            return new CommandResult(true, null);
        }

        public static CommandResult Failed(string errorMessage)
        {
            return new CommandResult(false, errorMessage);
        }
    }
}

