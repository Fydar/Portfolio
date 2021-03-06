﻿using RPGCore.Data;

namespace Portfolio.Models
{
	[EditableType]
	public class QualificationModel : ActivityModel
	{
		public string FullQualificationName { get; set; } = string.Empty;
		public string Grade { get; set; } = string.Empty;
		public string TimeSpan { get; set; } = string.Empty;
	}
}
