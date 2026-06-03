using System;
using Fiddler;
using System.Windows.Forms;
using Standard;
using System.Reflection;

namespace MessagePackInspector;
public class MessagePackRequestInspector : Inspector2, IRequestInspector2
{
    private TextBox viewControl;
    private JSONResponseViewer jSONResponseViewer = new JSONResponseViewer();
    private FieldInfo jsonViewField;
    private MethodInfo setJsonMethod;

    private byte[] binaryContent;

    private static Logger log = new Logger(true);

    public override void AddToTab(TabPage o)
    {
        jSONResponseViewer.AddToTab(o);
        o.Text = "MessagePack (.NET)";

        viewControl = new TextBox { Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Both, WordWrap = false };
        viewControl.BackColor = CONFIG.colorDisabledEdit;
        viewControl.Dock = DockStyle.Bottom;
        o.Controls.Add(viewControl);
    }

    /// <summary>
    /// Update the controls with the new binary content
    /// </summary>
    private void UpdateControlContent()
    {
        try
        {
            if (binaryContent == null || binaryContent.Length == 0)
            {
                this.Clear();
            }
            else
            {
                ShowJson(MessagePackJsonConverter.ConvertToJson(binaryContent));
            }
        }
        catch (Exception ex)
        {
            log.LogString(ex.ToString());
            Clear();
            viewControl.Text = ex.ToString();
        }
    }

    private void ShowJson(string json)
    {
        jsonViewField ??= typeof(JSONResponseViewer).GetField("myControl", BindingFlags.NonPublic | BindingFlags.Instance);

        object jsonView = jsonViewField
                .GetValue(jSONResponseViewer);

        setJsonMethod ??= jsonView.GetType().GetMethod("SetJSON");
        setJsonMethod.Invoke(jsonView, [json]);
    }

    #region IBaseInspector2 members

    public override int GetOrder()
    {
        return 1;
    }

    public bool bDirty
    {
        get { return false; }
        set { }
    }

    public bool bReadOnly
    {
        get { return true; }
        set { }
    }

    public void Clear()
    {
        if (viewControl != null)
        {
            viewControl.Clear();
        }

        jSONResponseViewer.Clear();
    }

    public byte[] body
    {
        get { return binaryContent; }
        set
        {
            // when fiddler updates this inspector's content, our controls' content
            binaryContent = value;
            UpdateControlContent();
        }
    }

    #endregion

    #region IRequestInspector2 members

    public HTTPRequestHeaders headers
    {
        get { return null; }
        set { }
    }

    #endregion
}

internal static class MessagePackJsonConverter
{
    private static readonly Nerdbank.MessagePack.MessagePackSerializer serializer = new Nerdbank.MessagePack.MessagePackSerializer();

    private static readonly Nerdbank.MessagePack.MessagePackSerializer.JsonOptions jsonOptions = new Nerdbank.MessagePack.MessagePackSerializer.JsonOptions()
    {
    };

    public static string ConvertToJson(byte[] binaryContent)
    {
        return serializer.ConvertToJson(binaryContent, jsonOptions);
    }
}
