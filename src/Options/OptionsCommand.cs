/*
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

using Zongsoft.Services;
using Zongsoft.Resources;

namespace Zongsoft.Options.Commands
{
	public class OptionsCommand : CommandBase<CommandContext>
	{
		#region 成员字段
		private IOptionProvider _options;
		#endregion

		#region 构造函数
		public OptionsCommand() : base("Options")
		{
			_options = Zongsoft.Services.ApplicationContext.Current.Options;
		}

		public OptionsCommand(string name) : base(name)
		{
			_options = Zongsoft.Services.ApplicationContext.Current.Options;
		}
		#endregion

		#region 公共属性
		public IOptionProvider Options
		{
			get
			{
				return _options;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_options = value;
			}
		}
		#endregion

		#region 执行方法
		protected override object OnExecute(CommandContext context)
		{
			var manager = _options as OptionManager;

			if(manager != null)
			{
				foreach(var node in manager.Nodes)
				{
					this.Print(context.Output, node, 0);
				}
			}

			return _options;
		}
		#endregion

		#region 私有方法
		private void Print(ICommandOutlet output, OptionNode node, int depth)
		{
			if(node == null)
				return;

			output.Write(CommandOutletColor.DarkYellow, (depth > 0 ? new string('\t', depth) : string.Empty) + node.Name);

			if(string.IsNullOrWhiteSpace(node.Title))
				output.WriteLine();
			else
				output.WriteLine(CommandOutletColor.DarkGray, " [{0}]", node.Title);

			if(node.Option != null && node.Option.OptionObject != null)
			{
				output.WriteLine(Zongsoft.Runtime.Serialization.Serializer.Text.Serialize(node.Option.OptionObject));
			}

			foreach(var child in node.Children)
			{
				Print(output, child, depth + 1);
			}
		}
		#endregion

		#region 静态方法
		internal static IOptionProvider GetOptionProvider(CommandTreeNode node)
		{
			if(node == null)
				return null;

			var command = node.Command as OptionsCommand;

			if(command != null)
				return command.Options;

			return GetOptionProvider(node.Parent);
		}
		#endregion
	}
}
