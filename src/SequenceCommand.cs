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
using System.Collections.Generic;

using Zongsoft.Common;
using Zongsoft.Services;
using Zongsoft.Resources;

namespace Zongsoft.Commands
{
	public class SequenceCommand : CommandBase<CommandContext>
	{
		#region 成员字段
		private ISequence _sequence;
		#endregion

		#region 构造函数
		public SequenceCommand() : base("Sequence")
		{
		}

		public SequenceCommand(ISequence sequence) : base("Sequence")
		{
			if(sequence == null)
				throw new ArgumentNullException("sequence");

			_sequence = sequence;
		}

		public SequenceCommand(string name) : base(name)
		{
		}

		public SequenceCommand(string name, ISequence sequence) : base(name)
		{
			if(sequence == null)
				throw new ArgumentNullException("sequence");

			_sequence = sequence;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置当前命令操作对应的<see cref="Zongsoft.Common.ISequence"/>序列号对象。
		/// </summary>
		public ISequence Sequence
		{
			get
			{
				return _sequence;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_sequence = value;
			}
		}
		#endregion

		#region 重写方法
		protected override object OnExecute(CommandContext context)
		{
			var sequence = this.Sequence;

			if(sequence == null)
				throw new CommandException(ResourceUtility.GetString("Text.CannotObtainCommandTarget", "Sequence"));

			if(context.Expression.Arguments.Length == 0)
			{
				context.Output.WriteLine(sequence);
				return null;
			}

			var result = new List<SequenceInfo>();

			for(int i = 0; i < context.Expression.Arguments.Length; i++)
			{
				var info = sequence.GetSequenceInfo(context.Expression.Arguments[i]);

				context.Output.Write(CommandOutletColor.DarkMagenta, "[{0}] ", i + 1);

				if(info == null)
					context.Output.WriteLine(ResourceUtility.GetString("Text.SequenceCommand.NotFoundSequence", context.Expression.Arguments[i]));
				else
				{
					context.Output.WriteLine(ResourceUtility.GetString("Text.SequenceCommand.Info", context.Expression.Arguments[i], info.Value, info.Interval, info.FormatString));
					result.Add(info);
				}
			}

			return result.ToArray();
		}
		#endregion

		#region 内部方法
		internal static ISequence FindSequence(CommandTreeNode node)
		{
			if(node == null)
				return null;

			var command = node.Command as SequenceCommand;

			if(command != null)
				return command.Sequence;

			return FindSequence(node.Parent);
		}
		#endregion
	}
}
