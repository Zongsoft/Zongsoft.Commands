/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015-2017 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Security.Commands
{
	/// <summary>
	/// 提供验证码校验的命令类。
	/// </summary>
	/// <example>
	///		<code>secret.verify -name:'user.email:100' 123456</code>
	/// </example>
	[CommandOption(KEY_NAME_OPTION, typeof(string), null, true, "Text.SecretVerifyCommand.Options.Name")]
	[System.ComponentModel.DisplayName("${Text.SecretVerifyCommand.Title}")]
	[System.ComponentModel.Description("${Text.SecretVerifyCommand.Description}")]
	public class SecretVerifyCommand : CommandBase<CommandContext>
	{
		#region 常量定义
		private const string KEY_NAME_OPTION = "name";
		#endregion

		#region 构造函数
		public SecretVerifyCommand() : base("Verify")
		{
		}

		public SecretVerifyCommand(string name) : base(name)
		{
		}
		#endregion

		#region 重写方法
		protected override object OnExecute(CommandContext context)
		{
			if(context.Expression.Arguments.Length == 0)
				throw new CommandException(Resources.ResourceUtility.GetString("Text.Command.MissingArguments"));

			//从环境中查找秘密提供程序
			var secretProvider = SecretCommand.FindSecretProvider(context.CommandNode);

			if(secretProvider == null)
				throw new CommandException("Missing required secret provider for the command.");

			if(secretProvider.Verify(context.Expression.Options.GetValue<string>(KEY_NAME_OPTION), context.Expression.Arguments[0], out var extra))
			{
				if(extra != null && extra.Length > 0)
					context.Output.WriteLine(extra);

				return true;
			}

			return false;
		}
		#endregion

		#region 私有方法
		private IDictionary<string, string> GetPairs(string[] arguments)
		{
			if(arguments == null)
				throw new ArgumentNullException(nameof(arguments));

			var dictionary = new Dictionary<string, string>(arguments.Length, StringComparer.OrdinalIgnoreCase);

			foreach(var text in arguments)
			{
				if(string.IsNullOrEmpty(text))
					continue;

				var index = text.IndexOf('=');

				if(index < 0)
					index = text.IndexOf(':');

				if(index > 0 && index < text.Length - 1)
					dictionary[text.Substring(0, index).Trim()] = text.Substring(index + 1).Trim();
			}

			return dictionary;
		}
		#endregion
	}
}
