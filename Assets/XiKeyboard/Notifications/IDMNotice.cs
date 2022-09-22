/* Copyright (c) 2021 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

namespace XiKeyboard
{
	public interface IDMNotice
	{
		#region Methods

		void Notify(DMMenuLine item, Color? nameColor = null, Color? valueColor = null);

		#endregion
	}
}