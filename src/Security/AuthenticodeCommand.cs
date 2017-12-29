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
using System.ComponentModel;

using Zongsoft.Services;
using Zongsoft.Runtime.Caching;

namespace Zongsoft.Security.Commands
{
	public class AuthenticodeCommand : CommandBase<CommandContext>
	{
		#region 常量字段
		private static readonly DateTime EPOCH = new DateTime(2000, 1, 1);
		#endregion

		#region 成员字段
		private ICache _cache;
		#endregion

		#region 构造函数
		public AuthenticodeCommand() : base("Authenticode")
		{
		}

		public AuthenticodeCommand(string name) : base(name)
		{
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置验证码命令依赖的缓存容器。
		/// </summary>
		[Zongsoft.Services.ServiceDependency]
		public ICache Cache
		{
			get
			{
				return _cache;
			}
			set
			{
				_cache = value ?? throw new ArgumentNullException();
			}
		}
		#endregion

		#region 重写方法
		protected override object OnExecute(CommandContext context)
		{
			if(context.Parameter is ICache)
				_cache = (ICache)context.Parameter;

			return null;
		}
		#endregion

		#region 内部方法
		internal static ICache FindCache(CommandTreeNode node)
		{
			if(node == null)
				return null;

			var command = node.Command as AuthenticodeCommand;

			if(command != null)
				return command.Cache;

			return FindCache(node.Parent);
		}

		internal static string GetCacheKey(string type, string id)
		{
			const string KEY_CACHE_PREFIX = "zongsoft.authenticode:";

			if(string.IsNullOrWhiteSpace(id))
				throw new ArgumentNullException(nameof(id));

			if(string.IsNullOrWhiteSpace(type))
				return KEY_CACHE_PREFIX + id.Trim().ToLowerInvariant();
			else
				return KEY_CACHE_PREFIX + type.Trim().ToLowerInvariant() + ":" + id.Trim().ToLowerInvariant();
		}

		internal static string GetCacheWrapper(string value)
		{
			if(string.IsNullOrEmpty(value))
				return null;

			var timestamp = (long)((DateTime.Now - EPOCH).TotalSeconds);

			return value + "|" + timestamp.ToString();
		}

		internal static DateTime GetCacheTimestamp(string wrapping)
		{
			if(string.IsNullOrEmpty(wrapping))
				return EPOCH;

			var index = wrapping.IndexOf('|');

			if(index > 0 && index < wrapping.Length - 1)
			{
				if(long.TryParse(wrapping.Substring(index + 1), out var number))
					return EPOCH.AddSeconds(number);
			}

			return EPOCH;
		}

		internal static string GetCacheValue(string wrapping)
		{
			if(string.IsNullOrEmpty(wrapping))
				return null;

			var index = wrapping.IndexOf('|');

			if(index > 0 && index < wrapping.Length - 1)
				return wrapping.Substring(0, index);

			return wrapping;
		}
		#endregion
	}
}
