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
	///		<code>authenticode.verify -type:user.register -erasure '13800000001=1234' '13800000002=7890' 'popeye@zongsoft.com=abcxyz'</code>
	/// </example>
	[CommandOption(KEY_TYPE_OPTION, typeof(string), null, false, "Text.AuthenticodeVerifyCommand.Options.Type")]
	[CommandOption(KEY_ERASURE_OPTION, Type = null, Description = "Text.AuthenticodeVerifyCommand.Options.Erasure")]
	public class AuthenticodeVerifyCommand : CommandBase<CommandContext>
	{
		#region 常量定义
		private const string KEY_TYPE_OPTION = "type";
		private const string KEY_ERASURE_OPTION = "erasure";
		#endregion

		#region 构造函数
		public AuthenticodeVerifyCommand() : base("Verify")
		{
		}

		public AuthenticodeVerifyCommand(string name) : base(name)
		{
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

			//将命令参数集转换成验证方式的字典，即“key=code”
			var pairs = this.GetPairs(context.Expression.Arguments);

			//如果转换的字典为空或空集，则说明命令参数集格式异常
			if(pairs == null || pairs.Count == 0)
				throw new CommandException("Invalid format of the command arguments.");

			IList<bool> list = null;

			if(pairs.Count > 1)
				list = new List<bool>(context.Expression.Arguments.Length);

			foreach(var pair in pairs)
			{
				//获取验证码的缓存键
				var cacheKey = AuthenticodeCommand.GetCacheKey(
					context.Expression.Options.GetValue<string>(KEY_TYPE_OPTION),
					pair.Key);

				//获取验证码的缓存值
				var cacheValue = cache.GetValue<string>(cacheKey);

				//核查验证码是否相同（不区分大小写）
				var succeed = string.Equals(
					pair.Value,
					AuthenticodeCommand.GetCacheValue(cacheValue),
					StringComparison.OrdinalIgnoreCase);

				//如果验证成功，并且指定了擦除选项则将验证码从缓存中删除
				if(succeed && context.Expression.Options.Contains(KEY_ERASURE_OPTION))
					cache.Remove(cacheKey);

				if(list == null)
					return succeed;

				//将当前验证结果加入到结果集中
				list.Add(succeed);
			}

			if(list.Count == 1)
				return list[0];

			return list;
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
