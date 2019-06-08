/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2019 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Linq;

using Zongsoft.Services;
using Zongsoft.Resources;

namespace Zongsoft.IO.Commands
{
	public class FileExistsCommand : CommandBase<CommandContext>
	{
		#region 构造函数
		public FileExistsCommand() : base("Exists")
		{
		}

		public FileExistsCommand(string name) : base(name)
		{
		}
		#endregion

		#region 重写方法
		protected override object OnExecute(CommandContext context)
		{
			if(context.Expression.Arguments.Length == 0)
				throw new CommandException(ResourceUtility.GetString("Text.Command.MissingArguments"));

			bool DeleteFile(string path)
			{
				var existed = FileSystem.File.Exists(path);

				if(existed)
					context.Output.WriteLine(CommandOutletColor.Green, ResourceUtility.GetString("Text.FileExisted", path));
				else
					context.Output.WriteLine(CommandOutletColor.Red, ResourceUtility.GetString("Text.FileNotExisted", path));

				return existed;
			}

			if(context.Expression.Arguments.Length == 1)
				return DeleteFile(context.Expression.Arguments[0]);
			else
				return context.Expression.Arguments.Select(path => DeleteFile(path)).ToArray();
		}
		#endregion
	}
}
