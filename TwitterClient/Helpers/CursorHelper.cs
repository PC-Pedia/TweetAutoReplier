using System.Windows.Forms;

namespace TwitterClient.Helpers
{
    class CursorHelper
    {
        public static void ShowWaitCursor()
        {
            Cursor.Current = Cursors.WaitCursor;
        }

        public static void ShowDefaultCursor()
        {
            Cursor.Current = Cursors.Default;
        }
    }
}
