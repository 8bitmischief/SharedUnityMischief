using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SharedUnityMischief.Entities;
using SharedUnityMischief.Lifecycle;

namespace StrikeOut.BossFight.Entities
{
	public abstract class EntityCommandController<TEntity> : EntityComponent<TEntity> where TEntity : Entity
	{
		private Queue<Command> _commands = new Queue<Command>();

		public IEnumerable<string> commands => _commands.Select(x => x.GetType().Name);
		public override int componentUpdateOrder => EntityComponent.ControllerUpdateOrder;

		public override void UpdateState()
		{
			if (!UpdateLoop.I.isInterpolating)
			{
				bool isOutOfCommands = ExecuteCommands();
				if (isOutOfCommands)
				{
					DecideNextAction();
					ExecuteCommands();
				}
			}
		}

		protected abstract void DecideNextAction();

		protected void QueueCommand(Command command)
		{
			if (command is Command<TEntity>)
				(command as Command<TEntity>).entity = entity;
			command.hasStarted = false;
			command.Reset();
			_commands.Enqueue(command);
		}

		protected void QueueCommands(params Command[] commands)
		{
			foreach (Command command in commands)
				QueueCommand(command);
		}

		protected void ClearCommands()
		{
			_commands.Clear();
		}

		protected bool ExecuteCommands()
		{
			while (_commands.Count > 0)
			{
				Command command = _commands.Peek();
				if (!command.hasStarted)
				{
					command.Start();
					command.hasStarted = true;
				}
				command.Update();
				if (command.IsDone())
				{
					_commands.Dequeue();
					command.End();
				}
				else
				{
					break;
				}
			}
			return _commands.Count == 0;
		}
	}
}