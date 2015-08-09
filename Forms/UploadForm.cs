using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Elmanager.Forms
{
    public partial class UploadForm : Form
    {
        public UploadForm()
        {
            InitializeComponent();
        }

        internal void UploadToZworqy(string file)
        {
            BeginUpload();
            NameValueCollection parameters = new NameValueCollection
                                                 {{"MAX_FILE_SIZE", "512000"}, {"upload", "Upload!"}};
            string response = Utils.HttpUploadFile("http://zworqy.com/up/index.php", file, "uploadfile",
                                                       "application/octet-stream", parameters);
            Regex r = new Regex("You have successfully uploaded this file");
            if (response == null)
            {
                uploadLabel.Text = "Upload failed!";
                return;
            }
            uploadLabel.Text = r.IsMatch(response)
                                   ? "Upload succeeded!"
                                   : "Upload failed probably! You can try anyway to get the file:";
            urlBox.Text = "http://zworqy.com/up/files/" + Path.GetFileName(file);
        }

        internal void UploadTok10X(string file)
        {
            BeginUpload();
            NameValueCollection parameters = new NameValueCollection {{"submit", "Upload"}};
            string response = Utils.HttpUploadFile("http://www.jappe2.net/upload/", file, "file", "application/octet-stream",
                                                       parameters);
            if (response == null)
            {
                uploadLabel.Text = "Upload failed!";
                return;
            }
            Regex r = new Regex(@"(http://jappe2\.net/upload/[a-z]+/" + Regex.Escape(Path.GetFileName(file)) + ")");
            if (r.IsMatch(response))
            {
                uploadLabel.Text = "Upload succeeded!";
                urlBox.Text = r.Match(response).Groups[0].Value;
            }
            else
            {
                uploadLabel.Text = "Upload failed!";
            }
        }

        private void BeginUpload()
        {
            Show();
            uploadLabel.Text = "Uploading...";
            Refresh();
        }
    }
}