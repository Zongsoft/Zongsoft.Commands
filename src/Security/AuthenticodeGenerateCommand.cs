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
	///		<code>authenticode.generate -type:user.register -code:#4 -timeout:15m 13800000001 13800000002 popeye@zongsoft.com</code>
	/// </example>
	/// <remarks>
	///		<para>命令‘code’选项即可以表示一个固定的验证码值，如果未指定或为空则生成6位数字的验证码；也可以表示生成验证码的规则，则大致如下所示：</para>
	///		<list type="bullet">
	///			<item>guid|uuid，表示生成一个GUID值</item>
	///			<item>#{number}，表示生成{number}个的数字字符，譬如：#4</item>
	///			<item>?{number}，表示生成{number}个的含有字母或数字的字符，譬如：?8</item>
	///			<item>*{number}，完全等同于?{number}。</item>
	///		</list>
	///		<para>命令‘timeout’选项表示验证码的缓存时长，支持“s(秒)”、“m(分钟)”、“h(小时)”和“d(天)”这几种单位，譬如：15m(15分钟)、24h(24小时)。</para>
	/// </remarks>
	[CommandOption(KEY_CODE_OPTION, typeof(string), null, false, "Text.AuthenticodeGenerateCommand.Options.Code")]
	[CommandOption(KEY_TYPE_OPTION, typeof(string), null, false, "Text.AuthenticodeGenerateCommand.Options.Type")]
	[CommandOption(KEY_TIMEOUT_OPTION, typeof(string), null, true, "Text.AuthenticodeGenerateCommand.Options.Timeout")]
	public class AuthenticodeGenerateCommand : CommandBase<CommandContext>
	{
		#region 常量定义
		private const string KEY_CODE_OPTION = "code";
		private const string KEY_TYPE_OPTION = "type";
		private const string KEY_TIMEOUT_OPTION = "timeout";
		#endregion

		#region 成员字段
		private int _period;
		#endregion

		#region 构造函数
		public AuthenticodeGenerateCommand() : base("Generate")
		{
			_period = 60;
		}

		public AuthenticodeGenerateCommand(string name) : base(name)
		{
			_period = 60;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置验证码的生成最小间隔时长，单位为秒。默认为60秒。
		/// </summary>
		public int Period
		{
			get
			{
				return _period;
			}
			set
			{
				_period = Math.Max(value, 0);
			}
		}
		#endregion

		#region 重写方法
		protected override object OnExecute(CommandContext context)
		{
			if(context.Expression.Arguments.Length == 0)
				throw new CommandException(Resources.ResourceUtility.GetString("Text.Command.MissingArguments"));

			//从环境中查找缓存容器
			var cache = AuthenticodeCommand.FindCache(context.CommandNode);

			if(cache == null)
				throw new CommandException("Missing required cache for the command.");

			var pattern = context.Expression.Options.GetValue<string>(KEY_CODE_OPTION);
			var timeout = this.GetTimeout(context.Expression.Options.GetValue<string>(KEY_TIMEOUT_OPTION));

			//定义返回验证码列表
			var list = new List<string>(context.Expression.Arguments.Length);

			for(int i = 0; i < context.Expression.Arguments.Length; i++)
			{
				//获取验证码的缓存键
				var cacheKey = AuthenticodeCommand.GetCacheKey(
					context.Expression.Options.GetValue<string>(KEY_TYPE_OPTION),
					context.Expression.Arguments[i]);

				//获取验证码的缓存值
				var cacheValue = cache.GetValue<string>(cacheKey);

				if(!string.IsNullOrEmpty(cacheValue))
				{
					//获取验证码缓存的时间戳（即验证码的生成时间）
					var timestamp = AuthenticodeCommand.GetCacheTimestamp(cacheValue);

					//如果当前时间与验证码时间戳的间隔太短，则抛出命令执行太频繁的异常
					if(_period > 0 && (DateTime.Now - timestamp).TotalSeconds < _period)
						throw new InvalidOperationException(Resources.ResourceUtility.GetString("Text.AuthenticodeGenerateCommand.Overfrequency"));
				}

				//生成一个新的验证码
				var code = this.GenerateCode(pattern);

				//将生成的验证码保存到缓存中（注：缓存值带有时间戳）
				if(cache.SetValue(cacheKey, AuthenticodeCommand.GetCacheWrapper(code), timeout))
				{
					list.Add(code);
				}
			}

			switch(list.Count)
			{
				case 0:
					return null;
				case 1:
					return list[0];
				default:
					return list.ToArray();
			}
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

		private string GenerateCode(string pattern)
		{
			if(string.IsNullOrEmpty(pattern))
				return Common.RandomGenerator.GenerateString(6, true);

			if(string.Equals(pattern, "guid", StringComparison.OrdinalIgnoreCase) || string.Equals(pattern, "uuid", StringComparison.OrdinalIgnoreCase))
				return Guid.NewGuid().ToString("N");

			if(pattern.Length > 1 && (pattern[0] == '?' || pattern[0] == '*' || pattern[0] == '#'))
			{
				if(int.TryParse(pattern.Substring(1), out var count))
					return Common.RandomGenerator.GenerateString(count, pattern[0] == '#');

				throw new CommandOptionValueException(KEY_CODE_OPTION, pattern);
			}

			return pattern;
		}
		#endregion
	}
}
