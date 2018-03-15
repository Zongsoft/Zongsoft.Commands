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

namespace Zongsoft.Security.Commands
{
	[DisplayName("${Text.SecretCommand.Title}")]
	[Description("${Text.SecretCommand.Description}")]
	public class SecretCommand : CommandBase<CommandContext>
	{
		#region 成员字段
		private ISecretProvider _secret;
		#endregion

		#region 构造函数
		public SecretCommand() : base("Secret")
		{
		}

		public SecretCommand(string name) : base(name)
		{
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置验证码提供程序。
		/// </summary>
		[Zongsoft.Services.ServiceDependency]
		public ISecretProvider Secret
		{
			get
			{
				return _secret;
			}
			set
			{
				_secret = value;
			}
		}
		#endregion

		#region 重写方法
		protected override object OnExecute(CommandContext context)
		{
			if(context.Parameter is ISecretProvider)
				_secret = (ISecretProvider)context.Parameter;

			return null;
		}
		#endregion

		#region 内部方法
		internal static ISecretProvider FindSecretProvider(CommandTreeNode node)
		{
			if(node == null)
				return null;

			var command = node.Command as SecretCommand;

			if(command != null)
				return command.Secret;

			return FindSecretProvider(node.Parent);
		}
		#endregion
	}
}
