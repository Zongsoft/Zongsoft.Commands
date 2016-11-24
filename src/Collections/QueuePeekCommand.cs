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
using System.Collections;
using System.Collections.Generic;

using Zongsoft.Services;
using Zongsoft.Resources;
using Zongsoft.Runtime.Serialization;

namespace Zongsoft.Collections.Commands
{
	[CommandOption("count", Type = typeof(int), DefaultValue = 1, Description = "${Text.QueueCommand.Options.Count}")]
	[CommandOption("queues", Type = typeof(string), Description = "${Text.QueueCommand.Options.Queues}")]
	public class QueuePeekCommand : CommandBase<CommandContext>
	{
		#region 构造函数
		public QueuePeekCommand() : this("Peek")
		{
		}

		public QueuePeekCommand(string name) : base(name)
		{
		}
		#endregion

		#region 执行方法
		protected override object OnExecute(CommandContext context)
		{
			var count = context.Expression.Options.GetValue<int>("count");
			var result = new List<object>(count);

			if(count < 1)
				throw new CommandOptionValueException("count", count);

			var queues = QueueCommandHelper.GetQueues(context.CommandNode, context.Expression.Options.GetValue<string>("queues"));

			foreach(var queue in queues)
			{
				if(queue.Count == 0)
				{
					context.Output.WriteLine(CommandOutletColor.DarkRed, ResourceUtility.GetString("Text.QueueIsEmpty", queue.Name));
					continue;
				}

				var items = queue.Peek(count);

				if(items == null)
					break;

				foreach(var item in items)
				{
					result.Add(item);
				}

				context.Output.WriteLine(Serializer.Text.Serialize(items));
			}

			return result.ToArray();
		}
		#endregion
	}
}
