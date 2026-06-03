using System;
using Fiddler;
using System.Windows.Forms;
using Standard;
using System.Reflection;
using MessagePackInspector;

namespace MessagePackInspector;

public class MessagePackResponseInspector : Inspector2, IResponseInspector2
{
    private TextBox viewControl;
    //private JSONView jSONView;
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
                string json = MessagePackJsonConverter.ConvertToJson(binaryContent);

                ShowJson(json);
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

        object jsonView = jsonViewField.GetValue(jSONResponseViewer);

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
    }

    public bool bReadOnly
    {
        get { return true; }
        set { }
    }

    public void Clear()
    {
        viewControl?.Text = string.Empty;

        if (jSONResponseViewer != null)
        {
            jSONResponseViewer.Clear();
        }
    }

    public byte[] body
    {
        get { return binaryContent; }
        set
        {
            // when fiddler updates this inspector's content, our control's content
            binaryContent = value;
            UpdateControlContent();
        }
    }

    #endregion

    #region IResponseInspector2 members

    public HTTPResponseHeaders headers
    {
        get { return null; }
        set { }
    }

    #endregion
}
