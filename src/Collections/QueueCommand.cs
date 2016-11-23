/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2011-2016 Zongsoft Corporation <http://www.zongsoft.com>
 *
 * This file is part of Zongsoft.Commands.
 *
 * Zongsoft.Commands is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * Zongsoft.Commands is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with Zongsoft.Commands; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 */

using System;
using System.Collections.Generic;

using Zongsoft.Services;
using Zongsoft.Resources;

namespace Zongsoft.Collections.Commands
{
	[CommandOption("name", typeof(string), Description = "${Text.QueueCommand.Options.Name}")]
	public class QueueCommand : CommandBase<CommandContext>
	{
		#region 成员字段
		private IQueue _queue;
		private IQueueProvider _queueProvider;
		#endregion

		#region 构造函数
		public QueueCommand() : this("Queue")
		{
		}

		public QueueCommand(string name) : base(name)
		{
		}
		#endregion

		#region 公共属性
		[ServiceDependency]
		public IQueue Queue
		{
			get
			{
				return _queue;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_queue = value;
			}
		}

		[ServiceDependency]
		public IQueueProvider QueueProvider
		{
			get
			{
				return _queueProvider;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_queueProvider = value;
			}
		}
		#endregion

		#region 执行方法
		protected override object OnExecute(CommandContext context)
		{
			var queueName = string.Empty;

			if(context.Expression.Options.TryGetValue("queue", out queueName))
			{
				var provider = this.QueueProvider;

				if(provider == null)
					throw new CommandException(ResourceUtility.GetString("Text.QueueCommand.MissingQueueProvider"));

				var queue = _queueProvider.GetQueue(queueName);

				if(queue == null)
					throw new CommandException(ResourceUtility.GetString("Text.QueueCommand.NotFoundQueue", queueName));

				_queue = queue;
			}

			if(_queue == null)
				throw new CommandException(ResourceUtility.GetString("Text.CannotObtainCommandTarget", "Queue"));

			//打印队列信息
			context.Output.WriteLine(ResourceUtility.GetString("Text.QueueCommand.Message", _queue.Name, _queue.Count, _queue.GetType().FullName, _queue.ToString()));

			return _queue;
		}
		#endregion
	}
}
