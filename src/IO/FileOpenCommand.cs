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
using System.IO;
using System.Collections.Generic;

using Zongsoft.Services;
using Zongsoft.Resources;

namespace Zongsoft.IO.Commands
{
	[CommandOption("mode", typeof(FileMode), FileMode.Open, "")]
	[CommandOption("access", typeof(FileAccess), FileAccess.Read, "")]
	[CommandOption("share", typeof(FileShare), FileShare.Read, "")]
	public class FileOpenCommand : CommandBase<CommandContext>, ICommandCompletion
	{
		#region 构造函数
		public FileOpenCommand() : base("Open")
		{
		}

		public FileOpenCommand(string name) : base(name)
		{
		}
		#endregion

		#region 执行方法
		protected override object OnExecute(CommandContext context)
		{
			if(context.Expression.Arguments.Length < 1)
				throw new CommandException(ResourceUtility.GetString("Text.Command.MissingArguments"));

			var list = new List<Stream>(context.Expression.Arguments.Length);

			try
			{
				foreach(var argument in context.Expression.Arguments)
				{
					list.Add(FileSystem.File.Open(argument,
						context.Expression.Options.GetValue<FileMode>("mode"),
						context.Expression.Options.GetValue<FileAccess>("access"),
						context.Expression.Options.GetValue<FileShare>("share")));
				}
			}
			catch
			{
				foreach(var item in list)
				{
					if(item != null)
						item.Dispose();
				}
			}

			return list.ToArray();
		}
		#endregion

		#region 完成回调
		public void OnCompleted(CommandCompletionContext context)
		{
			var streams = context.Result as IEnumerable<Stream>;

			if(streams == null)
				return;

			foreach(var stream in streams)
			{
				if(stream != null)
					stream.Close();
			}
		}
		#endregion
	}
}
