using Microsoft.Ajax.Utilities;

namespace Core.Services.Implementations
{
    public class MicrosoftMinifierCssCompresionService:ICssScriptCompressionService
    {
        #region IScriptCompressionService Members

        public string Compress(string content)
        {
			var cssSettings = new CssSettings();
			cssSettings.ColorNames = CssColor.Hex;
			cssSettings.OutputMode = OutputMode.MultipleLines;
            cssSettings.CommentMode = CssComment.None;

			var value = new Minifier().MinifyStyleSheet(content, cssSettings);
			return value;
        }

        #endregion
    }
}