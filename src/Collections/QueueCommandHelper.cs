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

namespace Zongsoft.Collections.Commands
{
	internal static class QueueCommandHelper
	{
		public static IEnumerable<IQueue> GetQueues(CommandTreeNode node, string names)
		{
			var result = new List<IQueue>();
			IQueue queue;

			if(string.IsNullOrWhiteSpace(names))
			{
				queue = FindQueue(node);

				if(queue == null)
					throw new CommandException();

				result.Add(queue);
			}
			else
			{
				foreach(var name in names.Split(',', ';'))
				{
					if(!string.IsNullOrWhiteSpace(name))
					{
						queue = FindQueue(node, name);

						if(queue == null)
							throw new CommandException();

						result.Add(queue);
					}
				}
			}

			return result;
		}

		private static IQueue FindQueue(CommandTreeNode node, string name = null)
		{
			if(node == null)
				return null;

			var queueCommand = node.Command as QueueCommand;

			if(queueCommand != null)
			{
				return name == null ? queueCommand.Queue : queueCommand.QueueProvider.GetQueue(name);
			}

			return FindQueue(node.Parent, name);
		}
	}
}
