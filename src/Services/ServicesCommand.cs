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

using Zongsoft.Resources;

namespace Zongsoft.Services.Commands
{
	[DisplayName("${Text.ServicesCommand.DisplayName}")]
	[Description("${Text.ServicesCommand.Description}")]
	[CommandOption("provider", typeof(string), Description = "${Text.ServicesCommand.Options.Provider}")]
	public class ServicesCommand : CommandBase<CommandContext>
	{
		#region 成员变量
		private IServiceProvider _serviceProvider;
		private IServiceProviderFactory _serviceFactory;
		#endregion

		#region 构造函数
		protected ServicesCommand() : base("Services")
		{
		}

		protected ServicesCommand(string name) : base(name)
		{
		}

		public ServicesCommand(IServiceProviderFactory serviceFactory) : this("Services", serviceFactory)
		{
		}

		protected ServicesCommand(string name, IServiceProviderFactory serviceFactory) : base(name)
		{
			if(serviceFactory == null)
				throw new ArgumentNullException("providerFactory");

			_serviceFactory = serviceFactory;
			_serviceProvider = serviceFactory.Default;
		}
		#endregion

		#region 公共属性
		public IServiceProviderFactory ServiceFactory
		{
			get
			{
				return _serviceFactory;
			}
		}

		public Zongsoft.Services.IServiceProvider ServiceProvider
		{
			get
			{
				return _serviceProvider;
			}
			set
			{
				_serviceProvider = value;
			}
		}
		#endregion

		#region 重写方法
		protected override object OnExecute(CommandContext context)
		{
			if(_serviceFactory == null)
				throw new CommandException(ResourceUtility.GetString("Text.CannotObtainCommandTarget", "ServiceProviderFactory"));

			string providerName;

			if(context.Expression.Options.TryGetValue("provider", out providerName))
			{
				if(string.IsNullOrWhiteSpace(providerName) || providerName == "~" || providerName == ".")
					_serviceProvider = _serviceFactory.Default;
				else
				{
					var provider = _serviceFactory.GetProvider(providerName);

					if(provider == null)
						throw new CommandException(ResourceUtility.GetString("Text.ServicesCommand.NotFoundProvider", providerName));

					_serviceProvider = provider;
				}

				//显示执行成功的信息
				context.Output.WriteLine(ResourceUtility.GetString("Text.CommandExecuteSucceed"));
			}

			var items = _serviceFactory as IEnumerable<KeyValuePair<string, Zongsoft.Services.IServiceProvider>>;

			if(items != null)
			{
				int index = 1;

				foreach(var item in items)
				{
					context.Output.Write(CommandOutletColor.DarkMagenta, "[{0}] ", index++);

					if(string.IsNullOrWhiteSpace(item.Key))
						context.Output.Write("<Default>");
					else
						context.Output.Write(item.Key);

					if(object.ReferenceEquals(item.Value, this.ServiceProvider))
						context.Output.WriteLine(CommandOutletColor.Green, " (Actived)");
					else
						context.Output.WriteLine();
				}
			}

			return _serviceProvider;
		}
		#endregion

		#region 静态方法
		internal static Zongsoft.Services.IServiceProvider GetServiceProvider(CommandTreeNode node)
		{
			if(node == null)
				return null;

			var command = node.Command as ServicesCommand;

			if(command != null)
				return command.ServiceProvider;

			return GetServiceProvider(node.Parent);
		}
		#endregion
	}
}
