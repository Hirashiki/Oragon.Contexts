﻿using System;

namespace Oragon.Contexts
{
	[AttributeUsage(AttributeTargets.Method, Inherited = true)]
	public abstract class AbstractContextAttribute : Attribute
	{
		#region Public Properties

		public string ContextKey { get; set; }

		#endregion Public Properties
	}
}