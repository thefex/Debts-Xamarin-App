using System;
using System.Threading.Tasks;
using Debts.Services;
using MvvmCross.Commands;

namespace Debts.Commands
{
    /// <summary>
    /// Why does this class exist?
    /// MvxCommand exposes Action (not Func'Task') constructors - therefore we need to wrap exception handling into that
    /// If we didn't do that -> exception would be posted to SynchronizationContext (exception would dodge the catch block :) ) 
    /// </summary>
    public class MvxExceptionGuardedCommand : MvxCommand
    {
        public MvxExceptionGuardedCommand(Action execute) : base(() =>
        {
            try
            {
                execute();
            }
            catch (Exception ex)
            {
                ServicesLocation.ExceptionGuard.OnException(ex);
            }
        })
        {
        }

        public MvxExceptionGuardedCommand(Func<Task> toExecute) : base(async () =>
        {
            try
            {
                await toExecute();
            }
            catch (Exception ex)
            {
	            ServicesLocation.ExceptionGuard.OnException(ex);
            }
        })
        {

        }
    }

    public class MvxExceptionGuardedCommand<TItem> : MvxCommand<TItem>
	{
		public MvxExceptionGuardedCommand(Action<TItem> execute) : base((item) =>
		{
			try
			{
				execute(item);
			}
			catch (Exception ex)
			{
				ServicesLocation.ExceptionGuard.OnException(ex);
			}
		})
		{
		}

		public MvxExceptionGuardedCommand(Func<TItem, Task> toExecute) : base(async (item) =>
		{
			try
			{
				await toExecute(item);
			}
			catch (Exception ex)
			{
				ServicesLocation.ExceptionGuard.OnException(ex);
			}
		})
		{

		}
	}
}