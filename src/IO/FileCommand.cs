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
using System.IO;
using System.Text;
using System.Collections.Generic;

using Zongsoft.Services;
using Zongsoft.Resources;

namespace Zongsoft.IO.Commands
{
	[CommandOption(KEY_MODE_OPTION, typeof(FileMode), FileMode.Open, "Text.FileCommand.Options.Mode")]
	[CommandOption(KEY_SHARE_OPTION, typeof(FileShare), FileShare.Read, "Text.FileCommand.Options.Share")]
	[CommandOption(KEY_ACCESS_OPTION, typeof(FileAccess), FileAccess.Read, "Text.FileCommand.Options.Access")]
	[CommandOption(KEY_ENCODING_OPTION, typeof(Encoding), null, "Text.FileCommand.Options.Encoding")]
	public class FileCommand : CommandBase<CommandContext>, ICommandCompletion
	{
		#region 常量定义
		private const string KEY_MODE_OPTION = "mode";
		private const string KEY_SHARE_OPTION = "share";
		private const string KEY_ACCESS_OPTION = "access";
		private const string KEY_ENCODING_OPTION = "encoding";
		#endregion

		#region 构造函数
		public FileCommand() : base("File")
		{
		}

		public FileCommand(string name) : base(name)
		{
		}
		#endregion

		#region 执行方法
		protected override object OnExecute(CommandContext context)
		{
			bool isSaving = context.Expression.Index > 0 && context.Expression.Next == null;

			if(!context.Expression.Options.TryGetValue<FileMode>(KEY_MODE_OPTION, out var mode))
				mode = isSaving ? FileMode.Create : FileMode.Open;

			if(!context.Expression.Options.TryGetValue<FileAccess>(KEY_ACCESS_OPTION, out var access))
				access = isSaving ? FileAccess.ReadWrite : FileAccess.Read;

			//打开一个或多个文件流
			var result = FileUtility.OpenFile(context, mode, access, context.Expression.Options.GetValue<FileShare>(KEY_SHARE_OPTION));

			//如果是写入操作则执行保存方法
			if(isSaving && result != null)
				FileUtility.Save(result, context.Parameter, context.Expression.Options.GetValue<Encoding>(KEY_ENCODING_OPTION));

			return result;
		}
		#endregion

		#region 执行完成
		public void OnCompleted(CommandCompletionContext context)
		{
			if(context.Result is IEnumerable<Stream> streams)
			{
				foreach(var stream in streams)
				{
					if(stream != null)
						stream.Close();
				}
			}
			else if(context.Result is Stream stream)
			{
				stream.Close();
			}
		}
		#endregion
	}
}
