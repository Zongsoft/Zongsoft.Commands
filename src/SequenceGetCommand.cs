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
using System.ComponentModel;
using System.Collections.Generic;

using Zongsoft.Common;
using Zongsoft.Services;
using Zongsoft.Resources;

namespace Zongsoft.Commands
{
	[CommandOption("type", Type = typeof(SequenceGetType), DefaultValue = SequenceGetType.Number, Description = "${SequenceGetCommand.Options.Type}")]
	[CommandOption("interval", Type = typeof(int), DefaultValue = 1, Description = "${SequenceGetCommand.Options.Interval}")]
	public class SequenceGetCommand : CommandBase<CommandContext>
	{
		public enum SequenceGetType
		{
			[Description("${Text.SequenceGetType.Number}")]
			Number,

			[Description("${Text.SequenceGetType.String}")]
			String,
		}

		#region 构造函数
		public SequenceGetCommand() : base("Get")
		{
		}

		public SequenceGetCommand(ISequence sequence) : base("Get")
		{
		}

		public SequenceGetCommand(string name) : base(name)
		{
		}

		public SequenceGetCommand(string name, ISequence sequence) : base(name)
		{
		}
		#endregion

		#region 重写方法
		protected override object OnExecute(CommandContext context)
		{
			if(context.Expression.Arguments.Length == 0)
				throw new CommandException(ResourceUtility.GetString("Text.SequenceCommand.MissingArguments"));

			int interval = 1;
			var result = new object[context.Expression.Arguments.Length];

			var sequence = SequenceCommand.FindSequence(context.CommandNode);

			if(sequence == null)
				throw new CommandException(ResourceUtility.GetString("Text.CannotObtainCommandTarget", "Sequence"));

			for(int i = 0; i < context.Expression.Arguments.Length; i++)
			{
				object value = null;

				switch(context.Expression.Options.GetValue<SequenceGetType>("type"))
				{
					case SequenceGetType.String:
						if(context.Expression.Options.TryGetValue("interval", out interval))
							value = sequence.GetSequenceString(context.Expression.Arguments[i], interval);
						else
							value = sequence.GetSequenceString(context.Expression.Arguments[i]);
						break;
					default:
						if(context.Expression.Options.TryGetValue("interval", out interval))
							value = sequence.GetSequenceNumber(context.Expression.Arguments[i], interval);
						else
							value = sequence.GetSequenceNumber(context.Expression.Arguments[i]);
						break;
				}

				context.Output.Write(CommandOutletColor.DarkMagenta, "[{0}] ", i + 1);
				context.Output.WriteLine(value);

				result[i] = value;
			}

			return result;
		}
		#endregion
	}
}
