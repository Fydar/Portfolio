﻿using RPGCore.DataEditor.CSharp;

namespace Portfolio.Models
{
	[EditorType]
	public class EmploymentModel : ActivityModel
	{
		public long EndTime { get; set; }
	}
}
