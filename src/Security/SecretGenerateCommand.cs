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
	/// 提供生成验证码的命令类。
	/// </summary>
	/// <example>
	///		<code>secret.generate -name:'user.phone.change:100' -pattern:#4 -timeout:15m 13800000001 13800000002 13800000003</code>
	/// </example>
	/// <remarks>
	///		<para>命令‘pattern’选项即可以表示一个固定的验证码值，如果未指定或为空则生成6位数字的验证码；也可以表示生成验证码的规则，则大致如下所示：</para>
	///		<list type="bullet">
	///			<item>guid|uuid，表示生成一个GUID值</item>
	///			<item>#{number}，表示生成{number}个的数字字符，譬如：#4</item>
	///			<item>?{number}，表示生成{number}个的含有字母或数字的字符，譬如：?8</item>
	///			<item>*{number}，完全等同于?{number}。</item>
	///		</list>
	///		<para>命令‘timeout’选项表示验证码的缓存时长，支持“s(秒)”、“m(分钟)”、“h(小时)”和“d(天)”这几种单位，譬如：15m(15分钟)、24h(24小时)、7d(7天)。</para>
	/// </remarks>
	[CommandOption(KEY_NAME_OPTION, typeof(string), null, true, "Text.SecretGenerateCommand.Options.Name")]
	[CommandOption(KEY_PATTERN_OPTION, typeof(string), null, false, "Text.SecretGenerateCommand.Options.Pattern")]
	[CommandOption(KEY_TIMEOUT_OPTION, typeof(string), null, false, "Text.SecretGenerateCommand.Options.Timeout")]
	[System.ComponentModel.DisplayName("${Text.SecretGenerateCommand.Title}")]
	[System.ComponentModel.Description("${Text.SecretGenerateCommand.Description}")]
	public class SecretGenerateCommand : CommandBase<CommandContext>
	{
		#region 常量定义
		private const string KEY_NAME_OPTION = "name";
		private const string KEY_PATTERN_OPTION = "pattern";
		private const string KEY_TIMEOUT_OPTION = "timeout";
		#endregion

		#region 构造函数
		public SecretGenerateCommand() : base("Generate")
		{
		}

		public SecretGenerateCommand(string name) : base(name)
		{
		}
		#endregion

		#region 重写方法
		protected override object OnExecute(CommandContext context)
		{
			//从环境中查找秘密提供程序
			var secretProvider = SecretCommand.FindSecretProvider(context.CommandNode);

			if(secretProvider == null)
				throw new CommandException("Missing required secret provider for the command.");

			var name = context.Expression.Options.GetValue<string>(KEY_NAME_OPTION);
			var pattern = context.Expression.Options.GetValue<string>(KEY_PATTERN_OPTION);
			var timeout = this.GetTimeout(context.Expression.Options.GetValue<string>(KEY_TIMEOUT_OPTION));

			switch(context.Expression.Arguments.Length)
			{
				case 0:
					return secretProvider.Generate(name, pattern, null, timeout);
				case 1:
					return secretProvider.Generate(name, pattern, context.Expression.Arguments[0], timeout);
			}

			//定义返回验证码的数组
			var results = new string[context.Expression.Arguments.Length];

			for(int i = 0; i < context.Expression.Arguments.Length; i++)
			{
				results[i] = secretProvider.Generate(name, pattern, context.Expression.Arguments[i], timeout);
			}

			return results;
		}
		#endregion

		#region 私有方法
		private TimeSpan GetTimeout(string text)
		{
			if(string.IsNullOrEmpty(text))
				return TimeSpan.FromMinutes(10);

			var unit = '\0';
			var number = 0;

			switch(text[text.Length - 1])
			{
				case 's':
				case 'm':
				case 'h':
				case 'd':
					if(text.Length < 2)
						throw new CommandOptionValueException(KEY_TIMEOUT_OPTION, text);

					unit = text[text.Length - 1];

					if(!int.TryParse(text.Substring(0, text.Length - 1), out number))
						throw new CommandOptionValueException(KEY_TIMEOUT_OPTION, text);

					break;
				default:
					unit = 'm';

					if(!int.TryParse(text, out number))
						throw new CommandOptionValueException(KEY_TIMEOUT_OPTION, text);

					break;
			}

			if(number < 0)
				number = 10;

			switch(unit)
			{
				case 's':
					return TimeSpan.FromSeconds(number);
				case 'm':
					return TimeSpan.FromMinutes(number);
				case 'h':
					return TimeSpan.FromHours(number);
				case 'd':
					return TimeSpan.FromDays(number);
			}

			return TimeSpan.FromMinutes(number);
		}
		#endregion
	}
}
