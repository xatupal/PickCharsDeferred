using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using PickCharsDeferred.Properties;
using KeePass.Plugins;
using KeePass.Util;
using KeePass.Util.Spr;
using KeePassLib;

namespace PickCharsDeferred
{
	public sealed class PickCharsDeferredExt : Plugin
	{
		private int _sequenceIndex = -1;
		private string[] _sequenceParts;

		public override bool Initialize(IPluginHost host)
		{
			if (host == null)
				return false;

			AutoType.FilterCompilePre += HandleAutoTypeFilterCompilePre;

			return true;
		}

		public override void Terminate()
		{
			AutoType.FilterCompilePre -= HandleAutoTypeFilterCompilePre;
		}

		private void HandleAutoTypeFilterCompilePre(object sender, AutoTypeEventArgs e)
		{
			if (_sequenceParts == null)
			{
				var newSequenceParts = Regex.Split(e.Sequence, @"(?=\{PICKCHARS)");
				if (newSequenceParts.Length == 1)
					return;

				_sequenceParts = newSequenceParts;
				_sequenceIndex = _sequenceParts.Length;
				e.Sequence = _sequenceParts[--_sequenceIndex];
				AutoType.PerformGlobal(new List<PwDatabase> { e.Database }, null);
			}
			else
			{
				e.Sequence = _sequenceParts[--_sequenceIndex];
				if (_sequenceIndex == 0)
				{
					_sequenceParts = null;
				}
				else
				{
					AutoType.PerformGlobal(new List<PwDatabase> { e.Database }, null);
				}
			}
		}

		public override string UpdateUrl
		{
			get { return "https://nibiru.pl/keepass/plugins.php?name=PickCharsDeferred"; }
		}

		public override Image SmallIcon
		{
			get { return KeePassLib.Utility.GfxUtil.ScaleImage(Resource.Deferred, 16, 16); }
		}
	}
}
