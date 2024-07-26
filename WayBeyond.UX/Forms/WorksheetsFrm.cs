using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WayBeyond.UX.Forms
{

    public partial class WorksheetsFrm : Form
    {
        public string WorkSheetName { get; set; }
        protected List<string> SheetNames { get; }
        public WorksheetsFrm(List<string> list)
        {
            InitializeComponent();
            SheetNames = list;
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            WorkSheetName = SheetNames[WrkShtLstBx.SelectedIndex];
            DialogResult = DialogResult.OK;
        }

        private void WorksheetsFrm_Load(object sender, EventArgs e)
        {
            WrkShtLstBx.DataSource = SheetNames.ToList();
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
