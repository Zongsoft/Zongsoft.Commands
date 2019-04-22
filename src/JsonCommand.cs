/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2011-2017 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.IO;
using System.ComponentModel;

using Zongsoft.Services;
using Zongsoft.Runtime.Serialization;

namespace Zongsoft.Commands
{
	[DisplayName("${Text.JsonCommand.Name}")]
	[Description("${Text.JsonCommand.Description}")]
	[CommandOption(KEY_DEPTH_OPTION, typeof(int), 3, "Text.JsonCommand.Options.Depth")]
	[CommandOption(KEY_TYPED_OPTION, typeof(bool), false, "Text.JsonCommand.Options.Typed")]
	[CommandOption(KEY_INDENTED_OPTION, typeof(bool), true, "Text.JsonCommand.Options.Indented")]
	[CommandOption(KEY_CASING_OPTION, typeof(SerializationNamingConvention), SerializationNamingConvention.None, "Text.JsonCommand.Options.Casing")]
	public class JsonCommand : CommandBase<CommandContext>
	{
		#region 常量定义
		private const string KEY_DEPTH_OPTION = "depth";
		private const string KEY_TYPED_OPTION = "typed";
		private const string KEY_CASING_OPTION = "casing";
		private const string KEY_INDENTED_OPTION = "indented";
		#endregion

		#region 构造函数
		public JsonCommand() : base("Json")
		{
		}

		public JsonCommand(string name) : base(name)
		{
		}
		#endregion

		#region 重写方法
		protected override object OnExecute(CommandContext context)
		{
			var graph = context.Parameter;

			if(graph == null)
				return null;

			//如果输入参数是文本或流或文本读取器，则反序列化它并返回
			if(graph is string raw)
				return Serializer.Json.Deserialize(raw);
			if(graph is System.Text.StringBuilder text)
				return Serializer.Json.Deserialize(text.ToString());
			if(graph is Stream stream)
				return Serializer.Json.Deserialize(stream);
			if(graph is TextReader reader)
				return Serializer.Json.Deserialize(reader);

			var settings = new TextSerializationSettings()
			{
				MaximumDepth = context.Expression.Options.GetValue<int>(KEY_DEPTH_OPTION),
				Typed = context.Expression.Options.GetValue<bool>(KEY_TYPED_OPTION),
				Indented = context.Expression.Options.GetValue<bool>(KEY_INDENTED_OPTION),
				NamingConvention = context.Expression.Options.GetValue<SerializationNamingConvention>(KEY_CASING_OPTION),
			};

			var json = Serializer.Json.Serialize(graph, settings);

			if(json != null)
				context.Output.WriteLine(json);

			return json;
		}
		#endregion
	}
}
