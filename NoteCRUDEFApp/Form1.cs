using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;

namespace NoteCRUDEFApp
{
    public partial class Form1 : Form
    {
        NotesTab model = new NotesTab();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Clear();
            PopulateDataGridView();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }

        void Clear()
        {
            txtTitle.Clear();
            txtNote.Clear();
            btnSave.Text = "Save";
            btnDelete.Enabled = false;
            model.Id = 0;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            model.Title = txtTitle.Text.Trim();
            model.Note = txtNote.Text.Trim();
            using (DBEntities db = new DBEntities())
            {
                if(model.Id == 0) //Insert
                db.NotesTab.Add(model);
                else //Update
                    db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
            Clear();
            PopulateDataGridView();
            MessageBox.Show("Submitted Successfully");
        }

        void PopulateDataGridView()
        {
            using(DBEntities db = new DBEntities())
            {
                dgvNotes.DataSource = db.NotesTab.ToList<NotesTab>();
                dgvNotes.Columns["Id"].Visible = false;
                dgvNotes.Columns["Note"].Visible = false;
            }
        }

        private void dgvNotes_DoubleClick(object sender, EventArgs e)
        {
            if(dgvNotes.CurrentRow.Index != -1)
            {
                model.Id = Convert.ToInt32(dgvNotes.CurrentRow.Cells["Id"].Value);
                using(DBEntities db = new DBEntities())
                {
                    model = db.NotesTab.Where(x => x.Id == model.Id).FirstOrDefault();
                    txtTitle.Text = model.Title;
                    txtNote.Text = model.Note;
                }
                btnSave.Text = "Update";
                btnDelete.Enabled = true;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Are you sure to delete this record?", "Easy Note", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using(DBEntities db =new DBEntities())
                {
                    var entry = db.Entry(model);
                    if (entry.State == EntityState.Detached)
                        db.NotesTab.Attach(model);
                    db.NotesTab.Remove(model);
                    db.SaveChanges();
                    PopulateDataGridView();
                    Clear();
                    MessageBox.Show("Deleted Successfully");
                }
            }
        }
    }
}
