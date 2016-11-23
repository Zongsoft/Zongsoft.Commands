﻿/*
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
using System.IO;
using System.Collections.Generic;
using System.Text;

using Zongsoft.Services;
using Zongsoft.Resources;

namespace Zongsoft.Collections.Commands
{
	[CommandOption("type", Type = typeof(ContentType), DefaultValue = ContentType.String)]
	[CommandOption("encoding", Type = typeof(Encoding), DefaultValue = "utf-8")]
	[CommandOption("round", Type = typeof(int), DefaultValue = 1, Description = "${Text.QueueCommand.Options.Round}")]
	[CommandOption("queues", Type = typeof(string), Description = "${Text.QueueCommand.Options.Queues}")]
	public class QueueInCommand : CommandBase<CommandContext>
	{
		#region 构造函数
		public QueueInCommand() : this("In")
		{
		}

		public QueueInCommand(string name) : base(name)
		{
		}
		#endregion

		#region 执行方法
		protected override object OnExecute(CommandContext context)
		{
			var round = (int)context.Expression.Options.GetValue<int>("round");
			var queues = QueueCommandHelper.GetQueues(context.CommandNode, context.Expression.Options.GetValue<string>("queues"));

			foreach(var queue in queues)
			{
				for(int i = 0; i < round; i++)
				{
					var count = context.Parameter == null ? 0 : 1;

					if(count > 0)
						queue.Enqueue(context.Parameter);

					count += this.ResolveValue(context, (value) =>
					{
						if(value != null)
							queue.Enqueue(value);
					});

					context.Output.WriteLine(CommandOutletColor.DarkGreen, string.Format(ResourceUtility.GetString("Text.QueueInCommand.Message", i + 1, count, queue.Name)));
				}
			}

			return null;
		}
		#endregion

		#region 私有方法
		private int ResolveValue(CommandContext context, Action<object> fallback)
		{
			int result = 0;

			switch(context.Expression.Options.GetValue<ContentType>("type"))
			{
				case ContentType.BinaryFile:
					foreach(var arg in context.Expression.Arguments)
					{
						if(!File.Exists(arg))
						{
							context.Output.WriteLine(ResourceUtility.GetString("FileOrDirectoryNotExists", arg));
							continue;
						}

						fallback(File.ReadAllBytes(arg.ToString()));
						result++;
					}
					break;
				case ContentType.TextFile:
					var encoding = context.Expression.Options.GetValue<Encoding>("encoding");

					foreach(var arg in context.Expression.Arguments)
					{
						string[] filePaths = null;

						if(arg.Contains("*") || arg.Contains("?"))
						{
							var directory = Path.GetDirectoryName(arg);

							if(!Directory.Exists(directory))
							{
								context.Output.WriteLine(ResourceUtility.GetString("FileOrDirectoryNotExists", arg));
								continue;
							}

							filePaths = Directory.GetFiles(directory, Path.GetFileName(arg));

							foreach(var filePath in filePaths)
							{
								fallback(File.ReadAllText(filePath, encoding));
							}

							result += filePaths.Length;
						}
						else
						{
							if(!File.Exists(arg))
							{
								context.Output.WriteLine(ResourceUtility.GetString("FileOrDirectoryNotExists", arg));
								continue;
							}

							fallback(File.ReadAllText(arg.ToString(), encoding));
							result++;
						}
					}
					break;
				case ContentType.String:
					foreach(var arg in context.Expression.Arguments)
					{
						fallback(arg);
					}
					break;
				case ContentType.Byte:
					foreach(var arg in context.Expression.Arguments)
					{
						fallback(Zongsoft.Common.Convert.ConvertValue<byte>(arg));
					}
					break;
				case ContentType.Short:
					foreach(var arg in context.Expression.Arguments)
					{
						fallback(Zongsoft.Common.Convert.ConvertValue<short>(arg));
					}
					break;
				case ContentType.Integer:
					foreach(var arg in context.Expression.Arguments)
					{
						fallback(Zongsoft.Common.Convert.ConvertValue<int>(arg));
					}
					break;
				case ContentType.Long:
					foreach(var arg in context.Expression.Arguments)
					{
						fallback(Zongsoft.Common.Convert.ConvertValue<long>(arg));
					}
					break;
				case ContentType.Date:
				case ContentType.DateTime:
					if(context.Expression.Arguments.Length < 1)
						fallback(DateTime.Now);
					else
					{
						foreach(var arg in context.Expression.Arguments)
						{
							fallback(Zongsoft.Common.Convert.ConvertValue<DateTime>(arg));
						}
					}
					break;
				case ContentType.Guid:
					if(context.Expression.Arguments.Length < 1)
						fallback(Guid.NewGuid());
					else
					{
						foreach(var arg in context.Expression.Arguments)
						{
							fallback(Zongsoft.Common.Convert.ConvertValue<Guid>(arg));
						}
					}
					break;
				default:
					return 0;
			}

			return Math.Max(1, result);
		}
		#endregion

		#region 枚举定义
		public enum ContentType
		{
			String,

			[Zongsoft.ComponentModel.Alias("int")]
			Integer,
			Short,
			Long,
			Byte,
			Date,
			DateTime,
			Guid,
			BinaryFile,
			TextFile,
		}
		#endregion
	}
}
