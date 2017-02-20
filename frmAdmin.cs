using System;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;

namespace SSC_Library_Management_System
{
    public partial class frmAdmin : Form
    {
        private bool dataModified = false;
        public frmAdmin()
        {
            InitializeComponent();
            dlgOpenFile.Filter = DataAccess.FileFilter;
            dlgSaveFile.Filter = DataAccess.FileFilter;
        }

        private void frmAdmin_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult r;
            // Allow the user to save the updated data
            if (!dataModified) return;
            r = MessageBox.Show("Save changes before closing?", "Save",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (r == DialogResult.No) return;
            // Save the data in a file.
            if (dlgSaveFile.ShowDialog() != DialogResult.OK)
            {
                e.Cancel = true;
                return;
            }
            try
            {
                LibraryApp.WriteBooks(dlgSaveFile.FileName,
                    lstBooks.Items);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                e.Cancel = true;
                return;
            }
        }

        private void mnuOpen_Click(object sender, EventArgs e)
        {
              try
            {
                lstBooks.Items.Clear();
                if (dlgOpenFile.ShowDialog() == DialogResult.OK)
                {
                    LibraryApp.ReadBooks(dlgOpenFile.FileName,lstBooks.Items);
                    if (lstBooks.Items.Count > 0)
                    {
                        lstBooks.SelectedIndex = 0;
                        mnuDelete.Enabled = true;
                    }
                    mnuOpen.Enabled = false;
                    mnuSave.Enabled = true;
                    mnuModify.Enabled = true;
                    mnuAdd.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void mnuSave_Click(object sender, EventArgs e)
        {
            if (dlgSaveFile.ShowDialog() == DialogResult.OK)
            {
                LibraryApp.WriteBooks(dlgSaveFile.FileName,lstBooks.Items);
                dataModified = false;
            }
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void mnuAdd_Click(object sender, EventArgs e)
        {
            frmAddBook frm = new frmAddBook();
            frm.ViewBook = null;  // Indicates Add rather than Modify
            if (frm.ShowDialog() == DialogResult.OK)
            {
                lstBooks.Items.Add(frm.ViewBook);
                lstBooks.SelectedItem = frm.ViewBook;
                dataModified = true;
            }
        }

        private void mnuModify_Click(object sender, EventArgs e)
        {
            int n = lstBooks.SelectedIndex;
            frmAddBook frm = new frmAddBook();
            frm.ViewBook = (Book)lstBooks.SelectedItem;
            if (frm.ShowDialog() == DialogResult.OK)
            {
                // Remove item and reinsert
                lstBooks.Items.RemoveAt(n);
                lstBooks.Items.Add(frm.ViewBook);
                lstBooks.SelectedItem = frm.ViewBook;
                dataModified = true;
            }
        }

        private void mnuDelete_Click(object sender, EventArgs e)
        {
            DialogResult r =
    MessageBox.Show("Are you sure you want to remove this Book?",
    "Delete?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (r == DialogResult.Yes)
            {
                int position = lstBooks.SelectedIndex;
                lstBooks.Items.RemoveAt(position);
                dataModified = true;
                if (lstBooks.Items.Count > 0)
                    lstBooks.SelectedIndex = 0;
                else
                    mnuDelete.Enabled = false;
            }
        }

        private void lstBooks_DoubleClick(object sender, EventArgs e)
        {
            if (lstBooks.SelectedIndex >= 0)
                mnuModify_Click(sender, e);
        }


     }

}
