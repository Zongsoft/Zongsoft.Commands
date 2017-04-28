/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2011-2017 Zongsoft Corporation <http://www.zongsoft.com>
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
using Zongsoft.Communication;

namespace Zongsoft.Messaging.Commands
{
	public class ListenerCommand : CommandBase<CommandContext>
	{
		#region 成员字段
		private IListener _listener;
		#endregion

		#region 构造函数
		public ListenerCommand() : base("Listener")
		{
		}

		public ListenerCommand(string name) : base(name)
		{
		}
		#endregion

		#region 公共属性
		public IListener Server
		{
			get
			{
				return _listener;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_listener = value;
			}
		}
		#endregion

		#region 重写方法
		protected override object OnExecute(CommandContext context)
		{
			return _listener;
		}
		#endregion

		#region 静态方法
		internal static IListener GetListener(CommandTreeNode node)
		{
			if(node == null)
				return null;

			var command = node.Command as ListenerCommand;

			if(command != null)
				return command.Server;

			return GetListener(node.Parent);
		}
		#endregion
	}
}
