﻿/*
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

using Zongsoft.Services;
using Zongsoft.Resources;

namespace Zongsoft.IO.Commands
{
	public class FileDeleteCommand : CommandBase<CommandContext>
	{
		#region 构造函数
		public FileDeleteCommand() : base("Delete")
		{
		}

		public FileDeleteCommand(string name) : base(name)
		{
		}
		#endregion

		#region 重写方法
		protected override object OnExecute(CommandContext context)
		{
			if(context.Expression.Arguments.Length == 0)
				throw new CommandException(ResourceUtility.GetString("Text.Command.MissingArguments"));

			for(int i=0; i< context.Expression.Arguments.Length; i++)
			{
				var filePath = context.Expression.Arguments[i];
				var message = string.Empty;
				var succeed = FileSystem.File.Delete(filePath);

				if(succeed)
					message = ResourceUtility.GetString("Text.FileDeleteSucceed.Message");
				else
					message = ResourceUtility.GetString("Text.FileDeleteFailed.Message");

				context.Output.WriteLine((succeed ? CommandOutletColor.Green : CommandOutletColor.Red), $"[{i+1}] `{filePath}` {message}");
			}

			return null;
		}
		#endregion
	}
}
