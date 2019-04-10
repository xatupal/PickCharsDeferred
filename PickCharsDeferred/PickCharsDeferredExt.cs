using System;
using System.Drawing;
using System.Text.RegularExpressions;
using PickCharsDeferred.Properties;
using KeePass.Plugins;
using KeePass.Util;

namespace PickCharsDeferred
{
	public sealed class PickCharsDeferredExt : Plugin
	{
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
			if (_sequenceParts != null)
				return;

			_sequenceParts = Regex.Split(e.Sequence, @"(?=\{PICKCHARS)");
			if (_sequenceParts.Length == 1)
			{
				_sequenceParts = null;
				return;
			}

			for (var i = 0; i < _sequenceParts.Length; i++)
			{
				var delayPlaceholder = string.Empty;
				for (int j = i - 1; j >= 0; j--)
				{
					delayPlaceholder = GetDelayPlaceholder(_sequenceParts[j]);
					if (delayPlaceholder != string.Empty)
						break;
				}

				var sequencePart = delayPlaceholder + _sequenceParts[i];
				var success = AutoType.PerformIntoCurrentWindow(e.Entry, e.Database, sequencePart);
				if (!success)
					break;
			}

			_sequenceParts = null;
			e.Sequence = string.Empty;
		}

		private string GetDelayPlaceholder(string sequencePart)
		{
			var startIndex = sequencePart.LastIndexOf("{DELAY=", StringComparison.OrdinalIgnoreCase);
			if (startIndex < 0)
				return string.Empty;

			var endIndex = sequencePart.IndexOf('}', startIndex);
			if (endIndex < 0)
				return string.Empty;

			return sequencePart.Substring(startIndex, endIndex - startIndex + 1);
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
