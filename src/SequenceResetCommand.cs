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
	[CommandOption("value", Type = typeof(int), DefaultValue = 0, Required = true, Description = "${SequenceResetCommand.Options.Value}")]
	[CommandOption("interval", Type = typeof(int), DefaultValue = 1, Description = "${SequenceResetCommand.Options.Interval}")]
	[CommandOption("format", Type = typeof(string), Description = "${SequenceResetCommand.Options.Format}")]
	public class SequenceResetCommand : CommandBase<CommandContext>
	{
		#region 构造函数
		public SequenceResetCommand() : base("Reset")
		{
		}

		public SequenceResetCommand(ISequence sequence) : base("Reset")
		{
		}

		public SequenceResetCommand(string name) : base(name)
		{
		}

		public SequenceResetCommand(string name, ISequence sequence) : base(name)
		{
		}
		#endregion

		#region 重写方法
		protected override object OnExecute(CommandContext context)
		{
			if(context.Expression.Arguments.Length == 0)
				throw new CommandException(ResourceUtility.GetString("Text.SequenceCommand.MissingArguments"));

			var sequence = SequenceCommand.FindSequence(context.CommandNode);

			if(sequence == null)
				throw new CommandException(ResourceUtility.GetString("Text.CannotObtainCommandTarget", "Sequence"));

			for(int i = 0; i < context.Expression.Arguments.Length; i++)
			{
				context.Output.Write(CommandOutletColor.DarkMagenta, "[{0}] ", i + 1);
				context.Output.Write(context.Expression.Arguments[i] + "   ...   ");

				sequence.Reset(context.Expression.Arguments[i],
					           context.Expression.Options.GetValue<int>("value"),
							   context.Expression.Options.GetValue<int>("interval"),
							   context.Expression.Options.GetValue<string>("format"));

				context.Output.WriteLine(CommandOutletColor.Green, "[OK]");
			}

			return context.Expression.Arguments;
		}
		#endregion
	}
}
